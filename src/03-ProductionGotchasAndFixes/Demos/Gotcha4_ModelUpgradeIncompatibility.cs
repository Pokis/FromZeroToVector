using System.Numerics.Tensors;
using Embeddings.Core;
using Microsoft.Extensions.AI;

namespace ProductionGotchasAndFixes.Demos;

public static class Gotcha4_ModelUpgradeIncompatibility
{
    public static async Task RunAsync(bool autoRunAll = false)
    {
        ConsoleUi.WriteSectionHeader(
            "GOTCHA 4 OF 4: What Happens When You Switch Embedding Models?",
            "Every model invents its own coordinate axes. Mixing Model A and Model B gives 0% random noise!");

        ConsoleUi.WriteCodeSnippet(
            "Demos/Gotcha4_ModelUpgradeIncompatibility.cs",
            "// NEVER overwrite your live vector column in place when upgrading models!",
            "// 1. Add a new column `embedding_v2` alongside `embedding_v1`",
            "// 2. Backfill `embedding_v2` in the background using the new model",
            "// 3. Switch search queries to `embedding_v2` only after backfill finishes!");

        var modelV1 = new DeterministicSemanticEmbeddingGenerator(modelId: "embed-model-v1", dimensions: 384, seedOffset: 0);
        var modelV2 = new DeterministicSemanticEmbeddingGenerator(modelId: "embed-model-v2", dimensions: 384, seedOffset: 9999);

        string docText = "Payment gateway returns 502 error at checkout";
        string queryText = "credit card checkout payment fails with 502 error";

        var docVecV1 = await modelV1.GenerateVectorAsync(docText);
        var docVecV2 = await modelV2.GenerateVectorAsync(docText);

        var queryVecV1 = await modelV1.GenerateVectorAsync(queryText);
        var queryVecV2 = await modelV2.GenerateVectorAsync(queryText);

        // 🔴 BREAKPOINT TIP: Put a breakpoint here to compare `docVecV1` and `docVecV2`!
        float v1ToV1 = TensorPrimitives.CosineSimilarity(queryVecV1.Span, docVecV1.Span);
        float v2ToV1_Broken = TensorPrimitives.CosineSimilarity(queryVecV2.Span, docVecV1.Span);
        float v2ToV2 = TensorPrimitives.CosineSimilarity(queryVecV2.Span, docVecV2.Span);

        ConsoleUi.WriteSubStepHeader("Step 4A", "Look at the coordinates produced for the EXACT same sentence by Model v1 vs Model v2:");
        Console.WriteLine($"    Sentence : \"{docText}\"");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"    Model v1 : {ConsoleUi.FormatVectorPreview(docVecV1.Span, 4)}");
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine($"    Model v2 : {ConsoleUi.FormatVectorPreview(docVecV2.Span, 4)} (Completely different axes!)");
        Console.ResetColor();

        ConsoleUi.PauseForSubStep("see what happens when you query Model v1's database using Model v2", autoRunAll);

        ConsoleUi.WriteSubStepHeader("Step 4B", $"Searching for: \"{queryText}\"");

        Console.Write("    1. Query(Model v1) vs DB(Model v1) [Before Upgrade]     : ");
        ConsoleUi.WriteScoreBar(v1ToV1);
        Console.WriteLine("  ✅ Works");

        Console.Write("    2. Query(Model v2) vs DB(Model v1) [MISMATCHED MODELS!] : ");
        ConsoleUi.WriteScoreBar(Math.Max(0f, v2ToV1_Broken));
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"  ❌ BROKEN ({v2ToV1_Broken:P1} random noise!)");
        Console.ResetColor();

        Console.Write("    3. Query(Model v2) vs DB(Model v2) [After Blue/Green]   : ");
        ConsoleUi.WriteScoreBar(v2ToV2);
        Console.WriteLine("  ✅ Works after re-indexing!");
    }
}
