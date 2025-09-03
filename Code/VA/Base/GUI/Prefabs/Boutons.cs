using System;
using Godot;

namespace VA.Base.GUI;

/// <summary>
/// Permet de construire rapidement des templates couramment utilisés
/// </summary>
public static partial class Templates
{
    /// <summary>
    /// Définit les paramètres de base d'un Bouton
    /// </summary>
    /// <param name="bouton_">Le BOuton à configurer</param>
    /// <param name="action_">L'action suivant la pression du bouton</param>
    /// <param name="nom_">Le nom du bouton</param>
    public static Button BoutonSimple(Action action_, string nom_ = "Bouton")
    {
        Button bouton = new();
        bouton.Pressed += action_;
        bouton.Name = nom_;
        bouton.Text = nom_;
        return bouton;
    }

    /// <summary>
    /// Construit une template basique de bouton
    /// </summary>
    /// <param name="action_">L'action suivant la pression du bouton</param>
    /// <param name="nom_">Le nom du bouton</param>
    /// <param name="style_">Le style de l'UI</param>
    /// <returns>Le bouton ainsi construit</returns>
    public static Button Bouton(Action action_, string nom_ = "Bouton", StyleBox style_ = null)
    {
        Button bouton = BoutonSimple(action_, nom_);
        bouton.AddThemeStyleboxOverride("normal", style_);
        return bouton;
    }
}

/// <summary>
/// Une classe héritant de Godot.Button pour simplifier son usage
/// </summary>
public partial class Bouton : Button
{
    public Bouton(): this(null, "<vide>") {    }
    
    /// <summary>
    /// Constructeur le plus basique
    /// </summary>
    /// <param name="action_">L'action suivant la pression du bouton</param>
    /// <param name="nom_">Le nom du bouton</param>
    public Bouton(Action action_, string nom_ = "Bouton")
    {
        Pressed += action_;
        Name = nom_;
        Text = nom_;
    }
    
    /// <summary>
    /// Construit un bouton avec des couleurs customisées
    /// </summary>
    /// <param name="action_">L'action suivant la pression du bouton</param>
    /// <param name="nom_">Le nom du bouton</param>
    /// <param name="couleurBouton_">La couleur du bouton</param>
    /// <param name="couleurTexte_">La couleur du nom</param>
    public Bouton(Action action_, string nom_, Color couleurBouton_, Color couleurNom_): this(action_, nom_)
    {
        AddThemeColorOverride("font_color", couleurNom_);
            
        var styleBox = new StyleBoxFlat();
        styleBox.BgColor = couleurBouton_;
        AddThemeStyleboxOverride("normal", styleBox);
    }
}