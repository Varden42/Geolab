using System;
using System.Collections.Generic;
using Godot;
using VA.Base.Utiles;

namespace VA.Base.GUI.Outils;

/// <summary>
/// Une boite contenant des outils et pouvant être placé de manière libre dans un Rect.
/// Les groupe d'outils seront placés en colonne verticale et les outils en grille horizontale
/// </summary>
public partial class BoiteOutils : Prefab.ControlPrefab, Elements.IElement
{
    public void Redimensionner(float taille_)
    {
        throw new NotImplementedException();
    }

    public void Réorienter(bool vertical_ = true)
    {
        throw new NotImplementedException();
    }

    protected override void Construire()
    {
        throw new NotImplementedException();
    }
}