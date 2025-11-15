# Furniture Inventory API

API REST modulaire en .NET pour la gestion d'inventaire de meubles avec intégration RFID.

## 🚀 Démarrage rapide

```bash
# Cloner le repository
git clone https://github.com/JulienGuyenet/HacktoHorizon2025.git
cd HacktoHorizon2025

# Compiler le projet
dotnet build

# Lancer l'API
cd src/FurnitureInventory.Api
dotnet run
```

Accédez à Swagger UI : https://localhost:5001/swagger

## 📖 Documentation complète

Consultez [ARCHITECTURE.md](ARCHITECTURE.md) pour :
- Architecture détaillée du projet
- Modèle de données
- Guide d'utilisation des endpoints
- Instructions d'intégration RFID
- Import de données Excel

## 🏗️ Architecture

- **FurnitureInventory.Core** : Entités du domaine et interfaces
- **FurnitureInventory.Infrastructure** : Implémentations (repositories, services, SQLite)
- **FurnitureInventory.Api** : Contrôleurs REST et configuration

## 🎯 Fonctionnalités

✅ Gestion complète d'inventaire de meubles  
✅ Localisation dans les bâtiments (étage, salle, zone)  
✅ Intégration RFID pour le tracking automatique  
✅ Base de données SQLite légère et portable  
✅ Architecture modulaire avec faible couplage  
✅ API REST documentée avec Swagger

## 📊 Technologies

- .NET 9.0
- Entity Framework Core
- SQLite
- Swagger/OpenAPI
- Architecture en couches

---

Projet d'inventaire de meubles pour la région Bourgogne-Franche-Comté
