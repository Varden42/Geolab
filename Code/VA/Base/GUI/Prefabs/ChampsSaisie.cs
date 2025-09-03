using System;
using Godot;

namespace VA.Base.GUI;

/// <summary>
/// Permet de construire rapidement des templates couramment utilisés
/// </summary>
public static partial class Templates
{
    /// <summary>
    /// Un champ texte modifiable précédé d'un nom
    /// </summary>
    /// <param name="nom_">Le nom du champ de saisie</param>
    /// <param name="texteDéfaut_">Le texte qui apparait par défaut.</param>
    /// <returns></returns>
    public static HBoxContainer ChampTexteSimple(string nom_ = "", string texteDéfaut_ = "")
    {
        HBoxContainer champTexte = new();
        {
            champTexte.Name = nom_;
            champTexte.SizeFlagsVertical = Control.SizeFlags.ShrinkBegin;
            champTexte.SizeFlagsVertical = Control.SizeFlags.Expand;

            Label nomVariable = new();
            {
                nomVariable.Name = "NomVariable";
                nomVariable.Text = nom_;
            }
            champTexte.AddChild(nomVariable);

            LineEdit valeur = new();
            {
                valeur.Name = "ChampTexte";
                valeur.PlaceholderText = texteDéfaut_;
                valeur.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
                //valeur.TextChanged += () => variable_ = valeur.Text;
            }
            champTexte.AddChild(valeur);
        }
        return champTexte;
    }
    
    public static HBoxContainer ChampTexteRéactif(LineEdit.TextChangedEventHandler eventHandler_, string nom_ = "", string texteDéfaut_ = "")
    {
        HBoxContainer temp = ChampTexteSimple(nom_, texteDéfaut_);
        temp.GetNode<LineEdit>("Champ texte").TextChanged += eventHandler_;
        return temp;
    }
    
    public static HBoxContainer ChampNumérique(string nom_ = "", double valeurParDéfaut_ = 0D)
    {
        HBoxContainer champNumérique = new();
        {
            champNumérique.Name = nom_;
            champNumérique.AddThemeConstantOverride("separation", 10);
            
            Label label = new Label();
            {
                label.Name = "NomValeur";
                label.Text = nom_;
                label.CustomMinimumSize = new(10, 20);
            }
            champNumérique.AddChild(label);
            
            SpinBox valeur = new();
            {
                valeur.Name = $"Valeur";
                valeur.CustomMinimumSize = new(10, 20);
                valeur.CustomArrowStep = 1;
                valeur.SelectAllOnFocus = true;
                valeur.Value = valeurParDéfaut_;
            }
            champNumérique.AddChild(valeur);
        }
        return champNumérique;
    }
}
public partial class ChampsTexte: HBoxContainer
{
    
}