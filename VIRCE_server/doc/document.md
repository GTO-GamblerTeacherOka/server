# About

This is a document for VIRCE server.

# Role Of Each Object

## Client

- Client is a player who wants to play the game.
- Client sends request to Match Making Server to join the game.

## Match Making Server

- Match Making Server is a server that receives match making request from client and throw it to Game Server Cluster.

## Game Server Cluster

- Game Server Cluster is a server that receives request from Match Making Server and throw it to Game Server.
- Game Server Cluster also checks the number of occupants of each Game Server and return the Game Server with least
  occupants.
- Game Server Cluster is also responsible for load balancing.

## Game Server

- Game Server is a server for relaying the data between clients.
- Game Server is also responsible for notifying other clients when a new player joined or a player exited.

## Database

- Database is a server that stores the information of each player.
- Database is also responsible for storing the information of each Game Server.

# Sequence Diagram

## Match Making

```mermaid
sequenceDiagram
    autonumber
    actor Client
    participant MMS as Match Making Server
    participant DB
    participant GSC as Game Server Cluster
    participant GS as Game Server
    actor OC as Other Client
    Client->>MMS: Match Making Request with VRoid license
    MMS->>GSC: Request to Join
    GSC->>DB: Check Occupants of each Game Server
    DB->>GSC: Return Game Server with least Occupants
    GSC->>GS: Request to Join
    GS->>DB: Add Player info
    GS->>Client: Return Join Success
    GS->>OC: Notify New Player Joined
```

## On Frame

```mermaid
sequenceDiagram
    autonumber
    actor Client
    participant GS as Game Server
    participant DB
    actor OC as Other Client
    GS->>DB: Get End Points of all Players
    DB->>GS: Return End Points of all Players
    loop
        Client->>GS: Send Player Point
        GS->>OC: Send Player Point
        alt New Player Joined
            GS->>DB: Get End Points of all Players
            DB->>GS: Return End Points of all Players
        end
    end
```

## On Exit

```mermaid
sequenceDiagram
    autonumber
    actor Client
    participant GS as Game Server
    participant DB
    actor OC as Other Client
    Client->>GS: Send Exit Request
    GS->>DB: Remove Player info
    GS->>OC: Notify Player Exited
```
