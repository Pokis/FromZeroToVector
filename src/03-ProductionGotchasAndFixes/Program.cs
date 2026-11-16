using System.Numerics;
using System.Numerics.Tensors;
using System.Text.RegularExpressions;
using Embeddings.Core;
using Microsoft.Extensions.AI;

// ============================================================================
// ACT 5 DEMO: Where Embeddings Lie — Production Gotchas & Engineering Fixes
// ============================================================================
// Demonstrates the 4 classic production failure modes of naive vector search
// and their battle-tested C# engineering solutions:
//   1. Negation & Exact ID Blindness -> Hybrid Search (Dense + BM25 + RRF)
//   2. The RAM Cliff (120GB+)        -> Matryoshka Slicing + int8 / 1-bit Quantization
//   3. Context-Destroying Chunking   -> Contextual Breadcrumb Chunking
//   4. Model Upgrade Lock-In         -> Blue/Green Dual-Vector Schema Migration
// ============================================================================

ConsoleUi.WriteBanner(
    "ACT 5: PRODUCTION REALITIES, GOTCHAS & FIXES IN C#",
    "Hybrid Search (BM25 + RRF), Matryoshka Slicing, Quantization & Blue/Green Migration");

var (embedder, engineInfo) = await EmbeddingGeneratorFactory.CreateAsync(args);
Console.WriteLine($"  Engine : {engineInfo}");

await RunGotcha1_NegationAndExactIds_HybridSearchFixAsync(embedder);
await RunGotcha2_MemoryCliff_MatryoshkaAndQuantizationFixAsync(embedder);
await RunGotcha3_ChunkingOrphanedPronouns_ContextualChunkingFixAsync(embedder);
await RunGotcha4_ModelMigration_BlueGreenSchemaFixAsync();

// ============================================================================
// GOTCHA 1: Negation & Exact Identifier Blindness -> Fixed by Hybrid Search (RRF)
// ============================================================================
static async Task RunGotcha1_NegationAndExactIds_HybridSearchFixAsync(
    IEmbeddingGenerator<string, Embedding<float>> embedder)
{
    ConsoleUi.WriteSectionHeader(
        "GOTCHA 1: Negation & Exact Error Code Blindness (Fix: Hybrid Search + RRF)",
        "Dense vectors capture general TOPIC, not exact IDs ('503' vs '504') or boolean negation ('enable' vs 'disable').");

    string[] knowledgeBase =
    [
        "DOC-101: How to ENABLE two-factor authentication (MFA) login security on your account",
        "DOC-102: How to RESET your two-factor authentication (MFA) login authenticator token",
        "DOC-103: How to permanently DISABLE and turn off two-factor authentication (MFA)",
        "DOC-201: Fixing payment checkout gateway HTTP Error 503 when credit card transaction fails",
        "DOC-202: Runbook for HTTP Error 504 upstream gateway timeout code"
    ];

    var kbVectors = await embedder.GenerateAsync(knowledgeBase);

    string[] trickyQueries =
    [
        "login account security: permanently disable and turn off MFA",
        "payment checkout credit card gateway HTTP Error 504 timeout code"
    ];

    foreach (var query in trickyQueries)
    {
        var queryVec = await embedder.GenerateVectorAsync(query);

        // 1. Dense Vector Ranking (Cosine Similarity)
        var denseRanked = knowledgeBase
            .Select((doc, idx) => (
                DocId: idx,
                Doc: doc,
                DenseScore: TensorPrimitives.CosineSimilarity(queryVec.Span, kbVectors[idx].Vector.Span)
            ))
            .OrderByDescending(x => x.DenseScore)
            .Select((item, rank) => (item.DocId, item.Doc, item.DenseScore, DenseRank: rank + 1))
            .ToList();

        // 2. Lexical Keyword Ranking (BM25-style IDF-weighted exact token match)
        var lexicalRanked = knowledgeBase
            .Select((doc, idx) => (
                DocId: idx,
                Doc: doc,
                Bm25Score: ComputeSimpleBm25(query, doc, knowledgeBase)
            ))
            .OrderByDescending(x => x.Bm25Score)
            .Select((item, rank) => (item.DocId, item.Doc, item.Bm25Score, LexicalRank: rank + 1))
            .ToList();

        // 3. Reciprocal Rank Fusion (RRF): Combines Dense Semantic Rank + Lexical BM25 Rank
        const int rrfK = 10;
        var hybridRanked = knowledgeBase
            .Select((doc, idx) =>
            {
                var d = denseRanked.First(x => x.DocId == idx);
                var l = lexicalRanked.First(x => x.DocId == idx);
                // Standard RRF with lexical exact-identifier tie-breaker
                double rrfScore = (1.0 / (rrfK + d.DenseRank)) + (1.05 / (rrfK + l.LexicalRank));
                return (Doc: doc, d.DenseScore, d.DenseRank, l.Bm25Score, l.LexicalRank, RrfScore: rrfScore);
            })
            .OrderByDescending(x => x.RrfScore)
            .ToList();

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"\n  Query: \"{query}\"");
        Console.ResetColor();
        Console.WriteLine($"  {"Document",-78} | {"Dense Rank",-12} | {"BM25 Rank",-11} | {"Hybrid RRF"}");
        Console.WriteLine($"  {new string('-', 78)}-+-{new string('-', 12)}-+-{new string('-', 11)}-+-{new string('-', 12)}");

        for (int i = 0; i < hybridRanked.Count; i++)
        {
            var row = hybridRanked[i];
            Console.ForegroundColor = i == 0 ? ConsoleColor.Green : ConsoleColor.Gray;
            string winnerTag = i == 0 ? " ★ #1" : $"   #{i + 1}";
            Console.WriteLine(
                $"  {row.Doc,-78} | #{row.DenseRank} ({row.DenseScore:P0})   | #{row.LexicalRank} ({row.Bm25Score,4:F1})  | {row.RrfScore:F4}{winnerTag}");
            Console.ResetColor();
        }
    }
}

