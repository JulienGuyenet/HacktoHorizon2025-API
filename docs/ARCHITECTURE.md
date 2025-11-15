# Furniture Inventory API - API d'Inventaire de Meubles

API REST modulaire en .NET pour la gestion d'inventaire de meubles avec intégration RFID pour la localisation dans les bâtiments.

## 🏗️ Architecture

Le projet suit une architecture en couches modulaire avec séparation des préoccupations :

```
FurnitureInventory/
├── src/
│   ├── FurnitureInventory.Core/          # Couche domaine - Entités et Interfaces
│   │   ├── Entities/                      # Entités du domaine
│   │   │   ├── Furniture.cs               # Meuble
│   │   │   ├── Location.cs                # Localisation dans le bâtiment
│   │   │   ├── RfidTag.cs                 # Tag RFID
│   │   │   └── RfidReader.cs              # Lecteur RFID
│   │   └── Interfaces/                    # Contrats d'interface
│   │       ├── IRepository.cs             # Interface générique repository
│   │       ├── IFurnitureRepository.cs    # Repository meubles
│   │       ├── ILocationRepository.cs     # Repository localisations
│   │       ├── IRfidTagRepository.cs      # Repository tags RFID
│   │       ├── IRfidReaderRepository.cs   # Repository lecteurs RFID
│   │       ├── IFurnitureService.cs       # Service meubles
│   │       ├── ILocationService.cs        # Service localisations
│   │       ├── IRfidService.cs            # Service RFID
│   │       └── IExcelImportService.cs     # Service import Excel
│   ├── FurnitureInventory.Infrastructure/ # Couche infrastructure - Implémentations
│   │   ├── Data/
│   │   │   └── FurnitureInventoryContext.cs  # DbContext EF Core
│   │   ├── Repositories/                  # Implémentations des repositories
│   │   │   ├── Repository.cs              # Repository générique
│   │   │   ├── FurnitureRepository.cs
│   │   │   ├── LocationRepository.cs
│   │   │   ├── RfidTagRepository.cs
│   │   │   └── RfidReaderRepository.cs
│   │   └── Services/                      # Implémentations des services
│   │       ├── FurnitureService.cs
│   │       ├── LocationService.cs
│   │       └── RfidService.cs
│   └── FurnitureInventory.Api/            # Couche présentation - API REST
│       ├── Controllers/
│       │   ├── FurnitureController.cs     # Endpoints meubles
│       │   ├── LocationController.cs      # Endpoints localisations
│       │   └── RfidController.cs          # Endpoints RFID
│       ├── Program.cs                     # Configuration de l'application
│       └── appsettings.json               # Configuration
```

## 📊 Modèle de Données

### Furniture (Meuble)
Champs provenant de l'Excel :
- `Reference` - Référence unique du meuble
- `Designation` - Nom/description du meuble
- `Famille` - Famille du meuble (ex: Bureau, Chaise, Armoire)
- `Type` - Type de meuble
- `Fournisseur` - Fournisseur du meuble
- `Utilisateur` - Utilisateur actuel
- `CodeBarre` - Code barre pour identification
- `NumeroSerie` - Numéro de série
- `Informations` - Informations complémentaires
- `Site` - Site où se trouve le meuble
- `DateLivraison` - Date de livraison

Champs supplémentaires pour la localisation et RFID :
- `LocationId` - Référence à la localisation actuelle
- `RfidTagId` - Référence au tag RFID associé

### Location (Localisation)
Permet de suivre la position des meubles dans les bâtiments :
- `BuildingName` - Nom du bâtiment
- `Floor` - Étage
- `Room` - Salle/Pièce
- `Zone` - Zone spécifique dans la pièce
- `Description` - Description complète
- `Latitude/Longitude` - Coordonnées GPS (optionnel)

### RfidTag (Tag RFID)
Tag RFID attaché aux meubles :
- `TagId` - Identifiant unique du tag RFID
- `TagType` - Type de tag (EPC Gen2, ISO 15693, etc.)
- `Status` - Statut (Actif, Inactif, Perdu)
- `LastReadDate` - Dernière date de lecture
- `LastReaderId` - Dernier lecteur ayant lu le tag

### RfidReader (Lecteur RFID)
Lecteurs RFID installés dans les bâtiments :
- `ReaderId` - Identifiant unique du lecteur
- `Name` - Nom du lecteur
- `Model` - Modèle du lecteur
- `IpAddress` - Adresse IP
- `Status` - Statut (En ligne, Hors ligne, En maintenance)
- `LocationId` - Localisation du lecteur

## 🚀 Démarrage

### Prérequis

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- Un éditeur de code (Visual Studio, VS Code, Rider)

### Installation

1. Clonez le repository :
```bash
git clone https://github.com/JulienGuyenet/HacktoHorizon2025.git
cd HacktoHorizon2025
```

2. Restaurez les packages NuGet :
```bash
dotnet restore
```

3. Compilez le projet :
```bash
dotnet build
```

4. Lancez l'API :
```bash
cd src/FurnitureInventory.Api
dotnet run
```

L'API sera accessible à :
- HTTP : `http://localhost:5000`
- HTTPS : `https://localhost:5001`
- Swagger UI : `https://localhost:5001/swagger`

## 📡 Endpoints API

### Meubles (/api/furniture)

