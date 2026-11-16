using System.Diagnostics;
using System.Numerics.Tensors;
using Embeddings.Core;
using Microsoft.Extensions.AI;

// ============================================================================
// ACT 3 LIVE CODING DEMO: Semantic Search in ~25 Lines of Modern C#
// ============================================================================
// Key .NET Primitives Shown on Stage:
//   1. IEmbeddingGenerator<string, Embedding<float>> (Microsoft.Extensions.AI)
//   2. TensorPrimitives.CosineSimilarity (System.Numerics.Tensors — SIMD accelerated!)
//
// Usage:
//   dotnet run                  -> Interactive live stage prompt
//   dotnet run -- --demo        -> Auto-runs the 4 stage queries + Negation Gotcha
//   dotnet run -- --onnx        -> Uses ElBruno.LocalEmbeddings ONNX model (all-MiniLM-L6-v2)
// ============================================================================

ConsoleUi.WriteBanner(
    "ACT 3: SEMANTIC SEARCH IN MODERN C# (~25 LINES)",
    "Microsoft.Extensions.AI (IEmbeddingGenerator) + System.Numerics.Tensors (SIMD)");

// 1. Create the embedding generator (swap Local ONNX / OpenAI / Ollama / Azure via 1 line of DI)
var (embedder, engineInfo) = await EmbeddingGeneratorFactory.CreateAsync(args);
Console.WriteLine($"  Engine : {engineInfo}");

// 2. Our "database" of support tickets
string[] tickets =
[
    "Payment gateway returns 502 error at checkout",
    "How do I reset my two-factor authentication?",
    "App crashes when uploading a profile picture",
    "Invoice PDF shows the wrong VAT rate",
    "Database connection pool exhausted during peak traffic",
    "Laptop screen is flickering after waking from sleep"
];

// 3. Turn strings into float[] coordinates in 384-dimensional meaning space
var sw = Stopwatch.StartNew();
var ticketVectors = await embedder.GenerateAsync(tickets);
sw.Stop();

ConsoleUi.WriteSectionHeader(
    $"Indexed {tickets.Length} Support Tickets in {sw.Elapsed.TotalMilliseconds:F1} ms",
    "Each ticket is now a normalized float[384] coordinate on a unit hypersphere:");

for (int i = 0; i < tickets.Length; i++)
{
    Console.ForegroundColor = ConsoleColor.DarkGray;
    Console.Write($"  #{i + 1} {ConsoleUi.FormatVectorPreview(ticketVectors[i].Vector.Span, 3)} ");
    Console.ResetColor();
    Console.WriteLine($"-> \"{tickets[i]}\"");
}

// Helper function to execute & visualize a semantic search query
async Task RunQueryAsync(string query, string? stageNote = null)
{
    Console.WriteLine();
    if (!string.IsNullOrWhiteSpace(stageNote))
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine($"  [Stage Cue] {stageNote}");
    }

    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine($"  Query  : \"{query}\"");
    Console.ResetColor();

    var querySw = Stopwatch.StartNew();
    var queryVector = await embedder.GenerateVectorAsync(query);
    querySw.Stop();

    Console.ForegroundColor = ConsoleColor.DarkGray;
    Console.WriteLine($"  Vector : {ConsoleUi.FormatVectorPreview(queryVector.Span, 4)} (embedded in {querySw.Elapsed.TotalMilliseconds:F2} ms)");
    Console.ResetColor();

    // 4. Search by MEANING, not exact keywords — using hardware SIMD TensorPrimitives!
    var rankedMatches = tickets
        .Zip(ticketVectors)
        .Select(x => (
            Ticket: x.First,
            Score: TensorPrimitives.CosineSimilarity(queryVector.Span, x.Second.Vector.Span)
        ))
        .OrderByDescending(x => x.Score)
        .ToList();

    var best = rankedMatches[0];
    Console.Write("  Best   : ");
    ConsoleUi.WriteScoreBar(best.Score);
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"  \"{best.Ticket}\"");
    Console.ResetColor();

    // Show runner-ups for intuition
    foreach (var item in rankedMatches.Skip(1).Take(2))
    {
        Console.Write("           ");
        ConsoleUi.WriteScoreBar(item.Score);
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine($"  \"{item.Ticket}\"");
        Console.ResetColor();
    }
}

// Pre-scripted stage demo queries (triggered via --demo or pressing Enter)
(string Query, string StageNote)[] stageQueries =
[
    ("credit card won't go through",
     "Zero shared words with 'Payment gateway returns 502 error at checkout', yet matches by meaning!"),

    ("troubleshooting display refresh glitches",
     "Act 1 Callback: Matches 'Laptop screen is flickering...' without SQL LIKE '%flicker%'"),

    ("my avatar photo freezes the mobile app",
     "Maps 'avatar photo freezes' -> 'uploading a profile picture crashes'"),

    ("How do I DISABLE two-factor authentication?",
     "ACT 5 GOTCHA (Negation Trap): Matches 'How do I RESET my two-factor authentication?' at ~90% because vectors capture TOPIC, not boolean negation!")
];

bool autoDemo = args.Any(a => a.Equals("--demo", StringComparison.OrdinalIgnoreCase)) || Console.IsInputRedirected;

if (autoDemo)
{
    ConsoleUi.WriteSectionHeader("Running Scripted Stage Queries (--demo)");
    foreach (var (query, note) in stageQueries)
    {
        await RunQueryAsync(query, note);
    }
    return;
}

// 4. Interactive Stage Loop
ConsoleUi.WriteSectionHeader(
    "Live Interactive Search",
    "Type any issue description, press [Enter] on empty line to run the 4 stage demo queries, or 'q' to exit.");

while (true)
{
    Console.WriteLine();
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.Write("Describe your issue > ");
    Console.ResetColor();

    var input = Console.ReadLine();
    if (input is null || input.Equals("q", StringComparison.OrdinalIgnoreCase) || input.Equals("exit", StringComparison.OrdinalIgnoreCase))
    {
        break;
    }

    if (string.IsNullOrWhiteSpace(input))
    {
        foreach (var (query, note) in stageQueries)
        {
            await RunQueryAsync(query, note);
        }
        continue;
    }

    await RunQueryAsync(input);
}
