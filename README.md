# rinha-de-backend-2026-dotnet

### Dump
```shell
docker exec -t rinha-de-backend-2026-dotnet-postgres-server-1 pg_dump -U postgres --create rinha-db | gzip > dump_`date +%d-%m-%Y"_"%H_%M_%S`.sql.gz
```