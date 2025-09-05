using Geolab.Code.Tests;
using Godot;

using VA.Base.Debug;
using VA.Base.GUI.Outils.Barre;
using VA.Base.Systèmes.Gestionnaires;

namespace VA.Base.Systèmes;

/// <summary>
/// Singleton qui s'assure que tout est bien chargé/configurer à tout moment
/// </summary>
public partial class Majordome : Node
{
    // définition du Singleton
    private static Majordome Singleton = null;
    private static readonly object Cadenas = new object();
    public static Majordome Instance => Singleton;

    private Node ScèneActive;
    private PanneauDebug PanDeb;

    #region Test

    

    #endregion
    
    public void QuitterApplication()
    { GetTree().Quit(); }
    

    public override void _Ready()
    {
        lock (Cadenas)
        {
            if (Singleton == null)
            { Singleton = this; }
        }

        CallDeferred("Init");
    }

    private void Init()
    {
        Name = "Majordome";
        
        Biblio.Démarrer();
        Journal.Démarrer();
        ContrôleurMaj.Démarrer();
        
        PanDeb = new();
        PanDeb.Name = "PanneauDebug";
        AddChild(PanDeb);

        ScèneActive = GetTree().CurrentScene;
        
        ScèneActive.AddChild(new TestsBarres());
    }

    public override void _Process(double delta)
    {
        ContrôleurMaj.Instance.Maj(delta);
    }

    


    // TODO: gérer le changement de scène ici (voir TankArena)
    
    // TODO: créer un système(singleton) qui gère les durées de vie des ressources et déclenche un event lorsqu'une ressource doit mourir afin que le gestionnaire qui la possède la supprime
    // TODO: mettre à jour le contrôleur de maj via la boucle _process au début, puis via son propre Thread par la suite
}
