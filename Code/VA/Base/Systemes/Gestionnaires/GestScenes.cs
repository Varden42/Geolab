namespace VA.Base.Systèmes.Gestionnaires;
public class GestScenes
{
    // Crée l'élément unique à l'initialisation
    private static GestScenes Singleton = new();
    private static readonly object Cadenas = new object();
    public static GestScenes Instance => Singleton;

    // Explicit static constructor to tell C# compiler
    // not to mark type as beforefieldinit
    static GestScenes()
    {
    }

    private GestScenes()
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