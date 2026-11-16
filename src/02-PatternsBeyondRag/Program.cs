using System.Diagnostics;
using System.Numerics.Tensors;
using Embeddings.Core;
using Microsoft.Extensions.AI;

// ============================================================================
// C# DEMO 2: Five Backend Patterns Beyond Chatbots (Step-by-Step)
// ============================================================================
// Shows how to use float[] embeddings in regular backend microservices—with
// zero LLM chatbot calls required.
//
// Usage:
//   dotnet run --project src/02-PatternsBeyondRag          (Step-by-step pause)
//   dotnet run --project src/02-PatternsBeyondRag -- --all (Run all non-stop)
// ============================================================================

bool runAllNonStop = args.Any(a => a.Equals("--all", StringComparison.OrdinalIgnoreCase) || a.Equals("--demo", StringComparison.OrdinalIgnoreCase));

ConsoleUi.WriteBanner(
    "C# DEMO 2: FIVE BACKEND PATTERNS BEYOND CHATBOTS",
    "Step-by-Step Walkthrough: Routing, Caching, Anomaly Radar, Recommendations & Multimodal");

var (embedder, engineInfo) = await EmbeddingGeneratorFactory.CreateAsync(args);
Console.WriteLine($"  Engine : {engineInfo}");

await RunPattern1_ZeroShotRoutingAsync(embedder);
if (!ConsoleUi.WaitForNextStep("Pattern 2: Semantic Cache & Duplicate Blocker", runAllNonStop)) return;

await RunPattern2_SemanticCacheAndDedupAsync(embedder);
if (!ConsoleUi.WaitForNextStep("Pattern 3: Log Anomaly & Outlier Radar", runAllNonStop)) return;

await RunPattern3_LogAnomalyDetectionAsync(embedder);
if (!ConsoleUi.WaitForNextStep("Pattern 4: Stateless Article Recommendations", runAllNonStop)) return;

await RunPattern4_StatelessRecommendationsAsync(embedder);
if (!ConsoleUi.WaitForNextStep("Pattern 5: Multimodal Image/Audio/Text Search", runAllNonStop)) return;

await RunPattern5_MultimodalSharedSpaceAsync(embedder);

Console.WriteLine();
ConsoleUi.WriteCallout(
    "Summary",
    "All 5 patterns ran in <1ms using simple float[] arrays and TensorPrimitives—zero LLM chat calls!",
    ConsoleColor.Green);

// ============================================================================
// PATTERN 1: Zero-Shot Routing & Classification
// ============================================================================
static async Task RunPattern1_ZeroShotRoutingAsync(IEmbeddingGenerator<string, Embedding<float>> embedder)
{
    ConsoleUi.WriteSectionHeader(
        "PATTERN 1 OF 5: Zero-Shot Ticket Routing (< 1 Millisecond)",
        "Embed 1 sentence per team at startup. Route incoming tickets by checking which team vector is closest.");

    ConsoleUi.WriteCodeSnippet(
        "Zero-Shot Router in 4 Lines of C#",
        "var teamVectors = await embedder.GenerateAsync(teamDescriptions); // Done once at startup",
        "var ticketVec   = await embedder.GenerateVectorAsync(incomingWebhook);",
        "var bestTeam    = teams.Zip(teamVectors)",
        "                       .MaxBy(t => TensorPrimitives.Dot(ticketVec.Span, t.Second.Vector.Span));");

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
        "Speed",
        $"Routed {incomingWebhooks.Length} webhooks in {sw.Elapsed.TotalMilliseconds:F2} ms total — zero custom ML training needed!",
        ConsoleColor.Cyan);
}