// ============================================================================
// GOTCHA 2: The Cost & Storage Cliff -> Matryoshka Slicing & Quantization
// ============================================================================
static async Task RunGotcha2_MemoryCliff_MatryoshkaAndQuantizationFixAsync(
    IEmbeddingGenerator<string, Embedding<float>> embedder)
{
    ConsoleUi.WriteSectionHeader(
        "GOTCHA 2: The Storage & RAM Cliff (Fix: Matryoshka Slicing + int8 / 1-bit Quantization)",
        "10M vectors at 3,072 float32 dims = ~122.9 GB of RAM! Let's shrink vectors 4x to 32x in C#.");

    string query = "credit card declined at payment checkout with 502 error";
    string matchingDoc = "Payment gateway returns 502 error at checkout when charging credit card";
    string unrelatedDoc = "Automated docker build pipeline deployed to internal kubernetes cluster";

    var vectors = await embedder.GenerateAsync([query, matchingDoc, unrelatedDoc]);
    ReadOnlySpan<float> qFull = vectors[0].Vector.Span;
    ReadOnlySpan<float> matchFull = vectors[1].Vector.Span;
    ReadOnlySpan<float> unrelFull = vectors[2].Vector.Span;

    Console.WriteLine("  1) Matryoshka Representation Learning (MRL) Dimension Slicing:");
    Console.WriteLine($"     {"Dimensions",-18} | {"Match Sim",-12} | {"Unrelated Sim",-15} | {"Separation Gap",-16} | {"RAM for 10M Vectors"}");
    Console.WriteLine($"     {new string('-', 18)}-+-{new string('-', 12)}-+-{new string('-', 15)}-+-{new string('-', 16)}-+-{new string('-', 20)}");

    foreach (int slicedDims in new[] { 384, 256, 128, 64 })
    {
        float[] qSlice = SliceAndNormalize(qFull, slicedDims);
        float[] mSlice = SliceAndNormalize(matchFull, slicedDims);
        float[] uSlice = SliceAndNormalize(unrelFull, slicedDims);

        float matchSim = TensorPrimitives.CosineSimilarity(qSlice, mSlice);
        float unrelSim = TensorPrimitives.CosineSimilarity(qSlice, uSlice);
        double ramGb = (10_000_000.0 * slicedDims * sizeof(float)) / (1024 * 1024 * 1024);

        Console.WriteLine(
            $"     float32[{slicedDims,3}]       | {matchSim,10:P1}   | {unrelSim,13:P1}   | +{matchSim - unrelSim,13:P1}   | {ramGb,6:F2} GB");
    }

    // 2) Scalar (int8) and Binary (1-bit) Quantization
    Console.WriteLine("\n  2) Vector Quantization (float32 -> int8 -> 1-bit Binary Hamming Distance):");

    sbyte[] qInt8 = QuantizeToInt8(qFull);
    sbyte[] mInt8 = QuantizeToInt8(matchFull);
    sbyte[] uInt8 = QuantizeToInt8(unrelFull);

    float int8MatchSim = CosineSimilarityInt8(qInt8, mInt8);
    float int8UnrelSim = CosineSimilarityInt8(qInt8, uInt8);

    ulong[] qBin = QuantizeToBinary1Bit(qFull);
    ulong[] mBin = QuantizeToBinary1Bit(matchFull);
    ulong[] uBin = QuantizeToBinary1Bit(unrelFull);

    float binMatchSim = BinaryHammingSimilarity(qBin, mBin, qFull.Length);
    float binUnrelSim = BinaryHammingSimilarity(qBin, uBin, qFull.Length);

    double floatRamGb = (10_000_000.0 * 384 * 4) / (1024 * 1024 * 1024);
    double int8RamGb = (10_000_000.0 * 384 * 1) / (1024 * 1024 * 1024);
    double binRamGb = (10_000_000.0 * (384 / 8.0)) / (1024 * 1024 * 1024);

    Console.WriteLine($"     • Full float32 (32-bit) : Match={TensorPrimitives.CosineSimilarity(qFull, matchFull):P1}, Unrelated={TensorPrimitives.CosineSimilarity(qFull, unrelFull):P1} | 10M RAM = {floatRamGb:F2} GB (1.0x)");
    Console.WriteLine($"     • Scalar int8  ( 8-bit) : Match={int8MatchSim:P1}, Unrelated={int8UnrelSim:P1} | 10M RAM =  {int8RamGb:F2} GB (4.0x smaller!)");
    Console.WriteLine($"     • Binary 1-bit (PopCnt) : Match={binMatchSim:P1}, Unrelated={binUnrelSim:P1} | 10M RAM =  {binRamGb:F2} GB (32.0x smaller!)");
}

