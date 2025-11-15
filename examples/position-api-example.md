# Exemple d'utilisation de l'endpoint de position

## Créer un meuble avec position

```bash
curl -X POST http://localhost:8080/api/Furniture \
  -H "Content-Type: application/json" \
  -d '{
    "reference": "BUR-001",
    "designation": "Bureau ergonomique",
    "famille": "Bureau",
    "positionX": 15.5,
    "positionY": 22.3
  }'
```

**Réponse:**
```json
{
  "id": 1,
  "reference": "BUR-001",
  "designation": "Bureau ergonomique",
  "famille": "Bureau",
  "positionX": 15.5,
  "positionY": 22.3,
  "createdAt": "2025-11-15T14:00:00Z"
}
```

## Récupérer la position d'un meuble

```bash
curl http://localhost:8080/api/Furniture/1/position
```

**Réponse:**
```json
{
  "x": 15.5,
  "y": 22.3
}
```

## Cas particuliers

### Meuble sans position définie

**Requête:**
```bash
curl http://localhost:8080/api/Furniture/2/position
```

**Réponse:**
```json
{
  "x": null,
  "y": null
}
```

### Meuble inexistant

**Requête:**
```bash
curl http://localhost:8080/api/Furniture/999/position
```

**Réponse:** HTTP 404 Not Found

## Utilisation dans un plan d'étage

Les coordonnées x,y peuvent représenter:
- Des mètres depuis un point d'origine dans le bâtiment
- Des pixels sur un plan d'étage numérique
- Toute autre unité de mesure définie par votre application

**Exemple d'intégration:**
```javascript
// Récupérer et afficher la position sur un plan
fetch('/api/Furniture/1/position')
  .then(response => response.json())
  .then(position => {
    if (position.x !== null && position.y !== null) {
      placeMarkerOnFloorPlan(position.x, position.y);
    }
  });
```
