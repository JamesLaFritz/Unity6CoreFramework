# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

[![Unreleased](https://img.shields.io/badge/Core_Framework-blue?logo=github&label=Unreleased)](https://github.com/JamesLaFritz/Unity6CoreFramework)

[![GitHub Release](https://custom-icon-badges.demolab.com/github/v/release/JamesLaFritz/Unity6CoreFramework?sort=date&display_name=release&style=plastic&label=Latest%20Release&logo=tag)](https://github.com/JamesLaFritz/Unity6CoreFramework/releases/latest)

[![GitHub Release](https://img.shields.io/github/v/release/JamesLaFritz/Unity6CoreFramework?sort=date&filter=*v1.2.0.0&display_name=release&style=plastic&label=%E2%9C%A8%20feat(Settings)%3A%20add%20version%20control%20and%20initialization%20support)](https://github.com/JamesLaFritz/Unity6CoreFramework/releases/tag/v1.2.0.0)
✨ feat(Settings): add version control and initialization support
- introduced new version control classes: `VersionInitialization`, `VersionProjectSettings`, `VersionProjectSettingsProvider`, and `VersionControl`
- updated existing settings files for integration with version management
- made related updates in `CHANGELOG.md` to document these changes

[![GitHub Release](https://img.shields.io/github/v/release/JamesLaFritz/Unity6CoreFramework?sort=date&filter=*v1.1.0.0&display_name=release&style=plastic&label=%E2%9C%A8%20feat(Runtime)%3A%20add%20Bootstrapper%20for%20initialization%20logic)](https://github.com/JamesLaFritz/Unity6CoreFramework/releases/tag/v1.1.0.0)
- introduced a new `Bootstrapper.cs` file to handle application initialization logic

[![GitHub Release](https://img.shields.io/github/v/release/JamesLaFritz/Unity6CoreFramework?sort=date&filter=*v1.0.1.0&display_name=release&style=plastic&label=%F0%9F%90%9B%20fix(Settings)%3A%20resolve%20issue%20with%20settings%20not%20saving)](https://github.com/JamesLaFritz/Unity6CoreFramework/releases/tag/v1.0.1.0)
- fixed a bug where the project settings were not saving correctly
- made adjustments in `CoreFrameworkProjectSettings`, `CoreFrameworkProjectSettingsProvider`, and `CoreFrameworkSettings`

[![GitHub Release](https://img.shields.io/github/v/release/JamesLaFritz/Unity6CoreFramework?sort=date&filter=*v1.0.0.2&display_name=release&style=plastic&label=%E2%99%BB%EF%B8%8F%20refactor(Editor)%3A%20update%20variable%20names%20and%20switch%20to%20UI%20Elements)](https://github.com/JamesLaFritz/Unity6CoreFramework/releases/tag/v1.0.0.2)
- updated variable names for clarity and consistency
- switched to using UI Elements for improved UI structure
- modified several settings-related files to reflect these changes

[![GitHub Release](https://img.shields.io/github/v/release/JamesLaFritz/Unity6CoreFramework?sort=date&filter=*v1.0.0.1&display_name=release&style=plastic&label=%F0%9F%99%88%20ci(.gitignore)%3A%20add%20.gitignore%20and%20project%20settings%20file)](https://github.com/JamesLaFritz/Unity6CoreFramework/releases/tag/v1.0.0.1)
- add a new `.gitignore` file to ignore unnecessary files
- add project settings updater configuration in the `.idea` directory

[![GitHub Release](https://img.shields.io/github/v/release/JamesLaFritz/Unity6CoreFramework?sort=date&filter=*v1.0.0.0&display_name=release&style=plastic&label=%E2%9C%A8%20feat(coreframework)%3A%20introduce%20Core%20Framework%20with%20initial%20setup%20and%20settings)](https://github.com/JamesLaFritz/Unity6CoreFramework/releases/tag/v1.0.0.0)
- Added CHANGELOG.md to document changes and updates.
- Introduced CoreFramework documentation in Documentation~/CoreFramework.md.
- Renamed LICENSE to LICENSE.md for consistency.
- Added README.md for project overview and instructions.
- Created assembly definitions for editor, runtime, and test assemblies.
- Added Third Party Notices for legal compliance.
- Included package.json for package management.
- Added runtime settings and configuration in Runtime/Settings.
- Added CoreFramework preferences and project settings in Editor/Settings.
