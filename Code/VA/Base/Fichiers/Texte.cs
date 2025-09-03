using System.IO;
using Godot;

namespace VA.Base.Fichiers
{
    public static class Texte
    {
        /// <summary>
        /// Charge dans un String le contenu d'un fichier situé à l'emplacement "chemin_"
        /// </summary>
        /// <param name="chemin_">L'emplacement du fichier à charger.</param>
        /// <returns>Le String contenant le fichier.</returns>
        public static string ChargerFichier(string chemin_)
        {
            chemin_ ??= "NULL";
            if (File.Exists(chemin_))
            {
                StreamReader lecteur = new StreamReader(chemin_);
                string texte = "";

                while (!lecteur.EndOfStream)
                { texte += lecteur.ReadLine(); }

                lecteur.Close();

                return texte;
            }
            else
            {
                GD.PrintErr($"<color=red>Error: </color>Impossible de charger le fichier à l'emplacement {chemin_} !!");

                return "";
            }
        }
    }
}