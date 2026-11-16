using System.Numerics.Tensors;
using Embeddings.Core;
using Microsoft.Extensions.AI;

namespace PatternsBeyondRag.Demos;

public static class Demo2_SemanticCacheAndDedup
{
    public static async Task RunAsync(
        IEmbeddingGenerator<string, Embedding<float>> embedder,
        bool autoRunAll = false)
    {
        ConsoleUi.WriteSectionHeader(
            "DEMO 2 OF 5: Semantic Cache & Duplicate Bug Blocker",
            "Problem: Users file the same Jira bug or ask your AI assistant the same question in different words.");

        ConsoleUi.WriteCodeSnippet(
            "Demos/Demo2_SemanticCacheAndDedup.cs",
            "float similarity = TensorPrimitives.CosineSimilarity(newVec.Span, cachedVec.Span);",
            "if (similarity >= 0.78f) return cachedEntry; // Instant cache hit / duplicate caught!");

        const float DuplicateThreshold = 0.78f;
        var cache = new List<(string Text, ReadOnlyMemory<float> Vector, string TicketId)>();

        // --------------------------------------------------------------------
        // SUB-STEP 2A: Seed 2 original tickets into our vector cache
        // --------------------------------------------------------------------
        ConsoleUi.WriteSubStepHeader("Step 2A", "First, 2 original bug reports arrive and get stored in our vector cache:");

        var initialTickets = new (string Text, string TicketId)[]
        {
            ("Payment gateway returns 502 error at checkout", "Jira #1042"),
            ("How do I reset my two-factor authentication?", "Jira #1043")
        };

        foreach (var (text, id) in initialTickets)
        {
            var vec = await embedder.GenerateVectorAsync(text);
            cache.Add((text, vec, id));
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"    [STORED {id}] ");
            Console.ResetColor();
            Console.WriteLine($"\"{text}\"");
        }

        ConsoleUi.PauseForSubStep("test incoming submissions for duplicates", autoRunAll);

        // --------------------------------------------------------------------
        // SUB-STEP 2B: Test new incoming submissions against the cache
        // --------------------------------------------------------------------
        ConsoleUi.WriteSubStepHeader("Step 2B", "Now 3 new submissions arrive (notice they use different words):");

        string[] incomingSubmissions =
        [
            "Checkout credit card payment fails with 502 gateway error",
            "How can I reset my 2FA two-factor authentication login?",
            "Invoice PDF shows the wrong VAT tax rate for EU customers"
        ];

        foreach (var submission in incomingSubmissions)
        {
            var vec = await embedder.GenerateVectorAsync(submission);

            // 🔴 BREAKPOINT TIP: Put a breakpoint here to inspect `bestMatch.Score`!
            var bestMatch = cache
                .Select(entry => (
                    Entry: entry,
                    Score: TensorPrimitives.CosineSimilarity(vec.Span, entry.Vector.Span)
                ))
                .OrderByDescending(x => x.Score)
                .First();

            if (bestMatch.Score >= DuplicateThreshold)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"    [⚡ DUPLICATE CAUGHT {bestMatch.Score:P0}] ");
                Console.ResetColor();
                Console.WriteLine($"\"{submission}\"");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($"       -> Linked to existing {bestMatch.Entry.TicketId}: \"{bestMatch.Entry.Text}\"");
                Console.ResetColor();
            }
            else
            {
                string newId = $"Jira #{1042 + cache.Count}";
                cache.Add((submission, vec, newId));
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"    [✅ NEW ISSUE        {bestMatch.Score:P0}] ");
                Console.ResetColor();
                Console.WriteLine($"\"{submission}\" -> Created {newId}");
            }
        }
    }
}
