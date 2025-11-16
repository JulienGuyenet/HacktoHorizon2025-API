# Mappage automatique des coordonnées de salles

Ce document explique le système de génération automatique de coordonnées X,Y pour les salles et les meubles.

## 🎯 Vue d'ensemble

Lors de l'import de données CSV/Excel, le système génère automatiquement des coordonnées X,Y pour chaque salle en fonction de son numéro et de l'étage. Ces coordonnées permettent de positionner les meubles sur un plan d'étage 2D.

## 📐 Algorithme de génération

### Coordonnée X (position horizontale)
```
X = (numéro de salle modulo 100) × 5 mètres
```

La coordonnée X est calculée à partir des deux derniers chiffres du numéro de salle :
- Salle 417 → 17 × 5 = **85 mètres**
- Salle 105 → 5 × 5 = **25 mètres**
- Salle 621 → 21 × 5 = **105 mètres**

### Coordonnée Y (position verticale)
```
Y = (numéro d'étage × 20) + (préfixe de salle × 5) mètres
```

La coordonnée Y combine l'étage et le premier chiffre du numéro de salle :
- Salle 417 au 4ème étage → (4 × 20) + (4 × 5) = **100 mètres**
- Salle 105 au 1er étage → (1 × 20) + (1 × 5) = **25 mètres**
- Salle 621 au 6ème étage → (6 × 20) + (6 × 5) = **150 mètres**

## 📊 Exemples de coordonnées

| Localisation complète | Salle | Étage | Coordonnées (X, Y) |
|----------------------|-------|-------|-------------------|
| 25\BESANCON\Siege\VIOTTE\4eme etage\417 | 417 | 4ème | (85, 100) |
| 25\BESANCON\Siege\VIOTTE\1er etage\105 | 105 | 1er | (25, 25) |
| 25\BESANCON\Siege\VIOTTE\6eme etage\621 | 621 | 6ème | (105, 150) |
| 25\BESANCON\Siege\VIOTTE\2eme etage\201 - Salle de réunion | 201 | 2ème | (5, 50) |
| 25\BESANCON\Siege\VIOTTE\3eme etage\318 | 318 | 3ème | (90, 75) |
| 25\BESANCON\Siege\VIOTTE\rdc | - | RDC | (null, null) |

## 🔍 Gestion des cas spéciaux

### Salles sans numéro
Si une localisation ne contient pas de numéro de salle (ex: "25\BESANCON\Siege\VIOTTE"), les coordonnées sont `null`.

### Salles avec descriptions
Le système extrait automatiquement le numéro de salle même si elle contient une description :
- "201 - Salle de réunion" → Salle 201
- "130 - Salle de convivialité" → Salle 130

### Étages spéciaux
- **RDC** (rez-de-chaussée) → Étage 0
- **Sous-sol** → Étage -1

### Salles avec noms non numériques
Si une salle n'a pas de numéro identifiable, le système utilise un hash du nom pour générer une position unique :
```
X = (hash du nom modulo 100) × 1 mètre
Y = numéro d'étage × 20 mètres
```

## 🎨 Utilisation pour la visualisation

Les coordonnées peuvent être utilisées pour :

1. **Afficher les meubles sur un plan d'étage**
   ```javascript
   fetch('/api/Furniture/1/position')
     .then(response => response.json())
     .then(position => {
       if (position.x !== null && position.y !== null) {
         drawFurnitureOnFloorPlan(position.x, position.y);
       }
     });
   ```

2. **Filtrer les meubles par zone**
   ```javascript
   // Meubles dans une zone spécifique (ex: X entre 50 et 100, Y entre 80 et 120)
   const furniture = allFurniture.filter(f => 
     f.positionX >= 50 && f.positionX <= 100 &&
     f.positionY >= 80 && f.positionY <= 120
   );
   ```

3. **Calculer les distances entre meubles**
   ```javascript
   function distance(furniture1, furniture2) {
     const dx = furniture1.positionX - furniture2.positionX;
     const dy = furniture1.positionY - furniture2.positionY;
     return Math.sqrt(dx * dx + dy * dy);
   }
   ```

