## v0.42.0

- Allow previewing JPEG XL images (`.jxl`) in the superuser UI.
    _Note that there is no thumbs support for JPEG XL yet._

- Allow reordering the configured OAuth2 providers in the UI _(mostly for the `listAuthMethods()` response)_.

- Added helper `app.CountLogs()` query method.

- Auto run `VACUUM` for the `auxiliary.db` after deleting more than 20k logs [#7854](https://ganigeorgiev.com/pocketbase/discussions/7854#comment-8lea6s7xy8x4abs).
    _It runs as part of the `__pbLogsCleanup__` cron job and it is an extra optimization to more eagerly free up unused disk space after large logs rotation._

- Fixed logs chart y-axis overlap when numbers with more than 5 digits are shown.

- Replaced `fmt.Sprintf("%x")` with the slightly more performant `encoding/hex.EncodeToString()`.

- Added guard for a `cron.Stop()` race in case it happens to be called right before the ticker initialization _(unlikely to trigger in real usage)_.

- Allowed the `update/Config.Channel` field to be optional so that it can be configured to fetch the latest release from any of the repo release channels.

- Added a global middleware to reject with 401 unknown or expired `Auhorization` tokens so that client-side apps can more easily handle auth state changes _(e.g. invalidating session screens after the user has changed password on another device)_. As a side-effect this also enables practically any endpoint, not just `authRefresh()`, to be used as a very rudimentary auth token verification.
    _⚠️ For most applications this should be a none breaking change but note that previously such `Authorization` header values were silently ignored to allow custom token handling because pre v0.23 middlewares didn't have a way to control the execution priority. Now this global middleware acts as a catch-all fallback and applies as latest as possible and only if `e.Auth` is not already loaded._

- Log harmless mid-response network errors with level `INFO (0)` ([#8361](https://ganigeorgiev.com/pocketbase/discussions/8361)).

- Bumped `modernc.org/sqlite` to 1.62.0 _([SQLite 3.54.1](https://sqlite.org/src/timeline?from=version-3.54.0&to=version-3.54.1&y=ci) and other minor fixes)_.

- Bumped `golang.org/x/*` dependencies and use Go 1.27.4 to generate the prebuilt binaries since it has some [minor security fixes](https://groups.google.com/g/golang-announce/c/iY8TY0wWgbM) _(none of them are critical for PocketBase but nevertheless it is advised to update)_.


## v0.41.1

- Added missing license info to the vendored fonts ([#8352](https://ganigeorgiev.com/pocketbase/discussions/8352)).

- Bumped `golang.org/x/net` since it has some [minor security fixes](https://groups.google.com/g/golang-announce/c/sqrs2vTx_Xs).

- Other minor fixes (readme broken link fix, updated npm dev deps to silence security reports, etc.).


## v0.41.0

- ⚠️ Migrated the Go module from GitHub to the new host `https://ganigeorgiev.com`.
    To migrate your existing Go projects:
    - replace all occurances of `https://ganigeorgiev.com/pocketbase/` with `https://ganigeorgiev.com/pocketbase/` in your `.go` source files
    - replace the pocketbase `require` statement in your `go.mod` with `ganigeorgiev.com/pocketbase v0.41.0`
    - run `go mod tidy`

    If you are maintaining a Docker image that relies on a predictable download release URL, you can use:
    `https://ganigeorgiev.com/meta/pocketbase/pocketbase/download/{releaseTag}/{filename}` _(it redirects to the actual API file URL)_.

- Added `Extra` map field to all hook events to allow sharing arbitrary data between non-record hook handlers.

- Added `ServeEvent.StartBannerFunc` field to customize the default startup console message.

- Replaced the tree-like console characters with simple `>` in the startup console message since a user reported poor font rending.

- Changed the default value of the recent `PB_THUMBS_MAX_SOURCE_PIXELS` limit to `100 << 20` since the old `250 << 20` was unnecessery too high _(there will be a warning log if it is ever exceeded and we can further adjust based on the reported use cases)_.

- Minor mailer fixes for [#7871](https://github.com/pocketbase/pocketbase/issues/7871):
    - (SMTP) skip `Message.HTML` multipart assignment if empty
    - (`sendmail`) set `Content-Type:text/plain` instead of `text/html` if the `Message.HTML` field is empty _(`multipart/mixed` and attachments support for the sendmailer will be further evaluated in the future)_
    - (`sendmail`) set a random `Message-ID` and load the custom user provided mail headers for consistency with the SMTP implementation.

- Minor UI fixes _(removed extra preview popup horizontal scroll for very small images, replaced github.com urls with the new host, etc.)_.


## v0.40.6

- Replaced the GitHub update command with update from the new host in relation to [#7841](https://ganigeorgiev.com/pocketbase/discussions/7841).
    Note that this will be most likely the last release here on GitHub. The next time when you run update, new releases will be fetched from `https://ganigeorgiev.com` _(there is nothing there yet)_.

- Additional fileblob bucket escape checks for just in case if the filesystem abstraction is used as standalone without the PocketBase validations.

- Added a global restriction for the max `WxH` pixels size when generating a thumb to reduce the risk of excessive memory usage.
    The issue was initially privately reported by [@reload3d](https://github.com/reload3d) but it is treated as regular public issue since it is a [known Go caveat](https://github.com/golang/go/issues/27830).
    The default limit is pretty generous and currently set to max `250<<20 (262144000)` pixels and can be changed with the env `PB_THUMBS_MAX_SOURCE_PIXELS`.
    **The workaround is not bulletproof and thumbs generation remains one of the main culprit for potential OOMs in PocketBase**, so better solutions will have to be investigated in the future, including eventually switching to [libvips](https://github.com/libvips/libvips) or other similar library once there is [better CGO portability](https://github.com/golang/go/issues/81450) _(if you have better suggestions, please open a discussion)_.

- Bumped the min Go GitHub action version to 1.27.3 as it includes a [security fix](https://groups.google.com/g/golang-announce/c/66pojueaQek) related to the recent eMudhra CA compromise.
    _I didn't rush a fix 2 days ago as the impact for most PocketBase servers is not that serious and requires to already have a MiTM, but nonetheless it is recommended to update. Please make sure to also check for OS/Docker image updates to make sure that you have the latest `ca-certificates` bundle._

- Bumped `golang.org/x/` dependencies ([#7865](https://ganigeorgiev.com/pocketbase/discussions/7865)).

- Bumped goja and its related dependencies _(string allocations optimization, `Date.getYear/setYear` support and other minor correctness fixes)_.

- Minor UI fixes - superuser IPs description grammar fix, rotating btn loader icon alignment fix, slight dark background correction in attempt to minimize the color profile differences between Chrome and Firefox, etc.


## v0.40.5

- Minor improvements for the JSVM migration error handling:
    - Included the recovered panic stack trace of `routine.SafeWrap` in the returned error (max 2KB).
    - Wrap the individual `up`/`down` JSVM migration arguments in `routine.SafeWrap` so that in case of panic we can still print the failed js migration filename.

- Updated `modernc.org/sqlite` to 1.60.1 _(minor performance improvements for queries with large text fields and/or many params)_.

- Bumped the min Go GitHub action version to 1.27.2 as it includes several [`net/http` security fixes](https://github.com/golang/go/issues?q=milestone%3AGo1.27.2).
    _One of the fixes is for a DoS that could potentially affect PocketBase applications and it is advised to update._

- Other minor fixes _(godoc typos, normalized negative jsvm pool size, extra CORS wildcard subdomain checks to prevent misuse, support `Promise` as autocomplete suggestions return, autoexpand textarea scrollbar-gutter fix, etc.)_.


## v0.40.4

- Fixed migration deadlock if a logs db write happens to run while the migration is still executing ([#7836](https://ganigeorgiev.com/pocketbase/discussions/7836)).

- `app.ResetBootstrapState()` was soft-deprecated in favour of `app.ClearBootstrap()`.
    _Additionally a new `app.OnBootstrapClear()` hook was added to allow clearing custom allocated `OnBootstrap` resources in case the app uses a non-standard initialization (e.g. doesn't call `Start()` or intentionally skip the `OnTerminate` hook)._

- Bumped `golang.org/x/*` dependencies.


## v0.40.3

- Write the status header for JSON responses only if the fields picker succeed or has acceptable fallback.
    _This is to allow custom response status code for failed json writes._

- Fixed collection index validator to allow expressions with parenthesis in the optional `WHERE` clause.

- Clamped arccosine to [-1,1] in the Harvesine formula for the `geoDistance()` filter function to workaround edge case related to float rounding errors for some coordinates.

- Prevent unnecessary body chunk read if we already known that we are beyond the allowed limit.

- Updated the `json` field validator to check the `encoding/json/v2` semantics and allow duplicated keys on record marshalize for compliance with old jsonv1 data.

- Fixed nested cascade delete of self-referenced relation records.

- Minor UI fixes (updated dark primary btn color contrast, force reload the records list if the deleted record has self-referenced cascade relation field, etc.).

- Changed JSVM `$app` variable definition from TS type to interface ([#7834](https://ganigeorgiev.com/pocketbase/discussions/7834)).

- Bumped `golang.org/x/*` dependencies to silence security scanners ([#7829](https://ganigeorgiev.com/pocketbase/discussions/7829)).


## v0.40.2

- Return an error when filter params fallback fails to json serialize and optimized params replacement to execute in a single pass.

- Fixed collection index parsing error for indexes with missing name.

- Minor UI autocomplete optimizations _(prefix match, autocomplete debounce, etc.)_.

- Fixed linter warnings and comment typos.

- Bumped goja and its related dependencies _(regex unescaped dash error fix and base64 optimizations)_.

- Bumped the min Go GitHub action version to 1.27.1 as it includes some [minor `database/sql` and `enconding/json/v2` bug fixes](https://github.com/golang/go/issues?q=milestone%3AGo1.27.1).


## v0.40.1

- Fixes for some reported regressions related to the `encoding/json/v2` update:
    - allow mangling invalid UTF8 characters when serializing json data ([#7814](https://ganigeorgiev.com/pocketbase/discussions/7814))
    - fixed OAuth2 providers config merge incorrectly replacing the entire slice ([#7815](https://ganigeorgiev.com/pocketbase/discussions/7815))


## v0.40.0

- Propagate console command errors and recovered panics to `app.Start()` so that the program can exit with non-zero code while still ensuring that `app.OnTerminate` hook was triggered _(responsible for the app graceful shutdown handling)_.
    _⚠️ Note that this could be a slight breaking change in case you are chaining PocketBase commands and relied on the previous `0` exit status for `Command.RunE` returned errors._
    _Or in other words, if you have `./pocketbase invalid && someothercommand` and previously relied that `someothercommand` will be always executed then this is no longer the case and you'll have to adjust it or replace `&&` with `;`._

- Added quotes around the default `Content-Disposition` serving filename in case custom name with special characters is provided.

- Added `Cross-Origin-Opener-Policy:same-origin` to the default security response headers.
    _This is an extra precaution to prevent tab-nabbing in case custom UI plugins use `target="_blank"` without `rel="noopener"`._

- Added `Record.GetInt64(field)` helper (note that the serializable max safe integer of the `number` field is ~2^53-1).

- Added `Store.Keys()` method that returns a slice with all of the store keys.

- Added new `DELETE /api/logs` endpoint and UI control to delete all logs without changing the `maxDays` retention setting.

- Added new log settings option to limit the max `Log.Data` size that will be saved in the database (default to ~16KB).
    _This is an extra precaution for the cases when logging user supplied data without validating it beforehand._
    _If the resulting `Log.Data` json is above the limit, it is truncated to the last valid decoded character and an extra `"__pb_truncated__":true` log data entry will be added.`_
    _Additionally, for just in case the log message is also truncated at max 8k characters._

- Added new `filesystem` low-level helper methods:
    - `filesystem.NewWriter(key, opts)` to allow direct file create from an `io.Reader` value.
    - `filesystem.OnNewWriter()` hook to allow listening for new/to-be-created files _(it is not exposed in `core.App` instance for now to avoid introducing breaking changes)_.
    - `filesystem.OnDelete()` hook to allow listening for deleted files _(it is not exposed in `core.App` instance for now to avoid introducing breaking changes)_.

- Optimized backups to no longer transaction lock the database during backup generation ([#7799](https://ganigeorgiev.com/pocketbase/discussions/7799#discussioncomment-18108244)).

- Updated `modernc.org/sqlite` to 1.57.0 and registered by default the new `_defensive=1` DSN query parameter to enable [SQLite's defensive mode](https://sqlite.org/c3ref/c_dbconfig_defensive.html#sqlitedbconfigdefensive).

- Bumped the min Go version to 1.27.0 and migrated to the new `encoding/json/v2` package.
    _⚠️ Please note that Go 1.27.0 retrofitted `encoding/json` to use the v2 package under the hood but unfortunately is not fully backward compatible._
    _I recommend to not push blindly an update on production and to test your PocketBase application first locally to see if everything works correctly._
