using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using VA.Base.Utiles;

namespace VA.Base.GUI.Outils.Elements;

public interface IElement
{
    public void Redimensionner(float taille_);

    public void Réorienter(bool vertical_ = true);
}

/// <summary>
/// Un groupe contient plusieurs éléments séparés par une barre verticale
/// </summary>
public partial class Groupe : Prefab.ControlPrefab, IElement
{
    /// <summary>
    /// Le compteur permettant de donner un identifiant unique à chaque instances
    /// </summary>
    private static CompteurInt IdsMatricules = new();

    private BoxContainer Conteneur;

    private List<IElement> Elements;
    
    public int NombreElements => Elements.Count;

    public Groupe(string nom_ = "")
    {
        Name = nom_ == "" ? Base.Utiles.String.GénérerMatricule("_", '#', IdsMatricules.Ajouter) : nom_;
        Elements = new();
    }

    public Groupe(IEnumerable<IElement> elements_, string nom_ = "")
    {
        foreach (IElement element in elements_.ToList())
        {
            AddChild(element as Control);
            Elements.Add(element);
        }
    }

    protected override void Construire()
    {
        Conteneur = new();
        {
            Conteneur.Vertical = false;
            // TODO: Finir le constructeur
        }
    }

    public void AjouterElement(IElement element_, int index_ = -1)
    {
        if (Noeuds.Utiles.AjouterNode(this, element_ as Control, index_))
        { Elements.Insert(index_, element_); }
        else
        { Elements.Add(element_); }
    }
    
    public void AjouterElements(IEnumerable<IElement> elements_, int index_ = -1)
    {
        index_ = index_ < 0 || index_ >= GetChildCount() ? GetChildCount() : index_;
        foreach (IElement element in elements_.ToList())
        {
            Noeuds.Utiles.AjouterNode(this, element as Control, index_);
            Elements.Insert(index_, element);
            ++index_;
        }
    }

    public bool RetraitElement(IElement element_)
    {
        int index = Elements.IndexOf(element_);
        if (index >= 0)
        {
            GetChild(index).QueueFree();
            Elements.RemoveAt(index);
            return true;
        }
        return false;
    }
    
    public void Redimensionner(float taille_)
    {
        // TODO: Calculer la taille des éléments en fonctions de la nouvelle taille du groupe.
    }

    public void RedimensionnerElements(float taille_)
    {
        foreach (IElement element in Elements)
        { element.Redimensionner(taille_); }
    }

    public void Réorienter(bool vertical_ = true)
    { Conteneur.Vertical = vertical_; }
}