using Godot;

using VA.Base.Debug;
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

    private int Test;
    private float Test2;
    private string Test3;
    private Vector3 Test4;
    private int Test5;

    public string MajTest()
    {
        Test++;
        return Test.ToString();
    }

    public string MajTest2()
    {
        Test2++;
        return Test2.ToString();
    }

    public string MajTest3()
    {
        Test3 = $"La Valeur du premier test est [{Test}]";
        return Test3;
    }

    public string MajTest4()
    {
        Test4 *= 2;
        return Test4.ToString();
    }
    
    public string MajTest5()
    {
        Test5++;
        return Test5.ToString();
    }

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
        
        Test = 0;
        Test2 = 0f;
        Test3 = "ceci est un test";
        Test4 = new(1f, 2f, 3f);
        Test5 = 0;
        
        PanDeb.AjoutLigne("Test", MajTest, 0.5D);
        PanDeb.AjoutLigne("Test2", MajTest2, 1D, "FLOAT");
        PanDeb.AjoutLigne("Test3", MajTest3, 1.5D, "string");
        PanDeb.AjoutLigne("Test4", MajTest4, 2D, "Vector3");
        PanDeb.AjoutLigne("Test5", MajTest5, 0D);
        
        AddChild(PanDeb);

        // ScèneActive = new();
        // ScèneActive.Name = "Scène Active";
        // AddSibling(ScèneActive);
        
        // TODO: afficher le Menu
        
        // Tests

        TestAffichageTexte2D();
    }

    public override void _Process(double delta)
    {
        ContrôleurMaj.Instance.Maj(delta);
    }

    private void TestAffichageTexte2D()
    {
        // AffichageTexte2D texte = new("Test Test Test Test Test Test Test Test Test Test Test Test Test " +
        //                              "Test Test Test Test Test Test Test Test Test Test Test Test Test " +
        //                              "Test Test Test Test Test Test Test Test Test Test Test Test Test " +
        //                              "Test Test Test Test Test Test Test Test Test Test Test Test Test " +
        //                              "Test Test Test Test Test Test Test Test Test Test Test Test Test " +
        //                              "Test Test Test Test Test Test Test Test Test Test Test Test Test " +
        //                              "Test Test Test Test Test Test Test Test Test Test Test Test Test ", Colors.Blue, new(300, 200));
        
        AffichageTexte2D texte = new("Test Test Test Test Test Test Test Test Test Test Test Test Test " +
                                     "Test Test Test Test Test Test Test Test Test Test Test Test Test " +
                                     "Test Test Test Test Test Test Test Test Test Test Test Test Test " +
                                     "Test Test Test Test Test Test Test Test Test Test Test Test Test " +
                                     "Test Test Test Test Test Test Test Test Test Test Test Test Test " +
                                     "Test Test Test Test Test Test Test Test Test Test Test Test Test " +
                                     "Test Test Test Test Test Test Test Test Test Test Test Test Test ", Colors.Blue);
        AddChild(texte);
    }


    // TODO: gérer le changement de scène ici (voir TankArena)
    
    // TODO: créer un système(singleton) qui gère les durées de vie des ressources et déclenche un event lorsqu'une ressource doit mourir afin que le gestionnaire qui la possède la supprime
    // TODO: mettre à jour le contrôleur de maj via la boucle _process au début, puis via son propre Thread par la suite
}
