# VIRCE server side application
This is a repository for the VIRCE server side application.

## Tech Stack
- .NET 7.0
- UniTask
- MasterMemory
- Docker
- kubernetes

## Installation
### Requirements
- .NET Runtime 7.0

### Steps
1. Clone the repository
2. Run `dotnet run` in the VIRCE_server directory

### Run with kubernetes
You have to install kubernetes , if you want to run with kubernetes.
1. Clone the repository
2. Run `docker build -t virce_server:kubernetes .`
3. Run `kubectl create namespace virce`
4. Run `kubectl apply -f kubernetes/deployment.yaml --namespace virce`

## architecture
The server side application is a .NET 7.0 application.
It uses the .NET Core framework and is written in C#.

This application consists of a matching server and a room servers for mini game and lobby.

And this app has In Memory Database Server featured by MasterMemoryDatabase(CySharp).
The servers are good perfomance by this Database Server.

### Room servers
The room servers are responsible for the game logic.
They are responsible for the communication between the clients and the game logic.

### Matching server
The matching server is responsible for matching players together.
It is responsible for the communication between the clients and the matching logic.
This server is accessed by the client when the client starts.

## LICENSE
This project is licensed under the GNU Lesser General Public License v3.0 - see the [LICENSE](LICENSE) file for details.