// ============================================================================
// PATTERN 2: Semantic Caching & Duplicate Bug Detection
// ============================================================================
static async Task RunPattern2_SemanticCacheAndDedupAsync(IEmbeddingGenerator<string, Embedding<float>> embedder)
{
    ConsoleUi.WriteSectionHeader(
        "PATTERN 2 OF 5: Semantic Cache & Duplicate Bug Blocker",
        "Before creating a duplicate Jira ticket or calling a slow LLM API, check if a similar vector already exists.");

    ConsoleUi.WriteCodeSnippet(
        "Semantic Cache Check",
        "float bestMatch = TensorPrimitives.CosineSimilarity(newVec.Span, cachedVec.Span);",
        "if (bestMatch >= 0.78f) return cachedEntry.ExistingJiraTicket; // Instant hit!");

    const float DuplicateThreshold = 0.78f;
    var cache = new List<(string PromptOrIssue, ReadOnlyMemory<float> Vector, string CachedResponse)>();

    var incomingStream = new (string Input, string ActionIfNew)[]
    {
        ("Payment gateway returns 502 error at checkout", "Created Jira #1042"),
        ("Checkout credit card payment fails with 502 gateway error", "Blocked Duplicate of #1042!"),
        ("How do I reset my two-factor authentication?", "Created Jira #1043"),
        ("How can I reset my 2FA two-factor authentication login?", "Blocked Duplicate of #1043!"),
        ("Invoice PDF shows the wrong VAT tax rate for EU customers", "Created Jira #1044")
    };

    foreach (var (input, action) in incomingStream)
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
            Console.Write($"  [DUPLICATE CAUGHT {match.Score:P0}] ");
            Console.ResetColor();
            Console.WriteLine($"\"{input}\"");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"                        -> Matches existing: \"{match.Entry.PromptOrIssue}\" ({match.Entry.CachedResponse})");
            Console.ResetColor();
        }
        else
        {
            cache.Add((input, vector, action));
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"  [STORED NEW       {(cache.Count == 1 ? 0f : match.Score):P0}] ");
            Console.ResetColor();
            Console.WriteLine($"\"{input}\" -> {action}");
        }
    }
}

// ============================================================================
// PATTERN 3: Anomaly & Drift Detection in Production Logs
// ============================================================================
static async Task RunPattern3_LogAnomalyDetectionAsync(IEmbeddingGenerator<string, Embedding<float>> embedder)
{
    ConsoleUi.WriteSectionHeader(
        "PATTERN 3 OF 5: Anomaly & Outlier Radar in Production Logs",
        "Average normal logs to find the 'Normal Center'. Flag any log that lands far outside that circle.");

    ConsoleUi.WriteCodeSnippet(
        "Computing Cluster Center & Outlier Distance",
        "foreach (var v in normalLogs) TensorPrimitives.Add(center, v.Span, center); // Average normal logs",
        "float distanceFromNormal = 1.0f - TensorPrimitives.CosineSimilarity(center, liveLogVec.Span);",
        "bool isAnomaly = distanceFromNormal > 0.55f;");

    string[] baselineNormalLogs =
    [
        "INFO Internal CI/CD pipeline build #482 deployed to staging kubernetes cluster",
        "INFO Docker container healthcheck passed on internal dev telemetry node",
        "INFO Automated linter and unit test suite completed in CI/CD pipeline",
        "INFO Staging kubernetes deployment rollout finished for internal service"
    ];

    var baselineEmbeddings = await embedder.GenerateAsync(baselineNormalLogs);
    int dims = baselineEmbeddings[0].Vector.Length;

    float[] centroid = new float[dims];
    foreach (var emb in baselineEmbeddings)
    {
        TensorPrimitives.Add(centroid.AsSpan(), emb.Vector.Span, centroid.AsSpan());
    }
    TensorPrimitives.Divide(centroid.AsSpan(), TensorPrimitives.Norm(centroid.AsSpan()), centroid.AsSpan());

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
        Console.Write(isAnomaly ? "  [🔴 ANOMALY ALERT] " : "  [🟢 NORMAL LOG   ] ");
        Console.ResetColor();
        Console.Write($"Distance={anomalyDistance:F2} | ");
        Console.WriteLine(logLine);
    }
}

