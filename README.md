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

### Further Documentation

See supplemental documentation for APRS# constituent projects:

* [AprsParser](src/AprsParser/AprsParser.md)
* [KissTnc](src/KissTnc/KissTnc.md)

## Contributions

Please see the [code of conduct](CODE_OF_CONDUCT.md) and
[contributing document](CONTRIBUTING.MD) for details on contributing to
this project.

## Development

### Dependencies

The target is .NET 6.0 for development.

### Building

Warnings are treated as errors in the release build (as in CI builds) but are
left as warnings in local development. To verify your code is warning-free, you
can run `dotnet build -c Release` to get the release build behavior.

### Generating / Publishing console application binary file

To generate the console application binary, go to the APRSsharp folder
(`AprsSharp\src\APRSsharp`) and run the command `dotnet publish -c Release` to generate
the console application.
The resulting binary will be placed in
`bin\Release\net8.0\win-x86\publish`.
Alternatively, use `dotnet publish -c Release -o <outputfolder>` to specify the
output directory.

### Nuget Packages and Release

All packages are versioned together for now via a field in
`Directory.Build.props`. This means if one package gets a new version, they all
do. While this is not exactly ideal, this is currently used to manage
inter-dependencies during early development.

Maintainers should ensure the version has been updated before creating a new
GitHub release. Creating a release will publish nuget packages to Nuget.org
list.

### Running the project/application binary file

To run the generated console application binary, go to the APRSsharp folder
(`src\AprsSharp\src\APRSsharp`).

Run the command `dotnet run` which will run AprsSharp with default parameters.
You can run the console app with command line arguments.
Examples of flags used with the arguments.

Callsign argument as {`dotnet run -- --callsign` with --callsign/-c/--cgn}.
Password as {`dotnet run -- --password` with --password/-p/--pwd/--pass}.
Server name as {`dotnet run -- --server` with --server or -s or --svr}.
Filter arguments as {`dotnet run -- --filter` and --filter or -f}.

For example `dotnet run -- -c "arg" --pwd "args" --server "args" -f "args"`.

You can specify different combinations of the commandline flags.
Either use 0, 1, 2, 3 or all the flags combination of flags.

For missing flags, the console application will use the default arguments.

To gracefully terminate the application, press the command `Q` or `q`.
This will exit the application after the last packet is received.

To force terminate the application, press the command `Ctrl + c`