- `GET /api/furniture` - Liste tous les meubles
- `GET /api/furniture/{id}` - Récupère un meuble par ID
- `GET /api/furniture/barcode/{barcode}` - Recherche par code barre
- `GET /api/furniture/search?reference=&famille=&site=` - Recherche avec critères
- `POST /api/furniture` - Crée un nouveau meuble
- `PUT /api/furniture/{id}` - Met à jour un meuble
- `DELETE /api/furniture/{id}` - Supprime un meuble
- `POST /api/furniture/{id}/location/{locationId}` - Assigne une localisation
- `POST /api/furniture/{id}/rfid/{rfidTagId}` - Assigne un tag RFID

### Localisations (/api/location)

- `GET /api/location` - Liste toutes les localisations
- `GET /api/location/{id}` - Récupère une localisation par ID
- `GET /api/location/{id}/furniture` - Liste les meubles à une localisation
- `GET /api/location/building/{buildingName}` - Recherche par bâtiment
- `POST /api/location` - Crée une nouvelle localisation
- `PUT /api/location/{id}` - Met à jour une localisation
- `DELETE /api/location/{id}` - Supprime une localisation

### RFID (/api/rfid)

#### Tags RFID
- `GET /api/rfid/tags` - Liste tous les tags actifs
- `GET /api/rfid/tags/{tagId}` - Récupère un tag par son identifiant
- `POST /api/rfid/tags` - Enregistre un nouveau tag
- `POST /api/rfid/tags/{tagId}/assign/{furnitureId}` - Associe un tag à un meuble
- `POST /api/rfid/read` - Traite une lecture de tag RFID
- `POST /api/rfid/tags/{tagId}/deactivate` - Désactive un tag

#### Lecteurs RFID
- `GET /api/rfid/readers` - Liste tous les lecteurs actifs
- `POST /api/rfid/readers` - Enregistre un nouveau lecteur
- `POST /api/rfid/readers/{readerId}/status` - Met à jour le statut d'un lecteur

## 🗄️ Base de Données

Le projet utilise **SQLite** avec Entity Framework Core pour un déploiement simple et sans dépendances externes.

La base de données est créée automatiquement au premier lancement dans le fichier `furnitureinventory.db`.

### Configuration

Dans `appsettings.json` :
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=furnitureinventory.db"
  }
}
```

### Migrations (si nécessaire)

Pour créer une nouvelle migration après modification du modèle :
```bash
cd src/FurnitureInventory.Infrastructure
dotnet ef migrations add NomDeLaMigration --startup-project ../FurnitureInventory.Api
dotnet ef database update --startup-project ../FurnitureInventory.Api
```

## 📥 Import Excel

Pour importer les données Excel existantes, vous pouvez :

1. **Via l'API** : Implémenter `IExcelImportService` dans Infrastructure (exemple à créer)
2. **Manuellement** : Utiliser un script de conversion Excel → SQL/JSON
3. **Via un outil** : Utiliser un outil comme [excel2sqlite](https://www.rebasedata.com/convert-excel-to-sqlite-online)

Exemple de format JSON pour import via API :
```json
{
  "reference": "REF-001",
  "designation": "Bureau ajustable",
  "famille": "Bureau",
  "type": "Électrique",
  "fournisseur": "IKEA",
  "utilisateur": "Jean Dupont",
  "codeBarre": "123456789",
  "numeroSerie": "SN-001",
  "informations": "Hauteur réglable 65-130cm",
  "site": "Dijon",
  "dateLivraison": "2024-01-15"
}
```

## 🔌 Intégration RFID

L'API est conçue pour être modulaire et s'intégrer facilement avec différents systèmes RFID :

1. **Configuration du lecteur** : Enregistrez vos lecteurs RFID via l'endpoint `/api/rfid/readers`
2. **Enregistrement des tags** : Associez des tags RFID aux meubles via `/api/rfid/tags`
3. **Traitement des lectures** : Envoyez les lectures RFID à `/api/rfid/read`

### Exemple d'intégration

```csharp
// Lecture RFID reçue du lecteur
var tagRead = new TagReadRequest("TAG-12345", "READER-001");

// Envoi à l'API
await httpClient.PostAsJsonAsync("/api/rfid/read", tagRead);

// L'API met à jour automatiquement :
// - La dernière date de lecture du tag
// - La localisation du meuble (si le lecteur a une localisation)
```

## 🔐 Sécurité

Points à considérer pour la production :

- [ ] Ajouter l'authentification (JWT, OAuth2)
- [ ] Implémenter l'autorisation basée sur les rôles
- [ ] Chiffrer les données sensibles
- [ ] Ajouter la validation des entrées
- [ ] Mettre en place les logs d'audit
- [ ] Configurer CORS selon vos besoins

## 🧪 Tests

(À implémenter)

```bash
dotnet test
```

## 📝 Licence

Ce projet est développé pour la région Bourgogne-Franche-Comté.

## 🤝 Contribution

Pour contribuer au projet :

1. Fork le repository
2. Créez une branche pour votre feature (`git checkout -b feature/AmazingFeature`)
3. Committez vos changements (`git commit -m 'Add some AmazingFeature'`)
4. Push vers la branche (`git push origin feature/AmazingFeature`)
5. Ouvrez une Pull Request

## 📧 Contact

Pour toute question concernant ce projet, contactez Julien Guyenet.
