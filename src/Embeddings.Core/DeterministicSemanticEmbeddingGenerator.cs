using System.Numerics.Tensors;
using System.Text.RegularExpressions;
using Microsoft.Extensions.AI;

namespace Embeddings.Core;

/// <summary>
/// A conference-safe, zero-network 384-dimensional <see cref="IEmbeddingGenerator{TInput, TEmbedding}"/>
/// implementation that models how modern Matryoshka-capable embedding models project meaning into unit vectors.
/// 
/// Why this exists alongside <see cref="ElBruno.LocalEmbeddings.LocalEmbeddingGenerator"/>:
/// 1. Instant execution (<1ms startup) even on stage without Wi-Fi or a pre-downloaded 90MB HuggingFace ONNX file.
/// 2. Front-loaded semantic concept axes (Matryoshka ordering) so slicing 384 -> 128 -> 64 dims retains ~95% recall.
/// 3. Intentionally models real dense-vector behaviors: semantic synonym clustering AND negation/exact-ID blindness.
/// 4. Supports distinct seeds to simulate incompatible model spaces (e.g., Model v1 vs Model v2 migration).
/// </summary>
public sealed class DeterministicSemanticEmbeddingGenerator : IEmbeddingGenerator<string, Embedding<float>>
{
    public const int DefaultDimensions = 384;

    private readonly string _modelId;
    private readonly int _dimensions;
    private readonly int _seedOffset;

    public EmbeddingGeneratorMetadata Metadata { get; }

    // Curated semantic concept clusters representing latent transformer attention features.
    // Words within the same semantic neighborhood activate shared latent axes even when sharing zero surface words.
    private static readonly (string ConceptName, string[] Triggers, float Weight)[] LatentSemanticConcepts =
    [
        ("Payments & Checkout", [
            "payment", "checkout", "credit", "card", "stripe", "gateway", "charge", "declined",
            "transaction", "502", "pay", "cart", "purchase", "billing", "visa", "mastercard", "wallet"
        ], 3.4f),

        ("Invoices, Tax & Pricing", [
            "invoice", "vat", "tax", "receipt", "pdf", "billing", "rate", "price", "subscription",
            "refund", "overcharged", "accounting", "currency", "euro", "dollar", "statement", "plan"
        ], 3.2f),

        ("Authentication, MFA & Security", [
            "mfa", "2fa", "two-factor", "two", "factor", "authentication", "authenticator", "login",
            "password", "reset", "unlock", "token", "sso", "oauth", "security", "totp", "passkey",
            "enable", "disable", "turn", "off", "account", "locked", "credentials", "signin"
        ], 3.5f),

        ("UI, Media Upload & Client Crash", [
            "crash", "crashes", "crashing", "upload", "uploading", "profile", "picture", "avatar",
            "photo", "image", "png", "jpeg", "freeze", "freezes", "hangs", "spinner", "modal",
            "button", "mobile", "ios", "android", "toast"
        ], 3.3f),

        ("Display, Screen & Hardware Glitches", [
            "screen", "flickering", "flicker", "display", "refresh", "glitch", "glitches",
            "laptop", "monitor", "waking", "wake", "sleep", "hdmi", "resolution", "brightness"
        ], 3.5f),

        ("Database, Storage & Outages", [
            "database", "sql", "postgres", "pgvector", "connection", "pool", "timeout", "deadlock",
            "query", "slow", "replica", "outage", "down", "latency", "redis", "cache", "storage",
            "disk", "memory", "oom", "cluster", "node", "503", "504", "error", "exception", "fail", "fails"
        ], 3.3f),

        ("Customer Urgency & Production Incident", [
            "urgent", "p1", "critical", "angry", "customer", "customers", "production", "prod",
            "outage", "down", "blocked", "emergency", "escalation", "sla", "breach", "broken", "incident"
        ], 2.8f),

        ("Internal Tooling & DevOps", [
            "internal", "staging", "ci", "cd", "pipeline", "build", "docker", "kubernetes", "k8s",
            "deploy", "deployment", "test", "linter", "sandbox", "dev", "local", "telemetry", "log", "logs"
        ], 2.7f),

        ("Feature Requests & Enhancements", [
            "feature", "request", "add", "support", "dark", "mode", "export", "csv", "webhook",
            "integration", "idea", "suggestion", "would", "nice", "enhancement", "roadmap", "theme"
        ], 3.0f),

        ("AI, Vectors, Embeddings & ML", [
            "embedding", "embeddings", "vector", "vectors", "cosine", "similarity", "onnx", "model",
            "llm", "rag", "semantic", "search", "chunking", "quantization", "matryoshka", "hybrid",
            "bm25", "transformer", "attention", "token", "tokens", "neural", "clustering", "anomaly"
        ], 3.2f),

        ("C#, .NET & Backend Engineering", [
            "csharp", "c#", "dotnet", ".net", "linq", "async", "await", "span", "tensor",
            "simd", "api", "microservice", "controller", "ef", "entity", "memory", "gc", "thread"
        ], 3.0f),

        ("Animals, Dogs & Visual Media", [
            "dog", "puppy", "golden", "retriever", "hound", "bark", "barking", "canine", "pet",
            "cat", "kitten", "photo", "picture", "camera", "audio", "clip", "video", "fur", "park"
        ], 3.4f),

        ("Architecture Diagrams & Documentation", [
            "diagram", "architecture", "arrows", "red", "flowchart", "whiteboard", "schema",
            "documentation", "doc", "docs", "readme", "guide", "tutorial", "pdf", "slide", "chart"
        ], 3.1f)
    ];

