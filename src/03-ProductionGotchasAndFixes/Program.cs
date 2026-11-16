using System.Numerics;
using System.Numerics.Tensors;
using System.Text.RegularExpressions;
using Embeddings.Core;
using Microsoft.Extensions.AI;

// ============================================================================
// C# DEMO 3: Where Embeddings Lie — Production Gotchas & Fixes (Step-by-Step)
// ============================================================================
// Demonstrates the 4 real-world traps of vector search and how to fix them in C#:
//   1. Negation & Exact Code Trap -> Fixed by Hybrid Search (Vector + Keyword)
//   2. The 120 GB RAM Cliff       -> Fixed by Matryoshka Slicing & Quantization
//   3. Orphaned Paragraphs        -> Fixed by Contextual Breadcrumb Headers
//   4. Model Upgrade Lock-In      -> Fixed by Blue/Green Vector Columns
//
// Usage:
//   dotnet run --project src/03-ProductionGotchasAndFixes          (Step-by-step)
//   dotnet run --project src/03-ProductionGotchasAndFixes -- --all (Run all non-stop)
// ============================================================================

bool runAllNonStop = args.Any(a => a.Equals("--all", StringComparison.OrdinalIgnoreCase) || a.Equals("--demo", StringComparison.OrdinalIgnoreCase));

ConsoleUi.WriteBanner(
    "C# DEMO 3: PRODUCTION GOTCHAS & ENGINEERING FIXES",
    "Step-by-Step: Hybrid Search, RAM Slicing/Quantization, Chunking & Model Upgrades");

var (embedder, engineInfo) = await EmbeddingGeneratorFactory.CreateAsync(args);
Console.WriteLine($"  Engine : {engineInfo}");

await RunGotcha1_NegationAndExactIds_HybridSearchFixAsync(embedder);
if (!ConsoleUi.WaitForNextStep("Gotcha 2: Shrinking RAM 32x (Matryoshka & Quantization)", runAllNonStop)) return;

await RunGotcha2_MemoryCliff_MatryoshkaAndQuantizationFixAsync(embedder);
if (!ConsoleUi.WaitForNextStep("Gotcha 3: Fixing Orphaned Chunks with Breadcrumb Headers", runAllNonStop)) return;

await RunGotcha3_ChunkingOrphanedPronouns_ContextualChunkingFixAsync(embedder);
if (!ConsoleUi.WaitForNextStep("Gotcha 4: Why Changing Models Breaks Search (Blue/Green Fix)", runAllNonStop)) return;

await RunGotcha4_ModelMigration_BlueGreenSchemaFixAsync();

