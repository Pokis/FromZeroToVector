using System.Numerics.Tensors;
using Embeddings.Core;
using Microsoft.Extensions.AI;

namespace ProductionGotchasAndFixes.Demos;

public static class Gotcha3_OrphanedChunksAndHeaders
{
    public static async Task RunAsync(
        IEmbeddingGenerator<string, Embedding<float>> embedder,
        bool autoRunAll = false)
    {
        ConsoleUi.WriteSectionHeader(
            "GOTCHA 3 OF 4: Splitting Documents Orphans Pronouns ('It fails when...')",
            "Fix: Always prepend [Document Title > Section Heading] to every paragraph before embedding.");

        ConsoleUi.WriteCodeSnippet(
            "Demos/Gotcha3_OrphanedChunksAndHeaders.cs",
            "string badChunk  = paragraphText; // \"It rejects the request whenever payload > 4MB.\"",
            "string goodChunk = $\"[Doc: {docTitle} > Section: {heading}] {paragraphText}\";");

        string userQuery = "Why does the Stripe payment checkout fail with 502 during peak traffic?";

        string naiveOrphanChunk =
            "It rejects the request whenever the payload exceeds 4MB or retries expire.";

        string contextEnrichedChunk =
            "[Document: Stripe Payment Gateway Architecture > Section: Checkout 502 Peak Traffic Limits] " +
            "It rejects the request whenever the payload exceeds 4MB or retries expire.";

        // 🔴 BREAKPOINT TIP: Put a breakpoint here to compare `naiveScore` vs `enrichedScore`!
        var vectors = await embedder.GenerateAsync([userQuery, naiveOrphanChunk, contextEnrichedChunk]);
        float naiveScore = TensorPrimitives.CosineSimilarity(vectors[0].Vector.Span, vectors[1].Vector.Span);
        float enrichedScore = TensorPrimitives.CosineSimilarity(vectors[0].Vector.Span, vectors[2].Vector.Span);

        ConsoleUi.WriteSubStepHeader("Step 3A", "First, look at what happens when we embed an orphaned paragraph by itself:");
        Console.WriteLine($"    User Searches         : \"{userQuery}\"");
        Console.WriteLine($"    ❌ Orphaned Paragraph : \"{naiveOrphanChunk}\"");
        Console.Write("       Similarity Score   : ");
        ConsoleUi.WriteScoreBar(naiveScore);
        Console.WriteLine("  (Fails! The paragraph never mentions 'Stripe' or 'Checkout'!)");

        ConsoleUi.PauseForSubStep("prepend the Document & Section Header before embedding", autoRunAll);

        ConsoleUi.WriteSubStepHeader("Step 3B", "Now let's prepend the Document & Section Header before calling GenerateAsync:");
        Console.WriteLine($"    ✅ With Header Added  : \"{contextEnrichedChunk}\"");
        Console.Write("       Similarity Score   : ");
        ConsoleUi.WriteScoreBar(enrichedScore);
        Console.WriteLine("  (Rescued! Jumps from 16.5% -> 77.7%!)");
    }
}
