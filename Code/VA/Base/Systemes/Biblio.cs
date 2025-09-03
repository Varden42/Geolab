using VA.Base.Systèmes.Gestionnaires;

namespace VA.Base.Systèmes;

/// <summary>
/// Class static simplifiant l'accès aux gestionnaires des différentes ressources.
/// </summary>
public static class Biblio
{
    public static GestTextures Textures => GestTextures.Instance;
    public static GestModèles3D Modèles3D => GestModèles3D.Instance;
    public static GestScenes Scènes => GestScenes.Instance;
    public static GestPlans Plans => GestPlans.Instance;
    public static Index Index => Index.Instance;
    
    static Biblio()
    {    }

    /// <summary>
    /// Méthode servant à déclencher l'appel du constructeur static
    /// </summary>
    public static void Démarrer()
    {    }
}