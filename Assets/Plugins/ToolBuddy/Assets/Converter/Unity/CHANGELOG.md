# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
This project does not adhere to Semantic Versioning.

When updating:
- Always remove the old version before installing the new one.
- Update all the related software (Curvy Splines and other converters)

## [1.3.2] - 2024-11-23

### Fixed
- Warnings appear on the console when using Curvy Splines 8.10.0.


## [1.3.1] - 2024-07-29

### Fixed
- Warnings appear on the console when compiling with Unity 6 preview.


## [1.3.0] - 2024-07-19

### Changed
- Unity To Curvy conversions now reuse existing Curvy Splines.
- Updated warning message when converting a Catmull-Rom spline

### Fixed
- Builds fail because of Converter.
- Updating a Unity Spline can lead to conversion even if conversion direction is Curvy To Unity.


## [1.2.0] - 2024-05-23

### Added
- Conversion from Curvy splines to Unity splines.
- Messages window, available at Window -> Converter -> Messages.
- Menu item to open the asset's publisher's page.

### Changed
- Converter now compiles even if Curvy Splines is missing.
- Minimum Curvy Splines compatible version is now 8.9.1.

### Fixed
- Newly created splines are not converted automatically.
- Converting a Unity spline removes all its children.


## [1.1.0] - 2024-05-16

### Added
- Real time spline conversion.
- Tooltips to overlay buttons.

### Changed
- Overlay buttons are now toggles. When a toggle is on, the conversion happens whenever the spline is changed.
- Hotkeys/shortcuts and menu items were modified to reflect the new conversion behaviour.

### Fixed
- Text of overlay buttons is not centered.


## [1.0.0] - 2024-04-20

- Initial Release