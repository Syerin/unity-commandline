# Changelog

All notable changes to this package are documented here. Format follows
[Keep a Changelog](https://keepachangelog.com/) and the project uses
[Semantic Versioning](https://semver.org/).

## [1.1.0]
### Added
- Built-in `/help` (and `/?`): lists the commands of **every** registered module, not any single
  subsystem. `Cli.GetUsage()` exposes the same text for an in-game console.
- `ICliModule` now declares `Name` and `Commands` (a list of `CliCommand`) so `/help` can list them.

### Changed
- **Breaking:** modules must implement `Name` and `Commands`. Registering a command twice — or shadowing
  `/help` / `/?` — now throws `InvalidOperationException` at `Cli.Initialize`.

## [1.0.0] - 2026-06-21
### Added
- Initial release.
