using System.Collections.Generic;
using System.Linq;
using Godot;

namespace VA.Base.GUI.Outils.Barre;

// public class Barre
// {
//     
// }

public interface IElement
{
    public void Redimensionner(float taille_);

    public void Réorienter(bool vertical_ = true);
}

public partial class Groupe : BoxContainer, IElement
{
    private List<IElement> Elements;

    public Groupe()
    { Elements = new(); }

    public Groupe(IEnumerable<IElement> elements_)
    {
        foreach (IElement element in elements_.ToList())
        {
            AddChild(element as Control);
            Elements.Add(element);
        }
    }
    
    public void Redimensionner(float taille_)
    {
        throw new System.NotImplementedException();
    }

    public void Réorienter(bool vertical_ = true)
    { Vertical = vertical_; }
}