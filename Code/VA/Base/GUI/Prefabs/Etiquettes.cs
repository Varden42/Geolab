using Godot;

namespace VA.Base.GUI;

/// <summary>
/// Permet de construire rapidement des templates couramment utilisés
/// </summary>
public static partial class Templates
{
    /// <summary>
    /// Définit les paramètres de base d'une Etiquette
    /// </summary>
    /// <param name="label_">L'Etiquette à configurer</param>
    /// <param name="texte_">Le texte affiché par l'étiquette</param>
    /// <param name="nom_">Le nom de l'Etiquette</param>
    public static Label LabelSimple(string texte_ = "<vide>", string nom_ = "Etiquette")
    {
        Label etiquette = new();
        etiquette.Text = texte_;
        etiquette.Name = nom_;
        return etiquette;       
    }

    /// <summary>
    /// Template d'Etiquette basique
    /// </summary>
    /// <param name="texte_">Le texte affiché par l'étiquette</param>
    /// <param name="nom_">Le nom de l'Etiquette</param>
    /// <param name="style_">Le style de l'UI</param>
    /// <returns>L'étiquette ainsi construite</returns>
    public static Label LabelStylisé(string texte_, string nom_ = "Etiquette", StyleBox style_ = null)
    {
        Label etiquette = LabelSimple(texte_, nom_);
        etiquette.AddThemeStyleboxOverride("normal", style_);
        return etiquette;
    }

    /// <summary>
    /// Template d'Etiquette positionné au centre gauche
    /// </summary>
    /// <param name="texte_">Le texte affiché par l'étiquette</param>
    /// <param name="nom_">Le nom de l'Etiquette</param>
    /// <param name="style_">Le style de l'UI</param>
    /// <returns>L'étiquette ainsi construite</returns>
    public static Label LabelStyliséGD(string texte_, string nom_ = "Etiquette", StyleBox style_ = null)
    {
        Label etiquette = LabelStylisé(texte_, nom_, style_);
        etiquette.SizeFlagsHorizontal = Control.SizeFlags.ShrinkBegin;
        return etiquette;
    }
}

/// <summary>
/// Une classe héritant de Godot.Label pour simplifier son usage
/// </summary>
public partial class Etiquette : Label
{
    public Etiquette(): this("<vide>", "Etiquette") {    }
    
    /// <summary>
    /// Constructeur le plus basique
    /// </summary>
    /// <param name="texte_">Le texte affiché par l'étiquette</param>
    /// <param name="nom_">Le nom de l'Etiquette</param>
    public Etiquette(string texte_, string nom_ = "Etiquette")
    {
        Text = texte_;
        Name = nom_;
    }
    
    /// <summary>
    /// Construit une Etiquette avec un style précis
    /// </summary>
    /// <param name="texte_">Le texte affiché par l'étiquette</param>
    /// <param name="nom_">Le nom de l'Etiquette</param>
    /// <param name="style_">Le style de l'UI</param>
    public Etiquette(string texte_, string nom_ = "Etiquette", StyleBox style_ = null) : this(texte_, nom_)
    { AddThemeStyleboxOverride("normal", style_); }

    /// <summary>
    /// Définit l'alignement d'une Etiquette
    /// </summary>
    /// <param name="alignementH_">alignement horizontal</param>
    /// <param name="alignementV_">alignement vertical</param>
    public void Alignement(AlignementHorizontal alignementH_, AlignementVertical alignementV_)
    {
        switch (alignementH_)
        {
            case AlignementHorizontal.Gauche:
                SizeFlagsHorizontal = Control.SizeFlags.ShrinkBegin;
                break;
            case AlignementHorizontal.Centre:
                SizeFlagsHorizontal = Control.SizeFlags.ShrinkCenter;
                break;
            case AlignementHorizontal.Droite:
                SizeFlagsHorizontal = Control.SizeFlags.ShrinkEnd;
                break;
            default:
                break;
        }
        
        switch (alignementV_)
        {
            case AlignementVertical.Haut:
                SizeFlagsVertical = Control.SizeFlags.ShrinkBegin;
                break;
            case AlignementVertical.Centre:
                SizeFlagsVertical = Control.SizeFlags.ShrinkCenter;
                break;
            case AlignementVertical.Bas:
                SizeFlagsVertical = Control.SizeFlags.ShrinkEnd;
                break;
            default:
                break;
        }
    }

}