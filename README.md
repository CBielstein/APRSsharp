# APRS# (APRSsharp)

APRS# - Modern APRS software for the amateur radio community

## About APRS

The [Automatic Packet Reporting System](https://en.wikipedia.org/wiki/Automatic_Packet_Reporting_System)
is a digital wireless network designed and run by [amateur (ham) radio](https://en.wikipedia.org/wiki/Amateur_radio)
operators around the world. Introduced in 1980, it is an ad-hoc, multi-hop,
distributed network through which packets are transmitted and retransmitted
between nodes to increase range. Data transmitted through APRS includes
positional information, personal messages, weather station readings, search and
rescue locations, and more. APRS packets are encoded and decoded by amateur
radio devices, specialized hardware or software, or a combination.

## APRS# Project

The protocol has been in use since its introduction in the 1980s
([see the site by the original creator here](http://aprs.org/)) and has seen
various incarnations of encode/decode/visualize software. This project exists
to eventually become a modern user interface for sending, receiving, and
visualizing APRS digital packets in a straight-forward manner.

This project is provided as [open source](LICENSE) and developed by the
community for the community.

## Contributions

Please see the [code of conduct](CODE_OF_CONDUCT.md) and
[contributing document](CONTRIBUTING.MD) for details on contributing to
this project.

## Development

### Dependencies

The target is .NET Core 3.1 for development (netcoreapp3.1)

### Building

Warnings are treated as errors in the release build (as in CI builds) but are
left as warnings in local development. To verify your code is warning-free, you
can run `dotnet build --configuration Release` to get the release build behavior.

### Generating / Publishing console application binary file

To generate the console application binary, go to the APRSsharp folder
(AprsSharp\src\APRSsharp) and run the command `dotnet publish` to generate
the console application and it will be stored in the
bin/netcoreapp2.2/Debug/publish folder or run `dotnet publish -o <outputfolder>`
to store your binary in a given output folder
