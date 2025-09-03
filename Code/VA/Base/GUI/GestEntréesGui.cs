using System.Collections.Generic;
using Godot;

namespace VA.Base.GUI;

/// <summary>
/// une classe qui permet d'assigner des actions à des event de souris
/// à revoir et continuer si l'idée est valide
/// </summary>
public class GestEntréesGui
{
    public delegate void ActionClicSouris(InputEventMouseButton event_);
    private Dictionary<MouseButton, ActionClicSouris> ActionsClicSouris = new();
    
    public delegate void ActionRelacherClicSouris(InputEventMouseButton event_);
    private Dictionary<MouseButton, ActionRelacherClicSouris> ActionsRelacherClicSouris = new();
    
    public delegate void ActionMouvementSouris(InputEventMouseButton event_);
    private Dictionary<MouseButton, ActionMouvementSouris> ActionsMouvementSouris = new();

    public GestEntréesGui()
    {
        ActionsClicSouris = new();
        ActionsRelacherClicSouris = new();
        ActionsMouvementSouris = new();
    }

    public void AjouterClicSouris(MouseButton bouton_, ActionClicSouris action_)
    { ActionsClicSouris.Add(bouton_, action_); }

    public void Entrées(InputEvent event_)
    {
        if (event_ is InputEventMouseButton mouseButtonEvent)
        {
            if (mouseButtonEvent.Pressed)
            {
                if (ActionsClicSouris.TryGetValue(mouseButtonEvent.ButtonIndex, out var action))
                {
                    action.Invoke(mouseButtonEvent);
                }
            }
            else
            {
                if (ActionsRelacherClicSouris.TryGetValue(mouseButtonEvent.ButtonIndex, out var action))
                {
                    action.Invoke(mouseButtonEvent);
                }
            }
        }
        else if (event_ is InputEventMouseMotion mouseMotionEvent)
        {
            // if (ActionsMouvementSouris.TryGetValue(mouseMotionEvent.ButtonMask, out var action))
            // {
            //     action.Invoke(mouseMotionEvent);
            // }
        }
    }
}