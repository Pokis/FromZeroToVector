using System.Diagnostics;
using System.Numerics.Tensors;
using Embeddings.Core;
using Microsoft.Extensions.AI;

// ============================================================================
// C# DEMO 1: Semantic Search in ~25 Lines of Modern C#
// ============================================================================
// What this shows:
//   1. IEmbeddingGenerator<string, Embedding<float>> (Microsoft.Extensions.AI)
//      -> Official .NET interface that turns strings into float[384] coordinate arrays.
//   2. TensorPrimitives.CosineSimilarity (System.Numerics.Tensors)
//      -> Built-in .NET method that compares the angle between two float[] arrays.
//
// Usage:
//   dotnet run                  -> Step-by-step interactive stage walkthrough
//   dotnet run -- --demo        -> Runs all 4 stage queries automatically
//   dotnet run -- --onnx        -> Uses local .onnx model file via ElBruno.LocalEmbeddings
// ============================================================================

ConsoleUi.WriteBanner(
    "C# DEMO 1: SEMANTIC SEARCH IN ~25 LINES",
    "Turning English Strings into float[384] Coordinates & Comparing Angles");

var (embedder, engineInfo) = await EmbeddingGeneratorFactory.CreateAsync(args);
Console.WriteLine($"  Engine : {engineInfo}");

ConsoleUi.WriteCodeSnippet(
    "How We Embed & Compare in C#",
    "var ticketVectors = await embedder.GenerateAsync(tickets); // Strings -> float[384]",
    "var queryVector   = await embedder.GenerateVectorAsync(query);",
    "float similarity  = TensorPrimitives.CosineSimilarity(queryVector.Span, ticketVec.Span);");

// Our "database" of support tickets
string[] tickets =
[
    "Payment gateway returns 502 error at checkout",
    "How do I reset my two-factor authentication?",
    "App crashes when uploading a profile picture",
    "Invoice PDF shows the wrong VAT rate",
    "Database connection pool exhausted during peak traffic",
    "Laptop screen is flickering after waking from sleep"
];

var sw = Stopwatch.StartNew();
var ticketVectors = await embedder.GenerateAsync(tickets);
sw.Stop();

ConsoleUi.WriteSectionHeader(
    $"Step 1: Indexed {tickets.Length} Support Tickets in {sw.Elapsed.TotalMilliseconds:F1} ms",
    "Every ticket is now stored as an array of 384 decimal coordinates:");

for (int i = 0; i < tickets.Length; i++)
{
    Console.ForegroundColor = ConsoleColor.DarkGray;
    Console.Write($"  #{i + 1} {ConsoleUi.FormatVectorPreview(ticketVectors[i].Vector.Span, 3)} ");
    Console.ResetColor();
    Console.WriteLine($"-> \"{tickets[i]}\"");
}

async Task RunQueryAsync(string query, string? explanation = null)
{
    Console.WriteLine();
    if (!string.IsNullOrWhiteSpace(explanation))
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine($"  ★ What to Watch: {explanation}");
    }

    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine($"  User Searches  : \"{query}\"");
    Console.ResetColor();

    var querySw = Stopwatch.StartNew();
    var queryVector = await embedder.GenerateVectorAsync(query);
    querySw.Stop();

    Console.ForegroundColor = ConsoleColor.DarkGray;
    Console.WriteLine($"  Query Vector   : {ConsoleUi.FormatVectorPreview(queryVector.Span, 4)} ({querySw.Elapsed.TotalMilliseconds:F2} ms)");
    Console.ResetColor();

    var rankedMatches = tickets
        .Zip(ticketVectors)
        .Select(x => (
            Ticket: x.First,
            Score: TensorPrimitives.CosineSimilarity(queryVector.Span, x.Second.Vector.Span)
        ))
        .OrderByDescending(x => x.Score)
        .ToList();

    var best = rankedMatches[0];
    Console.Write("  #1 Best Match  : ");
    ConsoleUi.WriteScoreBar(best.Score);
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"  \"{best.Ticket}\"");
    Console.ResetColor();

    foreach (var item in rankedMatches.Skip(1).Take(2))
    {
        Console.Write("  Runner-up      : ");
        ConsoleUi.WriteScoreBar(item.Score);
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine($"  \"{item.Ticket}\"");
        Console.ResetColor();
    }
}

(string Query, string Explanation)[] stageQueries =
[
    ("credit card won't go through",
     "Zero shared words with 'Payment gateway returns 502 error at checkout', yet matches by meaning!"),

    ("troubleshooting display refresh glitches",
     "Matches 'Laptop screen is flickering...' without needing SQL LIKE '%flicker%'!"),

    ("my avatar photo freezes the mobile app",
     "Understands that 'avatar photo freezes' means 'uploading a profile picture crashes'."),

    ("How do I DISABLE two-factor authentication?",
     "CLIFFHANGER TRAP: Matches 'How do I RESET my two-factor authentication?' at 98% because vectors only see TOPIC, not 'DISABLE'! (We fix this in Demo 3!)")
];

bool autoDemo = args.Any(a => a.Equals("--demo", StringComparison.OrdinalIgnoreCase) || a.Equals("--all", StringComparison.OrdinalIgnoreCase))
                || Console.IsInputRedirected;

if (autoDemo)
{
    ConsoleUi.WriteSectionHeader("Step 2: Running 4 Stage Queries");
    foreach (var (query, explanation) in stageQueries)
    {
        await RunQueryAsync(query, explanation);
    }
    return;
}

ConsoleUi.WriteSectionHeader(
    "Step 2: Interactive Stage Walkthrough",
    "Just press [Enter] to step through the 4 prepared stage queries one at a time, or type any custom sentence! ('q' to quit)");

int stageIdx = 0;
while (true)
{
    Console.WriteLine();
    Console.ForegroundColor = ConsoleColor.Yellow;
    if (stageIdx < stageQueries.Length)
    {
        Console.Write($"[Press Enter for Stage Query #{stageIdx + 1}/{stageQueries.Length}: \"{stageQueries[stageIdx].Query}\"] or type custom > ");
    }
    else
    {
        Console.Write("Type a custom issue description (or 'q' to quit) > ");
    }
    Console.ResetColor();

    var input = Console.ReadLine();
    if (input is null || input.Equals("q", StringComparison.OrdinalIgnoreCase) || input.Equals("exit", StringComparison.OrdinalIgnoreCase))
    {
        break;
    }

    if (string.IsNullOrWhiteSpace(input))
    {
        if (stageIdx < stageQueries.Length)
        {
            var (q, exp) = stageQueries[stageIdx++];
            await RunQueryAsync(q, exp);
        }
        else
        {
            stageIdx = 0;
            var (q, exp) = stageQueries[stageIdx++];
            await RunQueryAsync(q, exp);
        }
        continue;
    }

    await RunQueryAsync(input, "Custom live audience query");
}
