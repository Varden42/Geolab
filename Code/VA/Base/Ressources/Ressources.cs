using System;
using System.Linq;
using Godot;

namespace VA.Base.Ressources;

public static class Ressources
{
    /// <summary>
    /// Récupère la liste des noms de resources à l'emplacement fournit
    /// </summary>
    /// <param name="cheminDossier_">Le dossier où se trouve les ressources à lister</param>
    /// <returns></returns>
    public static string[] RécupListeRessources(string cheminDossier_)
    {
        DirAccess dossier = DirAccess.Open(cheminDossier_);
        if (dossier != null)
        {
            //GD.Print($"Création de la liste des ressources à l'emplacement [{cheminDossier_}]");
            string[] liste = dossier.GetFiles().Where(r_ => r_.EndsWith(".import")).ToArray();
            for (int f = 0; f < liste.Length; ++f)
            { liste[f] = liste[f].Split('.')[0]; }
            return liste;
        }
        return Array.Empty<string>();
    }
}