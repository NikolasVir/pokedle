:::mermaid
erDiagram
POKEMON }o--o{ POKEMON_TYPE : "has"
TYPE ||--o{ POKEMON_TYPE : "used by"
POKEMON }o--|| HABITAT : "lives in"
POKEMON }o--|| COLOR : "has color"

    POKEMON {
        int    Id             PK
        string Name
        int    Generation
        int    EvolutionStage
        int    HabitatId      FK
        int    ColorId        FK
    }

    POKEMON_TYPE {
        int PokemonId  PK, FK
        int TypeId     PK, FK
        int Slot
    }

    TYPE {
        int    Id   PK
        string Name
    }

    HABITAT {
        int    Id   PK
        string Name
    }

    COLOR {
        int    Id   PK
        string Name
    }

:::
