# Quick Start Guide - Furniture Inventory API

Ce guide vous permet de démarrer rapidement avec l'API.

## 1. Démarrer l'API

```bash
cd src/FurnitureInventory.Api
dotnet run
```

L'API sera accessible sur `https://localhost:5001` et la documentation Swagger sur `https://localhost:5001/swagger`

## 2. Créer une localisation

```bash
curl -X POST https://localhost:5001/api/location \
  -H "Content-Type: application/json" \
  -d '{
    "buildingName": "Bâtiment A",
    "floor": "1",
    "room": "101",
    "zone": "Coin fenêtre",
    "description": "Bureau principal"
  }'
```

## 3. Créer un meuble

```bash
curl -X POST https://localhost:5001/api/furniture \
  -H "Content-Type: application/json" \
  -d '{
    "reference": "REF-001",
    "designation": "Bureau ajustable électrique",
    "famille": "Bureau",
    "type": "Électrique",
    "fournisseur": "IKEA",
    "utilisateur": "Jean Dupont",
    "codeBarre": "123456789",
    "numeroSerie": "SN-001",
    "informations": "Hauteur réglable 65-130cm",
    "site": "Dijon",
    "dateLivraison": "2024-01-15T00:00:00",
    "locationId": 1
  }'
```

## 4. Créer un tag RFID

```bash
curl -X POST https://localhost:5001/api/rfid/tags \
  -H "Content-Type: application/json" \
  -d '{
    "tagId": "RFID-TAG-001",
    "tagType": "EPC Gen2"
  }'
```

## 5. Associer le tag au meuble

```bash
curl -X POST https://localhost:5001/api/rfid/tags/RFID-TAG-001/assign/1
```

## 6. Créer un lecteur RFID

```bash
curl -X POST https://localhost:5001/api/rfid/readers \
  -H "Content-Type: application/json" \
  -d '{
    "readerId": "READER-001",
    "name": "Lecteur Entrée Bâtiment A",
    "locationId": 1
  }'
```

## 7. Simuler une lecture RFID

```bash
curl -X POST https://localhost:5001/api/rfid/read \
  -H "Content-Type: application/json" \
  -d '{
    "tagId": "RFID-TAG-001",
    "readerId": "READER-001"
  }'
```

Cette action met à jour automatiquement la localisation du meuble selon la position du lecteur RFID.

## 8. Rechercher des meubles

### Par code barre
```bash
curl https://localhost:5001/api/furniture/barcode/123456789
```

### Par famille
```bash
curl https://localhost:5001/api/furniture/search?famille=Bureau
```

### Par site
```bash
curl https://localhost:5001/api/furniture/search?site=Dijon
```

## 9. Récupérer les meubles d'une localisation

```bash
curl https://localhost:5001/api/location/1/furniture
```

## 10. Lister tous les meubles

```bash
curl https://localhost:5001/api/furniture
```

## Données d'exemple

Des fichiers JSON d'exemple sont disponibles dans le dossier `examples/` :
- `sample-furniture-data.json` - Données de meubles
- `sample-locations.json` - Données de localisations

## Swagger UI

Pour une exploration interactive de l'API, visitez : `https://localhost:5001/swagger`

Vous y trouverez :
- Documentation complète des endpoints
- Possibilité de tester l'API directement depuis le navigateur
- Schémas des modèles de données
- Exemples de requêtes et réponses
