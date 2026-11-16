using Embeddings.Core;
using PatternsBeyondRag.Demos;

// ============================================================================
// C# DEMO 2: Five Backend Patterns Beyond Chatbots
// ============================================================================
// Tip for Live Presentation:
//   • Each demo lives in its own small file under the `Demos/` folder!
//   • You can comment/uncomment any line below to run only the demos you want,
//     OR pass `--demo 1` (or 2, 3, 4, 5) on the command line.
//   • Pass `--all` to run everything without pausing for [Enter].
// ============================================================================

bool autoRunAll = args.Any(a => a.Equals("--all", StringComparison.OrdinalIgnoreCase) || a.Equals("--demo", StringComparison.OrdinalIgnoreCase) && args.Length == 1);
string? selectedDemo = GetSelectedDemoArg(args);

ConsoleUi.WriteBanner(
    "C# DEMO 2: FIVE BACKEND PATTERNS BEYOND CHATBOTS",
    "Modular Step-by-Step Walkthrough (See Demos/ folder for individual files)");

var (embedder, engineInfo) = await EmbeddingGeneratorFactory.CreateAsync(args);
Console.WriteLine($"  Engine : {engineInfo}");

// ----------------------------------------------------------------------------
// Uncomment or comment out any pattern below during your live talk:
// ----------------------------------------------------------------------------

if (ShouldRun("1", selectedDemo))
{
    await Demo1_ZeroShotTicketRouting.RunAsync(embedder, autoRunAll);
    if (selectedDemo is null && !ConsoleUi.WaitForNextStep("Demo 2: Semantic Cache & Duplicate Blocker", autoRunAll)) return;
}

if (ShouldRun("2", selectedDemo))
{
    await Demo2_SemanticCacheAndDedup.RunAsync(embedder, autoRunAll);
    if (selectedDemo is null && !ConsoleUi.WaitForNextStep("Demo 3: Log Anomaly & Outlier Radar", autoRunAll)) return;
}

if (ShouldRun("3", selectedDemo))
{
    await Demo3_LogAnomalyRadar.RunAsync(embedder, autoRunAll);
    if (selectedDemo is null && !ConsoleUi.WaitForNextStep("Demo 4: Stateless Article Recommendations", autoRunAll)) return;
}

if (ShouldRun("4", selectedDemo))
{
    await Demo4_StatelessRecommendations.RunAsync(embedder, autoRunAll);
    if (selectedDemo is null && !ConsoleUi.WaitForNextStep("Demo 5: Multimodal Image/Audio/Text Search", autoRunAll)) return;
}

if (ShouldRun("5", selectedDemo))
{
    await Demo5_MultimodalSearch.RunAsync(embedder, autoRunAll);
}

static bool ShouldRun(string demoNumber, string? filter) =>
    string.IsNullOrWhiteSpace(filter) || filter.Equals(demoNumber, StringComparison.OrdinalIgnoreCase);

static string? GetSelectedDemoArg(string[] args)
{
    for (int i = 0; i < args.Length - 1; i++)
    {
        if (args[i].Equals("--demo", StringComparison.OrdinalIgnoreCase) ||
            args[i].Equals("--pattern", StringComparison.OrdinalIgnoreCase))
        {
            return args[i + 1];
        }
    }
    return null;
}
