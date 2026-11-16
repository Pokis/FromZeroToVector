using System.Numerics.Tensors;
using Embeddings.Core;
using Microsoft.Extensions.AI;

namespace PatternsBeyondRag.Demos;

public static class Demo5_MultimodalSearch
{
    public static async Task RunAsync(
        IEmbeddingGenerator<string, Embedding<float>> embedder,
        bool autoRunAll = false)
    {
        ConsoleUi.WriteSectionHeader(
            "DEMO 5 OF 5: Searching Untagged Photos, Audio & PDFs with Text",
            "Multimodal models (CLIP, SigLIP, Gemini Embedding 2) map Text, Images, and Audio into ONE float[] space.");

        ConsoleUi.WriteCodeSnippet(
            "Demos/Demo5_MultimodalSearch.cs",
            "// Image (16x16 pixel squares), Audio (Spectrogram squares), and Text all become float[384]!",
            "// Your vector search code doesn't care what file type the float[] came from:",
            "float score = TensorPrimitives.CosineSimilarity(textQueryVec.Span, mediaFileVec.Span);");

        // --------------------------------------------------------------------
        // SUB-STEP 5A: Show the untagged media files in our vault
        // --------------------------------------------------------------------
        ConsoleUi.WriteSubStepHeader("Step 5A", "Our vault has 4 untagged media files (already converted to float[384] coordinates):");

        var untaggedMediaVault = new (string FileName, string HowItWasConstructed, string UnderlyingMeaning)[]
        {
            ("IMG_0042.jpg",  "🖼️ Photo (sliced into 16x16 RGB squares)",      "visual photo of a golden retriever dog puppy running in the park"),
            ("AUD_9910.wav",  "🔊 Audio (plotted as spectrogram & sliced)",    "audio clip of a canine dog barking loudly outdoors"),
            ("IMG_0108.png",  "🖼️ Screenshot (sliced into 16x16 RGB squares)", "screenshot of mobile app crash when uploading user profile picture avatar"),
            ("DOC_arch.pdf",  "📄 PDF Page (visual diagram patches + text)",   "architecture diagram flowchart with red arrows showing database failover")
        };

        var mediaVectors = await embedder.GenerateAsync(untaggedMediaVault.Select(m => m.UnderlyingMeaning));

        for (int i = 0; i < untaggedMediaVault.Length; i++)
        {
            var m = untaggedMediaVault[i];
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"    • {m.FileName,-14} ");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write($"{m.HowItWasConstructed,-45} ");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(ConsoleUi.FormatVectorPreview(mediaVectors[i].Vector.Span, 2));
            Console.ResetColor();
        }

        ConsoleUi.PauseForSubStep("search these untagged media files using plain English text", autoRunAll);

        // --------------------------------------------------------------------
        // SUB-STEP 5B: Search the untagged media files with plain text
        // --------------------------------------------------------------------
        ConsoleUi.WriteSubStepHeader("Step 5B", "Searching untagged photos, audio, and PDFs with English text:");

        string[] textQueries =
        [
            "golden retriever puppy",
            "diagram with red arrows"
        ];

        foreach (var query in textQueries)
        {
            var queryVec = await embedder.GenerateVectorAsync(query);

            var topMatches = untaggedMediaVault
                .Zip(mediaVectors)
                .Select(m => (
                    Media: m.First,
                    Score: TensorPrimitives.CosineSimilarity(queryVec.Span, m.Second.Vector.Span)
                ))
                .OrderByDescending(x => x.Score)
                .Take(2)
                .ToList();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\n    Text Search: \"{query}\"");
            Console.ResetColor();

            foreach (var hit in topMatches)
            {
                Console.Write("      ");
                ConsoleUi.WriteScoreBar(hit.Score, 14);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($" -> {hit.Media.FileName,-13} ");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine($"({hit.Media.HowItWasConstructed})");
                Console.ResetColor();
            }
        }
    }
}
