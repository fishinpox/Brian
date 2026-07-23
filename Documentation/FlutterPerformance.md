# Flutter Frontend Research

Research notes from evaluating whether to integrate the existing Flutter calendar app (`C:\Users\Gasco\source\repos\Horderp\CalAppTest2`, package `custom_calendar_pro`) into the Brian platform, vs. rewriting it in HTML/CSS/JS.

**Date:** July 2026

---

## Flutter Web compilation & JS target

Flutter Web doesn't compile down to hand-writable HTML/CSS — it's fundamentally different from a normal web frontend:

- **Compiler**: Dart source compiles via `dart2js` (JS output) or the newer `dart2wasm` (WebAssembly). As of 2026 the ecosystem is "Wasm-first" — Flutter builds emit a WasmGC module plus a JS bootstrap shim that does feature-detection at load time, falling back to the pure-JS (`dart2js`) build only on browsers without WasmGC support (pre-Chromium 119-ish / older Safari).
- **JS version**: there's no fixed "ES5/ES6" target you'd choose — the JS glue code Flutter emits targets modern evergreen browsers; legacy/IE-era output was dropped years ago. You don't hand-write or version JS yourself in a Flutter app.
- **Rendering**: this is the key nuance for the "convert to HTML/CSS/JS?" question — Flutter Web paints pixels via Skia compiled to WebAssembly (**CanvasKit**, the default, ~1.5MB) or the newer **Skwasm** renderer, onto a `<canvas>` element. The old **HTML renderer** (real DOM nodes/CSS) is now considered a legacy fallback, being phased out. So a Flutter Web build is not "HTML/CSS" in the conventional sense even on the web — it's a canvas-rendered app shipped as Wasm+JS. If SEO, screen-reader-first accessibility, or lightweight static pages matter, that's a real constraint against Flutter Web specifically (native mobile/desktop builds don't have this concern at all).

## Common package stack — and how CalAppTest2 compares

| Concern | 2026 consensus pick | What CalAppTest2 already uses |
|---|---|---|
| State management | **Riverpod 3.x** — only Flutter Favorite in this category, lowest boilerplate | ✅ `flutter_riverpod ^3.3.1` |
| Routing | **go_router** — official Flutter-team package | ✅ `go_router ^17.2.3` |
| HTTP client | **Dio** for production (interceptors, cancellation, auth headers); plain `http` is considered tutorial-grade | ⚠️ `http ^1.2.2` — worth revisiting once wiring real JWT auth against the Gateway |
| Immutable models/DTOs | **Freezed** + `json_serializable`, paired with Riverpod | ❌ not used — models are hand-written |
| Dependency injection | Riverpod's own providers (GetIt is the Bloc-world equivalent) | ✅ not needed, consistent with using Riverpod |
| Calendar widget | `table_calendar` is a standard choice | ✅ `table_calendar ^3.2.0` |
| Animation | `flutter_animate` is a common utility package | ✅ `flutter_animate ^4.5.0` |

The existing app's stack is already aligned with current mainstream Flutter practice — Riverpod + go_router is exactly what's recommended in 2026. The one gap worth flagging before wiring it to the real backend is `http` → `Dio` (or an equivalent auth-interceptor layer), since talking to Identity/Calendar's JWT-protected endpoints benefits from Dio's interceptor support for attaching/refreshing bearer tokens automatically rather than threading headers through every call by hand.

---

## Flutter vs. native mobile performance

Short answer: yes, slightly slower in a few specific areas, but for a typical CRUD-style app like this calendar it's not something users would notice. Caveat: most of what's published on this is marketing/SEO agency content, not rigorous academic benchmarking, so treat exact percentages as directional, not precise.

### Where the gap shows up

- **Cold start time**: native wins fairly consistently — Android native lands around ~250ms vs. Flutter's AOT-compiled apps generally landing under ~300ms–2s depending on the source (the spread itself tells you these aren't controlled benchmarks). Real-world difference is on the order of tens to a couple hundred milliseconds.
- **Raw CPU-bound computation**: native has a genuine edge — one benchmark found native completing equivalent computation in ~50% less time than Dart. This matters for things like heavy image processing or complex algorithms, not for rendering a calendar grid.
- **Memory footprint**: Flutter apps run somewhat heavier because they bundle their own rendering engine (Skia/Impeller) rather than using the OS's native UI toolkit — estimates put this at tens of MB extra, sometimes described as "roughly double" on the low end. Only matters on older/low-RAM devices.

### Where Flutter is essentially at parity

- **Frame rate / animation smoothness**: since Flutter shipped its Impeller rendering engine, it consistently hits a stable 60fps (120fps on ProMotion displays) for typical UI work — comparable to native, and several 2026 benchmarks even show it beating React Native here.

### Relevance to Brian

For a calendar/task app specifically — month grids, event lists, forms, live-stream avatars — this is squarely in the "UI-bound, not compute-bound" category where the consensus across these sources holds: performance is not a real differentiator, and CalAppTest2 is already built with Impeller-era Flutter. The one place worth watching is cold-start time if this becomes a frequently-launched mobile app, but that's a minor, fixable-later concern, not a reason to avoid Flutter.

---

## Sources

- [Support for WebAssembly (Wasm)](https://docs.flutter.dev/platform-integration/web/wasm)
- [Flutter Web & WebAssembly in 2026: What Actually Changed](https://amgres.com/blog/flutter-web-webassembly-wasm-2026-guide)
- [Web renderers](https://docs.flutter.dev/platform-integration/web/renderers)
- [Flutter Web & Desktop 2026: Production Readiness Honest Guide](https://softaims.com/blog/flutter-web-desktop-production-ready-2026)
- [Flutter State Management in 2026: Riverpod vs Bloc vs Provider](https://samioda.com/en/blog/flutter-state-management-2026)
- [Flutter State Management in 2026: Decision Guide for Production Apps](https://ishaqhassan.dev/blog/flutter-state-management-2026-guide.html)
- [Top Flutter HTTP Client, Caching and other HTTP Utility packages](https://fluttergems.dev/http-client-utilities/)
- [Top Flutter Routing, Navigator, Navigator 2.0 packages](https://fluttergems.dev/routing/)
- [React Native vs Flutter 2026: Benchmarks & Performance Guide](https://adevs.com/blog/react-native-vs-flutter/)
- [Flutter vs React Native 2026: Performance Cost DX](https://www.agilesoftlabs.com/blog/2026/02/flutter-vs-react-native-2026-cost-dx_17)
- [What is the Performance of Flutter vs. Native vs. React-Native?](https://www.techaheadcorp.com/blog/what-is-the-performance-of-flutter-vs-native-vs-react-native/)
- [Comparing Real-World Apps Developed with Flutter & Native Android](https://www.aubergine.co/insights/comparing-the-performance-of-an-app-built-with-native-android-and-flutter)
- [Flutter and React Native performance overview](https://sudolabs.com/insights/flutter-and-react-native-performance-overview)
