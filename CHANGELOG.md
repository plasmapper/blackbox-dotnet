# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]
### Added
- Async support.
- Index property to IHardwareInterface and IServer.

### Changed
- Configuration and state properties to be null if not applicable.
- Documented ArgumentOutOfRangeException, Exception and NotSupportedException on IClient, IHardwareInterface and IServer members.
- Added messages to NotSupportedException thrown by IHardwareInterface and IServer setters not applicable to the current type.

### Fixed
- Wrong IPv6 address returned by SetIpV6GlobalAddress.
- Hardware interface/server index out of range exception type.
- Unclear exception when SetDeviceName, SetWiFiSsid or SetWiFiPassword value is too long.
- Silent corruption of non-ASCII characters in SetDeviceName, SetWiFiSsid and SetWiFiPassword values.

## [1.1.1] - 2026-05-27
### Fixed
- Package documentation.

## [1.1.0] - 2026-05-21
### Added
- Device compatibility validation.

## [1.0.0] - 2024-09-25
Initial release.