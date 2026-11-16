// ============================================================================
// SHARED SLIDE DECK & SPEAKER PRESENTER DATA
// Powers both slides/index.html (Audience Screen) and slides/presenter.html (Speaker Screen)
// ============================================================================

window.TALK_SLIDES = [
  // --------------------------------------------------------------------------
  // SLIDE 1: TITLE
  // --------------------------------------------------------------------------
  {
    id: "title",
    type: "title",
    title: "Coordinates for Meaning",
    subtitle: "How AI Turns Words, Images, and Code into Searchable Geometry",
    stepsCount: 3,
    html: `
      <div class="hero-center">
        <div class="constellation-badge">V⃗</div>
        <h1 class="hero-title">Coordinates for Meaning</h1>
        <p class="hero-sub">From Zero Math to Production Vector Systems in C#</p>

        <div class="cards-3" style="margin-top:36px;">
          <div class="v-card step-item" data-step="1">
            <div class="v-icon">🗺️</div>
            <h3>1. Visual Intuition</h3>
            <p>How computers turn fuzzy human ideas into GPS coordinates.</p>
          </div>
          <div class="v-card step-item" data-step="2">
            <div class="v-icon">⚡</div>
            <h3>2. Practical C#</h3>
            <p>Semantic search, routing, and recommendations in ~25 lines of .NET.</p>
          </div>
          <div class="v-card step-item" data-step="3">
            <div class="v-icon">🛡️</div>
            <h3>3. Production Truths</h3>
            <p>Where vectors fail, how to shrink 120 GB to 600 MB, and how to upgrade models safely.</p>
          </div>
        </div>
      </div>
    `,
    speakerNotes: [
      "Welcome everyone! Today we are looking under the hood of one of the most useful building blocks in modern software: Vector Embeddings.",
      "Click 1 (Visual Intuition): You don't need a math degree for this. By the end of the first 10 minutes, you'll be able to picture embeddings as simple GPS coordinates.",
      "Click 2 (Practical C#): Then we'll build semantic search and 5 backend patterns in clean C# running 100% locally on this laptop.",
      "Click 3 (Production Truths): And finally, we'll look at where embeddings fail in production—and how to fix them."
    ],
    glossary: [
      ["Embedding / Vector", "Simply an array of decimal numbers (like float[384]) that represents the meaning of a piece of text, image, or audio."],
      ["Semantic", "Matching by *meaning and intent* rather than matching exact letters."]
    ]
  },

  // --------------------------------------------------------------------------
  // SLIDE 2: THE PROBLEM (BYTES VS MEANING)
  // --------------------------------------------------------------------------
  {
    id: "problem-bytes-vs-meaning",
    type: "concept",
    title: "Computers Read Letters. Humans Mean Ideas.",
    subtitle: "Why traditional search breaks the moment a user picks a different word",
    stepsCount: 3,
    html: `
      <div class="flow-vertical">
        <div class="compare-row step-item" data-step="1">
          <div class="chat-bubble user-bubble">
            <div class="bubble-label">👤 What your user types into search:</div>
            <div class="bubble-text">"My laptop <span class="hl-red">screen is flickering</span>"</div>
          </div>
          <div class="vs-arrow">🔍 SQL <code>LIKE '%flicker%'</code></div>
          <div class="chat-bubble db-bubble fail-border">
            <div class="bubble-label">🗄️ Database Response:</div>
            <div class="bubble-text" style="color:var(--danger);">0 Results Found ❌</div>
          </div>
        </div>

        <div class="step-item" data-step="2" style="text-align:center; margin: 12px 0;">
          <div class="reveal-banner">
            Meanwhile, sitting right inside your database unread:
            <div class="doc-pill">📄 Article #402: "Troubleshooting <span class="hl-green">display refresh glitches</span>"</div>
          </div>
        </div>

        <div class="cards-2 step-item" data-step="3">
          <div class="v-card" style="border-color:rgba(248,113,113,0.4);">
            <h3 style="color:var(--danger);">Traditional Keyword Search</h3>
            <p>Compares <strong>spelling</strong> byte-by-byte.<br/><code>"screen" != "display"</code><br/><code>"flickering" != "glitches"</code></p>
          </div>
          <div class="v-card" style="border-color:rgba(52,211,153,0.4);">
            <h3 style="color:var(--accent-3);">Vector Embedding Search</h3>
            <p>Places both sentences at the <strong>same spot on a map of meaning</strong>—even when they share <strong>zero words</strong>.</p>
          </div>
        </div>
      </div>
    `,
    speakerNotes: [
      "Think about the last time you used a help-center search bar on a website. You type a normal human sentence...",
      "Click 1: Say a user types 'My laptop screen is flickering'. A traditional SQL LIKE query checks for the exact letters 'f-l-i-c-k-e-r'—and returns 0 results.",
      "Click 2: What's frustrating is that the exact answer IS in the database! It's just titled 'Troubleshooting display refresh glitches'. Zero shared words.",
      "Click 3: That is the core problem embeddings solve: moving from matching *spelling* to matching *location on a map of meaning*."
    ],
    glossary: [
      ["Lexical / Keyword Search", "Searching by exact words or characters (like SQL LIKE or Ctrl+F). Fast, but blind to synonyms."]
    ]
  },

  // --------------------------------------------------------------------------
  // SLIDE 3: ELI5 — THE 2D TASTE MAP
  // --------------------------------------------------------------------------
  {
    id: "eli5-taste-map",
    type: "concept",
    title: "Explain Like I'm 5: The 2D Taste Map",
    subtitle: "How do you turn a concept into numbers without doing any math?",
    stepsCount: 4,
    html: `
      <div class="two-col-slide">
        <div class="visual-box">
          <svg viewBox="0 0 460 320" style="width:100%; height:auto;">
            <!-- Axes -->
            <line x1="55" y1="270" x2="430" y2="270" stroke="#64748b" stroke-width="2" />
            <line x1="55" y1="270" x2="55" y2="25" stroke="#64748b" stroke-width="2" />
            <text x="160" y="302" fill="#cbd5e1" font-size="13" font-weight="bold">X-Axis: Sweetness (0 → 10)</text>
            <text x="16" y="190" fill="#cbd5e1" font-size="13" font-weight="bold" transform="rotate(-90 16,190)">Y-Axis: Crunchiness (0 → 10)</text>

            <!-- Step 1: Apple -->
            <g class="step-svg" data-step="1">
              <circle cx="335" cy="70" r="9" fill="#f87171" />
              <text x="260" y="48" fill="#f8fafc" font-size="14" font-weight="bold">🍎 Apple [7.5, 8.0]</text>
            </g>

            <!-- Step 2: Pear lands right next to Apple! -->
            <g class="step-svg" data-step="2">
              <circle cx="315" cy="90" r="9" fill="#34d399" />
              <text x="220" y="116" fill="#34d399" font-size="14" font-weight="bold">🍐 Crispy Pear [7.0, 7.5]</text>
              <circle cx="325" cy="80" r="34" fill="none" stroke="#34d399" stroke-dasharray="4 4" stroke-width="1.5" />
            </g>

            <!-- Step 3: Potato Chip & Cake far away -->
            <g class="step-svg" data-step="3">
              <circle cx="100" cy="45" r="9" fill="#fbbf24" />
              <text x="115" y="50" fill="#fbbf24" font-size="13" font-weight="bold">🥔 Potato Chip [1.2, 9.2]</text>

              <circle cx="400" cy="235" r="9" fill="#a78bfa" />
              <text x="260" y="255" fill="#a78bfa" font-size="13" font-weight="bold">🍰 Chocolate Cake [9.4, 1.4]</text>
            </g>
          </svg>
        </div>

        <div class="steps-column">
          <div class="v-card step-item" data-step="1">
            <h3>Step 1: Pick 2 Traits</h3>
            <p>Give every food two scores from 0 to 10: <strong>Sweetness (X)</strong> and <strong>Crunchiness (Y)</strong>. An 🍎 Apple is <code>[7.5, 8.0]</code>.</p>
          </div>
          <div class="v-card step-item" data-step="2">
            <h3>Step 2: Similar Things Clump Together</h3>
            <p>Where does a 🍐 <strong>Crispy Pear</strong> go? At <code>[7.0, 7.5]</code>—it lands right next to the Apple automatically!</p>
          </div>
          <div class="v-card step-item" data-step="3">
            <h3>Step 3: Different Things Land Far Apart</h3>
            <p>A salty 🥔 <strong>Potato Chip</strong> <code>[1.2, 9.2]</code> and soft 🍰 <strong>Cake</strong> <code>[9.4, 1.4]</code> land in totally different corners.</p>
          </div>
          <div class="v-card step-item" data-step="4" style="border-color:var(--accent); background:rgba(56,189,248,0.1);">
            <h3 style="color:var(--accent);">🎉 That's an Embedding!</h3>
            <p><code>[7.5, 8.0]</code> is a <strong>2-dimensional embedding</strong>. You turned a food into GPS coordinates where <strong>closeness = similarity</strong>.</p>
          </div>
        </div>
      </div>
    `,
    speakerNotes: [
      "Let's strip away all the AI buzzwords. Imagine a blank chart with two simple scores from 0 to 10: Sweetness on the bottom, Crunchiness on the side.",
      "Click 1: Where do we put an Apple? It's sweet (7.5) and crunchy (8.0). So in code, an Apple is just a 2-number array: [7.5, 8.0].",
      "Click 2: Now where does a Crispy Pear go? [7.0, 7.5]—right next to the Apple!",
      "Click 3: Meanwhile, a Potato Chip (not sweet, super crunchy) and Chocolate Cake (super sweet, soft) land in totally different corners.",
      "Click 4: Without doing any fancy math, you just created a 2-dimensional vector embedding! Closeness on the map equals similarity in meaning."
    ],
    glossary: [
      ["2D Embedding", "An array of 2 numbers [X, Y] representing two traits (here: Sweetness and Crunchiness)."],
      ["Dimension", "One number (or axis) in the coordinate array. A 2D map has 2 dimensions; a 384D model has 384 numbers per item."]
    ]
  },

  // --------------------------------------------------------------------------
  // SLIDE 4: DEMO 1 — INTERACTIVE 2D TASTE MAP & ANGLE PLAYGROUND
  // --------------------------------------------------------------------------
  {
    id: "demo-2d-map",
    type: "demo",
    title: "🎬 Live Interactive Demo: The 2D Taste Map & Angle vs. Ruler",
    subtitle: "Why AI measures the compass angle between arrows instead of ruler distance",
    stepsCount: 1,
    demoUrl: "../visualizer/index.html?tab=tab-2d",
    html: `
      <div class="demo-hero">
        <div class="demo-badge">🎬 INTERACTIVE VISUALIZER • TAB 1</div>
        <h2>Let's Test Coordinates & Angles Live</h2>
        <p style="max-width:760px; margin: 12px auto 24px; color:var(--text-1); font-size:18px; line-height:1.55;">
          We'll plot items on the 2D Taste Map, inspect the 3 similarity metrics (<strong>Cosine Similarity</strong>, <strong>Angle</strong>, and <strong>Ruler Distance</strong>), and see why a <em>Short Tweet</em> and a <em>Long Book</em> point at the exact same angle!
        </p>
        <div class="demo-actions">
          <a class="btn-launch" href="../visualizer/index.html?tab=tab-2d" target="_blank">
            🚀 Open 2D Taste Map (Visualizer Tab 1) ↗
          </a>
        </div>
        <div class="cards-3" style="margin-top:28px; text-align:left;">
          <div class="v-card">
            <h3>1. Plot a Custom Food</h3>
            <p>Enter a Sweetness (0–10) and Crunchiness (0–10) score to drop a new dot onto the map.</p>
          </div>
          <div class="v-card">
            <h3>2. Hover the 3 Metrics</h3>
            <p>Compare <strong>Cosine %</strong> vs <strong>Angle °</strong> vs <strong>Ruler Distance</strong> in plain English.</p>
          </div>
          <div class="v-card">
            <h3>3. Click "Why Angle Beats Ruler"</h3>
            <p>See why a short tweet <code>[3.5, 4.0]</code> and a long book <code>[7.0, 8.0]</code> have a <strong>0° angle (100% match)</strong>!</p>
          </div>
        </div>
      </div>
    `,
    speakerNotes: [
      "Click the button to switch to Tab 1 of our Visualizer ('1. 2D Taste Map').",
      "1. Show how Apple and Crispy Pear point in almost the exact same direction from (0,0) -> 99.9% Cosine Similarity.",
      "2. Hover over the 3 metric boxes (Cosine Similarity, Angle, Ruler Distance) to show the audience what they mean.",
      "3. Click the button 'Demo: Why Angle Beats Ruler' at the top of the chart! Point out that a Short Tweet about Apples [3.5, 4.0] and a Long Book about Apples [7.0, 8.0] look far apart by ruler distance (4.95), but their arrows point in the EXACT same compass direction (0.0° angle = 100% Cosine match)!"
    ],
    glossary: [
      ["Cosine Similarity", "Measures whether two arrows from (0,0) point in the same compass direction (100% = same direction, 0% = 90° apart)."],
      ["Euclidean (Ruler) Distance", "Straight tape-measure distance between two dots' tips. Easily tricked when one document is longer than another."]
    ]
  },

  // --------------------------------------------------------------------------
  // SLIDE 5: SCALING FROM 2 AXES TO 384 AXES
  // --------------------------------------------------------------------------
  {
    id: "scaling-2-to-384",
    type: "concept",
    title: "From 2 Human Axes to 384 AI Axes",
    subtitle: "How real embedding models describe anything—not just food",
    stepsCount: 3,
    html: `
      <div class="flow-vertical">
        <div class="v-card step-item" data-step="1">
          <h3>1. Our Toy Food Map Had 2 Axes (<code>float[2]</code>)</h3>
          <div class="axis-pills">
            <span class="ax-pill">Axis 1: Sweetness</span>
            <span class="ax-pill">Axis 2: Crunchiness</span>
          </div>
        </div>

        <div class="v-card step-item" data-step="2">
          <h3>2. What If We Want to Map Support Tickets, Code, or Movies?</h3>
          <p style="margin-bottom:10px;">We just add more trait axes to the array:</p>
          <div class="axis-pills">
            <span class="ax-pill">Is it about money/billing?</span>
            <span class="ax-pill">Is the customer angry/urgent?</span>
            <span class="ax-pill">Is it a login/password issue?</span>
            <span class="ax-pill">Is it mobile or desktop?</span>
            <span class="ax-pill">... + 380 more learned traits!</span>
          </div>
        </div>

        <div class="v-card step-item" data-step="3" style="border-color:var(--accent-3);">
          <h3 style="color:var(--accent-3);">3. The Only Formula You Need Today</h3>
          <div class="formula-box">
            <span>Any Text / Image / Audio</span>
            <span class="formula-arrow">──► <strong>AI Embedding Model</strong> ──►</span>
            <span class="mono" style="color:var(--accent);">float[384]</span>
            <span style="font-size:14px; color:var(--text-2);">(384 coordinates between -1.0 and +1.0)</span>
          </div>
        </div>
      </div>
    `,
    speakerNotes: [
      "So how do we go from apples and pears to real software engineering?",
      "Click 1: On our food chart, we had 2 human-named axes: Sweetness and Crunchiness.",
      "Click 2: If we want to map support tickets or code, 2 axes aren't enough. We need traits like 'Is this about billing?', 'Is it an urgent outage?', 'Is it about authentication?'.",
      "Click 3: An AI embedding model is simply a function that takes any text and returns an array of 384 (or up to 3,072) decimal numbers—each number scoring one learned trait."
    ],
    glossary: [
      ["float[384]", "A standard C# array of 384 single-precision decimal numbers. This *is* the embedding."],
      ["Latent Trait / Axis", "Because the AI learns these 384 traits on its own by reading the internet, no human manually names Axis #42—the model discovers the most useful traits automatically."]
    ]
  },

  // --------------------------------------------------------------------------
  // SLIDE 6: HOW DOES THE AI CREATE THOSE 384 NUMBERS? (4-STAGE PIPELINE)
  // --------------------------------------------------------------------------
  {
    id: "four-stage-pipeline",
    type: "concept",
    title: "Under the Hood: How Are Embeddings Generated?",
    subtitle: "It's not a giant dictionary lookup—it's a 4-step conveyor belt",
    stepsCount: 4,
    html: `
      <div class="cards-4">
        <div class="v-card step-item" data-step="1">
          <div class="step-badge">Step 1</div>
          <h3>🔪 Chop (Tokenize)</h3>
          <p>Chop the input into small puzzle pieces (tokens):</p>
          <div class="mini-code">
            "unbelievable"<br/>
            ↓<br/>
            ["un", "believ", "able"]<br/>
            ↓<br/>
            IDs: [348, 9102, 481]
          </div>
        </div>

        <div class="v-card step-item" data-step="2">
          <div class="step-badge">Step 2</div>
          <h3>👀 Context Check</h3>
          <p>Words look at their neighbors to figure out what they mean right now:</p>
          <div class="mini-code">
            "cracked my <strong>apple</strong> screen" → 📱 Tech<br/><br/>
            "ate a crisp <strong>apple</strong>" → 🍎 Fruit
          </div>
        </div>

        <div class="v-card step-item" data-step="3">
          <div class="step-badge">Step 3</div>
          <h3>🗜️ Squash (Pool)</h3>
          <p>Average all the word vectors in the sentence into <strong>one single array</strong>:</p>
          <div class="mini-code">
            50 word vectors<br/>
            ↓ (Average / Mean Pool)<br/>
            <strong>1 × float[384]</strong>
          </div>
        </div>

        <div class="v-card step-item" data-step="4">
          <div class="step-badge">Step 4</div>
          <h3>🎯 Scale to 1.0</h3>
          <p>Scale the arrow so its length is exactly <code>1.0</code> (put every dot on the same circle).</p>
          <div class="mini-code" style="color:var(--accent-3);">
            Bonus: Once length = 1.0,<br/>
            <strong>Angle == Dot Product!</strong>
          </div>
        </div>
      </div>
    `,
    speakerNotes: [
      "A common question people ask is: 'Is an embedding model just a giant dictionary lookup table?' No! What if you type a sentence nobody has ever written before? Here is the 4-step conveyor belt inside every model:",
      "Click 1 (Chop / Tokenize): First, the model chops text into sub-word pieces called tokens and turns them into integer IDs.",
      "Click 2 (Context Check / Self-Attention): Next, every word looks at the words around it. That's how the word 'apple' moves toward iPhones when you say 'cracked my apple screen', and toward bananas when you say 'ate a crisp apple'.",
      "Click 3 (Squash / Mean Pooling): If your paragraph had 50 tokens, you now have 50 vectors. The pooling layer averages them together into ONE single float[384] coordinate for the whole paragraph.",
      "Click 4 (Scale to 1.0 / Normalize): Finally, it scales the arrow to length 1.0 so short tweets and long docs live on the same unit sphere. How did it learn where to put things? By playing a billion rounds of 'Hot or Cold' (Contrastive Learning) on matching vs non-matching pairs!"
    ],
    glossary: [
      ["Tokenization", "Splitting text into sub-word chunks (roughly 3/4 of an English word per token)."],
      ["Self-Attention (Transformer)", "The neural network layer where words exchange context with surrounding words in the sentence."],
      ["Mean Pooling", "Averaging all the individual word vectors in a sentence to get a single summary vector."],
      ["Contrastive Learning", "Training the model by feeding it matching pairs (Query + Right Answer) and pushing them together, while pushing random wrong answers apart."]
    ]
  },

  // --------------------------------------------------------------------------
  // SLIDE 7: DEMO 2 — 3D MEANING UNIVERSE & VECTOR ARITHMETIC
  // --------------------------------------------------------------------------
  {
    id: "demo-3d-universe",
    type: "demo",
    title: "🎬 Live Demo: Flying Through the 3D Meaning Universe",
    subtitle: "Watching clusters form & doing live math on concepts (A + B − C)",
    stepsCount: 1,
    demoUrl: "../visualizer/index.html?tab=tab-3d",
    html: `
      <div class="demo-hero">
        <div class="demo-badge">🎬 INTERACTIVE VISUALIZER • TABS 2 & 3</div>
        <h2>Step Inside 384-Dimensional Space</h2>
        <p style="max-width:760px; margin: 12px auto 24px; color:var(--text-1); font-size:18px; line-height:1.55;">
          Let's rotate a live cloud of 180 support tickets, drop a brand-new sentence from the audience into 3D space, and add/subtract concepts using vector arithmetic!
        </p>
        <div class="demo-actions">
          <a class="btn-launch" href="../visualizer/index.html?tab=tab-3d" target="_blank">
            🌌 Open 3D Meaning Universe (Tab 2) ↗
          </a>
          <a class="btn-launch btn-alt" href="../visualizer/index.html?tab=tab-math" target="_blank">
            ➕ Open Vector Arithmetic Lab (Tab 3) ↗
          </a>
        </div>
        <div class="cards-3" style="margin-top:28px; text-align:left;">
          <div class="v-card">
            <h3>1. Self-Organizing Clusters</h3>
            <p>Billing, MFA Security, Outages, and UI bugs clump into neighborhoods with zero manual tags.</p>
          </div>
          <div class="v-card">
            <h3>2. Audience Live Drop</h3>
            <p>Type any weird sentence—watch the camera fly to where it lands and connect to its top 5 matches.</p>
          </div>
          <div class="v-card">
            <h3>3. Vector Math (Tab 3)</h3>
            <p><code>"Database latency" + "Angry customer" − "Internal test" = "P1 Production Outage!"</code></p>
          </div>
        </div>
      </div>
    `,
    speakerNotes: [
      "Click the button to open Tab 2 ('2. 3D Meaning Universe') in the Visualizer.",
      "1. Drag to rotate the 3D cloud. Point out that nobody tagged these tickets—the model grouped Billing, MFA, and Outages by meaning alone.",
      "2. Ask someone in the audience for a bug description (or click a quick chip like 'credit card won't go through at checkout') and click 'Embed & Fly'.",
      "3. Click the yellow '3D Shadow Warning' button in the top-right to remind them: squashing 384 numbers to 3D is like a shadow on a wall—always compute similarity in full 384D!",
      "4. Switch to Tab 3 ('3. Vector Arithmetic') and click Preset 1: 'Database latency + Angry customer - Internal test' -> lands right on 'P1 Production database outage!'."
    ],
    glossary: [
      ["UMAP / Dimensionality Reduction", "An algorithm that squashes 384 dimensions down to 3D (X, Y, Z) so humans can visualize clusters on a screen—like flattening a 3D globe into a 2D map."]
    ]
  },

  // --------------------------------------------------------------------------
  // SLIDE 8: LIVE C# CODE DEMO 1 — SEMANTIC SEARCH IN ~25 LINES
  // --------------------------------------------------------------------------
  {
    id: "code-demo-semantic-search",
    type: "code",
    title: "💻 Live C# Code: Semantic Search in ~25 Lines",
    subtitle: "Running 100% locally in .NET 10 — zero cloud API keys or Python needed",
    stepsCount: 4,
    html: `
      <div class="two-col-slide">
        <div>
          <pre class="code-box">
<span class="code-step step-item" data-step="1"><span class="cm">// Step 1: Load a local AI model file (.onnx) in C#</span>
<span class="ty">IEmbeddingGenerator</span>&lt;<span class="kw">string</span>, <span class="ty">Embedding</span>&lt;<span class="kw">float</span>&gt;&gt; embedder =
    <span class="kw">await</span> <span class="ty">LocalEmbeddingGenerator</span>.CreateAsync();</span>

<span class="code-step step-item" data-step="2"><span class="cm">// Step 2: Turn strings into float[384] coordinate arrays</span>
<span class="kw">string</span>[] tickets = [
    <span class="st">"Payment gateway returns 502 error at checkout"</span>,
    <span class="st">"How do I reset my two-factor authentication?"</span>,
    <span class="st">"App crashes when uploading a profile picture"</span>,
    <span class="st">"Laptop screen is flickering after waking from sleep"</span>
];
<span class="kw">var</span> ticketVectors = <span class="kw">await</span> embedder.GenerateAsync(tickets);</span>

<span class="code-step step-item" data-step="3"><span class="cm">// Step 3: Embed the user's search query</span>
<span class="kw">var</span> queryVector = <span class="kw">await</span> embedder.GenerateVectorAsync(
    <span class="st">"credit card won't go through"</span>);

<span class="cm">// Step 4: Find the smallest angle using built-in .NET math!</span>
<span class="kw">var</span> best = tickets.Zip(ticketVectors)
    .Select(x =&gt; (
        Ticket: x.First,
        Score: <span class="ty">TensorPrimitives</span>.CosineSimilarity(
                   queryVector.Span, x.Second.Vector.Span)
    ))
    .OrderByDescending(x =&gt; x.Score)
    .First();</span></pre>
        </div>

        <div class="steps-column">
          <div class="v-card step-item" data-step="1">
            <h3>1. <code>IEmbeddingGenerator</code></h3>
            <p>Microsoft's official interface in <code>Microsoft.Extensions.AI</code>. Write your code once; swap between a local offline model, OpenAI, Azure, or Ollama in 1 line.</p>
          </div>
          <div class="v-card step-item" data-step="2">
            <h3>2. Strings → <code>float[384]</code></h3>
            <p><code>GenerateAsync(tickets)</code> turns each English sentence into 384 coordinates.</p>
          </div>
          <div class="v-card step-item" data-step="3">
            <h3>3. <code>TensorPrimitives.CosineSimilarity</code></h3>
            <p>Built right into .NET! Multiplies arrays in parallel using fast CPU instructions—no external math library needed.</p>
          </div>
          <div class="v-card step-item" data-step="4" style="border-color:var(--accent-3); background:rgba(52,211,153,0.1);">
            <h3 style="color:var(--accent-3);">▶ Run It Step-by-Step in Terminal</h3>
            <div class="mono" style="font-size:13px; background:#050811; padding:10px; border-radius:8px; margin-top:6px;">
              dotnet run --project src/01-LiveCoding-SemanticSearch
            </div>
          </div>
        </div>
      </div>
    `,
    speakerNotes: [
      "Let's see how this looks in real C# code. We don't need Python, we don't need a massive framework, and we don't even need an internet connection.",
      "Click 1: First, we create an IEmbeddingGenerator. That's the official standard interface in Microsoft.Extensions.AI. Here we load a local ONNX model file—think of ONNX like a PDF or MP3 file, but for a trained neural network.",
      "Click 2: Next, we have 4 support tickets as plain strings, and we call GenerateAsync(tickets). That turns each string into a float[384] coordinate array.",
      "Click 3: When a user types 'credit card won't go through', we embed their query and use LINQ + TensorPrimitives.CosineSimilarity—which is built right into .NET and uses fast parallel CPU math—to sort by the best match.",
      "Click 4: Switch to your terminal and run `dotnet run --project src/01-LiveCoding-SemanticSearch`! Press Enter at each step to walk the audience through the 4 live queries—including the final 'DISABLE MFA' cliffhanger!"
    ],
    glossary: [
      ["ONNX (Open Neural Network Exchange)", "A universal file format for AI models (like a .pdf for neural networks). Lets you download a model once and run it directly inside C# on your CPU with zero Python or cloud APIs."],
      ["Microsoft.Extensions.AI", "Microsoft's official .NET library that gives standard interfaces (like IEmbeddingGenerator and IChatClient) so you can swap AI providers without rewriting code."],
      ["TensorPrimitives / SIMD", "Built-in .NET class (`System.Numerics.Tensors`) that uses your CPU's wide math registers to multiply 8 to 16 numbers in a single clock tick instead of 1 by 1 in a slow loop."]
    ]
  },

  // --------------------------------------------------------------------------
  // SLIDE 9: ARCHITECTURAL PATTERNS #1 & #2 BEYOND CHATBOTS
  // --------------------------------------------------------------------------
  {
    id: "patterns-routing-and-caching",
    type: "concept",
    title: "Beyond Chatbots: Fast Backend Building Blocks",
    subtitle: "You don't need an LLM chatbot to get massive value from embeddings",
    stepsCount: 2,
    html: `
      <div class="cards-2">
        <div class="v-card step-item" data-step="1">
          <div class="step-badge">Pattern #1 • &lt; 1 Millisecond</div>
          <h3>🧭 Zero-Shot Ticket & Webhook Routing</h3>
          <p>How do you route incoming tickets to 5 teams without training a custom ML model?</p>
          <div class="diagram-box" style="margin-top:12px;">
            <div class="d-node">Incoming Ticket: <em>"Visa card declined on checkout"</em></div>
            <div class="d-arrow">↓ Compare angle against 5 Team Descriptions</div>
            <div class="d-row">
              <span class="d-pill win">💳 Billing (86%)</span>
              <span class="d-pill">🔐 Security (19%)</span>
              <span class="d-pill">📱 Mobile (11%)</span>
            </div>
          </div>
          <p style="margin-top:10px; font-size:14px; color:var(--accent-3);">
            <strong>Why engineers love it:</strong> Adding a 6th team is just adding 1 string to your config!
          </p>
        </div>

        <div class="v-card step-item" data-step="2">
          <div class="step-badge">Pattern #2 • Save 40%+ API Cost</div>
          <h3>⚡ Semantic Cache & Duplicate Blocker</h3>
          <p>Before creating a duplicate Jira bug or paying for a slow 3-second LLM call, check if a similar coordinate already exists:</p>
          <div class="diagram-box" style="margin-top:12px;">
            <div class="d-node">New Bug: <em>"Checkout fails with 502 on credit card"</em></div>
            <div class="d-arrow">↓ Check Cosine Similarity against recent items</div>
            <div class="d-node" style="border-color:var(--warn); color:var(--warn);">
              🎯 <strong>94% Match Found!</strong> Existing Jira #1042<br/>
              <em>"Payment gateway returns 502 error at checkout"</em>
            </div>
          </div>
          <p style="margin-top:10px; font-size:14px; color:var(--accent-3);">
            <strong>Rule of thumb:</strong> If similarity <code>&gt;= 90%</code>, return the cached answer immediately!
          </p>
        </div>
      </div>
    `,
    speakerNotes: [
      "Most people hear 'embeddings' and only think of RAG—stuffing text into a ChatGPT prompt. But some of the coolest uses of embeddings don't use an LLM at all! They are fast, deterministic backend building blocks.",
      "Click 1 (Pattern #1 — Zero-Shot Routing): Suppose you need to route incoming support webhooks to Billing, Security, or Mobile. Instead of training a classifier, embed 1 sentence describing each team at startup. When a ticket arrives, check which team vector it's closest to. Takes under 1 millisecond!",
      "Click 2 (Pattern #2 — Semantic Caching & Dedup): Before filing a duplicate Jira ticket or calling a slow $0.03 LLM API, check if a query with >90% similarity is already in your cache."
    ],
    glossary: [
      ["Zero-Shot Classification", "Categorizing data into buckets without training a custom machine learning model on labeled examples—just measuring distance to category descriptions."],
      ["RAG (Retrieval-Augmented Generation)", "Searching a database for relevant paragraphs and pasting them into an LLM prompt so the chatbot can answer using your private data."]
    ]
  },

  // --------------------------------------------------------------------------
  // SLIDE 10: ARCHITECTURAL PATTERNS #3 & #4 (ANOMALIES & RECOMMENDATIONS)
  // --------------------------------------------------------------------------
  {
    id: "patterns-anomaly-and-recs",
    type: "concept",
    title: "Outlier Radar & Instant Recommendations",
    subtitle: "Two more patterns using nothing but simple array averages",
    stepsCount: 2,
    html: `
      <div class="cards-2">
        <div class="v-card step-item" data-step="1">
          <div class="step-badge">Pattern #3 • Catch Unknown Bugs</div>
          <h3>🚨 Anomaly & Log Drift Detection</h3>
          <p>Regex alerts only catch errors you already know how to spell. Embeddings catch <strong>anything weird</strong>:</p>
          <div class="diagram-box" style="margin-top:12px; text-align:center;">
            <div style="display:inline-block; padding:14px 22px; border:2px dashed var(--accent-3); border-radius:999px; color:var(--accent-3);">
              🟢 Normal Logs Cluster<br/>
              <span style="font-size:12px; color:var(--text-1);">(CI builds, healthchecks, routine deploys)</span>
            </div>
            <div style="margin:10px 0; color:var(--danger); font-weight:700;">← Distance &gt; Threshold! →</div>
            <div class="d-node" style="border-color:var(--danger); color:#fecaca;">
              🔴 <strong>OUTLIER ALERT:</strong> Novel payment failure log landed far outside the normal circle!
            </div>
          </div>
        </div>

        <div class="v-card step-item" data-step="2">
          <div class="step-badge">Pattern #4 • Zero User-Tracking DB</div>
          <h3>🎯 Stateless "More Like This" Recommendations</h3>
          <p>How do you recommend articles without building a massive collaborative-filtering pipeline?</p>
          <div class="diagram-box" style="margin-top:12px;">
            <div class="d-node">User just read 3 articles → <code>Vec1, Vec2, Vec3</code></div>
            <div class="d-arrow">↓ Average the 3 arrays together: <code>(Vec1 + Vec2 + Vec3) / 3</code></div>
            <div class="d-node" style="border-color:var(--accent); color:var(--accent);">
              <strong>User's Live Taste Vector (<code>float[384]</code>)</strong><br/>
              Query nearest unread articles to that average point!
            </div>
          </div>
        </div>
      </div>
    `,
    speakerNotes: [
      "Here are two more patterns that just use simple averages of float arrays:",
      "Click 1 (Pattern #3 — Anomaly Detection): Average 100 normal application logs to find the center of 'normal behavior'. Whenever a new log line lands far away from that center circle, trigger an alert—catching brand-new failure modes that regex rules would never match.",
      "Click 2 (Pattern #4 — Stateless Recommendations): Want a 'Recommended For You' feed? Just average the vectors of the last 3 or 5 items the user clicked on! That average coordinate *is* their taste profile. Find the closest unread articles to that spot."
    ],
    glossary: [
      ["Centroid", "The center point of a cluster—calculated simply by averaging all the vectors in that group (`(v1 + v2 + ... + vN) / N`)."]
    ]
  },

  // --------------------------------------------------------------------------
  // SLIDE 11: PATTERN #5 — MULTIMODAL SHARED SPACE (TEXT + IMAGE + AUDIO)
  // --------------------------------------------------------------------------
  {
    id: "multimodal-shared-space",
    type: "concept",
    title: "How Text, Images & Audio Share the Exact Same Map",
    subtitle: "Searching untagged screenshots and audio clips with plain English text",
    stepsCount: 3,
    html: `
      <div class="flow-vertical">
        <div class="cards-3 step-item" data-step="1">
          <div class="v-card">
            <h3>📝 Text Input</h3>
            <p><code>"golden retriever puppy"</code><br/>Chopped into word tokens.</p>
          </div>
          <div class="v-card">
            <h3>🖼️ Image Input</h3>
            <p><code>IMG_0042.jpg</code> (Untagged photo)<br/>Sliced into <strong>16×16 pixel puzzle patches</strong>.</p>
          </div>
          <div class="v-card">
            <h3>🔊 Audio Input</h3>
            <p><code>barking.wav</code><br/>Converted into a <strong>visual sound wave image</strong> (spectrogram).</p>
          </div>
        </div>

        <div class="v-card step-item" data-step="2" style="border-color:var(--accent-3); text-align:center;">
          <h3 style="color:var(--accent-3);">🎯 Trained Side-by-Side Into ONE Shared Coordinate Space!</h3>
          <p>Models like <strong>CLIP</strong>, <strong>SigLIP</strong>, and <strong>Gemini Embedding 2</strong> force the photo of a dog, the sound of a bark, and the text <em>"golden retriever"</em> to output the <strong>exact same coordinates</strong>: <code>[ +0.21, -0.44, +0.79, ... ]</code></p>
        </div>

        <div class="v-card step-item" data-step="3" style="border-color:var(--warn); background:rgba(251,191,36,0.08);">
          <h3 style="color:var(--warn);">💡 Pro Nuance: "The Modality Gap" (Oil & Water)</h3>
          <p>Even in a shared space, image dots and text dots hover in slightly offset clouds (like oil and water). Newer models (SigLIP / Gemini Embedding 2) shrink this gap so you can search untagged bug screenshots, PDFs, and docs in a single query!</p>
        </div>
      </div>
    `,
    speakerNotes: [
      "What about images, PDFs, and audio? How can you type 'diagram with red arrows' or 'golden retriever' and search a folder of untagged JPG files?",
      "Click 1: First, how do images and audio turn into tokens? An image is sliced into a grid of 16x16 pixel puzzle pieces. Audio is turned into a visual frequency heatmap (spectrogram) and sliced the exact same way.",
      "Click 2: Multimodal models (like CLIP, SigLIP, or Gemini Embedding 2) train the Text, Image, and Audio encoders together on millions of captioned photos and videos—forcing all three to output the exact same coordinate neighborhood!",
      "Click 3: One cool engineering subtlety called 'The Modality Gap': even in shared models, image vectors and text vectors tend to hover in slightly separated sub-clouds, like oil and water. Now let's run all 5 of these patterns in C#!"
    ],
    glossary: [
      ["Multimodal Model (CLIP / SigLIP / Gemini Embedding 2)", "An AI model that embeds different media types (text, images, audio, PDFs) into the exact same coordinate space so you can search images using text."],
      ["ViT (Vision Transformer) Patches", "Chopping an image into small 16x16 pixel squares and feeding them into a Transformer just like words in a sentence."],
      ["The Modality Gap", "The tendency for image embeddings and text embeddings to sit in slightly offset sub-regions of the shared space."]
    ]
  },

  // --------------------------------------------------------------------------
  // SLIDE 12: LIVE C# CODE DEMO 2 — 5 PATTERNS BEYOND RAG
  // --------------------------------------------------------------------------
  {
    id: "code-demo-5-patterns",
    type: "code",
    title: "💻 Live C# Demo: 5 Backend Patterns in Action",
    subtitle: "Step-by-step interactive console walkthrough (src/02-PatternsBeyondRag)",
    stepsCount: 1,
    html: `
      <div class="demo-hero">
        <div class="demo-badge">💻 STEP-BY-STEP C# CONSOLE DEMO</div>
        <h2>Run All 5 Patterns One by One</h2>
        <p style="max-width:760px; margin: 12px auto 20px; color:var(--text-1); font-size:17px;">
          Our second C# project pauses cleanly between each pattern so we can inspect the code and live output one step at a time:
        </p>
        <div class="mono" style="font-size:16px; background:#050811; border:1px solid var(--accent); padding:14px 22px; border-radius:10px; display:inline-block; color:var(--accent-3);">
          dotnet run --project src/02-PatternsBeyondRag
        </div>
        <div class="cards-3" style="margin-top:24px; text-align:left;">
          <div class="v-card">
            <h3>Patterns 1 & 2</h3>
            <p><strong>Zero-Shot Webhook Router</strong> (&lt;1ms) + <strong>Duplicate Jira Bug Catcher</strong>.</p>
          </div>
          <div class="v-card">
            <h3>Patterns 3 & 4</h3>
            <p><strong>Production Log Anomaly Radar</strong> + <strong>Stateless Article Recommender</strong>.</p>
          </div>
          <div class="v-card">
            <h3>Pattern 5</h3>
            <p><strong>Cross-Modal Vault Search</strong> (Searching untagged images, audio & PDFs with text).</p>
          </div>
        </div>
      </div>
    `,
    speakerNotes: [
      "Switch to your terminal and run `dotnet run --project src/02-PatternsBeyondRag`.",
      "Because we updated it to be step-by-step, it will show you Pattern #1 (Zero-Shot Routing) and wait for you to press Enter before clearing/moving to Pattern #2, #3, #4, and #5!",
      "You only need to spend ~45 seconds on each pattern—point out the tiny C# snippet printed at the top of each step and the green result bars below it."
    ],
    glossary: [
      ["TensorPrimitives.Add / Divide", "Built-in .NET methods that add or divide entire float[] arrays in one line—used here to average vectors into a cluster centroid or user taste profile."]
    ]
  },

  // --------------------------------------------------------------------------
  // SLIDE 13: WHY EMBEDDING MODELS DIFFER (5 SIMPLE QUESTIONS)
  // --------------------------------------------------------------------------
  {
    id: "how-models-differ",
    type: "concept",
    title: "Why Are There 100 Different Embedding Models?",
    subtitle: "5 practical differences that actually matter when you pick one",
    stepsCount: 5,
    html: `
      <div class="flow-vertical" style="gap:10px;">
        <div class="v-card step-item" data-step="1" style="padding:14px 18px;">
          <h3>1. Short Question vs. Long Answer (Symmetric vs. Asymmetric)</h3>
          <p>A 4-word search query doesn't look like a 500-word manual page. Retrieval models trained for Q&A often require adding <code>"query: "</code> before user searches and <code>"passage: "</code> before documents.</p>
        </div>

        <div class="v-card step-item" data-step="2" style="padding:14px 18px; border-color:rgba(248,113,113,0.4);">
          <h3 style="color:var(--danger);">2. The Silent Cutoff Trap (Context Window: 512 vs. 8,000+ Words)</h3>
          <p>Older models cap at <strong>512 tokens (~1 page)</strong> and <strong>silently delete</strong> pages 2–10 without throwing an error! Always check the model's max token limit.</p>
        </div>

        <div class="v-card step-item" data-step="3" style="padding:14px 18px;">
          <h3>3. Coordinate Array Size (384 vs. 3,072 Numbers)</h3>
          <p><code>384 numbers</code> (like local MiniLM) is tiny and fast. <code>3,072 numbers</code> (like OpenAI large / Gemini) captures subtle nuance, but takes <strong>8x more RAM</strong>.</p>
        </div>

        <div class="v-card step-item" data-step="4" style="padding:14px 18px;">
          <h3>4. Language & Domain (English vs. 100 Languages vs. Code)</h3>
          <p>An English-only model maps Lithuanian, Spanish, or C# code almost randomly. Pick a multilingual (<code>BGE-M3</code>, <code>Qwen3</code>) or code-specific (<code>Voyage-Code</code>) model when needed.</p>
        </div>

        <div class="v-card step-item" data-step="5" style="padding:14px 18px; border-color:var(--accent-3);">
          <h3 style="color:var(--accent-3);">5. Local <code>.onnx</code> File (Free & Private) vs. Cloud API</h3>
          <p>Local ONNX models run inside your .NET process in <code>&lt;2ms</code> for <code>$0</code> and never send sensitive data outside your server. Cloud APIs save you from managing CPU/GPU batch jobs.</p>
        </div>
      </div>
    `,
    speakerNotes: [
      "If you look online, there are hundreds of embedding models. How do they actually differ? Walk through these 5 quick points one click at a time:",
      "Click 1 (Short Query vs Long Doc): Matching two short sentences is different from matching a 3-word query against a 2-page article. Many Q&A models expect you to prefix queries with 'query: ' and docs with 'passage: '.",
      "Click 2 (The Silent Cutoff Trap): This burns so many teams! Older models stop reading after 512 tokens (about 350 words) and silently throw away the rest of your document without raising an exception!",
      "Click 3 (Array Size): Models output anywhere from 384 numbers to 3,072 numbers. More numbers = finer nuance, but 8x more memory.",
      "Click 4 (Language & Code): Don't use an English-only model on multilingual text or source code.",
      "Click 5 (Local ONNX vs Cloud API): You can run great 384- to 768-dimension models 100% locally in C# for free, or call OpenAI/Gemini/Voyage via the exact same IEmbeddingGenerator interface."
    ],
    glossary: [
      ["Context Window", "The maximum number of words/tokens a model can read at once before it chops off the rest."],
      ["Asymmetric Retrieval", "Matching a very short search query against a long document passage."]
    ]
  },

  // --------------------------------------------------------------------------
  // SLIDE 14: ACT 5 — WHERE EMBEDDINGS LIE (NEGATION & EXACT CODES)
  // --------------------------------------------------------------------------
  {
    id: "gotcha-negation-and-codes",
    type: "concept",
    title: "Where Embeddings Lie: The 'NOT' & Exact Code Trap",
    subtitle: "Why pure vector search can confidently give you the exact opposite answer",
    stepsCount: 3,
    html: `
      <div class="flow-vertical">
        <div class="cards-2 step-item" data-step="1">
          <div class="v-card" style="border-color:var(--danger);">
            <h3 style="color:var(--danger);">Trap A: Blind to "NOT / WITHOUT / DISABLE"</h3>
            <p style="margin-top:6px;">
              • <em>"Snacks <strong>WITHOUT</strong> peanuts"</em> matches <strong>Peanut Butter Bars at 94%!</strong><br/>
              • <em>"How do I <strong>DISABLE</strong> MFA?"</em> matches <strong>"How to ENABLE MFA" at 95%!</strong>
            </p>
          </div>
          <div class="v-card" style="border-color:var(--danger);">
            <h3 style="color:var(--danger);">Trap B: Blind to Exact Error Codes & IDs</h3>
            <p style="margin-top:6px;">
              • <em>"Fix HTTP Error <strong>504</strong>"</em> matches <strong>"HTTP Error 503" at 93%!</strong><br/>
              • Part number <code>SKU-8841</code> vs <code>SKU-8842</code> look identical to a vector.
            </p>
          </div>
        </div>

        <div class="v-card step-item" data-step="2" style="text-align:center;">
          <h3>Why Does This Happen?</h3>
          <p>Embeddings measure <strong>overall topic neighborhood</strong> (<em>"this sentence is about peanuts"</em> or <em>"this is about MFA"</em>). They do <strong>not</strong> execute boolean logic!</p>
        </div>

        <div class="v-card step-item" data-step="3" style="border-color:var(--accent-3); background:rgba(52,211,153,0.1);">
          <h3 style="color:var(--accent-3);">✅ The Production Fix: Hybrid Search (Vectors + Keywords)</h3>
          <p>Run <strong>Vector Search</strong> (for meaning & synonyms) AND <strong>Keyword Search (BM25)</strong> (for exact words & numbers) side-by-side, then combine their top rankings!</p>
        </div>
      </div>
    `,
    speakerNotes: [
      "Remember in our first C# demo when we asked 'How do I DISABLE two-factor authentication?' and it matched 'How do I RESET MFA' at 98%? Here is why—and this is the #1 gotcha in production.",
      "Click 1: Dense vectors are blind to negation ('without', 'disable') and exact numbers ('Error 503' vs 'Error 504'). If someone says 'I am allergic to peanuts, show me snacks WITHOUT peanuts', pure vector search sees the word 'peanuts' and happily recommends Crunchy Peanut Butter Bars at #1!",
      "Click 2: Why? Because embeddings only measure general topic neighborhood ('this is about peanuts' or 'this is about MFA').",
      "Click 3: The fix is Hybrid Search: always combine Vector Search (which understands synonyms) with classic Keyword Search (which checks exact words like 'DISABLE' or '504'), and merge their rankings!"
    ],
    glossary: [
      ["BM25 (Best Match 25)", "The gold-standard keyword search algorithm used by Elasticsearch and PostgreSQL full-text search. Counts exact word matches and gives extra weight to rare words like '504'."],
      ["Hybrid Search + RRF (Reciprocal Rank Fusion)", "Running Vector Search and Keyword Search together, then combining their rank positions (#1, #2, #3) so a document that scores well in both wins #1."]
    ]
  },

  // --------------------------------------------------------------------------
  // SLIDE 15: DEMO 3 — BREAKING VECTORS & FIXING THEM WITH HYBRID SEARCH
  // --------------------------------------------------------------------------
  {
    id: "demo-hybrid-search",
    type: "demo",
    title: "🎬 Live Demo: Watching Hybrid Search Rescue #1",
    subtitle: "Testing the Peanut Allergy, Disable MFA, and Error 504 traps live",
    stepsCount: 1,
    demoUrl: "../visualizer/index.html?tab=tab-gotchas",
    html: `
      <div class="demo-hero">
        <div class="demo-badge">🎬 INTERACTIVE VISUALIZER • TAB 4</div>
        <h2>See Pure Vectors Fail — Then Flip On Hybrid Search</h2>
        <p style="max-width:760px; margin: 12px auto 24px; color:var(--text-1); font-size:18px; line-height:1.55;">
          In Tab 4 of the Visualizer, we can test 3 real-world traps and watch the right document jump from <strong>#3 or #4 straight to ★ #1 WINNER</strong> the moment we switch from <em>Pure Vector Search</em> to <em>Hybrid Search</em>!
        </p>
        <div class="demo-actions">
          <a class="btn-launch" href="../visualizer/index.html?tab=tab-gotchas" target="_blank">
            🛡️ Open Negation & Hybrid Search Lab (Tab 4) ↗
          </a>
        </div>
        <div class="cards-3" style="margin-top:28px; text-align:left;">
          <div class="v-card">
            <h3>🥜 Trap 1: Peanut Allergy</h3>
            <p>Watch Pure Vector rank <em>Peanut Butter Bars #1</em>, then watch Hybrid Search promote <em>Safe Nut-Free Granola</em> from #4 to ★ #1!</p>
          </div>
          <div class="v-card">
            <h3>🔐 Trap 2: Disable MFA</h3>
            <p>Watch <em>"Permanently DISABLE MFA"</em> jump past <em>"How to ENABLE MFA"</em> to ★ #1.</p>
          </div>
          <div class="v-card">
            <h3>🚨 Trap 3: HTTP Error 504</h3>
            <p>Watch exact keyword matching rescue the <em>Error 504 Runbook</em> over <em>Error 503</em>.</p>
          </div>
        </div>
      </div>
    `,
    speakerNotes: [
      "Click the button to open Tab 4 ('4. Where Vectors Fail & Hybrid Fix') in the Visualizer.",
      "1. Start on Trap #1 ('Peanut Allergy — Snacks WITHOUT Peanuts'). Point out the big red #1 card: Pure Vector Search puts Crunchy Peanut Butter Bars at #1 while Safe Nut-Free Granola is buried at #4!",
      "2. Now click the button '✅ 2. Hybrid Search (Vector + Keyword)'. Watch Safe Nut-Free Granola jump +3 spots to ★ #1 WINNER!",
      "3. Click Trap #2 ('Disable MFA') and Trap #3 ('Error 504') and flip between Pure Vector and Hybrid Search to show how consistent this pattern is."
    ],
    glossary: [
      ["Reciprocal Rank Fusion (RRF)", "Instead of trying to add a percentage score to a keyword score, RRF just uses rank positions: `1/(60 + VectorRank) + 1/(60 + KeywordRank)`."]
    ]
  },

  // --------------------------------------------------------------------------
  // SLIDE 16: THE 120 GB RAM BILL SHOCK & STEP-BY-STEP SHRINKER DEMO
  // --------------------------------------------------------------------------
  {
    id: "demo-ram-shrinker",
    type: "demo",
    title: "🎬 Step-by-Step Demo: Shrinking 120 GB of RAM Down to 600 MB",
    subtitle: "Two clever tricks: Matryoshka Slicing (Chop Length) & Quantization (Round Numbers)",
    stepsCount: 3,
    demoUrl: "../visualizer/index.html?tab=tab-mrl",
    html: `
      <div class="flow-vertical">
        <div class="cards-3">
          <div class="v-card step-item" data-step="1" style="border-color:var(--danger);">
            <div class="step-badge">Step 1 • The Problem</div>
            <h3 style="color:var(--danger);">💸 The 120 GB RAM Cliff</h3>
            <p>10 million documents × 3,072 decimal numbers × 4 bytes each = <strong>~120 GB of RAM</strong> just to hold the vectors!</p>
          </div>

          <div class="v-card step-item" data-step="2" style="border-color:var(--accent-3);">
            <div class="step-badge">Step 2 • Trick #1 (Length)</div>
            <h3 style="color:var(--accent-3);">🪆 Matryoshka Slicing</h3>
            <p>Like Russian Nesting Dolls, modern models put the <strong>big picture in the first 256–512 numbers</strong>. Chop off the tail → <strong>6x–12x smaller</strong>!</p>
          </div>

          <div class="v-card step-item" data-step="3" style="border-color:var(--warn);">
            <div class="step-badge">Step 3 • Trick #2 (Size)</div>
            <h3 style="color:var(--warn);">🗜️ Quantization (Rounding)</h3>
            <p>Round 32-bit decimals (<code>+0.142857</code>) to 8-bit whole numbers (<code>+18</code>, <strong>4x smaller</strong>) or 1-bit signs (<code>+ / -</code>, <strong>32x smaller</strong>)!</p>
          </div>
        </div>

        <div style="text-align:center; margin-top:18px;">
          <a class="btn-launch" href="../visualizer/index.html?tab=tab-mrl" target="_blank">
            🪆 Open 4-Step Interactive RAM Shrinker (Visualizer Tab 5) ↗
          </a>
        </div>
      </div>
    `,
    speakerNotes: [
      "What happens when your database grows to 10 million chunks? Let's reveal this step by step, and then walk through our 4-step wizard in Tab 5 of the Visualizer.",
      "Click 1 (The Problem): 10 million documents times 3,072 numbers times 4 bytes per float equals 120 Gigabytes of RAM! Cloud RAM is expensive.",
      "Click 2 (Trick #1 — Matryoshka Slicing): Matryoshka means Russian nesting dolls. Modern models are trained to put the most important meaning in the first 256 or 512 numbers, and tiny details at the end. So you can literally take scissors and chop a 3,072-number array down to 512 numbers—cutting RAM 6x while keeping ~96% accuracy!",
      "Click 3 (Trick #2 — Quantization): Second, instead of storing 32-bit decimals with 7 decimal places, round each number to a 1-byte integer (-127 to +127) or even a single bit (1 for positive, 0 for negative). Now click the button to open Tab 5 ('5. Step-by-Step RAM Shrinker') and click through Steps 1, 2, 3, and 4 live!"
    ],
    glossary: [
      ["Matryoshka Representation Learning (MRL)", "Training an embedding model so the early numbers in the array hold the main topic and later numbers hold fine details—allowing you to truncate the array to any shorter length."],
      ["Quantization (int8 / 1-bit)", "Compressing each number in the vector from a 4-byte float32 down to a 1-byte integer (int8, 4x smaller) or a 1-bit sign (binary, 32x smaller)."]
    ]
  },

  // --------------------------------------------------------------------------
  // SLIDE 17: TWO FINAL PRODUCTION TRAPS (CHUNKING & MODEL MIGRATION)
  // --------------------------------------------------------------------------
  {
    id: "chunking-and-migration",
    type: "concept",
    title: "Two Final Production Rules Before You Ship",
    subtitle: "Context-Preserving Chunking & What Happens When You Change Models",
    stepsCount: 2,
    html: `
      <div class="cards-2">
        <div class="v-card step-item" data-step="1">
          <div class="step-badge">Trap #3 • Orphaned Pronouns</div>
          <h3>📄 Never Embed a Paragraph Without Its Header</h3>
          <p>If you chop a PDF every 500 characters, you get orphaned chunks like:</p>
          <div class="mini-code" style="border-color:var(--danger); color:#fecaca;">
            ❌ <em>"It returns a 502 error when payload exceeds 4MB."</em><br/>
            (Wait—what is <strong>"It"</strong>?! Score = <strong>16%</strong>)
          </div>
          <p style="margin-top:8px;"><strong>The Fix:</strong> Prepend the document & section title before embedding:</p>
          <div class="mini-code" style="border-color:var(--accent-3); color:var(--accent-3);">
            ✅ <em>"[Doc: Stripe Checkout &gt; Limits] It returns a 502 error..."</em><br/>
            (Score jumps to <strong>78%</strong>!)
          </div>
        </div>

        <div class="v-card step-item" data-step="2">
          <div class="step-badge">Trap #4 • Model Upgrade Lock-In</div>
          <h3>🔄 Vector Spaces Are 100% Incompatible Across Models!</h3>
          <p>Even if <strong>Model A</strong> and <strong>Model B</strong> both output <code>float[384]</code>, Axis #1 in Model A might mean <em>"Urgency"</em> while Axis #1 in Model B means <em>"Food"</em>!</p>
          <div class="mini-code" style="border-color:var(--warn); color:#fde68a;">
            ⚠️ Comparing a Model B query against Model A database rows gives <strong>~0% random noise!</strong>
          </div>
          <p style="margin-top:8px;"><strong>The Fix (Blue/Green Columns):</strong> Store <code>model_version</code> on every row. When upgrading, backfill a new column <code>embedding_v2</code> in the background before switching live traffic!</p>
        </div>
      </div>
    `,
    speakerNotes: [
      "Two final engineering lessons from real production systems—and these are directly from our docs and third C# demo:",
      "Click 1 (Orphaned Chunks): When you split a long document into paragraphs, a paragraph might say 'It fails when the payload exceeds 4MB.' What is 'It'?! The word 'Stripe Checkout' was in the heading at the top of the page! Always prepend `[Document Title > Section Heading]` to every chunk before you embed it.",
      "Click 2 (Changing Models): What happens next year when a better embedding model comes out and you swap your model in code? Total disaster if you don't re-index! Every model invents its own coordinate system. Comparing Model B's query vector against Model A's database vectors gives 0% random noise. Always store `model_id` in your table and use Blue/Green columns (`embedding_v1`, `embedding_v2`) to backfill before flipping traffic!"
    ],
    glossary: [
      ["Chunking", "Splitting a long document (like a 20-page PDF) into smaller paragraph-sized pieces before embedding each piece."],
      ["Blue/Green Vector Migration", "Keeping your existing `embedding_v1` column serving live users while a background worker computes `embedding_v2` with the new model, then switching over with zero downtime."]
    ]
  },

  // --------------------------------------------------------------------------
  // SLIDE 18: LIVE C# CODE DEMO 3 & TAKE-HOME CHEAT SHEET
  // --------------------------------------------------------------------------
  {
    id: "final-code-and-cheatsheet",
    type: "code",
    title: "💻 Live C# Gotchas Demo & Take-Home Cheat Sheet",
    subtitle: "Everything in this repository is ready to run on your laptop today",
    stepsCount: 2,
    html: `
      <div class="two-col-slide">
        <div class="v-card step-item" data-step="1" style="border-color:var(--accent);">
          <h3>▶ Run C# Demo #3 (All 4 Gotchas & Fixes Step-by-Step)</h3>
          <div class="mono" style="font-size:14px; background:#050811; padding:12px; border-radius:8px; margin:10px 0; color:var(--accent-3);">
            dotnet run --project src/03-ProductionGotchasAndFixes
          </div>
          <p style="font-size:14.5px;">Walks step-by-step through:</p>
          <ul style="padding-left:20px; margin-top:8px; color:var(--text-1); line-height:1.65;">
            <li><strong>Step 1:</strong> Hybrid Search (Vector + BM25 + RRF) in C#</li>
            <li><strong>Step 2:</strong> Matryoshka Slicing & <code>int8</code> / <code>1-bit</code> Quantization</li>
            <li><strong>Step 3:</strong> Contextual Breadcrumb Chunking (<code>16%</code> → <code>78%</code>)</li>
            <li><strong>Step 4:</strong> Model v1 vs Model v2 Incompatibility & Blue/Green Flip</li>
          </ul>
        </div>

        <div class="v-card step-item" data-step="2" style="border-color:var(--accent-3);">
          <h3 style="color:var(--accent-3);">🏆 Where Should You Store Vectors?</h3>
          <div class="mini-code" style="margin-top:8px;">
            <strong>• Up to 25,000 items:</strong><br/>
            Just use in-memory C# <code>TensorPrimitives</code> (&lt; 2ms, $0 infra!)
          </div>
          <div class="mini-code" style="margin-top:8px;">
            <strong>• 25,000 to 25 Million items:</strong><br/>
            <strong>PostgreSQL + <code>pgvector</code></strong> (Keep SQL + vectors in one DB)
          </div>
          <div class="mini-code" style="margin-top:8px;">
            <strong>• 50 Million to 1 Billion+ items:</strong><br/>
            Dedicated Vector DB (<strong>Qdrant, Milvus, Weaviate</strong>)
          </div>
        </div>
      </div>
    `,
    speakerNotes: [
      "Click 1: Switch to your terminal and run `dotnet run --project src/03-ProductionGotchasAndFixes` to step through the 4 fixes in C#.",
      "Click 2: Leave this final slide up during Q&A! It answers the #1 question architects ask: 'Do I need to buy a specialized Vector Database?' Under 25K items, C# in-memory is <2ms. Up to 25M items, Postgres with pgvector is all you need. Above 50M+, look at Qdrant or Milvus. Thank you, and let's open it up for questions!"
    ],
    glossary: [
      ["pgvector", "The open-source PostgreSQL extension that adds a `vector(N)` column type and fast HNSW vector search indexes directly inside regular Postgres."],
      ["HNSW (Hierarchical Navigable Small World)", "The standard graph index used by pgvector, Qdrant, and Milvus to find nearest neighbors among millions of vectors in milliseconds without scanning every row."]
    ]
  }
];