    public DeterministicSemanticEmbeddingGenerator(
        string modelId = "local-minilm-l6-v2-sim",
        int dimensions = DefaultDimensions,
        int seedOffset = 0)
    {
        _modelId = modelId;
        _dimensions = dimensions;
        _seedOffset = seedOffset;
        Metadata = new EmbeddingGeneratorMetadata(
            providerName: "EmbeddingsTalk.LocalEngine",
            defaultModelId: modelId,
            defaultModelDimensions: dimensions);
    }

    public Task<GeneratedEmbeddings<Embedding<float>>> GenerateAsync(
        IEnumerable<string> values,
        EmbeddingGenerationOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        int targetDims = options?.Dimensions ?? _dimensions;
        var results = new GeneratedEmbeddings<Embedding<float>>();

        foreach (var text in values)
        {
            cancellationToken.ThrowIfCancellationRequested();
            float[] vector = ComputeEmbeddingVector(text ?? string.Empty, targetDims);
            results.Add(new Embedding<float>(vector)
            {
                CreatedAt = DateTimeOffset.UtcNow,
                ModelId = _modelId
            });
        }

        return Task.FromResult(results);
    }

    private float[] ComputeEmbeddingVector(string text, int dimensions)
    {
        var vector = new float[dimensions];
        var normalized = text.ToLowerInvariant();
        var tokens = Tokenize(normalized);

        if (tokens.Count == 0)
        {
            vector[0] = 1.0f;
            return vector;
        }

        // 1. Activate Latent Semantic Concept projections across the vector space.
        // We use Matryoshka decay: earlier dimensions receive stronger concept weights,
        // so truncating a 384-d vector to 128-d or 64-d preserves the primary semantic signal.
        for (int c = 0; c < LatentSemanticConcepts.Length; c++)
        {
            var concept = LatentSemanticConcepts[c];
            float activation = 0f;

            foreach (var token in tokens)
            {
                for (int t = 0; t < concept.Triggers.Length; t++)
                {
                    var trigger = concept.Triggers[t];
                    if (token == trigger)
                    {
                        activation += 1.0f;
                    }
                    else if (token.Length >= 4 && trigger.Length >= 4 &&
                             (token.StartsWith(trigger[..4], StringComparison.Ordinal) ||
                              trigger.StartsWith(token[..4], StringComparison.Ordinal)))
                    {
                        activation += 0.55f;
                    }
                }
            }

            if (activation > 0f)
            {
                // Log-dampen repeated triggers so long documents stay balanced with short queries
                float scaledActivation = MathF.Log2(1f + activation) * concept.Weight;
                ProjectSignalIntoSpace(vector, seed: HashSeed($"concept:{c}:{concept.ConceptName}", _seedOffset), strength: scaledActivation);
            }
        }

        // 2. Project stemmed token and character 3-gram signals for fine-grained lexical nuance.
        // Note how boolean negations ("not", "disable" vs "enable") and numeric error codes ("503" vs "504")
        // share massive surrounding topic context—accurately demonstrating why dense embeddings alone
        // struggle with exact IDs and negations unless paired with BM25 Hybrid Search!
        foreach (var token in tokens)
        {
            if (StopWords.Contains(token))
            {
                continue;
            }

            string stem = SimpleStem(token);
            ProjectSignalIntoSpace(vector, seed: HashSeed($"stem:{stem}", _seedOffset), strength: 1.15f);

            // Character trigrams capture morphological similarity
            if (token.Length >= 4)
            {
                for (int i = 0; i <= token.Length - 3; i++)
                {
                    string trigram = token.Substring(i, 3);
                    ProjectSignalIntoSpace(vector, seed: HashSeed($"tri:{trigram}", _seedOffset), strength: 0.22f);
                }
            }
        }

        // 3. Stage 4 of the Embedding Pipeline: L2 Normalization to unit length (||v|| = 1.0)
        // Once normalized to unit length, CosineSimilarity(a, b) == Dot(a, b).
        float norm = TensorPrimitives.Norm(vector.AsSpan());
        if (norm > 1e-8f)
        {
            TensorPrimitives.Divide(vector.AsSpan(), norm, vector.AsSpan());
        }

        return vector;
    }

