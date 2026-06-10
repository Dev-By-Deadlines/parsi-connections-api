# Kalamboot API

Backend for [Kalamboot](https://kalamboot.ir) — a Persian-language Connections puzzle game. Players group 16 words into 4 categories of 4, with 4 lives to spare.

> **Live game:** [kalamboot.ir](https://kalamboot.ir)  
> **Frontend repo:** [Dev-By-Deadlines/parsi-connections](https://github.com/Dev-By-Deadlines/parsi-connections)

## Tech Stack

- **ASP.NET Core 10** — minimal APIs
- **Entity Framework Core** — SQLite
- **FluentValidation** — request validation
- **Serilog** — structured logging with daily rolling files

## Project Structure

```
Connections.Api/
├── Endpoints/
│   └── PuzzleHandlers/   # One file per endpoint
├── Services/             # PuzzleService, GameStateService, GuessService
├── Models/               # EF Core entities
├── Dtos/                 # Request/response shapes
├── Mapping/              # Entity → DTO mapping + shuffle logic
├── Validators/           # FluentValidation rules
├── Filters/              # API key auth filter
├── Data/                 # DbContext
└── Utils/                # Constants
```

## Getting Started

### Prerequisites

- .NET 10 SDK

### Run locally

```bash
git clone https://github.com/Dev-By-Deadlines/parsi-connections-api.git
cd parsi-connections-api
```

Create `appsettings.Development.json`:
```json
{
  "ApiKeys": {
    "AdminKey": "your-key-here"
  },
  "ConnectionStrings": {
    "Connections": "Data Source=connections.db"
  }
}
```

```bash
dotnet run
```

- API: `http://localhost:5001`
- Swagger docs: `http://localhost:5001/docs`

## API

### Player Endpoints

| Method | Route | Description |
|--------|-------|-------------|
| `GET` | `/puzzles/daily` | Get today's puzzle and current game state |
| `POST` | `/puzzles/{id}/guess` | Submit a guess |
| `GET` | `/puzzles/stats` | Get stats for the current player after game ends |

### Admin Endpoints

Require `X-Api-Key` header.

| Method | Route | Description |
|--------|-------|-------------|
| `GET` | `/puzzles` | List all puzzles (paginated) |
| `GET` | `/puzzles/{id}` | Get a single puzzle |
| `POST` | `/puzzles` | Create a puzzle |
| `PUT` | `/puzzles/{id}` | Update a puzzle |
| `DELETE` | `/puzzles/{id}` | Delete a puzzle |

### Sample Responses

<details>
<summary>GET /puzzles/daily</summary>

```json
{
  "puzzleId": 1,
  "outcome": "Playing",
  "remainingHealth": 4,
  "unSolvedWords": [
    "هواپیما",
    "اخبار",
    "نمودار",
    "داده",
    "انبردست",
    "مزاحم نشوید",
    "شیر",
    "مار",
    "بی صدا",
    "اره",
    "لرزش",
    "دوچرخه",
    "بنزین",
    "سطل",
    "شمشیر",
    "آمار"
  ],
  "solvedCategoryDtos": []
}
```
</details>

<details>
<summary>POST /puzzles/{id}/guess</summary>

Request:
```json
{
  "words": ["هواپیما", "لرزش", "بی صدا", "مزاحم نشوید"]
}
```

Response:
```json
{
  "correct": true,
  "oneAway": false,
  "gameStateDto": {
    "puzzleId": 1,
    "outcome": "Playing",
    "remainingHealth": 4,
    "unSolvedWords": ["اخبار", "نمودار", "داده", "انبردست", "شیر", "مار", "اره", "دوچرخه", "بنزین", "سطل", "شمشیر", "آمار"],
    "solvedCategoryDtos": [
      {
        "name": "حالت های موبایل",
        "words": [
          { "text": "لرزش" },
          { "text": "بی صدا" },
          { "text": "هواپیما" },
          { "text": "مزاحم نشوید" }
        ]
      }
    ]
  }
}
```
</details>

<details>
<summary>GET /puzzles/stats</summary>

```json
{
  "totalPlayers": 142,
  "winRate": 67.4,
  "averageRemainingHealth": 2.3,
  "playerPercentile": 85.0,
  "playerHealth": 3,
  "playerOutcome": "Won"
}
```
</details>

### Outcome values

| Value | Meaning |
|-------|---------|
| `Playing` | Game in progress |
| `Won` | All 4 categories solved |
| `Lost` | Health reached 0 — all categories revealed |

## Game Rules

- 16 words, 4 categories, 4 words per category
- 4 lives — each wrong guess costs one
- Words are shuffled once per session and stay in that order
- On win or loss, all categories are revealed
- Session is tracked via an `HttpOnly` cookie — no login required

---

Made by [Dev By Deadlines](https://github.com/Dev-By-Deadlines)

