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
