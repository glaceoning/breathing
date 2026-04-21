# Breathing Overlay (Windows)

A tiny WinForms app that shows a movable, resizable, always-on-top breathing shape overlay (rectangle or locked square) and cycles colors by breathing phase.

## Features

- Always-on-top overlay window you can drag/resize anywhere on screen.
- Rectangle by default, with an option to lock to a square.
- Four configurable breathing phases:
  - Inhale (default blue: `0,0,255`)
  - Hold after inhale (default red: `255,0,0`)
  - Exhale (default green: `0,255,0`)
  - Hold after exhale (default white: `255,255,255`)
- Optional hold-in and hold-out phases (toggle each on/off).
- Per-phase duration in seconds (`0-300`).
- Lightweight (single timer tick per second, no elevated privileges, no input hooks).

## Build and run

```bash
dotnet build -c Release
```

Run the built `.exe` from `bin/Release/net8.0-windows/` on Windows.

## Notes about game anti-cheat systems

This tool is a normal desktop overlay utility and is not designed for cheating. However, no software can *guarantee* how third-party anti-cheat systems will classify or react to running processes. If you are concerned, check the game's policy and run this only when you are comfortable with that risk.
