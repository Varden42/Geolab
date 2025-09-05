using System.Collections.Generic;
using Godot;

namespace VA.Base.GUI.Outils.Barre;

// TODO: Une barre qui peut être placé sur les bords de l'écran, dans laquelle ont peut placer des outils ou palettes
// Elle aura une épaisseur définie et redimensionnera ses enfants en fonction
// Ceux qui déborderont de la largeur de la barre seront ajouter sur un second niveau, doublant l'épaisseur
// une barre pouvant être placé à l'horizontale ou verticale, devra toujours contenir des éléments pouvant se dimensionner dans les deux sens, donc pas de textes ou champs de saisie.

// /// <summary>
// /// Définit un élément pouvant être placer dans une barre
// /// </summary>
// public interface IPlacableBarreMultiDir
// {
//     
// }

public partial class BarreMultiDir: PanelContainer
{
    public const float TAILLE_ELEMENT = 20f;
    public enum EnumBord { Haut, Bas, Gauche, Droite }

    private EnumBord Bord;
    private FlowContainer Lignes;
    private List<IElement> Elements;
    private int Zoom_;
    
    public int Zoom { get => Zoom_; set => ChangerZoom(value); }

    private void Init(EnumBord bord_ = EnumBord.Haut)
    {
        Bord = bord_;
        
        ClipContents = true;
        
        FlowContainer Lignes = new();
        Lignes.Name = "Lignes";
        Lignes.ClipContents = true;
        Lignes.AddThemeConstantOverride("h_separation", 1);
        Lignes.AddThemeConstantOverride("v_separation", 1);
        AddChild(Lignes);
        
        ChangerBord(Bord);
        
        Resized += MajTailleActions;
    }
    
    public BarreMultiDir()
    {
        Init();
    }

    public BarreMultiDir(EnumBord bord_)
    {
        Init(bord_);
    }

    /// <summary>
    /// Modifie la barre en fonction du bord sur lequel elle se trouve.
    /// </summary>
    /// <param name="bord_"></param>
    private void ChangerBord(EnumBord bord_)
    {
        // reconfigurer les controles en fonction du bord choisit
        switch (Bord)
        {
            case EnumBord.Haut:
                AnchorsPreset = (int)LayoutPreset.TopWide;
                Lignes.Vertical = false;
                break;
            case EnumBord.Bas:
                AnchorsPreset = (int)LayoutPreset.BottomWide;
                Lignes.Vertical = false;
                break;
            case EnumBord.Gauche:
                AnchorsPreset = (int)LayoutPreset.LeftWide;
                Lignes.Vertical = true;
                break;
            case EnumBord.Droite:
                AnchorsPreset = (int)LayoutPreset.RightWide;
                Lignes.Vertical = true;
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Calcule La taille des actions en fonction de la largeur/hauteur de la barre de manière à ce qu'ils prennent toute la longueur en gardant un ratio correct
    /// Les actions devront toujours se rapprocher d'une taille de 20x20 multiplier par le ratio de zoom
    /// </summary>
    private void MajTailleActions()
    {
        float tailleEléments = TAILLE_ELEMENT * Zoom_;
        float longueurBarre = Bord == EnumBord.Bas || Bord == EnumBord.Haut ? GetViewportRect().Size.X : GetViewportRect().Size.Y;
        tailleEléments = longueurBarre / Mathf.Round(longueurBarre / tailleEléments);
        foreach (IElement élément in Elements)
        { élément.Redimensionner(tailleEléments); }
    }
    
    
    private void ChangerZoom(int zoom_)
    {
        // recalculer la taille des actions et les redimensionner
        MajTailleActions();
    }

    public void AjouterElement(IElement element_, int index_ = -1)
    {
        Elements.Add(element_);
        Lignes.AddChild(element_ as Control);
        if (index_ >= 0 && index_ < Elements.Count)
        { Lignes.MoveChild(element_ as Control, index_); }
    }

    public bool RetraitElement(IElement element_)
    {
        IElement element = Elements.Find(e => e == element_);
        bool réussite = Elements.Remove(element_);
        (element as Control)?.QueueFree();
        return réussite;
    }
}