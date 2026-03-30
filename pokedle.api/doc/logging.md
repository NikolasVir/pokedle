## What and where to log

- **Seeding logs**
  - **What:** Seeding start/end, counts, failures, external API errors
  - **Where:** JSON file sink (`logs/seeding-.json`, rolling daily) [codewithmukesh](https://codewithmukesh.com/blog/structured-logging-with-serilog-in-aspnet-core/)

- **HTTP request logs**
  - **What:** Each incoming HTTP request
  - **Where:** JSON file sink (`logs/requests-.json`, rolling daily) [github](https://github.com/serilog/serilog/wiki/Getting-Started)

- **Guess/audit logs (DB)**
  - **What:** Each guess (timestamp, anonymous client id from cookie, daily Pokémon id, guessed Pokémon id, full result)
  - **Where:** `GuessHistory` table in PostgreSQL

---

## Seeding logfile JSON schema (`logs/seeding-.json`)

Each log entry:

- `timestamp`: ISO 8601 string
- `level`: `"Information" | "Error" | "Warning"`
- `action`: event type (`SeedingInvocation`, `SeedingPokemon`, `SeedingEnd`, …)
- `success`: boolean
- `metadata`: extra context (pokemon name, counts, error message, etc.)

(Examples stay as previously defined.)

---

## HTTP request logfile JSON schema (`logs/requests-.json`)

Each log entry:

- `timestamp`: ISO 8601 string
- `method`: `"GET" | "POST" | "PUT" | "DELETE" | "PATCH"`
- `endpoint`: request path (e.g. `"/graphql"`)
- `body`: raw request body (for GraphQL: query/mutation + variables)

---

## GuessHistory table (cookie-based client id)

The game will **not** use user accounts; instead it uses a persistent cookie id.

- `Id` (PK, bigserial)
- `ClientId` (text, not null)
  - Anonymous identifier read from a browser cookie, e.g. `pokedleClientId`
- `Time` (timestamptz, default `now()`)
- `DailyPokemonId` (int, FK to `Pokemons.Id`)
- `GuessedPokemonId` (int, FK to `Pokemons.Id`)
- `Results` (jsonb, full `GuessResult` payload)

Postgres shape:

```sql
create table guess_history (
    id                  bigserial primary key,
    client_id           text not null,
    time                timestamptz not null default now(),
    daily_pokemon_id    int not null references pokemons(id),
    guessed_pokemon_id  int not null references pokemons(id),
    results             jsonb not null
);
```