## 🔧 Personnalisation

### Mapper explicitement une salle
Si vous souhaitez définir des coordonnées spécifiques pour certaines salles, vous pouvez étendre le service `RoomCoordinateMapper` :

```csharp
public class CustomRoomCoordinateMapper : RoomCoordinateMapper
{
    public CustomRoomCoordinateMapper(ILogger<RoomCoordinateMapper> logger) 
        : base(logger)
    {
        // Définir des coordonnées personnalisées pour des salles spécifiques
        AddMapping("VIOTTE", "4eme etage", "417", 150.5, 200.3);
        AddMapping("VIOTTE", "1er etage", "105", 50.0, 30.0);
    }
}
```

### Adapter l'échelle
Les coordonnées sont en mètres par défaut, mais peuvent représenter n'importe quelle unité :
- **Mètres** : pour des plans physiques
- **Pixels** : pour des images de plans d'étage
- **Unités arbitraires** : pour des représentations abstraites

## 📝 API Endpoints

### Obtenir la position d'un meuble
```http
GET /api/Furniture/{id}/position
```

**Réponse :**
```json
{
  "x": 85.0,
  "y": 100.0
}
```

### Obtenir tous les meubles avec positions
```http
GET /api/Furniture
```

**Réponse :**
```json
[
  {
    "id": 1,
    "reference": "FAUTDACTYOPE",
    "designation": "Fauteuil dactylo opérateur",
    "positionX": 85.0,
    "positionY": 100.0,
    "locationId": 12,
    ...
  }
]
```

### Obtenir les localisations avec coordonnées
```http
GET /api/Location
```

**Réponse :**
```json
[
  {
    "id": 12,
    "buildingName": "VIOTTE",
    "floor": "4eme etage",
    "room": "417",
    "positionX": 85.0,
    "positionY": 100.0,
    ...
  }
]
```

## 🚀 Avantages du système

1. **Automatique** : Aucune saisie manuelle de coordonnées nécessaire
2. **Cohérent** : Les salles avec le même numéro ont toujours les mêmes coordonnées
3. **Scalable** : Fonctionne pour n'importe quel nombre de salles et d'étages
4. **Flexible** : Peut être personnalisé avec des mappings explicites si nécessaire
5. **Compatible** : Fonctionne avec des localisations incomplètes (gère les cas null)

## 🎓 Exemples d'utilisation

### Exemple 1 : Visualisation simple en JavaScript
```javascript
// Créer un canvas pour afficher le plan d'étage
const canvas = document.getElementById('floorPlan');
const ctx = canvas.getContext('2d');

// Récupérer tous les meubles du 4ème étage
fetch('/api/Furniture?floor=4eme etage')
  .then(response => response.json())
  .then(furnitures => {
    furnitures.forEach(furniture => {
      // Dessiner chaque meuble à sa position
      if (furniture.positionX && furniture.positionY) {
        ctx.fillRect(furniture.positionX * 2, furniture.positionY * 2, 5, 5);
      }
    });
  });
```

### Exemple 2 : Recherche de proximité
```javascript
// Trouver tous les meubles proches d'une position donnée
function findNearbyFurniture(targetX, targetY, maxDistance) {
  return fetch('/api/Furniture')
    .then(response => response.json())
    .then(furnitures => {
      return furnitures.filter(f => {
        if (!f.positionX || !f.positionY) return false;
        const distance = Math.sqrt(
          Math.pow(f.positionX - targetX, 2) + 
          Math.pow(f.positionY - targetY, 2)
        );
        return distance <= maxDistance;
      });
    });
}

// Exemple : trouver tous les meubles dans un rayon de 20 mètres autour du point (85, 100)
findNearbyFurniture(85, 100, 20).then(nearby => {
  console.log(`Trouvé ${nearby.length} meubles à proximité`);
});
```

## 📖 En savoir plus

- [Guide d'import CSV/Excel](IMPORT.md)
- [Exemple d'utilisation de l'API de position](examples/position-api-example.md)
- [Architecture du système](ARCHITECTURE.md)