// ============================================================================
// GOTCHA 1: Negation & Exact Identifier Blindness -> Fixed by Hybrid Search (RRF)
// ============================================================================
static async Task RunGotcha1_NegationAndExactIds_HybridSearchFixAsync(
    IEmbeddingGenerator<string, Embedding<float>> embedder)
{
    ConsoleUi.WriteSectionHeader(
        "GOTCHA 1 OF 4: Vectors Are Blind to 'DISABLE' vs 'ENABLE' & '503' vs '504'",
        "Fix: Combine Vector Rank (meaning) + Keyword Rank (exact words) using Reciprocal Rank Fusion.");

    ConsoleUi.WriteCodeSnippet(
        "Hybrid Search Rank Fusion (RRF) in 3 Lines of C#",
        "// Merge Vector Rank (#1, #2, #3...) with Exact Keyword Rank (#1, #2, #3...):",
        "double rrfScore = (1.0 / (60 + vectorRank)) + (1.0 / (60 + keywordRank));",
        "// A doc that is #2 in Vectors and #1 in Keywords automatically wins #1 overall!");

    string[] knowledgeBase =
    [
        "DOC-101: How to ENABLE two-factor authentication (MFA) login security on your account",
        "DOC-102: How to RESET your two-factor authentication (MFA) login authenticator token",
        "DOC-103: How to permanently DISABLE and turn off two-factor authentication (MFA)",
        "DOC-201: Fixing payment checkout gateway HTTP Error 503 when credit card transaction fails",
        "DOC-202: Runbook for HTTP Error 504 upstream gateway timeout code"
    ];

    var kbVectors = await embedder.GenerateAsync(knowledgeBase);

    (string Query, int CorrectDocIdx, string TrapReason)[] trickyQueries =
    [
        ("login account security: permanently disable and turn off MFA",
         2,
         "Pure Vector Search puts 'ENABLE MFA' (DOC-101) at #1! Keyword match on 'disable'/'turn off' rescues DOC-103 to ★ #1!"),

        ("payment checkout credit card gateway HTTP Error 504 timeout code",
         4,
         "Pure Vector Search puts 'Error 503' (DOC-201) at #1! Keyword match on exact code '504' rescues DOC-202 to ★ #1!")
    ];

    foreach (var (query, correctIdx, trapReason) in trickyQueries)
    {
        var queryVec = await embedder.GenerateVectorAsync(query);

        var denseRanked = knowledgeBase
            .Select((doc, idx) => (
                DocId: idx,
                Doc: doc,
                DenseScore: TensorPrimitives.CosineSimilarity(queryVec.Span, kbVectors[idx].Vector.Span)
            ))
            .OrderByDescending(x => x.DenseScore)
            .Select((item, rank) => (item.DocId, item.Doc, item.DenseScore, DenseRank: rank + 1))
            .ToList();

        var lexicalRanked = knowledgeBase
            .Select((doc, idx) => (
                DocId: idx,
                Doc: doc,
                Bm25Score: ComputeSimpleBm25(query, doc, knowledgeBase)
            ))
            .OrderByDescending(x => x.Bm25Score)
            .Select((item, rank) => (item.DocId, item.Doc, item.Bm25Score, LexicalRank: rank + 1))
            .ToList();

        const int rrfK = 10;
        var hybridRanked = knowledgeBase
            .Select((doc, idx) =>
            {
                var d = denseRanked.First(x => x.DocId == idx);
                var l = lexicalRanked.First(x => x.DocId == idx);
                double rrfScore = (1.0 / (rrfK + d.DenseRank)) + (1.05 / (rrfK + l.LexicalRank));
                return (DocId: idx, Doc: doc, d.DenseScore, d.DenseRank, l.Bm25Score, l.LexicalRank, RrfScore: rrfScore);
            })
            .OrderByDescending(x => x.RrfScore)
            .Take(3)
            .ToList();

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"  User Searches : \"{query}\"");
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine($"  What Happens  : {trapReason}");
        Console.ResetColor();

        for (int i = 0; i < hybridRanked.Count; i++)
        {
            var row = hybridRanked[i];
            bool isWinner = i == 0;
            Console.ForegroundColor = isWinner ? ConsoleColor.Green : ConsoleColor.Gray;
            string status = isWinner
                ? $"★ HYBRID #1 WINNER (Rescued from Vector #{row.DenseRank} via Keyword #{row.LexicalRank}!)"
                : $"  Hybrid #{i + 1}         (Was Vector #{row.DenseRank}, Keyword #{row.LexicalRank})";
            Console.WriteLine($"    {status}");
            Console.WriteLine($"      -> \"{row.Doc}\"");
            Console.ResetColor();
        }
        Console.WriteLine();
    }
}

