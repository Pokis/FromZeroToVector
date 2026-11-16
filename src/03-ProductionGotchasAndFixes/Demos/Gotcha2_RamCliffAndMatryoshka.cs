using System.Numerics;
using System.Numerics.Tensors;
using Embeddings.Core;
using Microsoft.Extensions.AI;

namespace ProductionGotchasAndFixes.Demos;

public static class Gotcha2_RamCliffAndMatryoshka
{
    public static async Task RunAsync(
        IEmbeddingGenerator<string, Embedding<float>> embedder,
        bool autoRunAll = false)
    {
        ConsoleUi.WriteSectionHeader(
            "GOTCHA 2 OF 4: The 120 GB RAM Bill Shock (Fix: Matryoshka Slicing & Quantization)",
            "Two tricks to shrink memory 4x to 32x: 1) Chop array length, 2) Round each number to fewer bits.");

        ConsoleUi.WriteCodeSnippet(
            "Demos/Gotcha2_RamCliffAndMatryoshka.cs",
            "// Trick #1 (Matryoshka Slicing): Keep only the first 128 of 384 numbers!",
            "float[] chopped = fullVector.AsSpan(0, 128).ToArray();",
            "TensorPrimitives.Divide(chopped, TensorPrimitives.Norm(chopped), chopped); // Scale back to length 1.0");

        string query = "credit card declined at payment checkout with 502 error";
        string rightAnswerDoc = "Payment gateway returns 502 error at checkout when charging credit card";
        string wrongAnswerDoc = "Automated docker build pipeline deployed to internal kubernetes cluster";

        var vectors = await embedder.GenerateAsync([query, rightAnswerDoc, wrongAnswerDoc]);
        ReadOnlySpan<float> qFull = vectors[0].Vector.Span;
        ReadOnlySpan<float> rightFull = vectors[1].Vector.Span;
        ReadOnlySpan<float> wrongFull = vectors[2].Vector.Span;

        // --------------------------------------------------------------------
        // SUB-STEP 2A: Matryoshka Slicing (Chopping the tail of the array)
        // --------------------------------------------------------------------
        ConsoleUi.WriteSubStepHeader(
            "Step 2A",
            "Trick #1 — Matryoshka Slicing (Chopping the 384-number array shorter and checking if the Right Answer still wins):");

        Console.WriteLine();
        Console.WriteLine($"    {"Numbers Kept per Doc",-22} | {"Right Answer Score",-18} | {"Wrong Answer Score",-18} | {"Picks Right #1?",-15} | {"RAM for 10M Docs"}");
        Console.WriteLine($"    {new string('-', 22)}-+-{new string('-', 18)}-+-{new string('-', 18)}-+-{new string('-', 15)}-+-{new string('-', 16)}");

        foreach (int keptCount in new[] { 384, 256, 128, 64 })
        {
            // 🔴 BREAKPOINT TIP: Put a breakpoint here to inspect the chopped arrays!
            float[] qSlice = SliceAndNormalize(qFull, keptCount);
            float[] rightSlice = SliceAndNormalize(rightFull, keptCount);
            float[] wrongSlice = SliceAndNormalize(wrongFull, keptCount);

            float rightScore = TensorPrimitives.CosineSimilarity(qSlice, rightSlice);
            float wrongScore = TensorPrimitives.CosineSimilarity(qSlice, wrongSlice);
            bool wins = rightScore > wrongScore + 0.5f;
            double ramGb = (10_000_000.0 * keptCount * sizeof(float)) / (1024 * 1024 * 1024);

            Console.WriteLine(
                $"    {keptCount,3} of 384 ({keptCount * 100 / 384,3}% kept) | {rightScore,16:P1}   | {wrongScore,16:P1}   | {(wins ? "✅ YES" : "❌ NO"),-15} | {ramGb,6:F2} GB");
        }

        ConsoleUi.WriteCallout(
            "What Does '96% Recall Retention (4% Loss)' Mean in Practice?",
            "It does NOT mean answers get 4% corrupted! It means in 96 out of 100 searches, the chopped array returns the EXACT same Top-10 list as the full array (and in 4 borderline searches, #2 and #3 might trade places).",
            ConsoleColor.Cyan);

        ConsoleUi.PauseForSubStep("see Trick #2: Quantization (rounding 32-bit floats to 8-bit ints & 1-bit signs)", autoRunAll);

        // --------------------------------------------------------------------
        // SUB-STEP 2B: Quantization (Rounding each number to fewer bits)
        // --------------------------------------------------------------------
        ConsoleUi.WriteSubStepHeader(
            "Step 2B",
            "Trick #2 — Quantization (Rounding each 4-byte decimal down to a 1-byte whole number or a 1-bit +/- sign):");

        sbyte[] qInt8 = QuantizeToInt8(qFull);
        sbyte[] rightInt8 = QuantizeToInt8(rightFull);
        float int8RightScore = CosineSimilarityInt8(qInt8, rightInt8);

        ulong[] qBin = QuantizeToBinary1Bit(qFull);
        ulong[] rightBin = QuantizeToBinary1Bit(rightFull);
        float binRightScore = BinaryHammingSimilarity(qBin, rightBin, qFull.Length);

        Console.WriteLine();
        Console.WriteLine($"    1. Full 32-bit decimal (float[384]) : Right Answer Score = {TensorPrimitives.CosineSimilarity(qFull, rightFull):P1} | 10M RAM = 14.31 GB (Baseline)");
        Console.WriteLine($"    2. 8-bit whole number  (sbyte[384]) : Right Answer Score = {int8RightScore:P1} | 10M RAM =  3.58 GB (4x Smaller!)");
        Console.WriteLine($"    3. 1-bit +/- sign only (ulong[6])   : Right Answer Score = {binRightScore:P1} | 10M RAM =  0.45 GB (32x Smaller!)");
    }

    private static float[] SliceAndNormalize(ReadOnlySpan<float> fullVector, int targetDims)
    {
        float[] sliced = fullVector[..targetDims].ToArray();
        float norm = TensorPrimitives.Norm(sliced.AsSpan());
        if (norm > 1e-8f)
        {
            TensorPrimitives.Divide(sliced.AsSpan(), norm, sliced.AsSpan());
        }
        return sliced;
    }

    private static sbyte[] QuantizeToInt8(ReadOnlySpan<float> vector)
    {
        float maxAbs = 0f;
        foreach (float v in vector)
        {
            if (MathF.Abs(v) > maxAbs) maxAbs = MathF.Abs(v);
        }
        float scale = maxAbs > 0 ? 127f / maxAbs : 1f;

        var result = new sbyte[vector.Length];
        for (int i = 0; i < vector.Length; i++)
        {
            result[i] = (sbyte)Math.Clamp(MathF.Round(vector[i] * scale), -127f, 127f);
        }
        return result;
    }

    private static float CosineSimilarityInt8(ReadOnlySpan<sbyte> a, ReadOnlySpan<sbyte> b)
    {
        long dot = 0, normA = 0, normB = 0;
        for (int i = 0; i < a.Length; i++)
        {
            dot += (long)a[i] * b[i];
            normA += (long)a[i] * a[i];
            normB += (long)b[i] * b[i];
        }
        return (float)(dot / (Math.Sqrt(normA) * Math.Sqrt(normB) + 1e-8));
    }

    private static ulong[] QuantizeToBinary1Bit(ReadOnlySpan<float> vector)
    {
        int numWords = (vector.Length + 63) / 64;
        var packed = new ulong[numWords];
        for (int i = 0; i < vector.Length; i++)
        {
            if (vector[i] >= 0f)
            {
                packed[i / 64] |= 1UL << (i % 64);
            }
        }
        return packed;
    }

    private static float BinaryHammingSimilarity(ReadOnlySpan<ulong> a, ReadOnlySpan<ulong> b, int totalBits)
    {
        int differingBits = 0;
        for (int i = 0; i < a.Length; i++)
        {
            differingBits += BitOperations.PopCount(a[i] ^ b[i]);
        }
        return 1.0f - ((float)differingBits / totalBits);
    }
}
