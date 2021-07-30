# C# Server-Client Example

Example implementation for a server-client application in C#.
Provides classes (and examples how to use them) to establish an encrypted connection between a server an various clients.
Server and client can send messages between each other.
The implementation is muli-threaded and uses events.

The encryption is a 256-bit AES.
The key exchange happens via Elliptic Curve Diffie-Hellman.
For both, the pre-implemented .NET classes are used.

![Client Window](client.png "Client Window")

![Server Window](server.png "Server Window")
