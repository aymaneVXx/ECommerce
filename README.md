# ECommerce API — Docker Compose (SQL Server + EF Migrations)

Ce projet déploie une API **ASP.NET Core (.NET 9)** avec une base **SQL Server 2022** dans une architecture **multi-container** via **Docker Compose**.

---

## Architecture

- **db** : SQL Server 2022 (données persistées via volume Docker)
- **migrate** : applique automatiquement les migrations EF Core au démarrage
- **api** : API ASP.NET Core exposée sur le port `8080`

---

## Prérequis

- Docker Desktop (ou Docker Engine) installé
- Docker Compose v2 (`docker compose`)
- Ports libres :
  - `8080` (API)
  - `1433` (SQL Server)

> Sur Windows / WSL : assure-toi que Docker Desktop est bien démarré.

---

## Fichiers Docker

- `docker-compose.yml` (à la racine)
- `.env` (à la racine)
- `ECommerce.API/Dockerfile` (API)
- `ECommerce.API/Dockerfile.migrate` (migrations EF Core)

---

## Configuration (.env)

Créer un fichier `.env` à la racine (même dossier que `docker-compose.yml`) :

```env
MSSQL_SA_PASSWORD=
JWT_KEY=
Optionnel:
STRIPE_SECRET_KEY=
STRIPE_WEBHOOK_SECRET=
```
## Lancer l’application (build + run)

Depuis la racine du projet :
```bash
docker compose up --build
```

Ce que fait cette commande :
- Démarre SQL Server
- Exécute les migrations EF Core via le service migrate
- Démarre l’API

## Vérifier l’état des containers
```bash
docker ps
```
Résultat attendu :
ecommerce-db : Up
ecommerce-api : Up
ecommerce-migrate : Exited (0) (normal : il s’arrête une fois les migrations terminées)

## Accéder à l’API

Swagger UI :
http://localhost:8080/swagger

## Logs (debug)

Afficher les logs de l’API :
```bash
docker compose logs -f api
```
Afficher les logs des migrations :
```bash
docker compose logs -f migrate
```
Afficher les logs SQL Server :
```bash
docker compose logs -f db
```

## Arrêter l’application
```bash
docker compose down
```


