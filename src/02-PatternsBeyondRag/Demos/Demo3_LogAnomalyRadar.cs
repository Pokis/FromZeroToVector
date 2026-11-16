using System.Numerics.Tensors;
using Embeddings.Core;
using Microsoft.Extensions.AI;

namespace PatternsBeyondRag.Demos;

public static class Demo3_LogAnomalyRadar
{
    public static async Task RunAsync(
        IEmbeddingGenerator<string, Embedding<float>> embedder,
        bool autoRunAll = false)
    {
        ConsoleUi.WriteSectionHeader(
            "DEMO 3 OF 5: Anomaly & Outlier Radar in Production Logs",
            "Problem: Regex rules only catch errors you already know how to spell. How do we catch novel failures?");

        ConsoleUi.WriteCodeSnippet(
            "Demos/Demo3_LogAnomalyRadar.cs",
            "// 1. Average healthy logs together to find the 'Normal Center' coordinate:",
            "foreach (var v in healthyLogs) TensorPrimitives.Add(normalCenter, v.Span, normalCenter);",
            "// 2. Measure how far each new log sits from that normal center:",
            "float distanceFromNormal = 1.0f - TensorPrimitives.CosineSimilarity(normalCenter, newLogVec.Span);");

        // --------------------------------------------------------------------
        // SUB-STEP 3A: Compute the "Normal Center" from routine healthy logs
        // --------------------------------------------------------------------
        ConsoleUi.WriteSubStepHeader("Step 3A", "Average 4 healthy baseline logs to compute our 'Normal Center' coordinate:");

        string[] healthyBaselineLogs =
        [
            "INFO Internal CI/CD pipeline build #482 deployed to staging kubernetes cluster",
            "INFO Docker container healthcheck passed on internal dev telemetry node",
            "INFO Automated linter and unit test suite completed in CI/CD pipeline",
            "INFO Staging kubernetes deployment rollout finished for internal service"
        ];

        var baselineEmbeddings = await embedder.GenerateAsync(healthyBaselineLogs);
        int dims = baselineEmbeddings[0].Vector.Length;

        float[] normalCenter = new float[dims];
        foreach (var emb in baselineEmbeddings)
        {
            TensorPrimitives.Add(normalCenter.AsSpan(), emb.Vector.Span, normalCenter.AsSpan());
        }
        TensorPrimitives.Divide(normalCenter.AsSpan(), TensorPrimitives.Norm(normalCenter.AsSpan()), normalCenter.AsSpan());

        foreach (var log in healthyBaselineLogs)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"    • {log}");
        }
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"    => Computed Normal Center Vector: {ConsoleUi.FormatVectorPreview(normalCenter, 4)}");
        Console.ResetColor();

        ConsoleUi.PauseForSubStep("stream live logs and check their distance from Normal Center", autoRunAll);

        // --------------------------------------------------------------------
        // SUB-STEP 3B: Stream live logs and check distance from Normal Center
        // --------------------------------------------------------------------
        ConsoleUi.WriteSubStepHeader("Step 3B", "Streaming 4 live logs (Anything with Distance > 0.55 triggers an alert!):");

        string[] liveLogStream =
        [
            "INFO Internal docker build pipeline deployed to staging k8s node",
            "INFO CI/CD test suite and linter passed on internal deployment",
            "CRITICAL Urgent production database outage! Customer checkout payment gateway 502 failing!",
            "ALERT Security breach: unauthorized MFA authentication token reset attempt on locked account"
        ];

        var liveEmbeddings = await embedder.GenerateAsync(liveLogStream);

        foreach (var (logLine, emb) in liveLogStream.Zip(liveEmbeddings))
        {
            // 🔴 BREAKPOINT TIP: Inspect `anomalyDistance` here!
            float similarityToNormal = TensorPrimitives.CosineSimilarity(normalCenter.AsSpan(), emb.Vector.Span);
            float anomalyDistance = 1.0f - similarityToNormal;
            bool isAnomaly = anomalyDistance > 0.55f;

            Console.ForegroundColor = isAnomaly ? ConsoleColor.Red : ConsoleColor.Green;
            Console.Write(isAnomaly ? "    [🔴 ANOMALY ALERT] " : "    [🟢 NORMAL LOG   ] ");
            Console.ResetColor();
            Console.Write($"Distance={anomalyDistance:F2} | ");
            Console.WriteLine(logLine);
        }
    }
}
