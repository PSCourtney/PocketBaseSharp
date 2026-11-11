# PocketBaseSharp Changelog

## v1.2.0

Targets **.NET 11** and **PocketBase v0.42.0**.

### Added

- `client.Sql.RunAsync(sql)` / `Run(sql)` - executes a raw SQL query via `POST /api/sql` (PocketBase v0.39.0+, superusers only). Returns `SqlResultModel` (`Columns`, `Rows`, `AffectedRows`, `ExecTime`).
- `client.Log.TruncateAsync()` / `Truncate()` - deletes all logs via `DELETE /api/logs` without changing the retention setting (PocketBase v0.40.0+, superusers only).
- `client.Collections.GetAllOAuth2ProvidersAsync()` / `GetAllOAuth2Providers()` - lists all configurable OAuth2 providers with inline SVG logos via `GET /api/collections/meta/oauth2-providers` (PocketBase v0.37.0+, superusers only).
- `client.Collections.DryRunViewQueryAsync(query)` / `DryRunViewQuery(query)` - previews a view collection query via `POST /api/collections/meta/dry-run-view` (PocketBase v0.37.0+, superusers only). Returns `DryRunViewResultModel` (`Fields`, `Sample`).

### Removed

- ⚠️ **Breaking:** removed the legacy request log methods from `LogService`:
    - `GetRequestAsync(id)` / `GetRequest(id)`
    - `GetRequestsAsync(...)` / `GetRequests(...)`
    - `GetRequestsStatisticsAsync(...)` / `GetRequestsStatistics(...)`

    They called the `api/logs/requests` route, which was removed in PocketBase v0.23.0, so they always failed with `404` against any supported PocketBase version.
- Removed the `LogRequestModel` and `LogRequestStatistic` models, which were only used by the methods above.

### Changed

- Updated target framework from `net10.0` to `net11.0` (SDK, tests and Blazor demo) and the demo's Microsoft packages to 11.0.0.
- Bundled PocketBase binary updated from v0.35.1 to v0.42.0.
- Since PocketBase v0.42.0, requests with an unknown or expired `Authorization` token are rejected with `401` on every endpoint. Clear the `AuthStore` and re-authenticate when you receive one.
- Fixed README examples to use `AuthenticateWithPasswordAsync` (`AuthWithPasswordAsync` never existed).


## v1.1.0

- Added backup management features (create, download, restore, delete backups).
- Compatible with PocketBase v0.35.1 and .NET 10.
