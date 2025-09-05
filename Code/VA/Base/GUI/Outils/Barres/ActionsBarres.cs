using Godot;

namespace VA.Base.GUI.Outils.Barre;

// Fichier pour définir les différentes actions placables dans les barres

public delegate void Action(bool état_);
public delegate void ActionOn();
public delegate void ActionOff();

public interface IAction: IElement
{
    public void Clic(InputEventMouse event_)
    {
        GD.Print($"Clic à la position {event_.GetPosition()}");
    }
}

// public partial class ActionBase : AspectRatioContainer, IActionBarre
// {
//     public ActionBase()
//     {
//         
//     }
// }

public partial class ActionOnOff : AspectRatioContainer, IAction
{
    private TextureButton Bouton;
    private ActionOn On;
    private ActionOff Off;
    
    public string Nom => Bouton.Name.ToString();
    public bool Etat => Bouton.ButtonPressed;
    
    public ActionOnOff(Vector2 taille_, string nom_, Texture2D textureOn_, Texture2D textureOff_, ActionOn actionOn_, ActionOff actionOff_)
    {
        CustomMinimumSize = taille_;
        
        Bouton = new TextureButton();
        Bouton.Name = nom_;
        Bouton.IgnoreTextureSize = true;
        Bouton.StretchMode = TextureButton.StretchModeEnum.Scale;
        Bouton.ToggleMode = true;
        Bouton.TextureNormal = textureOff_;
        Bouton.TexturePressed = textureOn_;
        Bouton.Toggled += BoutonCliqué;
        
        On = actionOn_;
        Off = actionOff_;
        
        AddChild(Bouton); 
    }
    
    public ActionOnOff() : this(new Vector2(20, 20), "<ActionOnOff>", null, null, null, null) { }

    private void BoutonCliqué(bool état_)
    {
        switch (état_)
        {
            case true: On(); break;
            case false: Off(); break;
        }
    }

    public void Redimensionner(float taille_)
    { CustomMinimumSize = new(taille_, taille_); }

    public void Réorienter(bool vertical_ = true)
    {    }
}