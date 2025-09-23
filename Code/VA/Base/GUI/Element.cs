using System.Collections.Generic;
using System.Linq;
using Godot;

namespace VA.Base.GUI;

/// <summary>
/// classe de base de tout les control personnalisés
/// </summary>
public abstract partial class Element: Control
{
    public abstract void Redimensionner(float taille_);

    public abstract void Réorienter(bool vertical_ = true);
}

/// <summary>
/// Un groupe contient plusieurs éléments séparés par une barre verticale
/// </summary>
public partial class Groupe : Conteneur
{
    private List<Element> Elements;
    private BoxContainer Conteneur;

    public override int NombreElements => Elements.Count;

    public Groupe()
    { Elements = new(); }

    public Groupe(IEnumerable<Element> elements_)
    {
        foreach (Element element in elements_.ToList())
        {
            AddChild(element as Control);
            Elements.Add(element);
        }
    }


    public void AjouterElement(Element element_, int index_ = -1)
    {
        if (Noeuds.Utiles.AjouterNode(this, element_ as Control, index_))
        { Elements.Insert(index_, element_); }
        else
        { Elements.Add(element_); }
    }
    
    public void AjouterElements(IEnumerable<Element> elements_, int index_ = -1)
    {
        index_ = index_ < 0 || index_ >= GetChildCount() ? GetChildCount() : index_;
        foreach (Element element in elements_.ToList())
        {
            Noeuds.Utiles.AjouterNode(this, element as Control, index_);
            Elements.Insert(index_, element);
            ++index_;
        }
    }

    public bool RetraitElement(Element element_)
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
    
    public override void Redimensionner(float taille_)
    {
        // TODO: Calculer la taille des éléments en fonctions de la nouvelle taille du groupe.
    }

    public void RedimensionnerElements(float taille_)
    {
        foreach (Element element in Elements)
        { element.Redimensionner(taille_); }
    }

    public override void Réorienter(bool vertical_ = true)
    { Vertical = vertical_; }
}