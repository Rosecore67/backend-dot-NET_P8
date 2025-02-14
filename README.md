# TourGuide - Application de recommandations touristiques

## Description
TourGuide est une application qui permet aux utilisateurs de découvrir des attractions touristiques à proximité de leur emplacement en temps réel. Le projet repose sur une architecture en microservices et optimise les performances grâce au multithreading et aux requêtes asynchrones.

## Fonctionnalités
- Recherche des attractions proches d’un utilisateur via une API REST.
- Gestion des récompenses pour chaque attraction visitée.
- Suivi en temps réel de la position utilisateur.
- API scalable permettant de supporter un grand nombre d’utilisateurs simultanément.

## Technologies utilisées
- .NET 7 (ASP.NET Core)
- Entity Framework Core
- GitHub Actions pour l’intégration et le déploiement continu
- XUnit pour les tests unitaires et d’intégration

## Déploiement
Le projet utilise GitHub Actions pour l'automatisation du build et des tests. Une fois le code validé sur `dev`, il peut être fusionné en `master` pour déclencher une publication.

## Installation et exécution
1. Cloner le projet :
   ```sh
   git clone https://github.com/username/TourGuide.git
