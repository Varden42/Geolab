namespace VA.Base.Systèmes.Gestionnaires;

public class GestModèles3D
{
    // Crée l'élément unique à l'initialisation
    private static GestModèles3D Singleton = new();
    private static readonly object Cadenas = new object();
    public static GestModèles3D Instance => Singleton;

    // Explicit static constructor to tell C# compiler
    // not to mark type as beforefieldinit
    static GestModèles3D()
    {
    }

    private GestModèles3D()
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