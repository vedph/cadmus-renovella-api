# Cadmus Re.Novella API

Quick Docker image build:

```bash
docker build . -t vedph2020/cadmus_renovella_api:2.0.0 -t vedph2020/cadmus_renovella_api:latest
```

(replace with the current version).

This is a Cadmus API layer customized for the PRJ project. Most of its code is derived from shared Cadmus libraries. See the [documentation](https://github.com/vedph/cadmus_doc/blob/master/guide/api.md) for more.

## History

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
