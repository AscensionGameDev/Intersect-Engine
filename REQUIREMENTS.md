# Requirements

## Dependencies

If you are unable to run the engine, please make sure to download the [.NET 8 Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/8.0).

## Support Matrix

--------------------------------------------------------------------------------------------
| OS                  | Official | Status | Notes                                          |
----------------------|----------|--------|-------------------------------------------------
| Android             |          |        | 🚫 Not Yet Supported                           |
| iOS                 |          |        | 🚫 Not Yet Supported                           |
| [Linux](#linux)     |    ✅    |   ✅   | ARM64 not currently supported                  |
| [MacOS](#macos)     |    ✅    |   ✅   | ARM64 only supported via [Rosetta][rosetta]    |
| [Windows](#windows) |    ✅    |   ✅   | Only 10+ on x64 supported, but others may work |
--------------------------------------------------------------------------------------------

## Linux

### Tested Distros

---------------------------------------------------------------------------------------------------------------------------------------------------
| Distro              | Kernel             | Status | Notes                            | Version tested                                           |
----------------------|--------------------|--------|----------------------------------|----------------------------------------------------------|
| Manjaro             | 5.15.133-1-MANJARO |   ✅   | Only tested on dev machine       |                                                          |
| Arch                | 6.13.2-arch1-1     |   ✅   | Only tested on dev machine       | v0.8.0.433-beta+7542bc79922660075708287220a648a150a54d8f |
---------------------------------------------------------------------------------------------------------------------------------------------------

## MacOS

> Note: Launching the Client/Server from Finder via clicking appears to launch it from `$HOME`.
>
> You need to launch them from the command line or via a wrapper (not included).

-------------------------------------------------------------------------------
| Version          | Official | Status                                        |
-------------------|----------|------------------------------------------------
| 11.7 Big Sur     |    🚫    | [⚠️](#macos "Untested")                       |
| 12.7 Monterey    |    🚫    | [⚠️](#macos "Untested")                       |
| 13.6 Ventura     |    ✅    | [✅*](#macos "Only tested on dev machine")    |
| 14.0 Sonoma      |    ✅    | [✅*](#macos "Untested")                      |
-------------------------------------------------------------------------------

## Windows

-------------------------------------------------------------------------------
| Version          | Official | Status                                        |
-------------------|----------|------------------------------------------------
| Windows 7        |    🚫    | [❓](#windows    "End-of-Life 2020-01-14")    |
| Windows 8        |    🚫    | [❓](#windows    "End-of-Life 2016-01-12")    |
| Windows 8.1      |    🚫    | [⚠️](#windows-81 "End-of-Life 2023-07-11")    |
| Windows 9        |    🪦    | [🪦](#windows    "They skipped it")           |
| Windows 10       |    ✅    | ✅                                            |
| Windows 11       |    ✅    | ✅                                            |
-------------------------------------------------------------------------------

### Windows 8.1

- [KB3118401](https://www.microsoft.com/en-us/download/details.aspx?id=51109)

[rosetta]: https://support.apple.com/en-us/HT211861
