using System;
using System.Collections.Generic;
using System.IO;
using Godot;

namespace VA.Base.Fichiers;

public static class Génériques
{
    /// <summary>
    /// Récupère les chemins de tous les fichiers présents dans un dossier
    /// </summary>
    /// <param name="chemin_">Le chemin du dossier à parcourir</param>
    /// <returns></returns>
    public static string[] RécupListeFichiers(string chemin_, string pattern_ = "*")
    {
        if (Directory.Exists(chemin_))
        { return Directory.GetFiles(chemin_, pattern_); }
        return Array.Empty<string>();
    }
}