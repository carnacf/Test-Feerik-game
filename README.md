# Test-Feerik-game

Version de unity utilisée : 2017.3

Fonctionnement :

Dans la scène se trouve un GameObject "Ressource_manager" qui possède un script "Manager_script". 
Ce script est le script s'occupe de télécharger et d'appliquer les textures aux objets de la scène.

L'objet "Need_tex" dans la scene permet d'indiquer que tous ses objets fils ont besoin d'une texture et possèdent le script "Object_script".
Cela à été implémenté de la sorte afin de faciliter la recherche des objets nécessitant une texture.
Le script Object_script permet uniquement d'attacher à un GameObject une string qui est le nom de la texture dont il a besoin.

Dans un souci de facilité, toutes les URL des textures sont stockées dans un fichier texte lu au début du script "Manager_script".


Le script "Manager_script" fonctionne de la sorte :
  Il crée à partir des GameObject nécessitant une texture une liste à valeur unique de noms de textures à charger.
  La liste est unique afin de charger qu'une seule fois chaque texture.
  
  Il lance ensuite autant de threads que paramétré dans l'éditeur. Chaque thread va parcourir une partie de la liste des textures à charger.
  Si cette texture n'est pas présente sur le stockage local alors le thread la télécharge et la sauvegarde dans le bon dossier.
  Ensuite le thread va ajouter à une table de hashage, s'il n'y est pas déja, le tableau d'octets de la texture avec comme clef son nom.
  
  Une fois l'exécution des threads terminée, le script parcours tous les GameObject ayant besoin d'une texture et leurs applique la bonne texture
  en la récupérant dans la table de hashage.
