using System;
using System.IO;
using Godot;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace VA.Base.Fichiers.JSON
{
    // public interface IJObject
    // {
    //     public void Init(JObject objectJson_);
    // }
    //
    // public interface IJArray
    // {
    //     public void Init(JArray tableauJson_);
    // }
    
    /// <summary>
    /// Classe slatic comprenant des methodes d'utilité générale concernant le format JSON
    /// </summary>
    public static class Utiles
    {
        public enum JsonTypes { Invalide, Valeur, Objet, Tableau }
        
        /// <summary>
        /// Vérifie que le premier et dernier caractère du fichier sont conformes à une structure Json
        /// </summary>
        /// <param name="fichierJson_"></param>
        /// <returns>La nature de l'élément racine de la structure</returns>
        public static JsonTypes VerifFichier(string fichierJson_)
        {
            char d = '\0', f = '\0';
            for (int i = 0; i < fichierJson_.Length; ++i)
            {
                d = fichierJson_[i];
                if (d == '[' || d == '{')
                {
                    for (int j = fichierJson_.Length - 1; j >= 0; --j)
                    {
                        f = fichierJson_[j];
                        if (f == ']' || f == '}')
                        { break; }
                    }
                    break;
                }
            }
            
            // TODO : v�rifier les deux carat�res englobants ne suffit pas, il faut v�rifier toute l'architecture

            if (d == '{' && f == '}')
            { return JsonTypes.Objet; }
            else if (d == '[' && f == ']')
            { return JsonTypes.Tableau; }
            else
            { return JsonTypes.Invalide; }
        }

        /// <summary>
        /// Retire les éléments superflus avant et après la structure Json
        /// </summary>
        /// <param name="fichierJson_"></param>
        /// <returns></returns>
        public static string RognerJson(string fichierJson_)
        {
            if (fichierJson_.Length > 0)
            {
                int a = fichierJson_.IndexOfAny(new char[] { '{', '[' });
                fichierJson_ = fichierJson_.Remove(0, a);

                int b = fichierJson_.LastIndexOfAny(new char[] { '}', ']' });
                fichierJson_ = fichierJson_.Remove(b + 1, fichierJson_.Length - b - 1);
            }

            return fichierJson_;
        }

        // TODO: créer une méthode permettant d'extraire une partie spécifique d'un fichier texte/Json pour la convertir en JObject/JArray/etc

        /// <summary>
        /// Convertit un fichier Json en JObject
        /// </summary>
        /// <param name="chemin_">Le chemin du fichier Json</param>
        /// <returns>Le JObject correspondant au fichier</returns>
        public static JObject Fichier_Objet(string chemin_)
        {
            JObject objet = null;
            if (File.Exists(chemin_))
            {
                using (StreamReader fichier = File.OpenText(chemin_))
                {
                    using (JsonTextReader lecteur = new JsonTextReader(fichier))
                    { objet = JToken.ReadFrom(lecteur) as JObject; }
                }
            }
            else
            { GD.PrintErr($"<color=red>Error: </color>Impossible de charger le fichier � l'emplacement {chemin_} !!"); }

            return objet;
        }

        /// <summary>
        /// Convertit un fichier Json en JObject
        /// </summary>
        /// <param name="chemin_">Le chemin du fichier Json</param>
        /// <param name="ligneDébut_">La ligne du fichier où commence l'Objet Json</param>
        /// <param name="ligneFin_">La ligne de fin de l'objet Json</param>
        /// <returns>Le JObject correspondant au fichier</returns>
        /// <exception cref="NotImplementedException"></exception>
        public static JObject Fichier_Objet(string chemin_, int ligneDébut_, int ligneFin_) => Fichier_Objet(chemin_, ligneDébut_, 0, ligneFin_, 0);

        /// <summary>
        /// Convertit un fichier Json en JObject
        /// </summary>
        /// <param name="chemin_">Le chemin du fichier Json</param>
        /// <param name="ligneDébut_">La ligne du fichier où commence l'Objet Json</param>
        /// <param name="caractèreDébut_">Le caractère où commencer l'extraction</param>
        /// <param name="ligneFin_">La ligne de fin de l'objet Json</param>
        /// <param name="caractèreFin_">le caractère où terminer l'extraction</param>
        /// <returns>Le JObject correspondant au fichier</returns>
        /// <exception cref="NotImplementedException"></exception>
        public static JObject Fichier_Objet(string chemin_, int ligneDébut_, int caractèreDébut_, int ligneFin_, int caractèreFin_)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Convertit un fichier Json en JArray
        /// </summary>
        /// <param name="chemin_">Le chemin du fichier Json</param>
        /// <returns></returns>
        public static JArray Fichier_Tableau(string chemin_)
        {
            JArray tableau = null;
            if (File.Exists(chemin_))
            {

                using (StreamReader fichier = File.OpenText(chemin_))
                {
                    using (JsonTextReader lecteur = new JsonTextReader(fichier))
                    { tableau = JToken.ReadFrom(lecteur) as JArray; }
                }
            }
            else
            { GD.PrintErr($"Impossible de charger le fichier à l'emplacement {chemin_} !!"); }

            return tableau;
        }

        /// <summary>
        /// Renomme un token ou son parent
        /// </summary>
        /// <param name="cible_">Le Token cible</param>
        /// <param name="nouveauNom_">Le nouveau nom de la JProperty</param>
        /// <returns></returns>
        public static JToken Renommer(this JToken cible_, string nouveauNom_)
        {
            if (cible_ != null)
            {
                JProperty property = null;

                if (cible_.Parent != null)
                {
                    if (cible_.Type == JTokenType.Property)
                    { property = (JProperty)cible_; }
                    else if (cible_.Parent.Type == JTokenType.Property)
                    { property = (JProperty)cible_.Parent; }
                }
                if (property != null)
                {
                    JToken val = property.Value;
                    property.Value = null;
                    JProperty nouvellePropriete = new JProperty(nouveauNom_, val);
                    property.Replace(nouvellePropriete);
                    cible_ = cible_.Type == JTokenType.Property ? nouvellePropriete : val;
                }
            }
            return cible_;
        }

        public static Vector3 JArray_Vector3(JArray tab_)
        {
            Vector3 conversion = Vector3.Zero;
            if (tab_.Count == 3)
            {
                conversion.X = tab_[0].Value<float>();
                conversion.Y = tab_[1].Value<float>();
                conversion.Z = tab_[2].Value<float>();
            }
            return conversion;
        }
    }
}