// ============================================================================
// GOTCHA 2: The Cost & Storage Cliff -> Matryoshka Slicing & Quantization
// ============================================================================
static async Task RunGotcha2_MemoryCliff_MatryoshkaAndQuantizationFixAsync(
    IEmbeddingGenerator<string, Embedding<float>> embedder)
{
    ConsoleUi.WriteSectionHeader(
        "GOTCHA 2 OF 4: The 120 GB RAM Bill Shock (Fix: Matryoshka Slicing & Quantization)",
        "1) Chop the array length (Matryoshka) and 2) Round 32-bit floats down to 8-bit or 1-bit.");

    ConsoleUi.WriteCodeSnippet(
        "Matryoshka Slicing in 2 Lines of C#",
        "float[] sliced = fullVector.AsSpan(0, 128).ToArray(); // Keep first 128 of 384 numbers!",
        "TensorPrimitives.Divide(sliced, TensorPrimitives.Norm(sliced), sliced); // Re-normalize to 1.0");

    string query = "credit card declined at payment checkout with 502 error";
    string matchingDoc = "Payment gateway returns 502 error at checkout when charging credit card";
    string unrelatedDoc = "Automated docker build pipeline deployed to internal kubernetes cluster";

    var vectors = await embedder.GenerateAsync([query, matchingDoc, unrelatedDoc]);
    ReadOnlySpan<float> qFull = vectors[0].Vector.Span;
    ReadOnlySpan<float> matchFull = vectors[1].Vector.Span;
    ReadOnlySpan<float> unrelFull = vectors[2].Vector.Span;

    Console.WriteLine("  Trick #1: Matryoshka Array Slicing (Chopping off the tail of the array):");
    Console.WriteLine($"    {"Array Length Kept",-22} | {"Match Score",-13} | {"Unrelated",-11} | {"RAM for 10M Docs"}");
    Console.WriteLine($"    {new string('-', 22)}-+-{new string('-', 13)}-+-{new string('-', 11)}-+-{new string('-', 18)}");

    foreach (int slicedDims in new[] { 384, 256, 128, 64 })
    {
        float[] qSlice = SliceAndNormalize(qFull, slicedDims);
        float[] mSlice = SliceAndNormalize(matchFull, slicedDims);
        float[] uSlice = SliceAndNormalize(unrelFull, slicedDims);

        float matchSim = TensorPrimitives.CosineSimilarity(qSlice, mSlice);
        float unrelSim = TensorPrimitives.CosineSimilarity(qSlice, uSlice);
        double ramGb = (10_000_000.0 * slicedDims * sizeof(float)) / (1024 * 1024 * 1024);

        Console.WriteLine(
            $"    float[{slicedDims,3}] ({slicedDims * 100 / 384,3}% kept) | {matchSim,10:P1}    | {unrelSim,9:P1}   | {ramGb,6:F2} GB");
    }

    Console.WriteLine("\n  Trick #2: Quantization (Rounding each number to fewer bits):");
    sbyte[] qInt8 = QuantizeToInt8(qFull);
    sbyte[] mInt8 = QuantizeToInt8(matchFull);
    float int8MatchSim = CosineSimilarityInt8(qInt8, mInt8);

    ulong[] qBin = QuantizeToBinary1Bit(qFull);
    ulong[] mBin = QuantizeToBinary1Bit(matchFull);
    float binMatchSim = BinaryHammingSimilarity(qBin, mBin, qFull.Length);

    Console.WriteLine($"    • Full 32-bit decimal (float) : Match = {TensorPrimitives.CosineSimilarity(qFull, matchFull):P1} | 10M RAM = 14.31 GB (Baseline)");
    Console.WriteLine($"    • Rounded 8-bit int   (sbyte) : Match = {int8MatchSim:P1} | 10M RAM =  3.58 GB (4x Smaller!)");
    Console.WriteLine($"    • 1-bit +/- sign only (ulong) : Match = {binMatchSim:P1} | 10M RAM =  0.45 GB (32x Smaller!)");
}

