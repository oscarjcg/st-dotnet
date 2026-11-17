## EF Core

```
dotnet ef dbcontext info --project st-dotnet
dotnet ef migrations add create_categories --project st-dotnet
dotnet ef database update --project st-dotnet
dotnet ef migrations remove --project st-dotnet
```

## Visual Studio

Requires user secrets (`GalleryMySql:Conn`, `S3:AccessKeyId`, `S3:SecretKey`) and a MySQL database.

## Docker Compose

Secrets live in `.env` (git-ignored). Create it once:

```
cp .env.example .env   # then fill in the values
```

| File | Purpose |
| --- | --- |
| `docker-compose.yml` | Shared base: app + MySQL, healthcheck, volume |
| `docker-compose.override.yml` | Dev: `Development`, ports 8086 and 3306, dev cert + user secrets mounts |
| `docker-compose.prod.yml` | Prod: `Production`, restart policy, app bound to `127.0.0.1:8086` for the reverse proxy, DB not published |

Dev (the override file is loaded automatically):

```
docker compose up --build
```

Prod:

```
docker compose -f docker-compose.yml -f docker-compose.prod.yml up -d --build
```

### Existing databases

`MYSQL_USER` / `MYSQL_PASSWORD` are only applied when MySQL initialises an empty volume.
For a volume created before this setup, create the app user once:

```
docker compose exec db mysql -uroot -p -e "CREATE USER 'gallery'@'%' IDENTIFIED BY '<password>'; GRANT ALL ON GALLERY.* TO 'gallery'@'%';"
```


## Running tests

Everything runs in Docker. No local .NET SDK is needed.

| Project | What it covers |
| --- | --- |
| `tests/st-dotnet.UnitTests` | Repositories (mocked data layer), EF data classes on in-memory SQLite, API controllers (mocked S3 and repositories), the Node notifier |
| `tests/st-dotnet.UITests` | Playwright (headless Chromium) end to end against the real app, MySQL and an S3 mock: login/register, the dev-only register rule, and the admin CRUD pages |

`docker-compose.test.yml` is a standalone stack named `st-dotnet-test`:
- It does not read `.env` and publishes no host ports.
- MySQL lives in `tmpfs`, and the app creates the schema on start through `Testing:EnsureDatabaseCreated`.
- Uploads go to [Adobe S3Mock](https://github.com/adobe/S3Mock) instead of AWS.
- The Node notification is disabled.
- It runs two app instances: one `Development` and one `Production` (used to check that registration is disabled).

Unit tests:

```
docker compose -f docker-compose.test.yml run --rm --build unit-tests
```

UI tests (the exit code is the test result):

```
docker compose -f docker-compose.test.yml up --build --abort-on-container-exit --exit-code-from ui-tests ui-tests
```

Clean up afterwards:

```
docker compose -f docker-compose.test.yml down -v
```

Results go to `test-results/` (git-ignored):
- `unit.trx` and `ui.trx`
- for every failing UI test, a screenshot `<Test>.png` and a Playwright trace `<Test>.zip`. To open a trace, drop it on https://trace.playwright.dev.
