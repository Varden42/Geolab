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
    enum EnumBord { Haut, Bas, Gauche, Droite }

    private EnumBord Bord;
    private FlowContainer Lignes;
    private List<IActionBarre> Actions;
    private int Zoom_;
    
    public int Zoom { get => Zoom_; set => ChangerZoom(value); }

    private void Init()
    {
        Bord = EnumBord.Haut;
        
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
        float tailleActions = 20 * Zoom_;
        // Récupérer la taille de l'écran et trouver une taille d'actions qui sois le plus proche de 20 tout en permettant de toutes les faire rentrer sans laisser de trou ou en avoir qui dépasse.
    }
    
    
    private void ChangerZoom(int zoom_)
    {
        // recalculer la taille des actions et les redimensionner
        MajTailleActions();
    }
}