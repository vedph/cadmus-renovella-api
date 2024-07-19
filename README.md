# Cadmus Re.Novella API

🐋 Quick Docker image build:

```bash
docker build . -t vedph2020/cadmus-renovella-api:5.0.1 -t vedph2020/cadmus-renovella-api:latest
```

(replace with the current version).

This is a Cadmus API layer customized for the PRJ project. Most of its code is derived from shared Cadmus libraries. See the [documentation](https://github.com/vedph/cadmus_doc/blob/master/guide/api.md) for more.

## History

- 2024-07-19: updated packages.
- 2024-06-24: updated packages.
- 2024-06-10: updated packages.
- 2024-05-24: updated packages.

### 5.0.1

- 2024-04-14: updated packages.

### 5.0.0

- 2024-04-08:
  - upgraded to .NET 8.
  - refactored logging.

### 4.0.5

- 2023-10-24: updated packages and added environment variable for file-based log (`SERILOG_LOGPATH`). See [docker-compose.yml](docker-compose.yml) for an example with volume based bind mounts.
- 2023-10-15: updated packages.

### 4.0.4

- 2023-09-29:
  - updated packages and added thesaurus importer.
  - added erotic to `tale-genres` thesaurus.

### 4.0.3

- 2023-07-26: updated packages.

### 4.0.2

- 2023-07-19: refactored [logging](https://myrmex.github.io/overview/cadmus/dev/history/b-logging).

### 4.0.1

- 2023-07-19: updated packages.

### 4.0.0

- 2023-06-23: [moved to PostgreSQL](https://myrmex.github.io/overview/cadmus/dev/history/b-rdbms/).
- 2023-05-19: updated packages and startup.

### 3.0.0

- 2023-03-15:
  - migrated to [new backend configuration](https://myrmex.github.io/overview/cadmus/dev/history/b-config).
  - fix to preview config for note.
- 2022-12-23: updated packages.
- 2022-11-10: upgraded to NET 7.
- 2022-10-11:
  - updated packages and injection in `Startup.cs` for new `IRepositoryProvider`.
  - added preview.

### 2.2.1

- 2022-05-13: added available witnesses part and removed manuscript item. This implies these changes in the existing MongoDB:
  - replace `facets` with those from `seed-profile.json`.
  - replace `thesauri/model-types@en` with that from the same file.

### 2.2.0

- 2022-05-01: upgraded core to NET6.0.
- 2022-04-21: updated packages.
- 2022-02-18: updated packages.

Update procedure for server data which still preserve the old facets:

(1) replace all the facets (`collection` and `tale`) from this repository `seed-profile.json`.

(2) replace these thesauri with the new versions from this repository `seed-profile.json`:

  - `model-types@en`

### 2.1.2

- 2022-02-16: fixed missing package upgrades.

### 2.1.1

- 2022-02-14: fixed thesaurus ID in profile.

### 2.1.0

- 2022-02-08: updated packages.

Update procedure for server:

(1) in MongoDB `cadmus-renovella-auth` execute:

```js
db.Users.updateMany({}, { $unset: {"AuthenticatorKey": 1, "RecoveryCodes": 1} });
```

(2) in MongoDB `cadmus-renovella` execute:

```js
// (1) TaleInfoPart for author:
// get records before update
db.parts.find({
    "typeId" : "it.vedph.renovella.tale-info",
    "content.author.name.parts" : {
        $exists : true
    }
});

// update renaming 'parts' into 'pieces'
db.parts.updateMany(
{
"typeId" : "it.vedph.renovella.tale-info",
"content.author.name.parts" : { $exists : true }
},
{
$rename: {
"content.author.name.parts": "content.author.name.pieces"
}
}, false, true);

// (2) TaleInfoPart for dedicatee:
// get records before update
db.parts.find({
    "typeId" : "it.vedph.renovella.tale-info",
    "content.dedicatee.name.parts" : {
        $exists : true
    }
});

// update renaming 'parts' into 'pieces'
db.parts.updateMany(
{
"typeId" : "it.vedph.renovella.tale-info",
"content.dedicatee.name.parts" : { $exists : true }
},
{
$rename: {
"content.dedicatee.name.parts": "content.dedicatee.name.pieces"
}
}, false, true);

// (3) TaleInfoPart for references:
// get all the author's sources
db.parts.find({
    "typeId" : "it.vedph.renovella.tale-info",
    "content.author.sources" : {
        $exists : true, $ne: []
    }
});
// manually adjust, only 1 case

// (4) TaleInfoPart for ID references:
// get all the author's IDs sources
db.parts.find({
    "typeId" : "it.vedph.renovella.tale-info",
    "content.author.ids.sources" : {
        $exists : true, $ne: []
    }
});
// manually adjust, only 6 cases
```

List of cases:

(3.1) Le novelle del Bianco Alfani e di Madonna Lisetta Levaldini: author.sources:

- Ed riferimento: Rossella Bessi
- Un dittico quattrocentesco
- Interpretes XIV 1994
- Bessi identifica il narratore della cornice, ovvero l'anonimo mittente dell'epistola comitatoria, con l'autore delle novelle. Piero di Filippo del Nero. Si noti per� che lo stesso, con l'epiteto di Piero Viniziano, vi compare poi come personaggio distinto da chi narra.

(4.1) I diporti: author.ids[0].sources:

- DBI
- Daniele Ghirlanda - Luigi Collarile
- PARABOSCO, Girolamo
- DBI vol 81 (2014)
- https://www.treccani.it/enciclopedia/girolamo-parabosco_(Dizionario-Biografico)/

(4.2) Le novelle del Bianco Alfani e di Madonna Lisetta Levaldini: author.ids[0].sources:

- DBI
- Liana Cellerino
- Del Nero, Piero
- DBI vol 38 (1990)

(4.3) Novella di madonna Lisetta Levaldini: author.ids[0].sources:

- DBI
- Liana Cellerino
- Del Nero, Piero
- DBI vol 38 (1990)

(4.4) Novella del Bianco Alfani: author.ids[0].sources:

- DBI
- Liana Cellerino
- Del Nero, Piero
- https://www.treccani.it/enciclopedia/piero-del-nero_res-40f2fcf6-87ec-11dc-8e9d-0016357eee51_(Dizionario-Biografico)/

(4.5) Il Novellino: author.ids[0].sources:

- DBI
- Fabio De Proprio
- Guardati, Tommaso
- DBI vol 60 (2003)
- https://www.treccani.it/enciclopedia/tommaso-guardati_%28Dizionario-Biografico%29/

(4.6) Novella di Giulietta e Romeo: author.ids[0].sources:

- DBI
- Giorgio Patrizi
- Da Porto, Luigi
- DBI vol 32 (1986)
- https://www.treccani.it/enciclopedia/luigi-da-porto_%28Dizionario-Biografico%29/

### 2.0.0

- 2021-12-19: adding new part.
- 2021-11-11: upgraded to NET 6.
- 2021-10-17: refactored `DocReference` and `PersonName` model now depending on bricks, while moving `CitedPerson` from Itinera as a submodel of this project. For breaking changes in the database see <https://github.com/vedph/cadmus-renovella>.
- 2021-10-17: breaking change for auth database by AspNetCore.Identity.Mongo 8.3.1 (used since Cadmus.Api.Controllers 1.3.0, Cadmus.Api.Services 1.2.0):

```js
/*
Removed fields:
AuthenticatorKey = null
RecoveryCodes = []
*/
db.Users.updateMany({}, { $unset: {"AuthenticatorKey": 1, "RecoveryCodes": 1} });
```
