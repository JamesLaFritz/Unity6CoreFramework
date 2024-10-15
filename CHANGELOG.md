# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [1.0.0.0] - 2024-05-28

### ✨ feat(coreframework): introduce Core Framework with initial setup and settings

- Added CHANGELOG.md to document changes and updates.
- Introduced CoreFramework documentation in Documentation~/CoreFramework.md.
- Renamed LICENSE to LICENSE.md for consistency.
- Added README.md for project overview and instructions.
- Created assembly definitions for editor, runtime, and test assemblies.
- Added Third Party Notices for legal compliance.
- Included package.json for package management.
- Added runtime settings and configuration in Runtime/Settings.
- Added CoreFramework preferences and project settings in Editor/Settings.

## [1.0.0.1] - 2024-10-01

### 🙈 ci(.gitignore): add .gitignore and project settings file

- add a new `.gitignore` file to ignore unnecessary files
- add project settings updater configuration in the `.idea` directory

## [1.0.0.2] - 2024-10-01

### ♻️ refactor(Editor): update variable names and switch to UI Elements

- updated variable names for clarity and consistency
- switched to using UI Elements for improved UI structure
- modified several settings-related files to reflect these changes

## [1.0.1.0] - 2024-10-01

### 🐛 fix(Settings): resolve issue with settings not saving

- fixed a bug where the project settings were not saving correctly
- made adjustments in `CoreFrameworkProjectSettings`, `CoreFrameworkProjectSettingsProvider`, and `CoreFrameworkSettings`

## [1.1.0.0] - 2024-10-1

### ✨ feat(Runtime): add Bootstrapper for initialization logic

- introduced a new `Bootstrapper.cs` file to handle application initialization logic

[unreleased]: https://github.com/JamesLaFritz/Unity6CoreFramework
[1.0.0.0]: https://github.com/JamesLaFritz/Unity6CoreFramework/commit/662e3a1daa66072d76d7eaebab442a1523257e1e
[1.0.0.1]: https://github.com/JamesLaFritz/Unity6CoreFramework/releases/tag/v1.0.0.1
[1.0.0.2]: https://github.com/JamesLaFritz/Unity6CoreFramework/releases/tag/v1.0.0.2
[1.0.1.0]: https://github.com/JamesLaFritz/Unity6CoreFramework/releases/tag/v1.0.1.0
[1.1.0.0]: https://github.com/JamesLaFritz/Unity6CoreFramework/releases/tag/v1.1.0.0
