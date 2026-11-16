# Coordinates for Meaning: From Zero to Vector in .NET

> **Companion Repository & Interactive Lab for the 60-Minute Technical Talk**  
> *Visual intuition, local C# ONNX embeddings, high-leverage backend architectures beyond RAG, and production engineering realities.*

---

## What This Talk & Repository Cover

While everyone is fatigued by generic prompt-engineering wrappers, **vector embeddings** are the actual architectural primitive powering modern semantic search, zero-shot classification, anomaly detection, recommendations, and multimodal retrieval.

This repository takes you from **zero linear-algebra anxiety** to **production-grade C# engineering** across five progressive acts:

| Act | Time | Topic | Interactive Artifact / Code in Repo |
| :--- | :--- | :--- | :--- |
| **Act 1** | `0:00 – 0:10` | **The "Zero-Math" Mental Model & Generation Pipeline** | [`visualizer/index.html`](visualizer/index.html) *(Tab 2: 2D Taste Map)* & [`docs/01-mental-model-and-pipeline.md`](docs/01-mental-model-and-pipeline.md) |
| **Act 2** | `0:10 – 0:22` | **Flying Through the 3D Meaning Universe** | [`visualizer/index.html`](visualizer/index.html) *(Tab 1: 3D Scatter & Tab 3: Vector Math)* |
| **Act 3** | `0:22 – 0:38` | **Live C# Coding: Semantic Search in ~25 Lines** | [`src/01-LiveCoding-SemanticSearch`](src/01-LiveCoding-SemanticSearch) |
| **Act 4** | `0:38 – 0:50` | **5 Backend Patterns Beyond RAG & Multimodal Spaces** | [`src/02-PatternsBeyondRag`](src/02-PatternsBeyondRag) & [`docs/03-five-patterns-beyond-rag.md`](docs/03-five-patterns-beyond-rag.md) |
| **Act 5** | `0:50 – 1:00` | **Where Embeddings Lie: Production Gotchas & Fixes** | [`src/03-ProductionGotchasAndFixes`](src/03-ProductionGotchasAndFixes) & [`visualizer/index.html`](visualizer/index.html) *(Tabs 4 & 5)* |

---

## Quick Start (Zero Cloud API Keys Required!)

Everything in this repository—both the browser visualizations and the C# .NET 10 demos—runs **100% locally and offline**.

### 1. Launch the Interactive 3D Vector & Embedding Lab
Open [`visualizer/index.html`](visualizer/index.html) directly in any modern browser (no build step or server required):
- **Tab 1 — 3D Meaning Universe:** Orbit through 180+ embedded items across 3 datasets (*Engineering Tickets*, *Movies*, *StackOverflow*), type custom phrases to watch the camera fly to where they land, and inspect 384-D vector barcodes.
- **Tab 2 — Act 1: 2D Taste Map:** Drag items on a `Sweetness × Crunchiness` grid to see how angle $\theta$ and Cosine Similarity work visually.
- **Tab 3 — Live Vector Arithmetic:** Run equations on meaning (`"Database latency" + "Angry customer" − "Internal test" = "P1 Production Outage!"`).
- **Tab 4 — Negation Trap & Hybrid Search (RRF):** Inspect side-by-side failure cases (`"Enable MFA"` vs `"Disable MFA"`, `"Error 503"` vs `"Error 504"`) and tune a live **Dense + BM25 Hybrid Search** slider.
- **Tab 5 — Matryoshka (MRL) & Quantization Calculator:** Calculate exact RAM savings when slicing dimensions (`3072 -> 512`) and quantizing `float32 -> int8 -> 1-bit binary`.

### 2. Open the Presentation Slide Deck
Open [`slides/index.html`](slides/index.html) in your browser:
- Use `←` / `→` or `Space` to navigate slides.
- Press `S` to toggle the **Speaker Notes drawer** at the bottom of the screen.
- Press `F` for fullscreen presentation mode.

### 3. Run the C# (.NET 10) Demos

