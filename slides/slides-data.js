// ============================================================================
// SHARED SLIDE DECK & SPEAKER PRESENTER DATA (v3 — Strict 1-to-1 Step Mapping)
// Every slide starts at Step 1.
// For every slide: `stepsCount === speakerNotes.length` (Step 1 = speakerNotes[0], etc.)
// ============================================================================

window.TALK_SLIDES = [
  // --------------------------------------------------------------------------
  // SLIDE 1: TITLE (3 Steps)
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
            <p>How computers turn fuzzy human ideas into simple GPS coordinates.</p>
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
      "Welcome everyone! Today we are looking under the hood of Vector Embeddings. First, we'll build pure visual intuition—you don't need a math degree to picture embeddings as simple GPS coordinates for meaning.",
      "Second, we'll write clean C# code running 100% locally on this laptop—showing not just search, but five backend patterns you can drop into regular microservices.",
      "And third, we'll look at production reality: where embeddings fail, how to shrink a 120-gigabyte index down to 600 megabytes, and what happens when you switch models."
    ],
    glossary: [
      ["Embedding / Vector", "Simply an array of decimal numbers (like float[384]) that represents the meaning of a piece of text, image, or audio."],
      ["Semantic", "Matching by meaning and intent rather than matching exact letters."]
    ]
  },

  // --------------------------------------------------------------------------
  // SLIDE 2: THE PROBLEM — BYTES VS MEANING (3 Steps)
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
      "Think about the last time you used a search bar on a support website. Say a user types 'My laptop screen is flickering'. A traditional SQL LIKE query checks for the exact letters F-L-I-C-K-E-R, finds nothing, and returns 0 Results Found.",
      "What makes this painful is that the exact answer IS sitting right inside your database—it's just titled 'Troubleshooting display refresh glitches'. Notice those two sentences share zero words.",
      "That is the exact gap embeddings bridge: instead of comparing spelling letter-by-letter, we convert both sentences into coordinates on a map of meaning."
    ],
    glossary: [
      ["Lexical / Keyword Search", "Searching by exact words or characters (like SQL LIKE or Ctrl+F). Fast, but blind to synonyms."]
    ]
  },

  // --------------------------------------------------------------------------
  // SLIDE 3: ELI5 — THE 2D TASTE MAP (4 Steps)
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
            <line x1="55" y1="270" x2="430" y2="270" stroke="#64748b" stroke-width="2" />
            <line x1="55" y1="270" x2="55" y2="25" stroke="#64748b" stroke-width="2" />
            <text x="160" y="302" fill="#cbd5e1" font-size="13" font-weight="bold">X-Axis: Sweetness (0 → 10)</text>
            <text x="16" y="190" fill="#cbd5e1" font-size="13" font-weight="bold" transform="rotate(-90 16,190)">Y-Axis: Crunchiness (0 → 10)</text>

            <g class="step-svg" data-step="1">
              <circle cx="335" cy="70" r="9" fill="#f87171" />
              <text x="260" y="48" fill="#f8fafc" font-size="14" font-weight="bold">🍎 Apple [7.5, 8.0]</text>
            </g>

            <g class="step-svg" data-step="2">
              <circle cx="315" cy="90" r="9" fill="#34d399" />
              <text x="220" y="116" fill="#34d399" font-size="14" font-weight="bold">🍐 Crispy Pear [7.0, 7.5]</text>
              <circle cx="325" cy="80" r="34" fill="none" stroke="#34d399" stroke-dasharray="4 4" stroke-width="1.5" />
            </g>

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
      "Let's strip away all the AI jargon. Imagine a chart with two simple scores from 0 to 10: Sweetness across the bottom, and Crunchiness up the side. An Apple is sweet—7.5—and crunchy—8.0. So inside the computer, an Apple is just a 2-number array: [7.5, 8.0].",
      "Now where does a Crispy Pear go? It's also sweet and crunchy, around [7.0, 7.5]. Notice how it naturally lands right next to the Apple!",
      "Meanwhile, a salty Potato Chip at [1.2, 9.2] and a soft slice of Chocolate Cake at [9.4, 1.4] land in completely different corners of the chart.",
      "Without doing any linear algebra, you just created a 2-dimensional vector embedding! An embedding is simply a list of trait scores where closeness on the map equals similarity in meaning."
    ],
    glossary: [
      ["2D Embedding", "An array of 2 numbers [X, Y] representing two traits (here: Sweetness and Crunchiness)."],
      ["Dimension", "One number (or axis) in the coordinate array. A 2D map has 2 dimensions; a 384D model has 384 numbers per item."]
    ]
  },

  // --------------------------------------------------------------------------
  // SLIDE 4: DEMO 1 — INTERACTIVE 2D TASTE MAP & WHY ANGLE BEATS RULER (1 Step)
  // --------------------------------------------------------------------------
  {
    id: "demo-2d-map",
    type: "demo",
    title: "🎬 Live Demo: The 2D Taste Map & Why Angle Beats Ruler Distance",
    subtitle: "Comparing Cosine Angle vs. Straight-Line Ruler Distance in practice",
    stepsCount: 1,
    demoUrl: "../visualizer/index.html?tab=tab-2d",
    stageCue: "Click the button on the slide (or Tab 1 in the Visualizer) to open the 2D Taste Map. Hover the 3 metric cards, then click '⚡ Demo: Why Angle Beats Ruler'.",
    html: `
      <div class="demo-hero step-item" data-step="1">
        <div class="demo-badge">🎬 INTERACTIVE VISUALIZER • TAB 1</div>
        <h2>Why Do We Measure the Angle Instead of Using a Ruler?</h2>
        <p style="max-width:820px; margin: 12px auto 24px; color:var(--text-1); font-size:17.5px; line-height:1.55;">
          Imagine a <strong>1-cup recipe</strong> (1 cup sugar, 1 cup flour = <code>[1, 1]</code>) vs. a <strong>5-cup batch of the exact same recipe</strong> (<code>[5, 5]</code>). A <strong>ruler</strong> says they are far apart just because one batch is bigger! The <strong>compass angle</strong> sees they have the exact same ratio (<code>0° angle = 100% match</code>).
        </p>
        <div class="demo-actions">
          <a class="btn-launch" href="../visualizer/index.html?tab=tab-2d" target="EmbeddingsVisualizerWindow">
            🚀 Open 2D Taste Map (Visualizer Tab 1) ↗
          </a>
        </div>
        <div class="cards-3" style="margin-top:26px; text-align:left;">
          <div class="v-card">
            <h3>1. Plot a Custom Food</h3>
            <p>Enter a Sweetness (0–10) and Crunchiness (0–10) score to create a live <code>float[2]</code> coordinate.</p>
          </div>
          <div class="v-card">
            <h3>2. Hover the 3 Metrics</h3>
            <p>See what <strong>Cosine Similarity %</strong>, <strong>Angle °</strong>, and <strong>Ruler Distance</strong> mean in plain English.</p>
          </div>
          <div class="v-card">
            <h3>3. Click "Why Angle Beats Ruler"</h3>
            <p>See why a short tweet <code>[3.5, 4.0]</code> and a long book <code>[7.0, 8.0]</code> have a <strong>0° angle (100% match)</strong>!</p>
          </div>
        </div>
      </div>
    `,
    speakerNotes: [
      "Let's open Tab 1 of our Visualizer to see how computers actually measure similarity on this map, and why AI measures the compass angle between arrows instead of using a straight ruler."
    ],
    glossary: [
      ["Why Angle Beats Ruler Distance", "A 1-sentence tweet about Apples has fewer words, so its arrow is shorter [3.5, 4.0]. A 10-page book about Apples repeats apple traits many times, making its arrow longer [7.0, 8.0]. A straight ruler says their tips are far apart—getting tricked by length! Measuring the angle from (0,0) ignores length and sees they point in the exact same compass direction (0° = 100% match)."],
      ["Why We Normalize Vectors (Scale Length to 1.0)", "If we shrink the long book's arrow and stretch the short tweet's arrow so every arrow has length 1.0, both dots land on the exact same spot on a unit circle!"]
    ]
  },

  // --------------------------------------------------------------------------
  // SLIDE 5: SCALING FROM 2 AXES TO 384 AXES (3 Steps)
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
      "So how do we go from a 2-number food chart to real software engineering? On our food map, we only had 2 traits: Sweetness and Crunchiness.",
      "If we want to represent support tickets, documentation, or code, 2 traits aren't enough. We need traits like: Is this about billing? Is the user angry? Is it a login issue? Is it on mobile?",
      "An AI embedding model is simply a function that takes any piece of text and outputs an array of 384 (or up to 3,072) decimal numbers—where each number scores one learned trait."
    ],
    glossary: [
      ["float[384]", "A standard C# array of 384 decimal numbers. That array *is* the vector embedding."]
    ]
  },

  // --------------------------------------------------------------------------
  // SLIDE 6: HOW DOES THE AI CREATE THOSE 384 NUMBERS? (4 Steps)
  // --------------------------------------------------------------------------
  {
    id: "four-stage-pipeline",
    type: "concept",
    title: "Under the Hood: How Are Embeddings Generated?",
    subtitle: "How a whole paragraph becomes one clean coordinate array without turning into mush",
    stepsCount: 4,
    html: `
      <div class="cards-4">
        <div class="v-card step-item" data-step="1">
          <div class="step-badge">Step 1</div>
          <h3>🔪 Chop (Tokenize)</h3>
          <p>Split the sentence into sub-word puzzle pieces (tokens):</p>
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
          <p>Every word absorbs meaning from its neighbors in the sentence:</p>
          <div class="mini-code">
            "cracked my <strong>apple</strong> screen" → 📱 Tech<br/><br/>
            "swiped my card and <strong>it</strong> failed" → <strong>"it"</strong> absorbs 💳 Card!
          </div>
        </div>

        <div class="v-card step-item" data-step="3">
          <div class="step-badge">Step 3</div>
          <h3>🗜️ Squash (Average)</h3>
          <p>Average all word vectors into <strong>1 paragraph vector</strong>:</p>
          <div class="mini-code">
            • Filler words (<em>"the", "yesterday"</em>) cancel out.<br/>
            • Key words (<em>"card", "checkout", "502"</em>) pull together!
          </div>
        </div>

        <div class="v-card step-item" data-step="4">
          <div class="step-badge">Step 4</div>
          <h3>🎯 Scale Length to 1.0</h3>
          <p>Shrink or stretch the arrow so its length is exactly <code>1.0</code> (a unit circle).</p>
          <div class="mini-code" style="color:var(--accent-3);">
            Why? Eliminates document length so <strong>short tweets == long docs</strong>!
          </div>
        </div>
      </div>
    `,
    speakerNotes: [
      "Step 1 is Chop (Tokenization): An embedding model isn't a static lookup dictionary—every input passes through a 4-step conveyor belt. First, it splits text into word pieces called tokens and turns them into integer IDs.",
      "Step 2 is the Context Check (Self-Attention): Before combining anything, every word looks at the words around it. That's how 'apple' knows it's an iPhone in 'cracked my apple screen'. Even better: in 'I swiped my card and it failed', the word 'it' absorbs the meaning of 'credit card' from earlier in the sentence!",
      "Step 3 is Squash (Mean Pooling): Now we average all 50 word vectors into one single float[384] array. People often ask: 'If you average 50 words together, why don't random filler words like yesterday, the, or weirdly ruin the paragraph's meaning?' Two reasons: first, Step 2 already aligned the words in context. Second, filler words point in tiny neutral directions that cancel each other out like ripples on water, while domain words like 'payment', 'card', and '502' all pull strongly in the exact same direction!",
      "Step 4 is Scale to 1.0 (L2 Normalization): Remember on our 2D map how a 10-page book had a much longer arrow than a 1-sentence tweet just because it had more words? Step 4 divides the vector by its length so every single arrow has a length of exactly 1.0. Now every dot sits on the same circle of radius 1.0—so document length never tricks our search, and checking the angle becomes a fast 1-step multiplication!"
    ],
    glossary: [
      ["Why Filler Words Don't Ruin Paragraph Averages", "1) Transformer self-attention gives strong magnitude to meaningful words and near-zero or context-aligned values to filler words. 2) Random/unrelated noise across 384 dimensions cancels out toward zero when averaged, while repeated topic signals ('card', 'checkout', '502') add up constructively."],
      ["Why Scale to 1.0 (L2 Normalization)?", "Scaling every arrow to length 1.0 puts short queries and long articles on the exact same footing. Mathematically, once length = 1.0, `CosineSimilarity(A, B)` is just `A[0]*B[0] + A[1]*B[1] + ...` (the Dot Product)."]
    ]
  },

  // --------------------------------------------------------------------------
  // SLIDE 7: DEMO 2A — 3D MEANING UNIVERSE (1 Step)
  // --------------------------------------------------------------------------
  {
    id: "demo-3d-universe",
    type: "demo",
    title: "🎬 Live Demo: Flying Through the 3D Meaning Universe",
    subtitle: "Watching 180 support tickets self-organize into clusters in 3D space",
    stepsCount: 1,
    demoUrl: "../visualizer/index.html?tab=tab-3d",
    stageCue: "Open Visualizer Tab 2 ('2. 3D Meaning Universe'). Rotate the 3D space, type a live sentence in the input box, click 'Embed & Fly', and click '⚠ 3D Shadow Warning'.",
    html: `
      <div class="demo-hero step-item" data-step="1">
        <div class="demo-badge">🎬 INTERACTIVE VISUALIZER • TAB 2</div>
        <h2>Step Inside 384-Dimensional Space</h2>
        <p style="max-width:760px; margin: 12px auto 24px; color:var(--text-1); font-size:18px; line-height:1.55;">
          Let's project 384 coordinates down to 3D so we can rotate the clusters, drop a live sentence from the audience into space, and see which neighborhood it lands in!
        </p>
        <div class="demo-actions">
          <a class="btn-launch" href="../visualizer/index.html?tab=tab-3d" target="EmbeddingsVisualizerWindow">
            🌌 Open 3D Meaning Universe (Visualizer Tab 2) ↗
          </a>
        </div>
        <div class="cards-3" style="margin-top:28px; text-align:left;">
          <div class="v-card">
            <h3>1. Self-Organizing Clusters</h3>
            <p>Billing, MFA Security, Outages, and UI bugs clump into neighborhoods with zero manual tags.</p>
          </div>
          <div class="v-card">
            <h3>2. Audience Live Drop</h3>
            <p>Type any phrase—watch the camera fly to its landing spot and draw laser lines to its top 5 matches.</p>
          </div>
          <div class="v-card">
            <h3>3. The 3D Shadow Warning</h3>
            <p>Why squashing 384 numbers to 3D for a human screen is like casting a shadow on a wall.</p>
          </div>
        </div>
      </div>
    `,
    speakerNotes: [
      "Let's jump into Tab 2 of our Visualizer and fly through a 3D projection of 180 real support tickets, movies, and developer questions."
    ],
    glossary: [
      ["UMAP (384D → 3D Projection)", "Humans can't see in 384 dimensions. UMAP is an algorithm that squashes 384 coordinates down to 3D (X, Y, Z) while trying to keep neighboring dots together—just like flattening a 3D globe into a 2D world map."]
    ]
  },

  // --------------------------------------------------------------------------
  // SLIDE 8: DEMO 2B — VECTOR ARITHMETIC (DOING MATH ON MEANING) (3 Steps)
  // --------------------------------------------------------------------------
  {
    id: "demo-vector-arithmetic",
    type: "demo",
    title: "🎬 Live Demo: Doing Math on Meaning (Vector Arithmetic)",
    subtitle: "What happens when you add and subtract concept coordinates? (A + B − C)",
    stepsCount: 3,
    demoUrl: "../visualizer/index.html?tab=tab-math",
    stageCue: "Reveal the 3 steps on this slide, then click the button to open Visualizer Tab 3 ('3. Vector Arithmetic') and click through the 4 presets!",
    html: `
      <div class="flow-vertical">
        <div class="cards-2">
          <div class="v-card step-item" data-step="1">
            <div class="step-badge">Step 1 • The Classic Textbook Example</div>
            <h3>👑 Royal Meaning Math</h3>
            <div class="mini-code" style="font-size:15px;">
              Vector(<strong>"King"</strong>)<br/>
              − Vector(<strong>"Man"</strong>)<br/>
              + Vector(<strong>"Woman"</strong>)<br/>
              ────────────────────────<br/>
              ≈ Vector(<strong style="color:var(--accent-3);">"Queen"</strong>)
            </div>
            <p style="margin-top:8px;">Because gender and royalty are directions on the map, you can add and subtract them!</p>
          </div>

          <div class="v-card step-item" data-step="2" style="border-color:var(--accent);">
            <div class="step-badge">Step 2 • The Software Engineering Version</div>
            <h3>🚨 Incident Escalation Math</h3>
            <div class="mini-code" style="font-size:15px;">
              Vector(<strong>"Database query latency"</strong>)<br/>
              + Vector(<strong>"Angry customer blocked"</strong>)<br/>
              − Vector(<strong>"Internal staging test"</strong>)<br/>
              ────────────────────────────────────<br/>
              ≈ Vector(<strong style="color:var(--danger);">"P1 Production Database Outage!"</strong>)
            </div>
            <p style="margin-top:8px;">Adding customer urgency and subtracting internal testing moves the coordinate straight to P1 Outages!</p>
          </div>
        </div>

        <div class="step-item" data-step="3" style="text-align:center; margin-top:14px;">
          <a class="btn-launch" href="../visualizer/index.html?tab=tab-math" target="EmbeddingsVisualizerWindow">
            ➕ Open Vector Arithmetic Lab (Visualizer Tab 3) ↗
          </a>
        </div>
      </div>
    `,
    speakerNotes: [
      "Because concepts are represented as coordinates, directions in space also have meaning—which means we can literally add and subtract ideas! You might have heard of the famous textbook example: if you take the coordinate for 'King', subtract 'Man', and add 'Woman', the resulting arrow lands right next to 'Queen'.",
      "What does that look like in software engineering? Take 'Database query latency', add 'Angry customer blocked at checkout', and subtract 'Internal staging sandbox test'—and the resulting coordinate lands right on 'P1 Production Database Outage!'",
      "Let's open Tab 3 of our Visualizer and test these vector equations live!"
    ],
    glossary: [
      ["Vector Addition / Subtraction", "Literally looping through the 384 numbers and doing `Result[i] = A[i] + B[i] - C[i]`, then scaling the resulting arrow back to length 1.0."]
    ]
  },

  // --------------------------------------------------------------------------
  // SLIDE 9: LIVE C# CODE DEMO 1 — SEMANTIC SEARCH IN ~25 LINES (4 Steps)
  // --------------------------------------------------------------------------
  {
    id: "code-demo-semantic-search",
    type: "code",
    title: "💻 Live C# Code: Semantic Search in ~25 Lines",
    subtitle: "Running 100% locally in .NET 10 — zero cloud API keys or Python needed",
    stepsCount: 4,
    stageCue: "After revealing Step 4, switch to your terminal/IDE and run `dotnet run --project src/01-LiveCoding-SemanticSearch` (press Enter 4 times to walk through the 4 queries).",
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

<span class="code-step step-item" data-step="3"><span class="cm">// Step 3: Compare angles using built-in .NET TensorPrimitives!</span>
<span class="kw">var</span> queryVector = <span class="kw">await</span> embedder.GenerateVectorAsync(
    <span class="st">"credit card won't go through"</span>);

<span class="kw">var</span> best = tickets.Zip(ticketVectors)
    .Select(x =&gt; (
        Ticket: x.First,
        Score: <span class="ty">TensorPrimitives</span>.CosineSimilarity(
                   queryVector.Span, x.Second.Vector.Span)
    ))
    .OrderByDescending(x =&gt; x.Score)
    .First();</span>

<span class="code-step step-item" data-step="4"><span class="cm">// Step 4: Print the winning match!</span>
Console.WriteLine(<span class="st">$"Match ({best.Score:P0}): {best.Ticket}"</span>);
<span class="cm">// Output: Match (75%): Payment gateway returns 502 error at checkout</span></span></pre>
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
            <h3 style="color:var(--accent-3);">4. ▶ Run It Step-by-Step in Terminal</h3>
            <div class="mono" style="font-size:13px; background:#050811; padding:10px; border-radius:8px; margin-top:6px;">
              dotnet run --project src/01-LiveCoding-SemanticSearch
            </div>
          </div>
        </div>
      </div>
    `,
    speakerNotes: [
      "Let's see how this looks in real C# code. First, we create an IEmbeddingGenerator—that is the official standard interface in Microsoft.Extensions.AI. Here we load a local ONNX model file, which is just like a PDF or MP3 file except it holds a trained neural network that runs right on your laptop CPU.",
      "Second, we have our support tickets as plain C# strings, and we call GenerateAsync. That turns each sentence into a float[384] array of coordinates.",
      "Third, when a user searches 'credit card won't go through', we turn their query into a vector and use LINQ plus TensorPrimitives.CosineSimilarity—which is built directly into .NET and uses parallel CPU math—to sort our tickets by the closest angle.",
      "And fourth, it prints the winning match! Let's run this project in the terminal and test four queries live."
    ],
    glossary: [
      ["ONNX (Open Neural Network Exchange)", "A universal file format for AI models (like a .pdf for neural networks). Lets you download a model once and run it directly inside C# on your CPU with zero Python or cloud APIs."],
      ["Microsoft.Extensions.AI", "Microsoft's official .NET library that provides standard interfaces (like IEmbeddingGenerator) so you can swap AI providers via dependency injection without rewriting your app."],
      ["TensorPrimitives", "Built-in .NET class (`System.Numerics.Tensors`) that uses your CPU's wide math registers to multiply 8 to 16 numbers in a single clock tick instead of 1 by 1 in a slow loop."]
    ]
  },

  // --------------------------------------------------------------------------
  // SLIDE 10: ARCHITECTURAL PATTERNS #1 & #2 BEYOND CHATBOTS (2 Steps)
  // --------------------------------------------------------------------------
  {
    id: "patterns-routing-and-caching",
    type: "concept",
    title: "Beyond Chatbots: Fast Backend Building Blocks",
    subtitle: "Using embeddings as sub-millisecond microservice primitives—zero LLM needed",
    stepsCount: 2,
    html: `
      <div class="cards-2">
        <div class="v-card step-item" data-step="1">
          <div class="step-badge">Pattern #1 • &lt; 1 Millisecond</div>
          <h3>🧭 Zero-Shot Ticket &amp; Webhook Routing</h3>
          <p>How do you route incoming tickets to 5 teams without training a custom ML model?</p>
          <div class="diagram-box" style="margin-top:10px;">
            <div class="d-node">Incoming Ticket: <em>"Visa card declined on checkout"</em></div>
            <div class="d-arrow">↓ Compare angle against 5 Team Descriptions</div>
            <div class="d-row">
              <span class="d-pill win">💳 Billing (86%)</span>
              <span class="d-pill">🔐 Security (19%)</span>
              <span class="d-pill">📱 Mobile (11%)</span>
            </div>
          </div>

          <details class="slide-expand">
            <summary>🔍 Click to Expand Concrete Example (How Team Descriptions Look)</summary>
            <div class="expand-body mono" style="font-size:12px;">
              <strong>// 1. Embed 1 description per team at startup:</strong><br/>
              BillingVec  = Embed("Credit card, invoice, VAT tax, checkout 502, refund");<br/>
              SecurityVec = Embed("Two-factor MFA reset, locked account, SSO password");<br/>
              MobileVec   = Embed("iOS Android app crash, avatar photo upload freeze");<br/><br/>
              <strong>// 2. Compare incoming ticket against those 3 vectors:</strong><br/>
              Dot(TicketVec, BillingVec)  → <span style="color:var(--accent-3);">0.86 (Route to Billing!)</span>
            </div>
          </details>
        </div>

        <div class="v-card step-item" data-step="2">
          <div class="step-badge">Pattern #2 • Save 40%+ API Cost</div>
          <h3>⚡ Semantic Cache &amp; Duplicate Blocker</h3>
          <p>Before creating a duplicate Jira bug or paying for a slow 3-second LLM call, check if a similar coordinate already exists:</p>
          <div class="diagram-box" style="margin-top:10px;">
            <div class="d-node">New Bug: <em>"Checkout fails with 502 on credit card"</em></div>
            <div class="d-arrow">↓ Check Cosine Similarity against recent items</div>
            <div class="d-node" style="border-color:var(--warn); color:var(--warn);">
              🎯 <strong>95% Match Found!</strong> Existing Jira #1042<br/>
              <em>"Payment gateway returns 502 error at checkout"</em>
            </div>
          </div>

          <details class="slide-expand">
            <summary>🔍 Click to Expand Concrete Example (LLM Cache &amp; Jira Dedup)</summary>
            <div class="expand-body mono" style="font-size:12px;">
              <strong>User 1 asks chatbot:</strong> "How do I reset my 2FA token?" → Calls LLM (2.8s, $0.02)<br/>
              <strong>User 2 asks chatbot:</strong> "Lost my phone, how to reset two-factor?"<br/>
              → CosineSimilarity = <strong>94% (&gt;= 90% threshold)</strong><br/>
              → <span style="color:var(--accent-3);">Returns cached answer in 0.4 milliseconds for $0.00!</span>
            </div>
          </details>
        </div>
      </div>
    `,
    speakerNotes: [
      "Most people hear 'embeddings' and only think of chatbots and RAG. But some of the highest-ROI ways to use embeddings don't involve a chatbot at all! Pattern #1 is Zero-Shot Ticket Routing. Suppose your backend receives customer support tickets and needs to route them to Billing, Security, or the Mobile team. Instead of collecting 10,000 labeled tickets to train a classifier, you write one descriptive sentence for each team when your service starts up: for example, Billing is 'credit card declined, invoice VAT rate, checkout 502, refund'. You embed those team descriptions once. When a ticket comes in saying 'My Visa card keeps getting rejected', you compare its vector against your team vectors. It matches Billing at 86% in under a millisecond—and adding a new team tomorrow is just adding one string to your config!",
      "Pattern #2 is Semantic Caching and Deduplication. Users file the exact same bug report in slightly different words, or ask your AI assistant the same question 500 different ways. By keeping recent vectors in memory or Redis, if a new request has over 90% similarity to an existing one, you immediately link the duplicate Jira ticket or return the cached answer in under a millisecond."
    ],
    glossary: [
      ["Zero-Shot Routing", "Categorizing incoming items into queues without training a custom ML model—just by measuring similarity to 1-sentence descriptions of each queue."]
    ]
  },

  // --------------------------------------------------------------------------
  // SLIDE 11: ARCHITECTURAL PATTERNS #3 & #4 (2 Steps)
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
          <h3>🚨 Anomaly &amp; Log Drift Detection</h3>
          <p>Regex alerts only catch errors you already know how to spell. Embeddings catch <strong>anything weird</strong>:</p>
          <div class="diagram-box" style="margin-top:10px; text-align:center;">
            <div style="display:inline-block; padding:10px 18px; border:2px dashed var(--accent-3); border-radius:999px; color:var(--accent-3);">
              🟢 Normal Logs Cluster Center<br/>
              <span style="font-size:12px; color:var(--text-1);">(CI builds, healthchecks, routine deploys)</span>
            </div>
            <div style="margin:6px 0; color:var(--danger); font-weight:700;">← Distance = 0.95 (Far Outside!) →</div>
            <div class="d-node" style="border-color:var(--danger); color:#fecaca;">
              🔴 <strong>OUTLIER ALERT:</strong> Novel payment failure log landed far outside the normal circle!
            </div>
          </div>

          <details class="slide-expand">
            <summary>🔍 Click to Expand Concrete Example (How Log Averaging Works)</summary>
            <div class="expand-body mono" style="font-size:12px;">
              <strong>1. Average 1,000 healthy logs into 1 "Normal Center" coordinate:</strong><br/>
              Normal1: "INFO Healthcheck passed on node-3"   → [+0.41, +0.38, ...]<br/>
              Normal2: "INFO CI build #482 deployed to k8s"  → [+0.43, +0.40, ...]<br/>
              <strong>NormalCenter</strong> = Average(Normal1..N)            → <strong>[+0.42, +0.39, ...]</strong><br/><br/>
              <strong>2. Check distance of new log line:</strong><br/>
              NewLog: "URGENT Checkout Stripe 502 failing!" → <strong>[-0.51, -0.28, ...]</strong><br/>
              Distance from NormalCenter = <strong>0.95 (&gt; 0.55 cutoff) → Trigger Alert!</strong>
            </div>
          </details>
        </div>

        <div class="v-card step-item" data-step="2">
          <div class="step-badge">Pattern #4 • Zero User-Tracking DB</div>
          <h3>🎯 Stateless "More Like This" Recommendations</h3>
          <p>How do you recommend articles without building a massive collaborative-filtering pipeline?</p>
          <div class="diagram-box" style="margin-top:10px;">
            <div class="d-node">User just read 3 articles → <code>Vec1, Vec2, Vec3</code></div>
            <div class="d-arrow">↓ Average the 3 arrays together: <code>(Vec1 + Vec2 + Vec3) / 3</code></div>
            <div class="d-node" style="border-color:var(--accent); color:var(--accent);">
              <strong>User's Live Taste Vector (<code>float[384]</code>)</strong><br/>
              Query nearest unread articles to that average point!
            </div>
          </div>

          <details class="slide-expand">
            <summary>🔍 Click to Expand Concrete Example (Averaging 3 Read Articles)</summary>
            <div class="expand-body mono" style="font-size:12px;">
              Read #1: "SIMD Vector Math in C#"     → [+0.60, +0.10, ...]<br/>
              Read #2: "Local ONNX Models in .NET"  → [+0.55, +0.20, ...]<br/>
              Read #3: "Replacing SQL LIKE Queries" → [+0.50, +0.15, ...]<br/>
              <strong>UserTaste = Average(Read 1..3)      → [+0.55, +0.15, ...]</strong><br/>
              Closest Unread Article → <span style="color:var(--accent-3);">"Quantization &amp; Hybrid Search in C#" (85% Match!)</span>
            </div>
          </details>
        </div>
      </div>
    `,
    speakerNotes: [
      "Pattern #3 is Log Anomaly Detection. Traditional monitoring relies on regex rules for errors you already anticipate. With embeddings, you take healthy logs from a normal Tuesday—things like healthchecks, routine deployments, and CI builds—and average their vectors together to find the center point of 'Normal Operations'. When a new log line comes in, you measure how far it sits from that normal center. If a weird third-party payment outage starts happening, its vector lands way outside the normal circle at a distance of 0.95, triggering an alert even if the log message doesn't contain the word 'Exception'!",
      "Pattern #4 is a Stateless Recommendation Engine. If a user just read three articles on your site—say one about C# performance, one about ONNX models, and one about semantic search—you just average those three float[384] arrays together. That average coordinate IS the user's current taste profile! Then you query which unread article sits closest to that spot."
    ],
    glossary: [
      ["Cluster Centroid (Normal Center)", "The average coordinate of a group of vectors—calculated by adding all vectors in the group together and dividing by the count (`(v1 + v2 + ... + vN) / N`)."]
    ]
  },

  // --------------------------------------------------------------------------
  // SLIDE 12: PATTERN #5 — MULTIMODAL SHARED SPACE (3 Steps)
  // --------------------------------------------------------------------------
  {
    id: "multimodal-shared-space",
    type: "concept",
    title: "How Text, Images & Audio Share the Exact Same Map",
    subtitle: "How a JPG photo and a WAV sound file get chopped into numbers just like text",
    stepsCount: 3,
    html: `
      <div class="flow-vertical">
        <div class="cards-3 step-item" data-step="1">
          <div class="v-card">
            <h3>📝 1. Text Construction</h3>
            <p><strong>Input:</strong> <code>"golden retriever"</code></p>
            <div class="mini-code">
              1. Split into words:<br/>
                 <code>["golden", "retriever"]</code><br/>
              2. Lookup Token IDs:<br/>
                 <code>[4812, 9021]</code><br/>
              3. Pass through Text Encoder:<br/>
                 → <strong style="color:var(--accent-3);">[ +0.21, -0.44, +0.79 ]</strong>
            </div>
          </div>

          <div class="v-card">
            <h3>🖼️ 2. Image Construction</h3>
            <p><strong>Input:</strong> <code>dog_photo.jpg</code></p>
            <div class="mini-code">
              1. Slice photo into a grid of<br/>
                 <strong>16×16 pixel puzzle squares</strong>.<br/>
              2. Read each square's RGB colors<br/>
                 <code>[Red:240, Green:190, Blue:80]</code><br/>
              3. Vision Encoder reads squares<br/>
                 just like words in a sentence!<br/>
                 → <strong style="color:var(--accent-3);">[ +0.20, -0.43, +0.78 ]</strong>
            </div>
          </div>

          <div class="v-card">
            <h3>🔊 3. Audio Construction</h3>
            <p><strong>Input:</strong> <code>dog_bark.wav</code></p>
            <div class="mini-code">
              1. Plot sound wave as a 2D picture<br/>
                 called a <strong>Spectrogram</strong><br/>
                 (X=Time, Y=Pitch, Color=Loudness).<br/>
              2. Slice that picture into<br/>
                 <strong>16×16 squares</strong> just like a photo!<br/>
              3. Pass through Audio Encoder:<br/>
                 → <strong style="color:var(--accent-3);">[ +0.19, -0.42, +0.77 ]</strong>
            </div>
          </div>
        </div>

        <div class="v-card step-item" data-step="2" style="border-color:var(--accent-3); text-align:center;">
          <h3 style="color:var(--accent-3);">🎯 Trained Together so All 3 Land on the Exact Same Spot!</h3>
          <p>During training, models like <strong>CLIP</strong>, <strong>SigLIP</strong>, and <strong>Gemini Embedding 2</strong> are shown millions of photos with their captions and audio—pulling all 3 encoders to output the <strong>exact same coordinates</strong>!</p>
        </div>

        <div class="v-card step-item" data-step="3" style="border-color:var(--warn); background:rgba(251,191,36,0.08);">
          <h3 style="color:var(--warn);">💡 Pro Nuance: "The Modality Gap" (Oil &amp; Water)</h3>
          <p>Even in a shared map, image vectors and text vectors hover in slightly offset clouds (like oil and water). Newer unified models shrink this gap so one text query searches screenshots, PDFs, audio, and docs at once!</p>
        </div>
      </div>
    `,
    speakerNotes: [
      "What about images and audio? Look at the three columns here to see how each media type is physically constructed into numbers. For text, we chop the sentence into words and token IDs. For an image, a Vision Transformer literally chops the photo into a grid of 16-by-16 pixel puzzle squares! Each square has Red, Green, and Blue brightness numbers, and the model reads those squares left-to-right just like words in a sentence. And for audio? We convert the sound wave into a visual heatmap of pitch over time called a spectrogram—which is just a 2D picture—and chop that picture into 16-by-16 squares too!",
      "During training, multimodal models like CLIP, SigLIP, and Gemini Embedding 2 are shown millions of paired photos, captions, and audio clips, and punished whenever the photo arrow and the text arrow point in different directions. So all three end up at [+0.21, -0.44, +0.79]!",
      "One practical nuance called The Modality Gap: even in a shared space, image vectors and text vectors still hover in slightly offset sub-clouds, like oil and water. Newer models shrink that gap dramatically, letting your vector database search untagged screenshots, architecture diagrams, and text in a single query."
    ],
    glossary: [
      ["How Images Become Tokens (ViT Patches)", "A 224×224 pixel image is sliced into 196 little squares of 16×16 pixels. Each square is flattened into a list of RGB numbers and fed into the Transformer just like a word token."],
      ["How Audio Becomes Tokens (Spectrogram)", "Sound waves are plotted as a 2D image where X is time, Y is frequency/pitch, and brightness is volume—then sliced into 16×16 squares just like a photograph."]
    ]
  },

  // --------------------------------------------------------------------------
  // SLIDE 13: LIVE C# CODE DEMO 2 — 5 PATTERNS BEYOND RAG (1 Step)
  // --------------------------------------------------------------------------
  {
    id: "code-demo-5-patterns",
    type: "code",
    title: "💻 Live C# Demo: 5 Backend Patterns in Separate Files",
    subtitle: "Modular, step-by-step C# demos you can run or debug individually (src/02-PatternsBeyondRag)",
    stepsCount: 1,
    stageCue: "Open `src/02-PatternsBeyondRag/Program.cs` in your IDE (or run `dotnet run --project src/02-PatternsBeyondRag`). Each pattern is in its own separate `.cs` file in `Demos/` and pauses at each sub-step!",
    html: `
      <div class="demo-hero step-item" data-step="1">
        <div class="demo-badge">💻 MODULAR STEP-BY-STEP C# DEMOS</div>
        <h2>Clean, Separate Demo Files Ready for Live Walkthrough</h2>
        <p style="max-width:780px; margin: 10px auto 18px; color:var(--text-1); font-size:16.5px;">
          In <code>src/02-PatternsBeyondRag/Program.cs</code>, each pattern is a clean 1-line call into its own file under <code>Demos/</code>—so you can comment/uncomment any demo, set breakpoints, or step through in the console with <code>[Enter]</code>:
        </p>
        <pre class="code-box" style="text-align:left; max-width:780px; margin:0 auto 18px; font-size:13px;">
<span class="cm">// src/02-PatternsBeyondRag/Program.cs — Uncomment any pattern to run solo!</span>
<span class="kw">await</span> <span class="ty">Demo1_ZeroShotTicketRouting</span>.RunAsync(embedder, autoRunAll);
<span class="kw">await</span> <span class="ty">Demo2_SemanticCacheAndDedup</span>.RunAsync(embedder, autoRunAll);
<span class="kw">await</span> <span class="ty">Demo3_LogAnomalyRadar</span>.RunAsync(embedder, autoRunAll);
<span class="kw">await</span> <span class="ty">Demo4_StatelessRecommendations</span>.RunAsync(embedder, autoRunAll);
<span class="kw">await</span> <span class="ty">Demo5_MultimodalSearch</span>.RunAsync(embedder, autoRunAll);</pre>
        <div class="mono" style="font-size:15px; background:#050811; border:1px solid var(--accent); padding:12px 20px; border-radius:10px; display:inline-block; color:var(--accent-3);">
          dotnet run --project src/02-PatternsBeyondRag
        </div>
      </div>
    `,
    speakerNotes: [
      "Let's switch to our IDE and terminal to run our second C# project, where each of these five backend patterns is in its own clean, self-contained C# file."
    ],
    glossary: [
      ["CLI Tip: Running a Single Pattern", "You can either comment out lines in `Program.cs` OR run `dotnet run --project src/02-PatternsBeyondRag -- --demo 1` (or `2`, `3`, `4`, `5`) to run just one pattern!"]
    ]
  },

  // --------------------------------------------------------------------------
  // SLIDE 14: WHY EMBEDDING MODELS DIFFER (5 Steps)
  // --------------------------------------------------------------------------
  {
    id: "how-models-differ",
    type: "concept",
    title: "Why Are There 100 Different Embedding Models?",
    subtitle: "5 practical differences that actually matter when you pick one",
    stepsCount: 5,
    html: `
      <div class="flow-vertical" style="gap:10px;">
        <div class="v-card step-item" data-step="1" style="padding:13px 18px;">
          <h3>1. Short Question vs. Long Answer (Symmetric vs. Asymmetric)</h3>
          <p>A 4-word search query doesn't look like a 500-word manual page. Retrieval models trained for Q&amp;A often require adding <code>"query: "</code> before user searches and <code>"passage: "</code> before documents.</p>
        </div>

        <div class="v-card step-item" data-step="2" style="padding:13px 18px; border-color:rgba(248,113,113,0.4);">
          <h3 style="color:var(--danger);">2. The Silent Cutoff Trap (Context Window: 512 vs. 8,000+ Words)</h3>
          <p>Older models cap at <strong>512 tokens (~1 page)</strong> and <strong>silently delete</strong> pages 2–10 without throwing an error! Always check the model's max token limit.</p>
        </div>

        <div class="v-card step-item" data-step="3" style="padding:13px 18px;">
          <h3>3. Coordinate Array Size (384 vs. 3,072 Numbers)</h3>
          <p><code>384 numbers</code> (like local MiniLM) is tiny and fast. <code>3,072 numbers</code> (like OpenAI large / Gemini) captures subtle nuance, but takes <strong>8x more RAM</strong>.</p>
        </div>

        <div class="v-card step-item" data-step="4" style="padding:13px 18px;">
          <h3>4. Language &amp; Domain (English vs. 100 Languages vs. Code)</h3>
          <p>An English-only model maps Lithuanian, Spanish, or C# code almost randomly. Pick a multilingual (<code>BGE-M3</code>, <code>Qwen3</code>) or code-specific (<code>Voyage-Code</code>) model when needed.</p>
        </div>

        <div class="v-card step-item" data-step="5" style="padding:13px 18px; border-color:var(--accent-3);">
          <h3 style="color:var(--accent-3);">5. Local <code>.onnx</code> File (Free &amp; Private) vs. Cloud API</h3>
          <p>Local ONNX models run inside your .NET process in <code>&lt;2ms</code> for <code>$0</code> and never send sensitive data outside your server. Cloud APIs save you from managing CPU/GPU batch jobs.</p>
        </div>
      </div>
    `,
    speakerNotes: [
      "When you go to pick an embedding model for your own project, you only need to care about five practical differences. First: Is it matching similar things, or short questions against long answers? Models designed for Q&A retrieval often expect you to prefix user searches with 'query: ' and documents with 'passage: ' so they align properly.",
      "Second: Watch out for the Silent Cutoff Trap! Older models have a maximum context window of 512 tokens—about one page of text. If you feed a 10-page PDF into a 512-token model, it doesn't throw an exception; it silently deletes pages 2 through 10!",
      "Third: How many numbers does it output? A 384-number model is super fast and light on RAM; a 3,072-number model captures finer nuance but costs 8 times more memory.",
      "Fourth: What languages and domains was it trained on? An English-only model will map other languages or source code almost randomly, whereas multilingual or code-specialized models understand them natively.",
      "And fifth: Do you want to run a local ONNX file inside your .NET app for zero dollars and complete data privacy, or call a hosted API like OpenAI, Gemini, or Voyage?"
    ],
    glossary: [
      ["Context Window", "The maximum number of words/tokens a model can read at once before it silently chops off the rest."],
      ["Asymmetric Retrieval", "Matching a very short search query against a long document passage."]
    ]
  },

  // --------------------------------------------------------------------------
  // SLIDE 15: ACT 5 — WHERE EMBEDDINGS LIE (NEGATION & EXACT CODES) (3 Steps)
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
            <h3 style="color:var(--danger);">Trap B: Blind to Exact Error Codes &amp; IDs</h3>
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
          <p>Run <strong>Vector Search</strong> (meaning &amp; synonyms) AND <strong>Keyword Search (BM25)</strong> (exact words &amp; numbers), then blend. Neither alone is always right, so blend them.</p>
        </div>
      </div>
    `,
    speakerNotes: [
      "Remember in our first C# demo when we asked 'How do I DISABLE two-factor authentication?' and it matched 'How do I RESET MFA' at 98%? Dense embeddings are blind to negation—words like NOT, WITHOUT, or DISABLE—and blind to exact alphanumeric codes like Error 503 versus Error 504. If a user searches 'I am allergic to peanuts, show me snacks WITHOUT peanuts', pure vector search sees the word 'peanuts' and puts Crunchy Peanut Butter Bars at number one!",
      "Why does that happen? Because an embedding only measures the general topic neighborhood—it sees that both sentences are about peanuts or about MFA settings. It does not execute boolean logic.",
      "The production fix is Hybrid Search: never rely on vectors alone! Always run Vector Search alongside classic Keyword Search (BM25), and merge their rankings."
    ],
    glossary: [
      ["BM25 (Best Match 25)", "The classic keyword search algorithm used by Elasticsearch and PostgreSQL full-text search. Counts exact word matches and gives high weight to rare words like '504' or 'DISABLE'."],
      ["Hybrid Search + RRF (Reciprocal Rank Fusion)", "Running Vector Search and Keyword Search in parallel, then combining their rank positions (`1/(60 + VectorRank) + 1/(60 + KeywordRank)`) so a document that scores well in both wins #1."]
    ]
  },

  // --------------------------------------------------------------------------
  // SLIDE 16: DEMO 3 — BREAKING VECTORS & FIXING THEM WITH HYBRID SEARCH (1 Step)
  // --------------------------------------------------------------------------
  {
    id: "demo-hybrid-search",
    type: "demo",
    title: "🎬 Live Demo: Vector vs Keyword vs Hybrid",
    subtitle: "Vector vs keyword vs hybrid: who actually wins, and when nobody does",
    stepsCount: 1,
    demoUrl: "../visualizer/index.html?tab=tab-gotchas",
    stageCue: "Open Visualizer Tab 4 ('4. Where Vectors Fail & Hybrid Fix'). Drag the Hybrid Slider from 0% (Pure Vector) to 50%+ (Hybrid) to watch #1 flip live, and toggle the 'Show C# & SQL Code' drawer if someone asks how it's coded!",
    html: `
      <div class="demo-hero step-item" data-step="1">
        <div class="demo-badge">🎬 INTERACTIVE VISUALIZER • TAB 4</div>
        <h2>No Single Search Wins Everywhere — Drag the Slider and See</h2>
        <p style="max-width:780px; margin: 12px auto 24px; color:var(--text-1); font-size:17.5px; line-height:1.55;">
          Four scenarios, each with a vector and a keyword score per document. Three are rescued by keyword weight (DISABLE MFA, Error 504, WITHOUT peanuts), and one is won by vector (keyword-only picks a candle article!). A green/red strip shows exactly which slider positions pick the right answer.
        </p>
        <div class="demo-actions">
          <a class="btn-launch" href="../visualizer/index.html?tab=tab-gotchas" target="EmbeddingsVisualizerWindow">
            🛡️ Open Negation &amp; Hybrid Search Lab (Visualizer Tab 4) ↗
          </a>
        </div>
        <div class="cards-3" style="margin-top:26px; text-align:left;">
          <div class="v-card">
            <h3>1. Vector Fails, Keyword Saves</h3>
            <p>DISABLE vs ENABLE MFA and Error 504 vs 503: exact words rescue the answer.</p>
          </div>
          <div class="v-card">
            <h3>2. Keyword Fails, Vector Saves</h3>
            <p>"Screen is flickering" vs a candle article: keyword-only picks the wrong one, so a hybrid mix wins.</p>
          </div>
          <div class="v-card">
            <h3>3. Negation: "WITHOUT peanuts"</h3>
            <p>Vector only sees "peanuts" and returns peanut snacks. Keyword catches the rare word "WITHOUT", so a blend lifts the safe product to #1.</p>
          </div>
        </div>
      </div>
    `,
    speakerNotes: [
      "Let's open Tab 4 and drag the slider. Notice there is no setting that wins every time: for DISABLE MFA and Error 504 keyword weight rescues the answer, but for the flickering screen too much keyword weight breaks it. That's why a balanced mix is the safe default."
    ],
    glossary: [
      ["How Hybrid Search Is Used in Practice", "In PostgreSQL (pgvector) or Azure AI Search / Qdrant, you run one query that fetches the Top 20 by vector cosine distance and Top 20 by full-text keyword search, and merges them with RRF in 3 milliseconds."]
    ]
  },

  // --------------------------------------------------------------------------
  // SLIDE 17: THE 120 GB RAM BILL SHOCK & STEP-BY-STEP SHRINKER DEMO (3 Steps)
  // --------------------------------------------------------------------------
  {
    id: "demo-ram-shrinker",
    type: "demo",
    title: "🎬 Step-by-Step Demo: Shrinking 120 GB of RAM Down to 600 MB",
    subtitle: "Matryoshka Slicing, Quantization & what '96% Recall Retention' actually means",
    stepsCount: 3,
    demoUrl: "../visualizer/index.html?tab=tab-mrl",
    stageCue: "Reveal the 3 cards on this slide, then open Visualizer Tab 5 ('5. Step-by-Step RAM Shrinker') and click through Steps 1, 2, 3, and 4.",
    html: `
      <div class="flow-vertical">
        <div class="cards-3">
          <div class="v-card step-item" data-step="1" style="border-color:var(--danger);">
            <div class="step-badge">1 • The Problem</div>
            <h3 style="color:var(--danger);">💸 The 120 GB RAM Cliff</h3>
            <p>10M docs × 3,072 numbers × 4 bytes each = <strong>~120 GB of RAM</strong> just to hold the vectors!</p>
          </div>

          <div class="v-card step-item" data-step="2" style="border-color:var(--accent-3);">
            <div class="step-badge">2 • Trick #1 &amp; #2</div>
            <h3 style="color:var(--accent-3);">🪆 Chop Length + 🗜️ Round Numbers</h3>
            <p><strong>Matryoshka Slicing:</strong> Keep first 512 of 3,072 numbers (6x smaller).<br/>
            <strong>Quantization:</strong> Round 32-bit floats to 8-bit ints (4x) or 1-bit signs (32x)!</p>
          </div>

          <div class="v-card step-item" data-step="3" style="border-color:var(--warn);">
            <div class="step-badge">3 • What Does "96% Recall" Mean?</div>
            <h3 style="color:var(--warn);">🎯 What Is That 4% Difference?</h3>
            <p>It does <strong>not</strong> mean answers become 4% garbled! It means <strong>96 out of 100 searches return the exact same Top-10 list</strong>, and in 4 borderline cases #1 and #2 swap places!</p>
          </div>
        </div>

        <div style="text-align:center; margin-top:14px;">
          <a class="btn-launch" href="../visualizer/index.html?tab=tab-mrl" target="EmbeddingsVisualizerWindow">
            🪆 Open 4-Step Interactive RAM Shrinker (Visualizer Tab 5) ↗
          </a>
        </div>
      </div>
    `,
    speakerNotes: [
      "What happens when your system grows to 10 million documents? 10 million documents times 3,072 numbers times 4 bytes per float equals 120 Gigabytes of RAM! Because vector indexes need to live in RAM for fast search, that gets expensive fast.",
      "Engineers use two tricks to shrink that bill. First, Matryoshka Slicing: modern models pack the big picture into the first 256 to 512 numbers, so you can chop off the remaining 2,500 numbers! Second, Quantization: rounding each 4-byte decimal number down to a 1-byte integer or a single plus/minus bit.",
      "People always ask: 'When the chart says 96% Recall Retention, what is the 4% we lose in practice?' It does NOT mean the text comes back corrupted! It simply means that out of 100 user searches, 96 times the compressed vector returns the exact same Top-10 results as the huge 120-gigabyte index, and in 4 borderline searches, rank #2 and rank #3 might trade places. Even better, if you grab the top 25 compressed matches and re-check just those 25 with full numbers from disk, you get 99.5% identical results! Let's walk through this in Tab 5 of the Visualizer."
    ],
    glossary: [
      ["What '96% Recall@10' Means in Practice", "If the uncompressed 3,072-float search returns a specific Top-10 list of documents, a 96% recall compressed index still includes 9.6 out of those 10 exact documents in its Top-10 list—while using 20x to 100x less RAM!"]
    ]
  },

  // --------------------------------------------------------------------------
  // SLIDE 18: TWO FINAL PRODUCTION TRAPS & C# DEMO 3 (3 Steps)
  // --------------------------------------------------------------------------
  {
    id: "chunking-and-migration",
    type: "code",
    title: "Two Final Production Rules & Live C# Fixes",
    subtitle: "Orphaned Paragraphs, Model Upgrade Lock-In & Take-Home Cheat Sheet",
    stepsCount: 3,
    stageCue: "After revealing Step 3, run `dotnet run --project src/03-ProductionGotchasAndFixes` (or open `src/03-ProductionGotchasAndFixes/Program.cs` to run any of the 4 separate Gotcha files).",
    html: `
      <div class="flow-vertical">
        <div class="cards-2">
          <div class="v-card step-item" data-step="1">
            <div class="step-badge">Trap #3 • Orphaned Pronouns</div>
            <h3>📄 Never Embed a Paragraph Without Its Header</h3>
            <p>Splitting a document every 500 characters creates orphaned chunks:</p>
            <div class="mini-code" style="border-color:var(--danger); color:#fecaca;">
              ❌ <em>"It rejects the request whenever payload exceeds 4MB."</em> (What is <strong>"It"</strong>?! Score = <strong>16%</strong>)
            </div>
            <div class="mini-code" style="border-color:var(--accent-3); color:var(--accent-3);">
              ✅ <strong>Fix:</strong> Prepend <code>[Doc: Stripe Checkout &gt; Limits]</code> before embedding → Score jumps to <strong>78%</strong>!
            </div>
          </div>

          <div class="v-card step-item" data-step="2">
            <div class="step-badge">Trap #4 • Model Upgrade Lock-In</div>
            <h3>🔄 Vector Spaces Are Incompatible Across Models!</h3>
            <p>Axis #1 in <strong>Model A</strong> might mean <em>"Urgency"</em> while Axis #1 in <strong>Model B</strong> means <em>"Food"</em>!</p>
            <div class="mini-code" style="border-color:var(--warn); color:#fde68a;">
              ⚠️ Comparing a Model B query against Model A database rows gives <strong>~5% random noise!</strong>
            </div>
            <div class="mini-code" style="border-color:var(--accent);">
              ✅ <strong>Fix (Blue/Green Columns):</strong> Keep <code>embedding_v1</code> live while backfilling <code>embedding_v2</code> in the background!
            </div>
          </div>
        </div>

        <div class="v-card step-item" data-step="3" style="border-color:var(--accent-3); display:flex; justify-content:space-between; align-items:center; flex-wrap:wrap; gap:16px;">
          <div>
            <h3 style="color:var(--accent-3); margin-bottom:4px;">💻 Run C# Demo #3 (4 Separate Files in <code>src/03-ProductionGotchasAndFixes</code>)</h3>
            <p style="font-size:14px;">Step-by-step console &amp; debugger walkthrough of Hybrid RRF, Matryoshka Slicing, Header Chunking, and Model v1 vs v2.</p>
          </div>
          <div class="mono" style="font-size:14px; background:#050811; border:1px solid var(--accent-3); padding:10px 16px; border-radius:8px; color:var(--accent-3);">
            dotnet run --project src/03-ProductionGotchasAndFixes
          </div>
        </div>
      </div>
    `,
    speakerNotes: [
      "First final rule: Never embed an orphaned paragraph without its heading! If you split a PDF every 500 characters, a chunk might say 'It rejects the request whenever the payload exceeds 4MB.' What is 'It'?! The words 'Stripe Checkout' were in the title at the top of the page! Always prepend the document title and section heading in brackets to every chunk before calling GenerateAsync.",
      "Second final rule: What happens next year when you want to switch from Model A to a newer Model B? Every model invents its own coordinate axes from scratch. Even if both models output 384 numbers, comparing a query from Model B against database rows from Model A produces total random noise! Always store the model version in your database table, and when upgrading, add a second column—embedding_v2—backfill it in the background while embedding_v1 serves live traffic, and flip over when complete.",
      "Let's run our third C# project to see all four of these production fixes in code, and then open up for questions!"
    ],
    glossary: [
      ["Where to Store Vectors (Quick Rule of Thumb)", "• Under 25,000 items: In-memory C# array + TensorPrimitives (<2ms, $0 infra).\n• 25,000 to 25M items: PostgreSQL + pgvector extension.\n• 50M to 1B+ items: Dedicated Vector DB (Qdrant, Milvus, Weaviate)."]
    ]
  }
];
