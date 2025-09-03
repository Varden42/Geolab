using System.Collections.Generic;
using Godot;

namespace VA.Base.Systèmes.Gestionnaires;

public class GestStyles
{
    public const string DossierStyleBox = "res://VA/Styles/";
    
    // Crée l'élément unique à l'initialisation
    private static GestStyles Singleton = new();
    private static readonly object Cadenas = new object();
    public static GestStyles Instance => Singleton;
    
    private Dictionary<string, StyleBox> Styles;

    // Explicit static constructor to tell C# compiler
    // not to mark type as beforefieldinit
    static GestStyles()
    {    }

    private GestStyles()
    {
        // Initialiser les variables de l'instance ici
    }

    public void Méthode()
    {
        // les méthodes du Singleton doivent se lock pour s'assurer de ne pas être appelées simultanément à plusieurs emplacements.
        lock (Cadenas)
        {
            // code ici ...
        }
    }
}