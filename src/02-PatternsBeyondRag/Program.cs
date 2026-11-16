using System.Diagnostics;
using System.Numerics.Tensors;
using Embeddings.Core;
using Microsoft.Extensions.AI;

// ============================================================================
// ACT 4 DEMO: Five Architectural Patterns Beyond Chatbots & RAG
// ============================================================================
// Demonstrates deterministic, sub-millisecond backend patterns using vectors
// as a core CS data structure—zero LLM text generation required!
//
// Usage:
//   dotnet run --project src/02-PatternsBeyondRag
//   dotnet run --project src/02-PatternsBeyondRag -- --pattern 1
// ============================================================================

ConsoleUi.WriteBanner(
    "ACT 4: FIVE HIGH-LEVERAGE PATTERNS BEYOND RAG",
    "Using float[] Embeddings as Fast, Deterministic Backend Building Blocks");

var (embedder, engineInfo) = await EmbeddingGeneratorFactory.CreateAsync(args);
Console.WriteLine($"  Engine : {engineInfo}");

await RunPattern1_ZeroShotRoutingAsync(embedder);
await RunPattern2_SemanticCacheAndDedupAsync(embedder);
await RunPattern3_LogAnomalyDetectionAsync(embedder);
await RunPattern4_StatelessRecommendationsAsync(embedder);
await RunPattern5_MultimodalSharedSpaceAsync(embedder);

Console.WriteLine();
ConsoleUi.WriteCallout(
    "Key Architectural Takeaway",
    "None of these 5 patterns called an LLM chat completion endpoint. Every decision ran in <1ms using float[] + TensorPrimitives!",
    ConsoleColor.Green);

// ============================================================================
// PATTERN 1: Zero-Shot Routing & Classification
// ============================================================================
static async Task RunPattern1_ZeroShotRoutingAsync(IEmbeddingGenerator<string, Embedding<float>> embedder)
{
    ConsoleUi.WriteSectionHeader(
        "PATTERN 1: Zero-Shot Ticket Routing & Classification",
        "Define anchor descriptions once at startup. Route incoming webhooks in microseconds via Dot Product.");

    var routeDefinitions = new (string QueueName, string AnchorDescription)[]
    {
        ("BILLING-TEAM",  "Credit card declined, payment gateway 502 checkout failure, invoice VAT tax rate, refund, subscription charge"),
        ("SECURITY-IAM",  "Two-factor authentication MFA 2FA reset, locked account, SSO OAuth token, password login credentials"),
        ("CLIENT-MOBILE", "Mobile app crashes when uploading profile picture avatar photo, screen flickering, UI modal freezes"),
        ("INFRA-SRE",     "Production database connection pool exhausted, SQL query timeout, deadlock, memory OOM outage"),
        ("PRODUCT-IDEAS", "Feature request, add dark mode theme support, export CSV webhook integration enhancement")
    };

    var anchorVectors = await embedder.GenerateAsync(routeDefinitions.Select(r => r.AnchorDescription));

    string[] incomingWebhooks =
    [
        "Customer says their Visa card keeps getting rejected on the checkout page",
        "Lost my phone and cannot get past the 6-digit authenticator code screen",
        "Every time I pick a PNG for my user avatar the iOS app hangs",
        "Would love a way to automatically push weekly reports to a Slack webhook",
        "Primary Postgres replica is rejecting connections with pool timeout"
    ];

    var sw = Stopwatch.StartNew();
    var webhookVectors = await embedder.GenerateAsync(incomingWebhooks);

    foreach (var (webhookText, webhookEmbedding) in incomingWebhooks.Zip(webhookVectors))
    {
        // Since vectors are L2-normalized to unit length (1.0), Dot Product == Cosine Similarity!
        var bestRoute = routeDefinitions
            .Zip(anchorVectors)
            .Select(anchor => (
                Queue: anchor.First.QueueName,
                Score: TensorPrimitives.Dot(webhookEmbedding.Vector.Span, anchor.Second.Vector.Span)
            ))
            .OrderByDescending(x => x.Score)
            .First();

        Console.Write("  ");
        ConsoleUi.WriteScoreBar(bestRoute.Score, 14);
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write($" -> [{bestRoute.Queue,-13}] ");
        Console.ResetColor();
        Console.WriteLine($"\"{webhookText}\"");
    }
    sw.Stop();

    ConsoleUi.WriteCallout(
        "Latency",
        $"Classified {incomingWebhooks.Length} incoming tickets in {sw.Elapsed.TotalMilliseconds:F2} ms total — zero custom model training!",
        ConsoleColor.Cyan);
}

