# CP2077 Save Exporter

![.NET](https://img.shields.io/badge/.NET-8.0-blue)
![Status](https://img.shields.io/badge/status-active-success)
![License](https://img.shields.io/badge/license-MIT-green)

---

## 🚀 Overview

**CP2077 Save Exporter** is a **read-only CLI tool** that converts Cyberpunk 2077 save files into structured JSON optimized for **AI-driven gameplay analysis**.

It uses the in-repo WolvenKit stack (via CyberCAT) to decode save data safely and deterministically.

It is designed to answer:

- What should I do next to reach 100% completion?
- Where am I under-progressed?
- Is my build efficient?
- What should I do in a 30 / 60 / 120 minute session?

---

## 🧠 Design Intent

This is **not a save editor** and does not depend on the CyberCAT GUI.

It is a **game-state intelligence layer** built for:

- structured interpretation
- AI reasoning
- repeatable decision-making

| Principle | Description |
|----------|------------|
| Read-only | Never modifies save files |
| Signal-first | Focus on actionable insights |
| AI-native | Designed for LLM consumption |
| Deterministic | Same input → same output |

---

## 📦 Output Modes

```bash
CP2077SaveExporter [--mode raw|full|insights|split] [--facts <path>] <sav.dat> [output-directory]
```

| Mode | Output | Use Case |
|------|--------|---------|
| raw | save.raw.json | Debug / inspection |
| full (default) | save.full.json | Canonical dataset (recommended for AI) |
| insights | save.insights.json | Derived-only (advanced use) |
| split | raw + insights | Separation workflows |

---

## 🧱 Data Model

### Full Output (Canonical)

```json
{
  "header": {},
  "raw": {},
  "normalized": {},
  "derived": {}
}
```

---

## 📊 Derived Signals (Core Value)

Derived signals summarize patterns in the data so AI models can reason efficiently without scanning the entire raw dataset:

- Progression stage
- Completion signals
- Build profile
- Cyberware utilization
- Inventory insights
- Exploration proxy
- Expansion detection

> These are **heuristics**, not exact values.

---

## 🖼️ Example Workflow

```bash
CP2077SaveExporter --mode full sav.dat
```

Then provide the JSON to an AI model with a structured prompt, for example:

> “Given this save state, what should I do next to efficiently reach 100% completion?”

---

## 🤖 AI Usage Guidance

For best results:

- Use **FULL mode**
- Combine:
  - derived signals (direction)
  - raw data (grounding)

---

## ⚙️ Installation & Build

### Requirements
- .NET 8 SDK

### Development Run

Run from the repository root (required for WolvenKit project references to resolve correctly):

```bash
dotnet run --project CP2077SaveExporter -- <path-to-save>
```

### Publish (Recommended)

Use the provided scripts to build standalone executables.

These builds are isolated to this component and do not affect the main solution or GUI project.

From inside `CP2077SaveExporter/`:

#### Linux / macOS
```bash
./publish.sh
```

#### Windows (PowerShell)
```powershell
.\publish.ps1
```

Outputs:

```
CP2077SaveExporter/publish/
  ├── win-x64/
  └── linux-x64/
```

Each directory contains a self-contained, single-file executable and required runtime components.

---

## 🧾 Facts.json Handling

`Facts.json` maps internal game fact hashes to human-readable names.

### Canonical Source

The authoritative upstream file is located in the CyberCAT repository:

```
https://github.com/Deweh/CyberCAT-SimpleGUI
./CP2077SaveEditor/Resources/Facts.json
```

This should be treated as the **baseline canonical mapping**.

---

### Runtime Resolution

The exporter resolves `Facts.json` in the following order:

1. `--facts <path>`  
   → Explicit override (recommended when testing or extending)

2. `Facts.json` beside the executable  
   → Drop-in file next to the published binary

3. Fallback  
   → If not found, exporter continues with hash-only values (with warning)

---

### Recommended Usage

```bash
CP2077SaveExporter --facts /path/to/Facts.json sav.dat
```

or place:

```
Facts.json
```

in the same directory as the executable.

---

### Future Enhancement

`Facts.json` is intentionally external and extensible.

You can:

- augment it with additional hash mappings
- maintain a personal extended version
- contribute improvements upstream

This allows progressive improvement of semantic decoding without modifying exporter code.

---

## ⚠️ Limitations

- Derived signals are heuristic
- Progress is approximate
- Fact coverage is incomplete

> The goal is **useful decisions**, not perfect simulation.

---

## 🛠️ Development Philosophy

We optimize for:

- interpretability
- signal clarity
- cross-model stability

We avoid:

- unnecessary complexity
- deep reverse engineering

---

## 📌 Roadmap

- Improve build signals
- Reduce ambiguity
- Expand fact coverage
- Improve cross-AI consistency

---

## Optional AI Usage Resources

Example downstream AI-analysis materials are available separately:

- Example analysis prompt: [[link](https://gist.github.com/generationtech/8f1def7976a2ab011118624672740107)]
- Prompt evaluation notes: [[link](https://gist.github.com/generationtech/41f97dd27ccb3b9bacb5ea6d7617a1a3)]

These are optional reference materials and are not required to use the exporter.

---

## 🤝 Contributing

Focus on:

- signal quality
- clarity
- consistency

Avoid:

- large refactors
- architectural changes

---

## 📄 License

MIT
