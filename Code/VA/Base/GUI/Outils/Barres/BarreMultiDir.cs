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
    enum Bord { Haut, Bas, Gauche, Droite }

    private List<IActionBarre> Actions;

    private void Init()
    {
        
    }
    
    public BarreMultiDir()
    {
        
    }

    private void ChangerBord(Bord bord_)
    {
        // reconfigurer les controles en fonction du bord choisit
    }
}