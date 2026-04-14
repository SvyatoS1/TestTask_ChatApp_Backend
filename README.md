# TestTask_ChatApp_Backend
ASP.NET Core 10 backend for the real-time chat application. Handles WebSocket connections via Azure SignalR, persists messages to Azure SQL Database, and runs sentiment analysis using Azure AI Language.

---
 
## Tech stack
 
| Layer | Technology |
|---|---|
| Framework | ASP.NET Core Web API |
| Real-time | Azure SignalR Service |
| Database | Azure SQL + Entity Framework Core |
| Sentiment | Azure AI Language (Text Analytics) |
| Deployment | Azure App Service |
 
---

## Getting started locally
 
### Prerequisites
 
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) or [Azure SQL](https://azure.microsoft.com/en-us/products/azure-sql/database)
- Azure SignalR Service instance
- Azure AI Language resource (for sentiment analysis)
 
### 1. Clone the repo
 
```bash
git clone https://github.com/your-username/chat-app.git
cd chat-app/ChatApp.Api
```
 
### 2. Configure secrets
 
Copy the example config and fill in your values:
 
```bash
cp appsettings.json appsettings.Development.json
```
 
Edit `appsettings.Development.json`

### 3. Apply database migrations
 
```bash
dotnet ef database update
```
 
### 4. Run
 
```bash
dotnet run
```
 
API will be available at `https://localhost:7099`.  
Swagger UI: `https://localhost:7099/swagger`
 
---
 
## API reference
 
### REST
 
| Method | Endpoint | Description |
|---|---|---|
| `GET` | `/api/messages?room=general` | Returns last 50 messages for a room, ordered chronologically. Used to seed chat history on page load. |
 
#### Response example
 
```json
[
  {
    "id": 1,
    "username": "Alex",
    "content": "Hello everyone!",
    "room": "general",
    "sentAt": "2024-11-01T10:24:00Z",
    "sentiment": {
      "label": "positive",
      "confidenceScore": 0.97
    }
  }
]
```
 
### SignalR hub — `/hubs/chat`
 
| Method (client → server) | Parameters | Description |
|---|---|---|
| `JoinRoom` | `room: string` | Adds the caller to a SignalR group for that room |
| `LeaveRoom` | `room: string` | Removes the caller from the group |
| `SendMessage` | `username, content, room` | Broadcasts the message, persists to DB, triggers async sentiment analysis |
 
| Event (server → client) | Payload | Description |
|---|---|---|
| `ReceiveMessage` | `MessageDto` | Fired immediately when a message is sent (sentiment is `null` at this point) |
| `SentimentUpdate` | `SentimentUpdateDto` | Fired ~1–2 s later when Azure AI Language returns the result |
 
---

## Available rooms
 
Rooms are SignalR groups — no database table required. The default rooms are `general`, `random`, and `dev`. Any string is a valid room name.
 
---
