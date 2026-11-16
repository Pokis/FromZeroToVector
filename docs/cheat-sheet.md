# Production Vector Embeddings — Attendee Take-Home Cheat Sheet

## 1. Where Should I Store & Search Vectors?

| Scale (Number of Vectors) | Recommended Architecture | Why? |
| :--- | :--- | :--- |
| **Up to ~25,000 vectors** | **In-Memory C# (`ReadOnlyMemory<float>` + `TensorPrimitives`)** | A SIMD brute-force scan over 20K `float[384]` vectors in .NET takes **< 2 ms** with 100% exact recall and zero database infrastructure. |
| **25,000 – 25,000,000 vectors** | **PostgreSQL + `pgvector` (HNSW Index)** | Keeps vectors, relational joins, ACID transactions, permissions, and `tsvector` BM25 hybrid search inside a single database you already know how to operate. |
| **50,000,000 – 1B+ vectors** | **Dedicated Vector DB (Qdrant, Milvus, Weaviate)** | Purpose-built distributed sharding, memory-mapped HNSW/DiskANN indexes, built-in scalar/binary quantization, and high-throughput ingestion. |

---

## 2. Quick Memory Math Formula

$$\text{Raw Vector RAM (Bytes)} = \text{Vector Count} \times \text{Dimensions} \times \text{Bytes Per Dimension}$$

- **`float32`**: `4 bytes` per dimension (`1M vectors @ 1536 dims` $\approx 6.14\text{ GB}$ raw, $\sim 8.5\text{ GB}$ with HNSW index).
- **`float16`**: `2 bytes` per dimension (**2x savings**).
- **`int8` (Scalar Quantization)**: `1 byte` per dimension (**4x savings**, `< 1%` recall loss).
- **`1-bit` (Binary Quantization)**: `0.125 bytes` per dimension (**32x savings**, best paired with 2-stage `float32` rescoring).

---

## 3. Essential .NET 10 Building Blocks

| NuGet Package / Namespace | Key Type / Method | Purpose |
| :--- | :--- | :--- |
| **`Microsoft.Extensions.AI`** | `IEmbeddingGenerator<string, Embedding<float>>` | Standard .NET DI abstraction for any embedding provider (Local ONNX, OpenAI, Azure, Ollama, Gemini). |
| **`System.Numerics.Tensors`** | `TensorPrimitives.CosineSimilarity(Span, Span)` | Hardware SIMD-accelerated (AVX2 / AVX-512 / NEON) vector math built into .NET. |
| **`System.Numerics.Tensors`** | `TensorPrimitives.Dot(Span, Span)` | Fastest similarity check when vectors are already L2-normalized (`||v|| == 1.0`). |
| **`System.Numerics`** | `BitOperations.PopCount(ulong)` | Hardware `POPCNT` instruction for 1-bit Binary Quantized Hamming distance search. |
| **`ElBruno.LocalEmbeddings`** | `LocalEmbeddingGenerator.CreateAsync()` | Runs local ONNX HuggingFace embedding models 100% offline in .NET. |

---

## 4. The 4 Golden Rules of Production Vector Systems

1. **Always Use Hybrid Search (Dense + BM25 + RRF):** Dense vectors understand synonyms; BM25 catches exact error codes (`503` vs `504`), part numbers, and boolean negations (`enable` vs `disable`).
2. **Prepend Context Before Chunking:** Never embed an orphaned paragraph (`"It fails when..."`). Prepend `[Doc Title > Section Header]` before calling `GenerateAsync()`.
3. **Shrink Before You Scale:** Use Matryoshka (MRL) dimension slicing (`1536 -> 512`) and `int8` quantization before paying for massive RAM instances.
4. **Version Every Vector Row:** Store `model_id` and `model_version` alongside every vector column so future model upgrades can use zero-downtime Blue/Green backfills.