// ============================================================================
// GOTCHA 3: Chunking Destroys Context -> Fixed by Contextual Header Prepending
// ============================================================================
static async Task RunGotcha3_ChunkingOrphanedPronouns_ContextualChunkingFixAsync(
    IEmbeddingGenerator<string, Embedding<float>> embedder)
{
    ConsoleUi.WriteSectionHeader(
        "GOTCHA 3: Naive Chunking Orphans Pronouns (Fix: Contextual Header Chunking)",
        "Splitting a document every N characters strips headers: 'It fails when...' — what is 'It'?!");

    string userQuery = "Why does the Stripe payment checkout fail with 502 during peak traffic?";

    string naiveOrphanChunk =
        "It rejects the request whenever the payload exceeds 4MB or retries expire.";

    string contextEnrichedChunk =
        "[Document: Stripe Payment Gateway Architecture > Section: Checkout 502 Peak Traffic Limits] " +
        "It rejects the request whenever the payload exceeds 4MB or retries expire.";

    var vectors = await embedder.GenerateAsync([userQuery, naiveOrphanChunk, contextEnrichedChunk]);
    float naiveScore = TensorPrimitives.CosineSimilarity(vectors[0].Vector.Span, vectors[1].Vector.Span);
    float enrichedScore = TensorPrimitives.CosineSimilarity(vectors[0].Vector.Span, vectors[2].Vector.Span);

    Console.WriteLine($"  Query                  : \"{userQuery}\"");
    Console.WriteLine($"  Naive Orphaned Chunk   : \"{naiveOrphanChunk}\"");
    Console.Write("  Naive Similarity       : ");
    ConsoleUi.WriteScoreBar(naiveScore);
    Console.WriteLine("  (Misses 'Stripe payment checkout' context!)");

    Console.WriteLine($"\n  Context-Enriched Chunk : \"{contextEnrichedChunk}\"");
    Console.Write("  Contextual Similarity  : ");
    ConsoleUi.WriteScoreBar(enrichedScore);
    Console.WriteLine("  (Breadcrumb restores full semantic context!)");
}

// ============================================================================
// GOTCHA 4: Model Lock-In & Incompatible Vector Spaces -> Blue/Green Migration
// ============================================================================
static async Task RunGotcha4_ModelMigration_BlueGreenSchemaFixAsync()
{
    ConsoleUi.WriteSectionHeader(
        "GOTCHA 4: Model Lock-In & Incompatible Spaces (Fix: Blue/Green Vector Columns)",
        "Even if Model A and Model B both output float[384], their coordinate axes are completely incompatible!");

    // Simulate Model V1 (existing database index) and Model V2 (newly upgraded model)
    var modelV1 = new DeterministicSemanticEmbeddingGenerator(modelId: "embed-model-v1", dimensions: 384, seedOffset: 0);
    var modelV2 = new DeterministicSemanticEmbeddingGenerator(modelId: "embed-model-v2", dimensions: 384, seedOffset: 9999);

    string docText = "Payment gateway returns 502 error at checkout";
    string queryText = "credit card checkout payment fails with 502 error";

    var docVecV1 = await modelV1.GenerateVectorAsync(docText);
    var docVecV2 = await modelV2.GenerateVectorAsync(docText);

    var queryVecV1 = await modelV1.GenerateVectorAsync(queryText);
    var queryVecV2 = await modelV2.GenerateVectorAsync(queryText);

    float v1ToV1 = TensorPrimitives.CosineSimilarity(queryVecV1.Span, docVecV1.Span);
    float v2ToV1_Broken = TensorPrimitives.CosineSimilarity(queryVecV2.Span, docVecV1.Span);
    float v2ToV2 = TensorPrimitives.CosineSimilarity(queryVecV2.Span, docVecV2.Span);

    Console.WriteLine($"  Document : \"{docText}\"");
    Console.WriteLine($"  Query    : \"{queryText}\"\n");

    Console.Write("  1. Query(v1) vs DB(v1) [Consistent Old Model] : ");
    ConsoleUi.WriteScoreBar(v1ToV1);
    Console.WriteLine("  (Works)");

    Console.Write("  2. Query(v2) vs DB(v1) [BROKEN MIXED MODELS!] : ");
    ConsoleUi.WriteScoreBar(Math.Max(0f, v2ToV1_Broken));
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"  (Raw: {v2ToV1_Broken:+0.000;-0.000} -> Total random noise!)");
    Console.ResetColor();

    Console.Write("  3. Query(v2) vs DB(v2) [After Blue/Green Flip]: ");
    ConsoleUi.WriteScoreBar(v2ToV2);
    Console.WriteLine("  (Works after re-indexing!)");

    ConsoleUi.WriteCallout(
        "Production Schema Rule",
        "Always store `model_id` + `model_version` alongside vectors, and use Blue/Green columns (`embedding_v1`, `embedding_v2`) when upgrading models!",
        ConsoleColor.Yellow);
}

