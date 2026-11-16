using Embeddings.Core;
using ProductionGotchasAndFixes.Demos;

// ============================================================================
// C# DEMO 3: Production Gotchas & Engineering Fixes
// ============================================================================
// Tip for Live Presentation:
//   • Each Gotcha & Fix lives in its own small file under `Demos/`!
//   • Comment/uncomment any line below to run only one demo,
//     OR pass `--demo 1` (or 2, 3, 4) on the command line.
//   • Pass `--all` to run everything without pausing for [Enter].
// ============================================================================

bool autoRunAll = args.Any(a => a.Equals("--all", StringComparison.OrdinalIgnoreCase) || a.Equals("--demo", StringComparison.OrdinalIgnoreCase) && args.Length == 1);
string? selectedDemo = GetSelectedDemoArg(args);

ConsoleUi.WriteBanner(
    "C# DEMO 3: PRODUCTION GOTCHAS & ENGINEERING FIXES",
    "Modular Step-by-Step Walkthrough (See Demos/ folder for individual files)");

var (embedder, engineInfo) = await EmbeddingGeneratorFactory.CreateAsync(args);
Console.WriteLine($"  Engine : {engineInfo}");

// ----------------------------------------------------------------------------
// Uncomment or comment out any Gotcha demo below during your live talk:
// ----------------------------------------------------------------------------

if (ShouldRun("1", selectedDemo))
{
    await Gotcha1_NegationAndHybridSearch.RunAsync(embedder, autoRunAll);
    if (selectedDemo is null && !ConsoleUi.WaitForNextStep("Gotcha 2: Shrinking RAM 32x (Matryoshka & Quantization)", autoRunAll)) return;
}

if (ShouldRun("2", selectedDemo))
{
    await Gotcha2_RamCliffAndMatryoshka.RunAsync(embedder, autoRunAll);
    if (selectedDemo is null && !ConsoleUi.WaitForNextStep("Gotcha 3: Fixing Orphaned Chunks with Breadcrumb Headers", autoRunAll)) return;
}

if (ShouldRun("3", selectedDemo))
{
    await Gotcha3_OrphanedChunksAndHeaders.RunAsync(embedder, autoRunAll);
    if (selectedDemo is null && !ConsoleUi.WaitForNextStep("Gotcha 4: Why Changing Models Breaks Search (Blue/Green Fix)", autoRunAll)) return;
}

if (ShouldRun("4", selectedDemo))
{
    await Gotcha4_ModelUpgradeIncompatibility.RunAsync(autoRunAll);
}

static bool ShouldRun(string demoNumber, string? filter) =>
    string.IsNullOrWhiteSpace(filter) || filter.Equals(demoNumber, StringComparison.OrdinalIgnoreCase);

static string? GetSelectedDemoArg(string[] args)
{
    for (int i = 0; i < args.Length - 1; i++)
    {
        if (args[i].Equals("--demo", StringComparison.OrdinalIgnoreCase) ||
            args[i].Equals("--gotcha", StringComparison.OrdinalIgnoreCase))
        {
            return args[i + 1];
        }
    }
    return null;
}
