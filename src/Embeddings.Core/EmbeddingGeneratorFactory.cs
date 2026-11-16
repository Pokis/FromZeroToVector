using ElBruno.LocalEmbeddings;
using Microsoft.Extensions.AI;

namespace Embeddings.Core;

public static class EmbeddingGeneratorFactory
{
    /// <summary>
    /// Creates an <see cref="IEmbeddingGenerator{TInput, TEmbedding}"/> for conference demos.
    /// Pass <c>--onnx</c> on the command line (or set <c>EMBEDDINGS_USE_ONNX=1</c>) to run the full
    /// HuggingFace ONNX transformer via <see cref="LocalEmbeddingGenerator"/> (downloads ~90MB once).
    /// By default, uses the instant zero-network 384-D semantic generator so demos start in &lt;1ms offline.
    /// </summary>
    public static async Task<(IEmbeddingGenerator<string, Embedding<float>> Generator, string EngineDescription)> CreateAsync(
        string[]? args = null,
        bool silent = false)
    {
        bool useOnnx = (args?.Any(a => a.Equals("--onnx", StringComparison.OrdinalIgnoreCase)) ?? false)
                       || string.Equals(Environment.GetEnvironmentVariable("EMBEDDINGS_USE_ONNX"), "1", StringComparison.OrdinalIgnoreCase);

        if (useOnnx)
        {
            try
            {
                if (!silent)
                {
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.WriteLine("  [Engine] Loading local ONNX transformer (ElBruno.LocalEmbeddings / all-MiniLM-L6-v2)...");
                    Console.ResetColor();
                }

                var onnxGenerator = await LocalEmbeddingGenerator.CreateAsync();
                return (onnxGenerator, "Local ONNX (sentence-transformers/all-MiniLM-L6-v2, 384 dims)");
            }
            catch (Exception ex)
            {
                if (!silent)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"  [Notice] ONNX model not cached offline ({ex.GetType().Name}). Falling back to instant 384-D local semantic engine.");
                    Console.ResetColor();
                }
            }
        }

        var instantGenerator = new DeterministicSemanticEmbeddingGenerator();
        return (instantGenerator, "Instant Local Semantic Engine (IEmbeddingGenerator<string, Embedding<float>>, 384 dims — pass --onnx for HuggingFace ONNX)");
    }
}