// ============================================================================
// Helper Methods for BM25, Matryoshka Slicing & Quantization
// ============================================================================
static double ComputeSimpleBm25(string query, string doc, IReadOnlyList<string> corpus)
{
    var qTokens = Regex.Matches(query.ToLowerInvariant(), @"[a-z0-9]+").Select(m => m.Value).ToHashSet();
    var dTokens = Regex.Matches(doc.ToLowerInvariant(), @"[a-z0-9]+").Select(m => m.Value).ToList();

    double score = 0.0;
    foreach (var qt in qTokens)
    {
        int tf = dTokens.Count(t => t == qt);
        if (tf == 0) continue;

        int df = corpus.Count(c => Regex.IsMatch(c.ToLowerInvariant(), $@"\b{Regex.Escape(qt)}\b"));
        double idf = Math.Log(1.0 + (corpus.Count - df + 0.5) / (df + 0.5));
        double exactCodeBoost = Regex.IsMatch(qt, @"^\d+$") ? 2.5 : 1.0;
        score += idf * ((tf * 2.2) / (tf + 1.2)) * exactCodeBoost;
    }
    return score;
}

static float[] SliceAndNormalize(ReadOnlySpan<float> fullVector, int targetDims)
{
    float[] sliced = fullVector[..targetDims].ToArray();
    float norm = TensorPrimitives.Norm(sliced.AsSpan());
    if (norm > 1e-8f)
    {
        TensorPrimitives.Divide(sliced.AsSpan(), norm, sliced.AsSpan());
    }
    return sliced;
}

static sbyte[] QuantizeToInt8(ReadOnlySpan<float> vector)
{
    float maxAbs = 0f;
    foreach (float v in vector)
    {
        if (MathF.Abs(v) > maxAbs) maxAbs = MathF.Abs(v);
    }
    float scale = maxAbs > 0 ? 127f / maxAbs : 1f;

    var result = new sbyte[vector.Length];
    for (int i = 0; i < vector.Length; i++)
    {
        result[i] = (sbyte)Math.Clamp(MathF.Round(vector[i] * scale), -127f, 127f);
    }
    return result;
}

static float CosineSimilarityInt8(ReadOnlySpan<sbyte> a, ReadOnlySpan<sbyte> b)
{
    long dot = 0, normA = 0, normB = 0;
    for (int i = 0; i < a.Length; i++)
    {
        dot += (long)a[i] * b[i];
        normA += (long)a[i] * a[i];
        normB += (long)b[i] * b[i];
    }
    return (float)(dot / (Math.Sqrt(normA) * Math.Sqrt(normB) + 1e-8));
}

static ulong[] QuantizeToBinary1Bit(ReadOnlySpan<float> vector)
{
    int numWords = (vector.Length + 63) / 64;
    var packed = new ulong[numWords];
    for (int i = 0; i < vector.Length; i++)
    {
        if (vector[i] >= 0f)
        {
            packed[i / 64] |= 1UL << (i % 64);
        }
    }
    return packed;
}

static float BinaryHammingSimilarity(ReadOnlySpan<ulong> a, ReadOnlySpan<ulong> b, int totalBits)
{
    int differingBits = 0;
    for (int i = 0; i < a.Length; i++)
    {
        // Hardware POPCNT instruction in .NET!
        differingBits += BitOperations.PopCount(a[i] ^ b[i]);
    }
    return 1.0f - ((float)differingBits / totalBits);
}
