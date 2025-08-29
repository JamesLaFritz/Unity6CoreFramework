# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

> **UPM versioning:** `package.json` uses **SemVer** (`MAJOR.MINOR.PATCH`).  
> Build/revision info is carried as **build metadata**, e.g. `1.4.0+rev.0`.  
> Package resolution ignores `+metadata`, so publish patches as `1.4.1`, `1.4.2`, … (don’t only change `+rev` if you want Package Manager to upgrade).

---

## [![Unreleased](https://img.shields.io/badge/Core_Framework-blue?logo=github&label=Unreleased)](https://github.com/JamesLaFritz/Unity6CoreFramework)

- _Add entries here for changes after 1.4.0._

[![GitHub Release](https://custom-icon-badges.demolab.com/github/v/release/JamesLaFritz/Unity6CoreFramework?sort=date&display_name=release&style=plastic&label=Latest%20Release&logo=tag)](https://github.com/JamesLaFritz/Unity6CoreFramework/releases/latest)

---

## [1.4.0] — 2025-08-29

### Added
- **Squirrel Noise Random**: stateless, deterministic noise/RNG suite.
  - Perlin & Simplex (2D/3D), Cellular/Worley (F1/F2, combos), fBm/Billow/Ridge stacks.
  - Domain warping utilities, slope/derivative helpers, seedable wrappers (32/64-bit).
- **Unity.Mathematics integration**:
  - `Unity.Mathematics.noise.cnoise` hook for baseline Perlin calls.
  - New numeric types: `ulong2`, `ulong4`.
- **RNG systems**:
  - Noise-based RNG and **crypto-safe RNG** options with consistent APIs.
- **Runtime extensions**:
  - Comprehensive set of common extension methods (collections, math, Unity types).
- **Patterns & interfaces**:
  - `IVisitor` / `IVisitable` for component visitation.
  - Generic **Flyweight** and **Factory** helpers.

### Changed
- **Packaging:** `package.json` now **SemVer** (`"version": "1.4.0+rev.0"`).  
  (Replaces previous 4-segment scheme like `1.3.0.0`.)
- Updated badges/docs to reflect new versioning and install instructions.

### Fixed
- Added missing `.meta` files to prevent import warnings.
- Removed orphaned `Gemfile.lock.meta`.

### Notes on compatibility
- No breaking API changes detected; all additions are backward-compatible.
- If you introduce **new auto-referenced assemblies** later, that may require a **MAJOR** bump per Unity’s guidance.

**Diff:** `v1.3.0.0...1.4.0`  
**UPM URL:** `https://github.com/JamesLaFritz/Unity6CoreFramework.git#1.4.0`  
*(Package resolution treats `1.4.0+rev.0` the same as `1.4.0`.)*

---

## [1.3.0.0] — 2024-10-16 [![GitHub Release](https://img.shields.io/github/v/release/JamesLaFritz/Unity6CoreFramework?sort=date&filter=*v1.3.0.0&display_name=release&style=plastic&label=%E2%9C%A8%20feat(Editor)%3A%20add%20build%20processing%20automation%20script)]([https://github.com/JamesLaFritz/Unity6CoreFramework/releases/tag/v1.3.0.0](https://github.com/JamesLaFritz/Unity6CoreFramework/releases/tag/v1.3.0.0))

### Added
- **Editor build processing automation** (`BuildProcessing.cs`):
  - Version incrementing, build pipeline execution, version file export.
  - Optional installer creation via build preprocess/postprocess hooks.

---

## [1.2.0.0] — 2024-10-16 [![GitHub Release](https://img.shields.io/github/v/release/JamesLaFritz/Unity6CoreFramework?sort=date&filter=*v1.2.0.0&display_name=release&style=plastic&label=%E2%9C%A8%20feat(Settings)%3A%20add%20version%20control%20and%20initialization%20support)](https://github.com/JamesLaFritz/Unity6CoreFramework/releases/tag/v1.2.0.0)

### Added
- **Version control & initialization**:
  - `VersionInitialization`, `VersionProjectSettings`, `VersionProjectSettingsProvider`, `VersionControl`.

---

## [1.1.0.0] — 2024-10-15 [![GitHub Release](https://img.shields.io/github/v/release/JamesLaFritz/Unity6CoreFramework?sort=date&filter=*v1.1.0.0&display_name=release&style=plastic&label=%E2%9C%A8%20feat(Runtime)%3A%20add%20Bootstrapper%20for%20initialization%20logic)](https://github.com/JamesLaFritz/Unity6CoreFramework/releases/tag/v1.1.0.0)

### Added
- **Runtime Bootstrapper** to centralize app initialization.

---

## [1.0.1.0] — 2024-10-15 [![GitHub Release](https://img.shields.io/github/v/release/JamesLaFritz/Unity6CoreFramework?sort=date&filter=*v1.0.1.0&display_name=release&style=plastic&label=%F0%9F%90%9B%20fix(Settings)%3A%20resolve%20issue%20with%20settings%20not%20saving)](https://github.com/JamesLaFritz/Unity6CoreFramework/releases/tag/v1.0.1.0)

### Fixed
- Project settings not saving correctly (`CoreFrameworkProjectSettings`, `CoreFrameworkProjectSettingsProvider`, `CoreFrameworkSettings`).

---

## [1.0.0.2] — 2024-10-15[![GitHub Release](https://img.shields.io/github/v/release/JamesLaFritz/Unity6CoreFramework?sort=date&filter=*v1.0.0.2&display_name=release&style=plastic&label=%E2%99%BB%EF%B8%8F%20refactor(Editor)%3A%20update%20variable%20names%20and%20switch%20to%20UI%20Elements)](https://github.com/JamesLaFritz/Unity6CoreFramework/releases/tag/v1.0.0.2)

### Changed
- Editor refactor: variable name clean-up; migrated UI to **UI Elements**.

---

## [1.0.0.1] — 2024-10-15 [![GitHub Release](https://img.shields.io/github/v/release/JamesLaFritz/Unity6CoreFramework?sort=date&filter=*v1.0.0.1&display_name=release&style=plastic&label=%F0%9F%99%88%20ci(.gitignore)%3A%20add%20.gitignore%20and%20project%20settings%20file)](https://github.com/JamesLaFritz/Unity6CoreFramework/releases/tag/v1.0.0.1)
- add a new `.gitignore` file to ignore unnecessary files
- add project settings updater configuration in the `.idea` directory
### CI
- Add `.gitignore` and project settings file.

---

## [1.0.0.0] — 2024-07-31 [![GitHub Release](https://img.shields.io/github/v/release/JamesLaFritz/Unity6CoreFramework?sort=date&filter=*v1.0.0.0&display_name=release&style=plastic&label=%E2%9C%A8%20feat(coreframework)%3A%20introduce%20Core%20Framework%20with%20initial%20setup%20and%20settings)](https://github.com/JamesLaFritz/Unity6CoreFramework/releases/tag/v1.0.0.0)

### Added
- Initial Core Framework:
  - Docs (`README.md`, `Documentation~/CoreFramework.md`, `Third Party Notices`), `package.json`.
  - Assembly definitions for editor, runtime, tests.
  - Runtime settings, editor preferences & project settings.
  - License normalization.

---

## Versioning & tagging

- **Current package version:** `1.4.0+rev.0` (SemVer + build metadata).  
- **Git tags:** publish `1.4.0` for UPM installs; keep legacy `v1.3.0.0`-style tags for history if desired.  
- **Pre-releases:** use `1.5.0-pre.1`, `1.5.0-pre.2`, …  
- **Hotfixes:** bump PATCH (`1.4.1`, `1.4.2`, …). Don’t rely on changing `+rev` alone.

---

[Unreleased]: https://github.com/JamesLaFritz/Unity6CoreFramework/compare/1.4.0...HEAD
[1.4.0]: https://github.com/JamesLaFritz/Unity6CoreFramework/compare/v1.3.0.0...1.4.0
[1.3.0.0]: https://github.com/JamesLaFritz/Unity6CoreFramework/releases/tag/v1.3.0.0
[1.2.0.0]: https://github.com/JamesLaFritz/Unity6CoreFramework/releases/tag/v1.2.0.0
[1.1.0.0]: https://github.com/JamesLaFritz/Unity6CoreFramework/releases/tag/v1.1.0.0
[1.0.1.0]: https://github.com/JamesLaFritz/Unity6CoreFramework/releases/tag/v1.0.1.0
[1.0.0.2]: https://github.com/JamesLaFritz/Unity6CoreFramework/releases/tag/v1.0.0.2
[1.0.0.1]: https://github.com/JamesLaFritz/Unity6CoreFramework/releases/tag/v1.0.0.1
[1.0.0.0]: https://github.com/JamesLaFritz/Unity6CoreFramework/releases/tag/v1.0.0.0
