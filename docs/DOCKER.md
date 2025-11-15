# Guide Docker - Furniture Inventory API

Ce guide explique comment utiliser Docker pour déployer l'API Furniture Inventory.

## Prérequis

- Docker Engine 20.10 ou supérieur
- Docker Compose 1.29 ou supérieur (optionnel)

## Construction de l'image Docker

### Méthode 1 : Utilisation de docker build

```bash
docker build -t furniture-inventory-api:latest .
```

### Méthode 2 : Utilisation de docker-compose

```bash
docker-compose build
```

## Exécution du conteneur

### Méthode 1 : Utilisation de docker run

```bash
docker run -d \
  --name furniture-inventory-api \
  -p 8080:8080 \
  -v furniture-data:/app/data \
  furniture-inventory-api:latest
```

### Méthode 2 : Utilisation de docker-compose (Recommandé)

```bash
docker-compose up -d
```

## Accès à l'API

Une fois le conteneur démarré, l'API est accessible à :

- **API Base URL**: http://localhost:8080
- **Documentation Swagger**: http://localhost:8080/swagger (en mode développement)
- **Endpoints**: 
  - http://localhost:8080/api/Furniture
  - http://localhost:8080/api/Location
  - http://localhost:8080/api/Rfid

## Gestion du conteneur

### Voir les logs

```bash
docker-compose logs -f api
# ou
docker logs -f furniture-inventory-api
```

### Arrêter le conteneur

```bash
docker-compose down
# ou
docker stop furniture-inventory-api
```

### Redémarrer le conteneur

```bash
docker-compose restart
# ou
docker restart furniture-inventory-api
```

## Persistance des données

La base de données SQLite est stockée dans un volume Docker nommé `furniture-data`. Les données persistent même après l'arrêt du conteneur.

### Sauvegarder la base de données

```bash
docker cp furniture-inventory-api:/app/data/furnitureinventory.db ./backup.db
```

### Restaurer la base de données

```bash
docker cp ./backup.db furniture-inventory-api:/app/data/furnitureinventory.db
docker-compose restart
```

## Configuration

### Variables d'environnement

Les variables d'environnement suivantes peuvent être configurées :

| Variable | Description | Valeur par défaut |
|----------|-------------|-------------------|
| `ASPNETCORE_ENVIRONMENT` | Environnement d'exécution | `Production` |
| `ASPNETCORE_URLS` | URLs d'écoute | `http://+:8080` |
| `ConnectionStrings__DefaultConnection` | Chaîne de connexion SQLite | `Data Source=/app/data/furnitureinventory.db` |

### Modifier le port d'écoute

Dans `docker-compose.yml`, modifiez la section `ports` :

```yaml
ports:
  - "3000:8080"  # Expose sur le port 3000 au lieu de 8080
```

## Architecture multi-stage

Le Dockerfile utilise une construction multi-stage pour optimiser la taille de l'image :

1. **Stage Build** : Utilise l'image SDK .NET 9.0 pour compiler l'application
2. **Stage Runtime** : Utilise l'image runtime .NET 9.0 (plus légère) pour exécuter l'application

Avantages :
- Image finale plus petite (~200 MB au lieu de ~700 MB)
- Temps de déploiement réduit
- Surface d'attaque de sécurité minimisée

## Dépannage

### Le conteneur ne démarre pas

Vérifiez les logs :
```bash
docker-compose logs api
```

### Problème de permissions sur le volume

```bash
docker-compose down -v
docker-compose up -d
```

### Réinitialiser complètement

```bash
docker-compose down -v
docker system prune -a
docker-compose up -d --build
```

## Production

Pour un déploiement en production, considérez :

1. **HTTPS** : Configurez un reverse proxy (nginx, traefik) avec certificat SSL
2. **Monitoring** : Intégrez des outils de monitoring (Prometheus, Grafana)
3. **Backups** : Automatisez les sauvegardes de la base de données
4. **Scaling** : Utilisez Docker Swarm ou Kubernetes pour le scaling horizontal

### Exemple avec reverse proxy nginx

```yaml
version: '3.8'

services:
  api:
    build: .
    expose:
      - "8080"
    networks:
      - backend

  nginx:
    image: nginx:alpine
    ports:
      - "80:80"
      - "443:443"
    volumes:
      - ./nginx.conf:/etc/nginx/nginx.conf:ro
      - ./certs:/etc/nginx/certs:ro
    networks:
      - backend
    depends_on:
      - api

networks:
  backend:
    driver: bridge

volumes:
  furniture-data:
```

## Ressources

- [Documentation Docker](https://docs.docker.com/)
- [Documentation .NET Docker](https://learn.microsoft.com/en-us/dotnet/core/docker/)
- [Best practices Docker](https://docs.docker.com/develop/dev-best-practices/)
