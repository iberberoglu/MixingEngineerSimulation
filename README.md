# Mixing Engineer Simulation

**A 2D pixel-art simulation game about freelancing as a mixing engineer, with a console that actually mixes.**

[![Unity](https://img.shields.io/badge/Unity-2022.3.50f1-000000?logo=unity)](https://unity.com/)
[![FMOD](https://img.shields.io/badge/FMOD%20Studio-2.02.25-ff6600)](https://www.fmod.com/)
[![C#](https://img.shields.io/badge/C%23-27%20scripts%20·%20~2k%20LOC-239120?logo=csharp&logoColor=white)](Assets/Scripts)
[![Render](https://img.shields.io/badge/URP-14.0.11%20·%202D-2196f3)](https://unity.com/srp/universal-render-pipeline)

📄 **[Türkçe README →](README.tr.md)**

The four faders on screen drive real `Gain` parameters on FMOD event instances, and the level meters
read live peak values out of FMOD's DSP metering API. The audio side isn't a mock-up of mixing. It
*is* mixing, wired straight into the game's scoring.

![Studio](docs/screenshots/studio.png)

---

## About

I built this in 2025 for *Serbest Proje Çalışması 3* (Independent Project Study 3) at **Istanbul
Technical University, Music Technology**. Dr. Ozan Sarıer supervised the project and I submitted it
in January 2025.

My background is audio, not games. Before this I had written an RMS compressor plug-in in C++/JUCE,
and I wanted a project that put music technology *inside* something interactive, so I learned Unity
and C# from scratch for it and brought in FMOD as the audio engine, because the concept needed
DAW-grade control over individual channels.

**The premise:** you play a freshly graduated engineer with no money and no credits. Jobs arrive on
the studio computer (*"The vocals are too quiet!"*), and you sit at the console, ride four faders
until each lands in the range the client asked for, and deliver. Accuracy pays: XP and cash both
scale with how close you got. Cash buys better monitors, and better monitors reveal the target
ranges on screen. It's a progression loop where the upgrade is literally *hearing better*.

---

## Screenshots

| Mixing console: four channels, live FMOD meters | Job board on the studio computer | Store: monitor upgrades |
|---|---|---|
| ![Mixer](docs/screenshots/mixer.png) | ![Tasks](docs/screenshots/tasks.png) | ![Store](docs/screenshots/store.png) |

---

## The interesting part: the mixer is real

The easy way to build this game is to fake it: dummy sliders and a scripted "score". I wanted the
audio to genuinely respond, because that's the part I actually knew something about.

### Faders → FMOD parameter

I authored each channel as its own FMOD event instance (`event:/Track 1/Bass`, `/Drums`,
`/GuitarKeyboard`, `/Vox`), each carrying a `Gain` parameter that automates volume on its master
track. [`MixerControl.cs`](Assets/Scripts/MixerControl.cs) maps the UI slider's 0–100 range onto
−80…+10 dB and pushes it straight into FMOD:

```csharp
float normalizedValue = Mathf.Clamp01(value / 100f);
float volume = Mathf.Lerp(-80f, 10f, normalizedValue);   // dB
track.setParameterByName("Gain", volume);
```

![FMOD Studio session](docs/screenshots/fmod-studio.png)

*The FMOD Studio project: two four-channel songs plus the piano and UI events, each with the `Gain`
parameter the game drives at runtime.*

### Meters → FMOD DSP metering

[`MeteringDisplay.cs`](Assets/Scripts/MeteringDisplay.cs) walks from each event instance down to its
channel group, grabs the head DSP, enables metering on it, and polls peak level every frame:

```csharp
track.getChannelGroup(out channelGroup);
channelGroup.getDSP(0, out dsp);
dsp.setMeteringEnabled(true, true);
// ...
dsp.getMeteringInfo(out _, out meteringInfo);
float db = 20f * Mathf.Log10(Mathf.Max(meteringInfo.peaklevel[0], 0.0001f));
```

Raw per-frame peaks are far too jittery to read, so I run the value through a **10-frame moving
average** before quantising it to one of the pixel-art meter sprites. That smoothing is the whole
difference between a meter you can read and a strobe light.

### Why FMOD instead of Unity's audio

This is the decision the whole project turned on. Unity's built-in `AudioSource` and `AudioMixer`
play back audio perfectly well, but they don't expose per-channel output levels in the way a visible
mixing console needs. FMOD does, and as a bonus it let me author the entire audio side like a DAW
session, with a real mixer tree, buses and parameters, instead of assembling it in code.

---

## Game loop

```
   ┌──────────────────────────────────────────────────────────┐
   │                                                          │
   │   Walk to computer (E)  →  Pick a job  →  Accept         │
   │                                  ↓                       │
   │        MixerControl.setSong()  →  4 FMOD stems load      │
   │                                  ↓                       │
   │        Ride the faders  →  setParameterByName("Gain")    │
   │                                  ↓                       │
   │        Complete Task  →  per-channel scoring  →  XP + $  │
   │                                  ↓                       │
   │        Store  →  buy monitors  →  tolerance bands unlock │
   │                                  ↓                       │
   │        Sleep in bed (E)  →  +8 hours  →  new day, new job│
   │                                  ↓                       │
   └──────────────────────────────────────────────────────────┘
```

An in-game clock runs at **10 game-minutes per real second**, starting at 06:00 on day 1. Jobs carry
a deadline in days, and sleeping advances time by 8 hours when you'd rather skip ahead.

---

## Scoring

Delivery is graded per channel against the job's ideal fader positions
([`MixTasksManager.cs`](Assets/Scripts/MixTasksManager.cs)):

```
for each of the 4 channels:
    distance = |fader − ideal|

    distance > tolerance      →  job FAILS immediately
    distance < tolerance / 4  →  factor = 1.0        (spot on)
    otherwise                 →  factor = 1 − (distance / tolerance)

multiplier = (mean factor of critical channels × 0.7)
           + (mean factor of the rest          × 0.3)

XP and money are both scaled by that multiplier
```

Each job flags which channels are *critical*, meaning the ones the client actually complained about,
so fixing the vocal counts for far more than not breaking the bass. Levelling uses a simple
quadratic curve: XP needed for the next level is `level² × 100`.

The shipped content is two jobs on two songs:

| Job | Song | Deadline | Reward | Tolerance | Critical channels |
|---|---|---|---|---|---|
| *"The vocals are too quiet!"* | Track 1: Bass / Drums / Guitar-Keys / Vox | 2 days | 100 XP · $100 | ±10 | Vox |
| *"Drums and keys are too loud, especially the keys!"* | Track 2: Bass / Drums / Guitar / Keyboard | 4 days | 200 XP · $200 | ±9 | Drums, Keyboard |

---

## Systems

| Script | Responsibility |
|---|---|
| [`MixerControl`](Assets/Scripts/MixerControl.cs) | Owns the four FMOD event instances: slider to `Gain` mapping, transport, cleanup |
| [`MeteringDisplay`](Assets/Scripts/MeteringDisplay.cs) | FMOD DSP metering → smoothed dB → meter sprites |
| [`MixTasksManager`](Assets/Scripts/MixTasksManager.cs) | Job pool, acceptance, deadlines, scoring, rewards *(the largest script, 554 lines)* |
| [`GameTimeManager`](Assets/Scripts/GameTimeManager.cs) | In-game clock and day counter, singleton across scenes |
| [`PlayerStats`](Assets/Scripts/PlayerStats.cs) | Money, XP, level curve, singleton across scenes |
| [`StoreManager`](Assets/Scripts/StoreManager.cs) | Equipment purchases and their gameplay effects |
| [`GiveMixTips`](Assets/Scripts/GiveMixTips.cs) | Draws the tolerance bands unlocked by monitor upgrades. The better speaker draws a narrower band, and I offset both by a random amount so the hint approximates the target instead of giving it away |
| [`PlayerController`](Assets/Scripts/PlayerController.cs) / [`PlayerInteraction`](Assets/Scripts/PlayerInteraction.cs) | New Input System movement, animation blend tree, proximity interaction |

I keep jobs, songs and store items as **ScriptableObjects**
([`Assets/Scriptable Objects/`](Assets/Scriptable%20Objects)), so new content goes in through the
Inspector rather than through code.

<details>
<summary><b>Repository layout</b></summary>

```
Assets/
├── Scripts/                 27 C# scripts, ~2,040 lines
├── Scriptable Objects/      Mix Task 1–2, Song 1–2, Item Costs
├── Scenes/                  MainMenu, MainScene
├── StreamingAssets/         7 compiled FMOD banks
├── Input System/            Input Actions asset (WASD + E)
├── Plugins/FMOD/            FMOD Unity Integration 2.02.25
├── Sprites/ · Animations/   Pixel-art assets
└── Prefabs/
docs/screenshots/            Images used by this README
```
</details>

---

## Running it

**Requirements:** Unity **2022.3.50f1** (this exact version) with the Universal 2D template.

```bash
git clone https://github.com/iberberoglu/MixingEngineerSimulation.git
```

Open the folder in Unity Hub, load `Assets/Scenes/MainMenu.unity`, and press Play. The compiled FMOD
banks are committed under `Assets/StreamingAssets/`, so **FMOD Studio is not needed to run the
game**, only to edit its audio.

| Control | Action |
|---|---|
| `WASD` / arrows | Move |
| `E` | Interact (computer · mixing console · bed · piano) |
| `Esc` | Pause menu |

> 🎹 There's a piano in the corner of the studio. Press `E` at it.

**Editing the audio** additionally requires the FMOD Studio source project
(`Producer-Simulation-FMOD.fspro`), which is not part of this repository. The compiled banks it
produces are, so the game runs without it.

---

## Scope and known limitations

This is an honest prototype, not a finished game, and I'd rather say so than oversell it. I shipped
it as coursework with the core loop complete end to end (mixing, scoring, economy, progression,
time) and two jobs' worth of content on top.

Coming back to the code in 2026, I reviewed it properly and wrote the findings down:

- **Content is thin.** Two jobs, two songs. The pool empties by day three, and a failed job gets
  marked completed rather than returning to the pool.
- **No save system.** Money, XP, level and purchases reset when the game closes, which is the single
  largest structural gap for a progression game.
- **The deadline mechanic never fires.** The clock resets to zero each day while job deadlines are
  computed as absolute minutes, so the comparison never triggers. The game's only time pressure is
  effectively dead code.
- Plus a handful of smaller issues: a `StopCoroutine` that stops nothing, mixed old and new input
  systems, a float equality comparison in the scoring path.

The full review, prioritised, is in [`YAPILACAKLAR.md`](YAPILACAKLAR.md) *(Turkish)* and doubles as
the project backlog.

### Where it would go next

Processing tasks beyond level balance (EQ, compression, reverb) · purchasable plug-ins as mechanics ·
studio expansion into new room types and job categories · recording sessions · a save system to make
the progression stick.

---

## Credits

- **Code, game design, FMOD project, audio:** İsmail Berberoğlu
- **Mixing console and custom pixel art:** I made these in Photoshop together with a friend
- **Character, interior and UI sprites:** free asset packs (Characters_free, Interiors_free,
  Complete UI Essential Pack, sierrassets furniture pack), each under its own licence
- **Supervisor:** Dr. Ozan Sarıer, İTÜ Music Technology

Built with [Unity](https://unity.com/) · [FMOD Studio](https://www.fmod.com/) by Firelight
Technologies · [TextMesh Pro](https://docs.unity3d.com/Manual/com.unity.textmeshpro.html) ·
[Cinemachine](https://unity.com/unity/features/editor/art-and-design/cinemachine)

---

<sub>I originally developed this under the name *Producer Simulation* and renamed it to *Mixing
Engineer Simulation* in 2026.</sub>
