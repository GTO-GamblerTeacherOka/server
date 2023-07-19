# VIRCE server side application
This is a repository for the VIRCE server side application.

## Installation
### Requirements
- .NET Runtime 7.0

### Steps
1. Clone the repository
2. Run `dotnet run` in the VIRCE_server directory

## architecture
The server side application is a .NET 7.0 application.
It uses the .NET Core framework and is written in C#.

This application consists of a proxy server, room servers for mini game and lobby and a matching server.

### Proxy server
The proxy server is the entry point of the application.
It is responsible for routing the requests to the correct room server.

### Room servers
The room servers are responsible for the game logic.
They are responsible for the communication between the clients and the game logic.

### Matching server
The matching server is responsible for matching players together.
It is responsible for the communication between the clients and the matching logic.

## LICENSE
This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details