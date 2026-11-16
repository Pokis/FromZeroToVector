namespace Embeddings.Core;

public static class ConsoleUi
{
    public static void WriteBanner(string title, string subtitle)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╔════════════════════════════════════════════════════════════════════════════════════════╗");
        Console.WriteLine($"║  {title.PadRight(86)}║");
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine($"║  {subtitle.PadRight(86)}║");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╚════════════════════════════════════════════════════════════════════════════════════════╝");
        Console.ResetColor();
    }

    public static void WriteSectionHeader(string title, string? description = null)
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"━━━ {title} ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"[..Math.Min(90, title.Length + 75)]);
        if (!string.IsNullOrWhiteSpace(description))
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine($"    {description}");
        }
        Console.ResetColor();
        Console.WriteLine();
    }

    public static void WriteSubStepHeader(string stepLabel, string explanation)
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write($"  [{stepLabel}] ");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(explanation);
        Console.ResetColor();
    }

    public static void WriteCodeSnippet(string caption, params string[] lines)
    {
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine($"  ┌── C# Code: {caption} ──────────────────────────────────────────────────");
        foreach (var line in lines)
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write("  │ ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(line);
        }
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine("  └──────────────────────────────────────────────────────────────────────────────");
        Console.ResetColor();
    }

    public static bool WaitForNextStep(string nextStepPrompt, bool autoRunAll = false)
    {
        if (autoRunAll || Console.IsInputRedirected)
        {
            return true;
        }

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.Write($"  ▶ Press [Enter] for {nextStepPrompt} (or 'q' to quit)... ");
        Console.ResetColor();
        var key = Console.ReadLine();
        if (key is not null && key.Trim().Equals("q", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }
        return true;
    }

    public static void PauseForSubStep(string whatHappensNext, bool autoRunAll = false)
    {
        if (autoRunAll || Console.IsInputRedirected)
        {
            return;
        }

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write($"\n    [Press Enter to {whatHappensNext}...] ");
        Console.ResetColor();
        Console.ReadLine();
    }

    public static void WriteScoreBar(float score, int width = 20)
    {
        float clamped = Math.Clamp(score, 0f, 1f);
        int filled = (int)Math.Round(clamped * width);
        string bar = new string('█', filled) + new string('░', Math.Max(0, width - filled));

        Console.ForegroundColor = score switch
        {
            >= 0.72f => ConsoleColor.Green,
            >= 0.48f => ConsoleColor.Yellow,
            _ => ConsoleColor.DarkGray
        };

        Console.Write($"[{bar}] {score,6:P1}");
        Console.ResetColor();
    }

    public static string FormatVectorPreview(ReadOnlySpan<float> vector, int previewCount = 5)
    {
        int count = Math.Min(previewCount, vector.Length);
        var parts = new string[count];
        for (int i = 0; i < count; i++)
        {
            parts[i] = $"{vector[i]:+0.000;-0.000;0.000}";
        }
        return $"[ {string.Join(", ", parts)}, ... ({vector.Length} numbers) ]";
    }

    public static void WriteCallout(string label, string message, ConsoleColor color = ConsoleColor.Magenta)
    {
        Console.WriteLine();
        Console.ForegroundColor = color;
        Console.Write($"  ★ {label}: ");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(message);
        Console.ResetColor();
    }
}
