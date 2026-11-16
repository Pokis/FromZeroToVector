# Coordinates for Meaning: From Zero to Vector in .NET

> **Companion Repository, Interactive Visual Lab & C# Demos**  
> *From "Explain Like I'm 5" 2D coordinates to 3D clusters, local C# semantic search, 5 backend patterns beyond chatbots, and production fixes.*

---

## How Everything in This Repository Fits Together

1. **Interactive Step-by-Step Slide Deck ([`slides/index.html`](slides/index.html)) — *Master Flow***  
   - Visual, animated presentation that starts with **ELI5 analogies** (The 2D Taste Map) and builds step-by-step into real engineering concepts and C# code.
   - Press `→` or `Space` to reveal **one visual step at a time** on each slide (or `↓` to reveal all steps on the current slide).
   - Includes **dedicated Demo & Live Code slides** that launch directly into the matching Visualizer tab or C# project.
   - **Dual-Screen Speaker Mode ([`slides/presenter.html`](slides/presenter.html)):** Click **`🎤 Pop Out Speaker Screen (2nd Monitor)`** (or press `P`) to open a separate presenter window for your laptop monitor—complete with step-by-step conversational notes, a plain-English glossary of every technical term, and remote slide/tab controls that stay 100% hidden from the audience screen!

2. **Interactive Vector & Embedding Lab ([`visualizer/index.html`](visualizer/index.html)) — *Live Browser Demos***  
   - **Tab 1 — 2D Taste Map (ELI5):** Plot foods on `Sweetness × Crunchiness`, hover over **Cosine Similarity**, **Angle**, and **Ruler Distance** to see plain-English explanations, and click **`⚡ Demo: Why Angle Beats Ruler`** to see why a short tweet and a long book point at the exact same `0.0°` angle.
   - **Tab 2 — 3D Meaning Universe:** Rotate 180+ items in 3D space, type any audience sentence to watch the camera fly to where it lands, and toggle the **3D Shadow Warning**.
   - **Tab 3 — Vector Arithmetic Lab:** Add and subtract meaning (`"Database query latency" + "Angry customer" − "Internal test" = "P1 Production Outage!"`).
   - **Tab 4 — Where Vectors Fail & Hybrid Search Rescue:** Test 3 dramatic real-world traps (*Peanut Allergy Negation*, *Disable vs. Enable MFA*, and *HTTP Error 504 vs. 503*) and watch **Hybrid Search (Vector + Keyword)** promote the right document from `#3/#4` to **★ #1 WINNER**.
   - **Tab 5 — 4-Step RAM Shrinker:** A progressive 4-step wizard showing how **Matryoshka Slicing** (chopping array length) and **Quantization** (rounding 32-bit floats to 8-bit or 1-bit) shrink a `114 GB` vector database down to `600 MB`.

3. **Step-by-Step C# (.NET 10) Console Demos (`src/`)**  
   - Every C# project now runs **step-by-step interactively**—printing the 3–4 line C# code snippet at the top of each step and waiting for you to press `[Enter]` before moving to the next step:
     ```bash
     # Demo 1 (Slide 8): Semantic Search in ~25 Lines of C#
     dotnet run --project src/01-LiveCoding-SemanticSearch

     # Demo 2 (Slide 12): 5 Backend Patterns Beyond Chatbots (Step-by-Step)
     dotnet run --project src/02-PatternsBeyondRag

     # Demo 3 (Slide 18): Production Gotchas & Fixes (Hybrid Search, RAM Slicing, Chunking & Model Upgrades)
     dotnet run --project src/03-ProductionGotchasAndFixes
     ```
   - *(Tip: Add `-- --all` or `-- --demo` to run all steps non-stop without waiting for `[Enter]`, or add `-- --onnx` to use the local HuggingFace `.onnx` model file via `ElBruno.LocalEmbeddings`.)*

4. **Written Reference Guides (`docs/`) — *Post-Talk Reading for Attendees***  
   - All diagrams and concepts in `docs/` are visually represented inside the slides (`slides/index.html`). The markdown files in `docs/` serve as the **take-home written reference manual** for attendees browsing the repository after the talk:
     - [`docs/01-mental-model-and-pipeline.md`](docs/01-mental-model-and-pipeline.md) — 2D Taste Map, Cosine Angle vs. Ruler Distance, and the 4-Step Generation Pipeline.
     - [`docs/02-multimodal-and-model-selection.md`](docs/02-multimodal-and-model-selection.md) — How Text, Images, and Audio share one coordinate space + how to pick a model.
     - [`docs/03-five-patterns-beyond-rag.md`](docs/03-five-patterns-beyond-rag.md) — C# blueprints for Routing, Semantic Caching, Anomaly Detection, Recommendations, and Multimodal Search.
     - [`docs/04-production-gotchas-and-migrations.md`](docs/04-production-gotchas-and-migrations.md) — Hybrid Search (RRF), Matryoshka Slicing, Quantization, Contextual Chunking, and Blue/Green Model Upgrades.
     - [`docs/cheat-sheet.md`](docs/cheat-sheet.md) — One-page production cheat sheet.
