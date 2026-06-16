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
| `GET` | `/puzzles/{id}/stats` | Get stats for a specific puzzle after game ends |
| `GET` | `/puzzles/archive` | Get a paginated list of all past daily puzzles |
| `GET` | `/puzzles/{id}/play` | Get game state for a specific archived puzzle |

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
<summary>GET /puzzles/{id}/stats</summary>

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

<details>
<summary>GET /puzzles/archive</summary>

```json
{
  "items": [
    {
      "puzzleId": 7,
      "remainingHealth": 2,
      "solvedCategories": 4,
      "outcome": "Won",
      "lastUsedInDaily": "2026-06-16"
    },
    {
      "puzzleId": 6,
      "remainingHealth": 0,
      "solvedCategories": 0,
      "outcome": "Lost",
      "lastUsedInDaily": "2026-06-15"
    },
    {
      "puzzleId": 5,
      "remainingHealth": null,
      "solvedCategories": 0,
      "outcome": null,
      "lastUsedInDaily": "2026-06-14"
    }
  ],
  "page": 1,
  "limit": 10,
  "total": 7,
  "totalPages": 1
}
```
</details>

### Outcome values

| Value | Meaning |
|-------|---------|
| `Playing` | Game in progress |
| `Won` | All 4 categories solved |
| `Lost` | Health reached 0 — all categories revealed |
| `null` | Player hasn't played this puzzle yet (archive only) |

## Game Rules

- 16 words, 4 categories, 4 words per category
- 4 lives — each wrong guess costs one
- Words are shuffled once per session and stay in that order
- On win or loss, all categories are revealed
- Session is tracked via an `HttpOnly` cookie — no login required
- Past daily puzzles are playable via the archive

---

Made by [Dev By Deadlines](https://github.com/Dev-By-Deadlines)
