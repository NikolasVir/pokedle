# Getting Started

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/)
- [Node.js](https://nodejs.org/) (LTS)
- [PostgreSQL](https://www.postgresql.org/) (default: `localhost:5432`, db
  `pokedle`, user/pass `pokedle`)

## Setup

```bash
# Create appsettings.Development.json with your PG connection and JWT key

# Apply migrations
dotnet ef database update --project Pokedle.Api

# Seed Pokémon data
dotnet run --project Pokedle.Api -- --seed
```

## Run

```bash
# API (http://localhost:5065)
dotnet run --project Pokedle.Api

# UI dev server
npm --prefix Pokedle.Ui start
```

## Test

```bash
dotnet test
npm --prefix Pokedle.Ui test
```