// ============================================================================
// PATTERN 2: Semantic Caching & Duplicate Bug Detection
// ============================================================================
static async Task RunPattern2_SemanticCacheAndDedupAsync(IEmbeddingGenerator<string, Embedding<float>> embedder)
{
    ConsoleUi.WriteSectionHeader(
        "PATTERN 2: Semantic Caching & Duplicate Detection",
        "Check if CosineSimilarity >= threshold before hitting a $0.03 LLM API or creating a duplicate Jira issue.");

    const float DuplicateThreshold = 0.78f;
    var cache = new List<(string PromptOrIssue, ReadOnlyMemory<float> Vector, string CachedResponse)>();

    var incomingStream = new (string Input, string MockExpensiveComputation)[]
    {
        ("Payment gateway returns 502 error at checkout", "Jira #1042: Investigate Stripe/Gateway 502 at checkout"),
        ("Checkout credit card payment fails with 502 gateway error", "Would be duplicate of #1042!"),
        ("How do I reset my two-factor authentication?", "Jira #1043: User MFA reset self-service flow"),
        ("How can I reset my 2FA two-factor authentication login?", "Would be duplicate of #1043!"),
        ("Invoice PDF shows the wrong VAT tax rate for EU customers", "Jira #1044: Fix EU VAT calculation on PDF invoices")
    };

    foreach (var (input, computation) in incomingStream)
    {
        var vector = await embedder.GenerateVectorAsync(input);

        var match = cache
            .Select(entry => (
                Entry: entry,
                Score: TensorPrimitives.CosineSimilarity(vector.Span, entry.Vector.Span)
            ))
            .OrderByDescending(x => x.Score)
            .FirstOrDefault();

        if (cache.Count > 0 && match.Score >= DuplicateThreshold)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"  [CACHE HIT  {match.Score:P0}] ");
            Console.ResetColor();
            Console.WriteLine($"\"{input}\"");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"                   -> Matched existing: \"{match.Entry.PromptOrIssue}\" ({match.Entry.CachedResponse})");
            Console.ResetColor();
        }
        else
        {
            cache.Add((input, vector, computation));
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"  [NEW ENTRY  { (cache.Count == 1 ? 0f : match.Score):P0}] ");
            Console.ResetColor();
            Console.WriteLine($"\"{input}\" -> Stored ({computation})");
        }
    }
}

// ============================================================================
// PATTERN 3: Anomaly & Drift Detection in Production Logs
// ============================================================================
static async Task RunPattern3_LogAnomalyDetectionAsync(IEmbeddingGenerator<string, Embedding<float>> embedder)
{
    ConsoleUi.WriteSectionHeader(
        "PATTERN 3: Anomaly & Drift Detection (Outlier Detection in Meaning Space)",
        "Compute the centroid of normal telemetry/logs. Flag anything falling far outside the normal cluster.");

    string[] baselineNormalLogs =
    [
        "INFO Internal CI/CD pipeline build #482 deployed to staging kubernetes cluster",
        "INFO Docker container healthcheck passed on internal dev telemetry node",
        "INFO Automated linter and unit test suite completed in CI/CD pipeline",
        "INFO Staging kubernetes deployment rollout finished for internal service"
    ];

    var baselineEmbeddings = await embedder.GenerateAsync(baselineNormalLogs);
    int dims = baselineEmbeddings[0].Vector.Length;

    // Compute the cluster centroid (mean vector of normal logs, then L2 normalize)
    float[] centroid = new float[dims];
    foreach (var emb in baselineEmbeddings)
    {
        TensorPrimitives.Add(centroid.AsSpan(), emb.Vector.Span, centroid.AsSpan());
    }
    float norm = TensorPrimitives.Norm(centroid.AsSpan());
    TensorPrimitives.Divide(centroid.AsSpan(), norm, centroid.AsSpan());

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
        float similarityToNormal = TensorPrimitives.CosineSimilarity(centroid.AsSpan(), emb.Vector.Span);
        float anomalyDistance = 1.0f - similarityToNormal;
        bool isAnomaly = anomalyDistance > 0.55f;

        Console.ForegroundColor = isAnomaly ? ConsoleColor.Red : ConsoleColor.Green;
        Console.Write(isAnomaly ? "  [ANOMALY ALERT] " : "  [NORMAL LOG   ] ");
        Console.ResetColor();
        Console.Write($"Distance={anomalyDistance:F2} (Sim={similarityToNormal:P0}) | ");
        Console.WriteLine(logLine);
    }
}