Prerequisites: [.NET 10 SDK](https://dotnet.microsoft.com/)

```bash
# Build the entire solution
dotnet build

# Act 3 Demo: Semantic Search in ~25 Lines (Interactive mode)
dotnet run --project src/01-LiveCoding-SemanticSearch

# Or run Act 3 with the 4 scripted stage queries + Negation Gotcha automatically:
dotnet run --project src/01-LiveCoding-SemanticSearch -- --demo

# Act 4 Demo: Five High-Leverage Backend Patterns Beyond RAG
dotnet run --project src/02-PatternsBeyondRag

# Act 5 Demo: Production Gotchas & Engineering Fixes (Hybrid RRF, Matryoshka, Quantization, Blue/Green Migration)
dotnet run --project src/03-ProductionGotchasAndFixes
```

> **Tip — Switching Between Instant Offline Engine and HuggingFace ONNX (`ElBruno.LocalEmbeddings`):**  
> By default, the C# demos use a deterministic, zero-network 384-D `IEmbeddingGenerator<string, Embedding<float>>` implementation so every demo starts in `< 1 ms` without waiting on a 90 MB download. Pass `--onnx` to any demo (e.g., `dotnet run --project src/01-LiveCoding-SemanticSearch -- --onnx --demo`) to download and run the full `sentence-transformers/all-MiniLM-L6-v2` ONNX model locally via `ElBruno.LocalEmbeddings`.

---

## The 25-Line C# Mental Model (Act 3)

Modern .NET makes working with embeddings trivially clean thanks to two standard primitives:
1. **`IEmbeddingGenerator<string, Embedding<float>>`** (`Microsoft.Extensions.AI`) — unified abstraction across Local ONNX, OpenAI, Azure, Ollama, and Gemini.
2. **`TensorPrimitives.CosineSimilarity`** (`System.Numerics.Tensors`) — hardware SIMD-accelerated vector math built directly into .NET.

```csharp
using System.Numerics.Tensors;
using ElBruno.LocalEmbeddings;
using Microsoft.Extensions.AI;

// 1. Load a local ONNX model (downloads once, runs offline on CPU/GPU)
IEmbeddingGenerator<string, Embedding<float>> embedder =
    await LocalEmbeddingGenerator.CreateAsync();

// 2. Our "database" of support tickets
string[] tickets = [
    "Payment gateway returns 502 error at checkout",
    "How do I reset my two-factor authentication?",
    "App crashes when uploading a profile picture",
    "Invoice PDF shows the wrong VAT rate"
];

// 3. Turn strings into float[384] coordinates
var ticketVectors = await embedder.GenerateAsync(tickets);

// 4. Search by MEANING, not exact keywords
var queryVector = await embedder.GenerateVectorAsync("credit card won't go through");

var bestMatch = tickets
    .Zip(ticketVectors)
    .Select(x => (
        Ticket: x.First,
        Score: TensorPrimitives.CosineSimilarity(queryVector.Span, x.Second.Vector.Span)
    ))
    .OrderByDescending(x => x.Score)
    .First();

Console.WriteLine($"Match ({bestMatch.Score:P0}): {bestMatch.Ticket}");
// Output: Match (75%): Payment gateway returns 502 error at checkout
```

---

## Deep-Dive Documentation & Architecture Guides

- **[1. The Core Mental Model & 4-Stage Generation Pipeline](docs/01-mental-model-and-pipeline.md)**  
  How `Tokenize -> Contextualize (Self-Attention) -> Pool -> L2 Normalize` and Contrastive Learning turn unstructured data into unit vectors, and why normalized Cosine Similarity equals Dot Product.
- **[2. Multimodal Shared Spaces & The 5 Model Selection Axes](docs/02-multimodal-and-model-selection.md)**  
  How Text, Image patches (ViT), and Audio spectrograms share one coordinate space (CLIP, SigLIP, Gemini Embedding 2), understanding **The Modality Gap**, and how to choose models across 5 engineering trade-offs.
- **[3. Five Architectural Patterns Beyond RAG](docs/03-five-patterns-beyond-rag.md)**  
  Zero-Shot Webhook Routing (`< 1 ms`), Semantic Caching & Deduplication, Log Anomaly Detection, Stateless Recommendations, and Cross-Modal Search.
- **[4. Production Realities, Gotchas & Model Migrations](docs/04-production-gotchas-and-migrations.md)**  
  Solving Negation & Exact ID Blindness with **Hybrid Search (BM25 + RRF)**, cutting RAM 4x–32x with **Matryoshka Slicing & Binary Quantization (`BitOperations.PopCount`)**, **Contextual Chunking**, and **Blue/Green Vector Schema Migrations**.
- **[5. Attendee Take-Home Cheat Sheet](docs/cheat-sheet.md)**  
  Decision matrix (`TensorPrimitives` vs `pgvector` vs Dedicated Vector DB), RAM formulas, and the 4 Golden Rules of Production Vectors.

---

## Repository Structure

```text
embeddings/
├── EmbeddingsTalk.slnx                         # .NET 10 Solution
├── README.md                                   # Listener landing page & quick start
├── slides/
│   └── index.html                              # Interactive 14-slide HTML presentation deck
├── visualizer/
│   └── index.html                              # 5-tab 3D Vector Universe & Embedding Lab
├── docs/
│   ├── 01-mental-model-and-pipeline.md         # 2D Taste Map, 4-stage pipeline & Contrastive Learning
│   ├── 02-multimodal-and-model-selection.md    # Shared Text/Image/Audio spaces & 5 model axes
│   ├── 03-five-patterns-beyond-rag.md          # Architectural patterns & C# blueprints
│   ├── 04-production-gotchas-and-migrations.md # Hybrid RRF, MRL, Quantization & Blue/Green upgrades
│   └── cheat-sheet.md                          # One-page production reference matrix
└── src/
    ├── Embeddings.Core/                        # Shared IEmbeddingGenerator engine & console UI helpers
    ├── 01-LiveCoding-SemanticSearch/           # Act 3: ~25-line C# semantic search + stage script
    ├── 02-PatternsBeyondRag/                   # Act 4: 5 backend patterns beyond RAG in C#
    └── 03-ProductionGotchasAndFixes/           # Act 5: Hybrid RRF, MRL slicing, int8/1-bit & migrations
```