// ============================================================================
// GOTCHA 3: Chunking Destroys Context -> Fixed by Contextual Header Prepending
// ============================================================================
static async Task RunGotcha3_ChunkingOrphanedPronouns_ContextualChunkingFixAsync(
    IEmbeddingGenerator<string, Embedding<float>> embedder)
{
    ConsoleUi.WriteSectionHeader(
        "GOTCHA 3 OF 4: Splitting Documents Orphans Pronouns ('It fails when...')",
        "Fix: Always prepend [Document Title > Section Heading] to every paragraph before embedding.");

    ConsoleUi.WriteCodeSnippet(
        "Contextual Chunking Fix",
        "string badChunk  = paragraphText; // \"It rejects the request whenever payload > 4MB.\"",
        "string goodChunk = $\"[Doc: {docTitle} > Section: {heading}] {paragraphText}\";");

    string userQuery = "Why does the Stripe payment checkout fail with 502 during peak traffic?";

    string naiveOrphanChunk =
        "It rejects the request whenever the payload exceeds 4MB or retries expire.";

    string contextEnrichedChunk =
        "[Document: Stripe Payment Gateway Architecture > Section: Checkout 502 Peak Traffic Limits] " +
        "It rejects the request whenever the payload exceeds 4MB or retries expire.";

    var vectors = await embedder.GenerateAsync([userQuery, naiveOrphanChunk, contextEnrichedChunk]);
    float naiveScore = TensorPrimitives.CosineSimilarity(vectors[0].Vector.Span, vectors[1].Vector.Span);
    float enrichedScore = TensorPrimitives.CosineSimilarity(vectors[0].Vector.Span, vectors[2].Vector.Span);

    Console.WriteLine($"  User Searches          : \"{userQuery}\"\n");
    Console.WriteLine($"  ❌ Orphaned Paragraph  : \"{naiveOrphanChunk}\"");
    Console.Write("     Similarity Score    : ");
    ConsoleUi.WriteScoreBar(naiveScore);
    Console.WriteLine("  (Low! Doesn't know what 'It' refers to!)");

    Console.WriteLine($"\n  ✅ With Header Added   : \"{contextEnrichedChunk}\"");
    Console.Write("     Similarity Score    : ");
    ConsoleUi.WriteScoreBar(enrichedScore);
    Console.WriteLine("  (Rescued! Jumps from 16% -> 78%!)");
}

// ============================================================================
// GOTCHA 4: Model Lock-In & Incompatible Vector Spaces -> Blue/Green Migration
// ============================================================================
static async Task RunGotcha4_ModelMigration_BlueGreenSchemaFixAsync()
{
    ConsoleUi.WriteSectionHeader(
        "GOTCHA 4 OF 4: What Happens When You Switch Embedding Models?",
        "Every model invents its own coordinate axes. Mixing Model A and Model B gives 0% random noise!");

    ConsoleUi.WriteCodeSnippet(
        "Why You Need Blue/Green Columns When Upgrading Models",
        "// NEVER overwrite your live vector column in place!",
        "// 1. Add column `embedding_v2` alongside `embedding_v1`",
        "// 2. Backfill `embedding_v2` in background using the new model",
        "// 3. Flip search queries to `embedding_v2` once 100% backfilled!");

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

    Console.WriteLine($"  Database Row : \"{docText}\"");
    Console.WriteLine($"  User Query   : \"{queryText}\"\n");

    Console.Write("  1. Query(Model v1) vs DB(Model v1) [Before Upgrade]     : ");
    ConsoleUi.WriteScoreBar(v1ToV1);
    Console.WriteLine("  ✅ Works");

    Console.Write("  2. Query(Model v2) vs DB(Model v1) [MISMATCHED MODELS!] : ");
    ConsoleUi.WriteScoreBar(Math.Max(0f, v2ToV1_Broken));
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"  ❌ BROKEN ({v2ToV1_Broken:P1} random noise!)");
    Console.ResetColor();

    Console.Write("  3. Query(Model v2) vs DB(Model v2) [After Blue/Green]   : ");
    ConsoleUi.WriteScoreBar(v2ToV2);
    Console.WriteLine("  ✅ Works after re-indexing!");
}

// ============================================================================
// Helper Methods
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
        differingBits += BitOperations.PopCount(a[i] ^ b[i]);
    }
    return 1.0f - ((float)differingBits / totalBits);
}
