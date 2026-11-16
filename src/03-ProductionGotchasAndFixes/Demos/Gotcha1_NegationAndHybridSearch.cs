using System.Numerics.Tensors;
using System.Text.RegularExpressions;
using Embeddings.Core;
using Microsoft.Extensions.AI;

namespace ProductionGotchasAndFixes.Demos;

public static class Gotcha1_NegationAndHybridSearch
{
    public static async Task RunAsync(
        IEmbeddingGenerator<string, Embedding<float>> embedder,
        bool autoRunAll = false)
    {
        ConsoleUi.WriteSectionHeader(
            "GOTCHA 1 OF 4: Vectors Are Blind to 'DISABLE' vs 'ENABLE' & '503' vs '504'",
            "Fix: Combine Vector Rank (meaning) + Keyword Rank (exact words) using Reciprocal Rank Fusion.");

        ConsoleUi.WriteCodeSnippet(
            "Demos/Gotcha1_NegationAndHybridSearch.cs",
            "// 1. Rank docs by Vector Cosine Similarity (#1, #2, #3...)",
            "// 2. Rank docs by Exact Keyword BM25 Match (#1, #2, #3...)",
            "// 3. Combine ranks using Reciprocal Rank Fusion (RRF):",
            "double hybridRrfScore = (1.0 / (60 + vectorRank)) + (1.0 / (60 + keywordRank));");

        string[] knowledgeBase =
        [
            "DOC-101: How to ENABLE two-factor authentication (MFA) login security on your account",
            "DOC-102: How to RESET your two-factor authentication (MFA) login authenticator token",
            "DOC-103: How to permanently DISABLE and turn off two-factor authentication (MFA)",
            "DOC-201: Fixing payment checkout gateway HTTP Error 503 when credit card transaction fails",
            "DOC-202: Runbook for HTTP Error 504 upstream gateway timeout code"
        ];

        var kbVectors = await embedder.GenerateAsync(knowledgeBase);

        (string Query, string Label)[] queries =
        [
            ("login account security: permanently disable and turn off MFA", "Trap A: Opposite Action ('DISABLE' vs 'ENABLE' MFA)"),
            ("payment checkout credit card gateway HTTP Error 504 timeout code", "Trap B: Exact Error Number ('Error 504' vs 'Error 503')")
        ];

        for (int qIdx = 0; qIdx < queries.Length; qIdx++)
        {
            var (query, label) = queries[qIdx];
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

            // 🔴 BREAKPOINT TIP: Put a breakpoint here to compare `denseRanked` vs `hybridRanked`!
            const int rrfK = 10;
            var hybridRanked = knowledgeBase
                .Select((doc, idx) =>
                {
                    var d = denseRanked.First(x => x.DocId == idx);
                    var l = lexicalRanked.First(x => x.DocId == idx);
                    double rrfScore = (1.0 / (rrfK + d.DenseRank)) + (1.05 / (rrfK + l.LexicalRank));
                    return (Doc: doc, d.DenseScore, d.DenseRank, l.Bm25Score, l.LexicalRank, RrfScore: rrfScore);
                })
                .OrderByDescending(x => x.RrfScore)
                .Take(2)
                .ToList();

            ConsoleUi.WriteSubStepHeader($"Step 1.{qIdx + 1}", label);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"    User Searches : \"{query}\"");
            Console.ResetColor();

            // Show Pure Vector #1 mistake first
            var vectorWinner = denseRanked[0];
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"    ❌ Pure Vector #1 (WRONG) : \"{vectorWinner.Doc}\" ({vectorWinner.DenseScore:P0} vector match)");

            // Show Hybrid #1 rescue
            var hybridWinner = hybridRanked[0];
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"    ✅ Hybrid RRF  #1 (FIXED) : \"{hybridWinner.Doc}\"");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"       (Why it won: Vector Rank #{hybridWinner.DenseRank} + Exact Keyword Rank #{hybridWinner.LexicalRank} -> #1 Overall!)");
            Console.ResetColor();

            if (qIdx == 0)
            {
                ConsoleUi.PauseForSubStep("test Trap B (HTTP Error 504 vs Error 503)", autoRunAll);
            }
        }
    }

    private static double ComputeSimpleBm25(string query, string doc, IReadOnlyList<string> corpus)
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
}
