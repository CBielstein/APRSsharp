# AprsSharp.AprsIsClient

`AprsSharp.AprsIsClient` is part of the [AprsSharp Project](https://github.com/CBielstein/APRSsharp).
`AprsIsClient` aims to be a simple library for interacting with the Automatic Packet Reporting System-Internet Service.

APRS-IS uses TCP messages and specific encodings to communicate amateur radio packets.
This library abstracts those aspects away.

Currently, only receiving packets is implemented.
The only outgoing communication to the server is that required for authorization and configuration.
Packets cannot be sent via APRS-IS at this time.

## Getting started

This package can be installed via [nuget.org](https://www.nuget.org):
[AprsSharp.AprsIsClient on nuget.org](https://www.nuget.org/packages/AprsSharp.AprsIsClient).

A simple meethod is via the command line:

```bash
dotnet add package AprsSharp.AprsIsClient
```

## Usage

AprsIsClient uses an event driven framework.
Major steps are to create a client, define callbacks, and then begin receiving.
See the code snippet below for examples.

```csharp
using AprsSharp.AprsIsClient;

// Create a client
using AprsIsClient aprsIs = new AprsIsClient();

// Print the sender and type of packet received.
aprsIs.ReceivedPacket += packet => {
    Console.WriteLine(packet.Sender);
    Console.WriteLine(packet.InfoField.Type);
};

// Optionally, you can print when the server connection changes state.
aprsIs.ChangedState += state => Console.WriteLine($"New state: {state} on server {aprsIs.ConnectedServer}");

// If you'd rather read the TCP messages directly, you can subscribe to those events
aprsIs.ReceivedTcpMessage += message => Console.WriteLine($"New TCP message: {message}");

// And if you'd like to see error messages, you can subscribe to those
aprsIs.DecodeFailed += (ex, message) => Console.WriteLine($"Failed to decode message {message} with exception {ex}"); 

// Once you've configured your events, begin receiving!
// Assuming `callsign` and `password` are variables, this connects to
// a server on the main rotation and requests all packets of type == message (t/m)
// You can read more about filter commands here: https://www.aprs-is.net/javAPRSFilter.aspx
//
// The `await` here is optional and may not be desirable if you're expecting additional input.
// If you're writing a simple script, the await is helpful to ensure your application doesn't terminate.
// The await will not return until AprsIsClient is instructed to stop receiving.
await aprsIs.Receive(callsign, password, "rotate.aprs2.net", "t/m");
```

## Additional documentation

For more information, you can visit:

* [AprsSharp on GitHub](https://github.com/CBielstein/APRSsharp)
* APRS-IS documentation on [aprs-is.net](https://www.aprs-is.net/)
* APRS documentation on [aprs.org](https://www.aprs.org/)

## Feedback

Bugs, feedback, and feature requests can be filed via [GitHub Issues on the AprsSharp repository](https://github.com/CBielstein/APRSsharp/issues).