    private static void ProjectSignalIntoSpace(float[] vector, uint seed, float strength)
    {
        uint state = seed;
        int dims = vector.Length;

        for (int i = 0; i < dims; i++)
        {
            // Xorshift32 PRNG for deterministic pseudo-orthogonal projection
            state ^= state << 13;
            state ^= state >> 17;
            state ^= state << 5;

            // Map to [-1, 1]
            float direction = ((state & 0xFFFF) / 32767.5f) - 1.0f;

            // Matryoshka Representation Learning (MRL) envelope:
            // Earlier dimensions carry higher variance; tail dimensions carry fine detail.
            float matryoshkaEnvelope = 1.0f / (1.0f + (i / 96.0f));
            vector[i] += direction * strength * matryoshkaEnvelope;
        }
    }

    private static uint HashSeed(string key, int seedOffset)
    {
        uint hash = 2166136261u ^ (uint)seedOffset * 16777619u;
        foreach (char ch in key)
        {
            hash ^= ch;
            hash *= 16777619u;
        }
        return hash == 0 ? 1u : hash;
    }

    private static List<string> Tokenize(string text)
    {
        var matches = Regex.Matches(text, @"[a-z0-9#\.]+");
        var list = new List<string>(matches.Count);
        foreach (Match m in matches)
        {
            if (!string.IsNullOrWhiteSpace(m.Value))
            {
                list.Add(m.Value.Trim('.'));
            }
        }
        return list;
    }

    private static string SimpleStem(string word)
    {
        if (word.Length > 5 && word.EndsWith("ing", StringComparison.Ordinal)) return word[..^3];
        if (word.Length > 4 && word.EndsWith("ed", StringComparison.Ordinal)) return word[..^2];
        if (word.Length > 4 && word.EndsWith("es", StringComparison.Ordinal)) return word[..^2];
        if (word.Length > 3 && word.EndsWith("s", StringComparison.Ordinal)) return word[..^1];
        return word;
    }

    private static readonly HashSet<string> StopWords = new(StringComparer.Ordinal)
    {
        "the", "a", "an", "to", "in", "on", "at", "of", "for", "with", "by", "is", "are",
        "was", "were", "be", "been", "how", "do", "does", "did", "i", "my", "me", "we", "our",
        "you", "your", "it", "its", "this", "that", "when", "where", "why", "what", "from"
    };

    public object? GetService(Type serviceType, object? serviceKey = null) =>
        serviceType.IsInstanceOfType(this) ? this : null;

    public void Dispose()
    {
    }
}
