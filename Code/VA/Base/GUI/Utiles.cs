using Godot;

namespace VA.Base.GUI;

public static class Utiles
{
    /// <summary>
    /// Créer un Node ColorRect et le définit comme parent du Control fournit afin de bloquer tous clics de souris en dehors du Control voulu
    /// </summary>
    /// <param name="noeud_">Le Control à isoler</param>
    /// <param name="r_">La valeur R de la couleur du fond</param>
    /// <param name="g_">La valeur G de la couleur du fond</param>
    /// <param name="b_">La valeur B de la couleur du fond</param>
    /// <param name="a_">La valeur A de la couleur du fond</param>
    public static void BloquerFocus(Control noeud_, float r_ = 0.4f, float g_ = 0.4f, float b_ = 0.4f, float a_ = 0.4f)
    {
        Node parent = noeud_.GetParent();
        if (parent != null)
        {
            ColorRect bloqueFocus = new();
            bloqueFocus.Name = "__FocusBloqué__";
            bloqueFocus.SetAnchorsPreset(Control.LayoutPreset.FullRect);
            bloqueFocus.Color = new Color(r_, g_, b_, a_);
            
            parent.RemoveChild(noeud_);
            bloqueFocus.AddChild(noeud_);
            parent.AddChild(bloqueFocus);
            
            // TODO: s'assurer que le ColorRect mis à la place se trouve au même emplacement dans la hiérarchie
        }
    }

    /// <summary>
    /// Retire le blocage créé par la fonction "BloquerFocus" si le parent de fenêtre_ est un ColorRect avec un parent valide
    /// </summary>
    /// <param name="fenêtre_">Le Control à libérer</param>
    public static void DébloquerFocus(Control fenêtre_)
    {
        Node parent = fenêtre_.GetParent();
        if (parent.Name == "__FocusBloqué__")
        {
            Noeuds.Utiles.EchangeParents(fenêtre_, parent.GetParent());
            parent.QueueFree();
        }
    }


    public static void CentrerControl(Control control_)
    {
        control_.SetAnchorsPreset(Control.LayoutPreset.Center);
        control_.GrowHorizontal = Control.GrowDirection.Both;
        control_.GrowVertical = Control.GrowDirection.Both;
    }
}