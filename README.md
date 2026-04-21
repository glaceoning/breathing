# Breathing Overlay (Windows)

A lightweight WinForms utility that shows a movable, resizable, borderless breathing overlay only while the session is running.

## Behavior

- Overlay is **frameless** and **always-on-top**.
- Overlay opens only when you press **Start** and closes on **Stop**.
- Drag from inside the overlay to move it.
- Resize from window edges/corners.
- Rectangle by default, with optional square lock.

## Breathing visualization

- **Inhale:** white background, blue (default) bar fills left → right in second-based steps.
- **Exhale:** white background, green (default) bar empties left → right in second-based steps.
- **Hold in (full lungs):** black background, red (default) fills from center outward in mirrored pairs.
- **Hold out (empty lungs):** black background, white (default) fills from center outward in mirrored pairs.

Per-phase durations are configurable in seconds, and hold phases can be turned on/off independently.

## Build and run

```bash
dotnet build -c Release
```

Run the built `.exe` from `bin/Release/net8.0-windows/` on Windows.

## Notes about game anti-cheat systems

This tool is a normal desktop overlay utility and is not designed for cheating. No software can guarantee how third-party anti-cheat systems classify or react to running processes.
