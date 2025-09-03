using Godot;
using Newtonsoft.Json.Linq;

namespace VA.Base.Ressources;

public class Json
{
    /// <summary>
    /// Charge un fichier Json en tant que string puis convertit le en JArray
    /// </summary>
    /// <param name="chemin_">L'emplacement de la ressource commencant par "res://"</param>
    /// <returns>la resource convertit en JArray</returns>
    public static JArray Ressource_Tableau(string chemin_)
    { return JArray.Parse(Fichier.ChargerFichier(chemin_)); }
    
    /// <summary>
    /// Charge un fichier Json en tant que string puis convertit le en JObject
    /// </summary>
    /// <param name="chemin_">L'emplacement de la ressource commencant par "res://"</param>
    /// <returns>la resource convertit en JArray</returns>
    public static JObject Ressource_Objet(string chemin_)
    { return JObject.Parse(Fichier.ChargerFichier(chemin_)); }
    /// <summary>
    /// Charge une partie d'un fichier Texte(Json) sous forme de string avant de la convertir en JObject
    /// </summary>
    /// <param name="chemin_">le chemin du fichier dans les ressources du jeu</param>
    /// <param name="Début_">la position en byte du début de la partie à récupéré dans le fichier</param>
    /// <param name="Longueur_">la longueur de la partie à récupéré dans le fichier</param>
    /// <returns></returns>
    public static JObject Ressource_Objet(string chemin_, ulong Début_, ulong Longueur_)
    { return JObject.Parse(Fichier.ChargerPartieFichier(chemin_, Début_, Longueur_)); }
}