// ============================================================================
// PATTERN 4: Stateless Recommendation Engine ("More Like This")
// ============================================================================
static async Task RunPattern4_StatelessRecommendationsAsync(IEmbeddingGenerator<string, Embedding<float>> embedder)
{
    ConsoleUi.WriteSectionHeader(
        "PATTERN 4: Stateless Recommendation Engine ('Users Who Read X')",
        "Represent a user's taste profile by averaging the vectors of the last N items they interacted with.");

    string[] userRecentlyRead =
    [
        "Building high-performance SIMD vector math in C# and .NET with TensorPrimitives",
        "Running local ONNX embedding models in .NET microservices without cloud APIs",
        "How semantic search and vector embeddings replace brittle SQL LIKE queries"
    ];

    string[] catalogArticles =
    [
        "Deep dive into Matryoshka vector quantization and hybrid search in C# .NET",
        "Async/Await and Span<T> memory optimization tricks for C# backend engineers",
        "Configuring EU VAT tax rates on PDF billing receipts and Stripe checkout",
        "Resetting two-factor authentication MFA passkeys when a user loses their phone",
        "Understanding transformer attention layers and cosine similarity for developers"
    ];

    var historyVectors = await embedder.GenerateAsync(userRecentlyRead);
    var catalogVectors = await embedder.GenerateAsync(catalogArticles);

    // Compute User Taste Vector = Normalized Average of History Vectors
    int dims = historyVectors[0].Vector.Length;
    float[] userTasteVector = new float[dims];
    foreach (var h in historyVectors)
    {
        TensorPrimitives.Add(userTasteVector.AsSpan(), h.Vector.Span, userTasteVector.AsSpan());
    }
    TensorPrimitives.Divide(userTasteVector.AsSpan(), TensorPrimitives.Norm(userTasteVector.AsSpan()), userTasteVector.AsSpan());

    Console.ForegroundColor = ConsoleColor.Gray;
    Console.WriteLine("  User Reading History (3 items averaged into 1 User Taste Vector):");
    foreach (var item in userRecentlyRead)
    {
        Console.WriteLine($"    • {item}");
    }
    Console.ResetColor();
    Console.WriteLine("\n  Recommended Next Reads (Ranked by Cosine Similarity to User Taste Vector):");

    var recommendations = catalogArticles
        .Zip(catalogVectors)
        .Select(c => (
            Title: c.First,
            Score: TensorPrimitives.CosineSimilarity(userTasteVector.AsSpan(), c.Second.Vector.Span)
        ))
        .OrderByDescending(x => x.Score);

    foreach (var rec in recommendations)
    {
        Console.Write("    ");
        ConsoleUi.WriteScoreBar(rec.Score, 16);
        Console.WriteLine($"  {rec.Title}");
    }
}

// ============================================================================
// PATTERN 5: Cross-Modal Shared Space (Text <-> Image <-> Audio)
// ============================================================================
static async Task RunPattern5_MultimodalSharedSpaceAsync(IEmbeddingGenerator<string, Embedding<float>> embedder)
{
    ConsoleUi.WriteSectionHeader(
        "PATTERN 5: Cross-Modal Search (Shared Coordinate Space + The Modality Gap)",
        "Multimodal models (CLIP, SigLIP, Gemini Embedding 2) map Text, Image patches, and Audio spectrograms into ONE vector space.");

    // In a multimodal model, the Image Encoder, Audio Encoder, and Text Encoder all output float[N] in the same space.
    var untaggedMediaVault = new (string MediaId, string Modality, string RawContentRepresentation)[]
    {
        ("IMG_0042.jpg",  "IMAGE (16x16 ViT Patches)",   "visual photo of a golden retriever dog puppy running in the park"),
        ("AUD_9910.wav",  "AUDIO (Spectrogram Patches)", "audio clip of a canine dog barking loudly outdoors"),
        ("IMG_0108.png",  "IMAGE (16x16 ViT Patches)",   "screenshot of mobile app crash when uploading user profile picture avatar"),
        ("DOC_arch.pdf",  "PDF   (Visual + Text)",       "architecture diagram flowchart with red arrows showing database failover"),
        ("IMG_0299.png",  "IMAGE (16x16 ViT Patches)",   "checkout payment modal freeze showing 502 credit card error")
    };

    var mediaVectors = await embedder.GenerateAsync(untaggedMediaVault.Select(m => m.RawContentRepresentation));

    string[] naturalLanguageQueries =
    [
        "golden retriever puppy",
        "diagram with red arrows",
        "error toast on mobile profile upload"
    ];

    foreach (var query in naturalLanguageQueries)
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
        Console.WriteLine($"\n  Text Query: \"{query}\"");
        Console.ResetColor();

        foreach (var hit in topMatches)
        {
            Console.Write("    ");
            ConsoleUi.WriteScoreBar(hit.Score, 14);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($" [{hit.Media.Modality,-27}] ");
            Console.ResetColor();
            Console.WriteLine($"{hit.Media.MediaId}");
        }
    }
}
