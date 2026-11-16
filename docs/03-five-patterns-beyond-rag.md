# 3. Five High-Leverage Architectural Patterns Beyond RAG

Most developers only associate embeddings with **RAG** (retrieving text chunks to paste into an LLM prompt). In practice, embeddings are even more powerful when used as **fast, cheap, deterministic building blocks** inside regular backend microservices—with **zero LLM text generation** required.

> Run all 5 patterns live in C#:
> ```bash
> dotnet run --project src/02-PatternsBeyondRag
> ```

---

## Pattern 1: Zero-Shot Routing & Classification (`< 1 ms`)

### The Problem
You have incoming support tickets, webhooks, or user feedback that need to be routed to one of 5 teams (`Billing`, `Security/IAM`, `Mobile Client`, `Infra/SRE`, `Product Ideas`). Training and deploying a custom classifier takes labeled data, ML pipelines, and retraining whenever you add a 6th category.

### The Vector Solution
Embed a rich one-sentence description ("anchor") for each category **once at application startup**. For every incoming webhook, embed the payload and compute the dot product against your 5 anchor vectors:

```csharp
var bestRoute = routeAnchors
    .Select(anchor => (
        Queue: anchor.QueueName,
        Score: TensorPrimitives.Dot(incomingVector.Span, anchor.Vector.Span)
    ))
    .OrderByDescending(x => x.Score)
    .First();

if (bestRoute.Score >= 0.55f)
    await RouteToQueueAsync(bestRoute.Queue, webhookPayload);
else
    await RouteToTriageFallbackAsync(webhookPayload);
```
- **Latency:** `< 1 ms` locally on CPU.
- **Extensibility:** Adding a new category is a single config string change.

---

## Pattern 2: Semantic Caching & Duplicate Detection

### The Problem
1. Users file duplicate Jira/GitHub bug reports phrased slightly differently (*"Payment gateway 502 at checkout"* vs. *"Checkout credit card fails with 502 error"*).
2. Users ask your AI assistant the same 200 questions in 5,000 slightly different ways, burning expensive LLM API tokens and adding 2–4 seconds of latency.

### The Vector Solution
Keep a rolling vector index of recent bug reports or answered LLM prompts. Before creating a new ticket or calling the LLM API, check if the nearest neighbor exceeds a strict similarity threshold (e.g., `CosineSimilarity >= 0.88–0.94`):

```csharp
float similarity = TensorPrimitives.CosineSimilarity(queryVec.Span, cachedEntry.Vector.Span);
if (similarity >= 0.90f)
{
    return cachedEntry.Response; // 0ms LLM cost, instant response!
}
```

---

## Pattern 3: Anomaly & Log Drift Detection

### The Problem
Regex alerting rules only catch failure modes you already know how to spell (`ERROR|Exception|500`). When a novel failure mode or subtle operational drift occurs, regex stays silent—or floods on-call engineers with false positives.

### The Vector Solution
1. Compute the **Cluster Centroid** (normalized mean vector) of baseline healthy telemetry or user session sequences using `TensorPrimitives.Add` and `TensorPrimitives.Divide`.
2. Measure the cosine distance (`1.0f - CosineSimilarity(centroid, liveLogVector)`) of incoming log batches.
3. Any log line landing far outside the known healthy cluster triggers an anomaly alert.

---

## Pattern 4: Stateless Recommendation Engines ("More Like This")

### The Problem
Building a classic collaborative-filtering recommendation engine requires heavy matrix factorization pipelines, user-item interaction tables, and cold-start headaches.

### The Vector Solution
Represent a user's live taste profile on the fly by taking the **weighted average of the `float[]` vectors** of the last $N$ articles, products, or talks they interacted with:

```csharp
float[] userTasteVector = new float[dimensions];
foreach (var itemVector in lastFiveInteractedItems)
{
    TensorPrimitives.Add(userTasteVector, itemVector.Span, userTasteVector);
}
// Normalize back to the unit hypersphere
TensorPrimitives.Divide(userTasteVector, TensorPrimitives.Norm(userTasteVector), userTasteVector);
```
Querying the nearest unread items to `userTasteVector` delivers personalized recommendations in milliseconds—even for anonymous session users with zero historical database rows.

---

## Pattern 5: Cross-Modal Search (Shared Coordinate Spaces)

### The Problem
Your engineering team has folders of untagged bug screenshots, architecture whiteboard photos, PDF runbooks, and call recordings. Searching them requires manual tagging or brittle OCR pipelines.

### The Vector Solution
Use a multimodal embedding model (**CLIP**, **SigLIP**, **Gemini Embedding 2**, or **Qwen3-VL**) where images, audio, and text are projected into the **exact same `float[N]` coordinate space**. Index the raw media files once, and query them using plain English text strings.