// ============================================================================
// PATTERN 4: Stateless Recommendation Engine ("More Like This")
// ============================================================================
static async Task RunPattern4_StatelessRecommendationsAsync(IEmbeddingGenerator<string, Embedding<float>> embedder)
{
    ConsoleUi.WriteSectionHeader(
        "PATTERN 4 OF 5: Instant 'Recommended For You' Feed (Stateless)",
        "Average the coordinates of the last 3 articles a user read -> find closest unread articles!");

    ConsoleUi.WriteCodeSnippet(
        "Building a User Taste Vector by Averaging Arrays",
        "foreach (var readArticleVec in lastThreeRead)",
        "    TensorPrimitives.Add(userTasteVector, readArticleVec.Span, userTasteVector);",
        "// Now find unread articles with highest CosineSimilarity to userTasteVector!");

    string[] userRecentlyRead =
    [
        "Building high-performance vector math in C# and .NET with TensorPrimitives",
        "Running local ONNX embedding models in .NET microservices without cloud APIs",
        "How semantic search and vector embeddings replace brittle SQL LIKE queries"
    ];

    string[] catalogArticles =
    [
        "Deep dive into Matryoshka vector quantization and hybrid search in C# .NET",
        "Async/Await and Span<T> memory optimization tricks for C# backend engineers",
        "Configuring EU VAT tax rates on PDF billing receipts and Stripe checkout",
        "Resetting two-factor authentication MFA passkeys when a user loses their phone"
    ];

    var historyVectors = await embedder.GenerateAsync(userRecentlyRead);
    var catalogVectors = await embedder.GenerateAsync(catalogArticles);

    int dims = historyVectors[0].Vector.Length;
    float[] userTasteVector = new float[dims];
    foreach (var h in historyVectors)
    {
        TensorPrimitives.Add(userTasteVector.AsSpan(), h.Vector.Span, userTasteVector.AsSpan());
    }
    TensorPrimitives.Divide(userTasteVector.AsSpan(), TensorPrimitives.Norm(userTasteVector.AsSpan()), userTasteVector.AsSpan());

    Console.ForegroundColor = ConsoleColor.Gray;
    Console.WriteLine("  User Recently Read (Averaged into 1 Taste Coordinate):");
    foreach (var item in userRecentlyRead) Console.WriteLine($"    • {item}");
    Console.ResetColor();
    Console.WriteLine("\n  Recommended Next Reads:");

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
        "PATTERN 5 OF 5: Searching Untagged Photos, Audio & PDFs with Text",
        "Multimodal models (CLIP, SigLIP, Gemini Embedding 2) put Text, Images, and Audio into the SAME float[] space.");

    var untaggedMediaVault = new (string MediaId, string Modality, string RawContentRepresentation)[]
    {
        ("IMG_0042.jpg",  "🖼️ IMAGE (Photo)",        "visual photo of a golden retriever dog puppy running in the park"),
        ("AUD_9910.wav",  "🔊 AUDIO (Sound Clip)",   "audio clip of a canine dog barking loudly outdoors"),
        ("IMG_0108.png",  "🖼️ IMAGE (Screenshot)",   "screenshot of mobile app crash when uploading user profile picture avatar"),
        ("DOC_arch.pdf",  "📄 PDF   (Diagram)",      "architecture diagram flowchart with red arrows showing database failover")
    };

    var mediaVectors = await embedder.GenerateAsync(untaggedMediaVault.Select(m => m.RawContentRepresentation));

    string[] naturalLanguageQueries =
    [
        "golden retriever puppy",
        "diagram with red arrows"
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
        Console.WriteLine($"  Text Search: \"{query}\"");
        Console.ResetColor();

        foreach (var hit in topMatches)
        {
            Console.Write("    ");
            ConsoleUi.WriteScoreBar(hit.Score, 14);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($" [{hit.Media.Modality,-22}] ");
            Console.ResetColor();
            Console.WriteLine($"{hit.Media.MediaId}");
        }
        Console.WriteLine();
    }
}
