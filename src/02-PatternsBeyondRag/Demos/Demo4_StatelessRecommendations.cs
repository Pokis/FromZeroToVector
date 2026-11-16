using System.Numerics.Tensors;
using Embeddings.Core;
using Microsoft.Extensions.AI;

namespace PatternsBeyondRag.Demos;

public static class Demo4_StatelessRecommendations
{
    public static async Task RunAsync(
        IEmbeddingGenerator<string, Embedding<float>> embedder,
        bool autoRunAll = false)
    {
        ConsoleUi.WriteSectionHeader(
            "DEMO 4 OF 5: Stateless 'Recommended For You' Feed",
            "Problem: How do we recommend content without building a massive user-tracking database?");

        ConsoleUi.WriteCodeSnippet(
            "Demos/Demo4_StatelessRecommendations.cs",
            "// Average the float[384] arrays of the last 3 articles the user clicked:",
            "foreach (var readVec in lastThreeRead)",
            "    TensorPrimitives.Add(userTasteVector, readVec.Span, userTasteVector);",
            "// Then rank unread catalog articles by CosineSimilarity to userTasteVector!");

        // --------------------------------------------------------------------
        // SUB-STEP 4A: Average the user's last 3 read articles into 1 Taste Vector
        // --------------------------------------------------------------------
        ConsoleUi.WriteSubStepHeader("Step 4A", "The user just read these 3 articles (we average their coordinates into 1 Taste Vector):");

        string[] userRecentlyRead =
        [
            "Building high-performance vector math in C# and .NET with TensorPrimitives",
            "Running local ONNX embedding models in .NET microservices without cloud APIs",
            "How semantic search and vector embeddings replace brittle SQL LIKE queries"
        ];

        var historyVectors = await embedder.GenerateAsync(userRecentlyRead);
        int dims = historyVectors[0].Vector.Length;

        float[] userTasteVector = new float[dims];
        foreach (var h in historyVectors)
        {
            TensorPrimitives.Add(userTasteVector.AsSpan(), h.Vector.Span, userTasteVector.AsSpan());
        }
        TensorPrimitives.Divide(userTasteVector.AsSpan(), TensorPrimitives.Norm(userTasteVector.AsSpan()), userTasteVector.AsSpan());

        foreach (var item in userRecentlyRead)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine($"    • {item}");
        }
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"    => User's Live Taste Coordinate: {ConsoleUi.FormatVectorPreview(userTasteVector, 4)}");
        Console.ResetColor();

        ConsoleUi.PauseForSubStep("rank unread articles against the User's Taste Coordinate", autoRunAll);

        // --------------------------------------------------------------------
        // SUB-STEP 4B: Rank unread catalog articles against userTasteVector
        // --------------------------------------------------------------------
        ConsoleUi.WriteSubStepHeader("Step 4B", "Ranking unread articles by closeness to the User's Taste Coordinate:");

        string[] catalogArticles =
        [
            "Deep dive into Matryoshka vector quantization and hybrid search in C# .NET",
            "Async/Await and Span<T> memory optimization tricks for C# backend engineers",
            "Configuring EU VAT tax rates on PDF billing receipts and Stripe checkout",
            "Resetting two-factor authentication MFA passkeys when a user loses their phone"
        ];

        var catalogVectors = await embedder.GenerateAsync(catalogArticles);

        // 🔴 BREAKPOINT TIP: Put a breakpoint here to inspect `recommendations`!
        var recommendations = catalogArticles
            .Zip(catalogVectors)
            .Select(c => (
                Title: c.First,
                Score: TensorPrimitives.CosineSimilarity(userTasteVector.AsSpan(), c.Second.Vector.Span)
            ))
            .OrderByDescending(x => x.Score)
            .ToList();

        for (int i = 0; i < recommendations.Count; i++)
        {
            var rec = recommendations[i];
            Console.Write("    ");
            ConsoleUi.WriteScoreBar(rec.Score, 16);
            Console.ForegroundColor = i == 0 ? ConsoleColor.Green : ConsoleColor.White;
            Console.WriteLine($"  {rec.Title}{(i == 0 ? "  ★ TOP PICK" : "")}");
            Console.ResetColor();
        }
    }
}
