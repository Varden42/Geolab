using Godot;

using System;
using System.Collections.Generic;

using VA.Base.Systèmes;
using Index = VA.Base.Systèmes.Gestionnaires.Index;

namespace VA.Base.GUI.Outils;

public partial class ListeOptions : Control
{
    private interface IOption
    {
        public void Synchroniser();
    }
    private partial class OptionNumérique : HBoxContainer, IOption
    {
        public Label Nom { get; private set; }
        public HBoxContainer Champs { get; private set; }
        
        private OptionNumérique(string nomOption_, int nombreDeChamps_, string[] nomsVariables_, double[] valeursParDéfaut_)
        {
            VérifierArgumentsChampsNumériques(nomsVariables_, valeursParDéfaut_, nombreDeChamps_);

            Name = $"Option_{nomOption_}";
            
            Nom = ListeOptions.CréerLabel(nomOption_);
            { Nom.Name = "Nom"; }
            AddChild(Nom);
            
            Champs = new();
            {
                Champs.Name = "Champs";
                Champs.SizeFlagsHorizontal = (SizeFlags)10; // SizeFlags.ShrinkEnd + SizeFlags.Expand
                Champs.AddThemeConstantOverride("separation", 10);
                for (int c = 0; c < nomsVariables_.Length; ++c)
                { Champs.AddChild(GUI.Templates.ChampNumérique(nomsVariables_[c], valeursParDéfaut_[c])); }
            }
            AddChild(Champs);
        }
        public OptionNumérique(string nomOption_, MajVariable1 fonctionMaj_, string[] nomsVariables_, double[] valeursParDéfaut_): this(nomOption_, 1, nomsVariables_, valeursParDéfaut_)
        {
            SpinBox valeur = Champs.GetChild<HBoxContainer>(0).GetChild<SpinBox>(1);
            valeur.ValueChanged += valeur_ => fonctionMaj_(valeur_);
            valeur.EmitSignal(SpinBox.SignalName.ValueChanged, valeur.Value);
        }
        public OptionNumérique(string nomOption_, MajVariable2 fonctionMaj_, string[] nomsVariables_, double[] valeursParDéfaut_): this(nomOption_, 2, nomsVariables_, valeursParDéfaut_)
        {
            List<SpinBox> valeurs = Noeuds.Utiles.TrouverNodesEnfant<SpinBox>(Champs);
            for (int c = 0; c < valeursParDéfaut_.Length; ++c)
            { valeurs[c].ValueChanged += (_ => fonctionMaj_(valeurs[0].Value, valeurs[1].Value)); }
            valeurs[0].EmitSignal(SpinBox.SignalName.ValueChanged, valeurs[0].Value);
        }
        public OptionNumérique(string nomOption_, MajVariable3 fonctionMaj_, string[] nomsVariables_, double[] valeursParDéfaut_): this(nomOption_, 3, nomsVariables_, valeursParDéfaut_)
        {
            List<SpinBox> valeurs = Noeuds.Utiles.TrouverNodesEnfant<SpinBox>(Champs);

            for (int c = 0; c < valeursParDéfaut_.Length; ++c)
            { valeurs[c].ValueChanged += (_ => fonctionMaj_(valeurs[0].Value, valeurs[1].Value, valeurs[2].Value)); }
            valeurs[0].EmitSignal(SpinBox.SignalName.ValueChanged, valeurs[0].Value);
        }
        public OptionNumérique(string nomOption_, MajVariable4 fonctionMaj_, string[] nomsVariables_, double[] valeursParDéfaut_): this(nomOption_, 4, nomsVariables_, valeursParDéfaut_)
        {
            List<SpinBox> valeurs = Noeuds.Utiles.TrouverNodesEnfant<SpinBox>(Champs);

            for (int c = 0; c < valeursParDéfaut_.Length; ++c)
            { valeurs[c].ValueChanged += (_ => fonctionMaj_(valeurs[0].Value, valeurs[1].Value, valeurs[2].Value, valeurs[3].Value)); }
            valeurs[0].EmitSignal(SpinBox.SignalName.ValueChanged, valeurs[0].Value);
        }
        
        private void VérifierArgumentsChampsNumériques(string[] nomsVariables_, double[] valeursParDéfaut_, int quantitéCible_)
        {
            if (nomsVariables_.Length != quantitéCible_ || nomsVariables_.Length != valeursParDéfaut_.Length)
            { throw new ArgumentException($"La quantité de variables n'est pas valide!"); }
        }
        
        // private HBoxContainer CréerChampNumérique(string nom_, double valeurParDéfaut_ = 0D)
        // {
        //     HBoxContainer champ = new();
        //     {
        //         champ.Name = $"Champ_{nom_}";
        //         champ.AddThemeConstantOverride("separation", 10);
        //     
        //         Label label = new Label();
        //         {
        //             label.Name = "NomValeur";
        //             label.Text = nom_;
        //             label.CustomMinimumSize = new(10, 20);
        //         }
        //         champ.AddChild(label);
        //     
        //         SpinBox valeur = new();
        //         {
        //             valeur.Name = $"Valeur";
        //             valeur.CustomMinimumSize = new(10, 20);
        //             valeur.CustomArrowStep = 1;
        //             valeur.SelectAllOnFocus = true;
        //             valeur.Value = valeurParDéfaut_;
        //         }
        //         champ.AddChild(valeur);
        //     }
        //     return champ;
        // }

        public void Synchroniser()
        {
            SpinBox valeur = Champs.GetChild<HBoxContainer>(0).GetChild<SpinBox>(1);
            valeur.EmitSignal(SpinBox.SignalName.ValueChanged, valeur.Value);
        }
    }

    private partial class OptionCouleur : HBoxContainer, IOption
    {
        public Label Nom { get; private set; }
        public ColorPickerButton Palette { get; private set; }
        
        public OptionCouleur(string nomOption_, MajVariableCouleur fonctionMaj_, Color couleurParDéfaut_)
        {
            Name = $"Option_{nomOption_}";
            
            Nom = ListeOptions.CréerLabel(nomOption_);
            { Nom.Name = "Nom"; }
            AddChild(Nom);
            
            Palette = new();
            {
                Palette.Name = "Palette";
                Palette.CustomMinimumSize = new(150f, 20f);
                Palette.SizeFlagsHorizontal = (SizeFlags)10; // SizeFlags.ShrinkEnd + SizeFlags.Expand
                Palette.Color = couleurParDéfaut_;
                Palette.ColorChanged += (valeur_ => fonctionMaj_(valeur_));
                Palette.EmitSignal(ColorPickerButton.SignalName.ColorChanged, Palette.Color);
            }
            AddChild(Palette);
        }

        public void Synchroniser()
        { Palette.EmitSignal(ColorPickerButton.SignalName.ColorChanged, Palette.Color); }
    }
    
    
    
    public delegate void MajVariable1(double nouvelleValeur_);
    public delegate void MajVariable2(double nouvelleValeurX_, double nouvelleValeurY_);
    public delegate void MajVariable3(double nouvelleValeurX_, double nouvelleValeurY_, double nouvelleValeurZ_);
    public delegate void MajVariable4(double nouvelleValeurX_, double nouvelleValeurY_, double nouvelleValeurZ_, double nouvelleValeurW_);
    
    public delegate void MajVariableCouleur(Color nouvelleValeur_);
    
    private VBoxContainer Liste;
    private List<IOption> Options;

    public ListeOptions()
    {
        Options = new();
        PanelContainer panneau = new();
        {
            panneau.Name = "Panneau";
            panneau.AddThemeStyleboxOverride("panel", ResourceLoader.Load<StyleBoxFlat>(Biblio.Index.RecupIndex(Index.CatégorieIndexs.Style, "StyleGuiDéfaut")));
            
            Liste = new();
            {
                Liste.Name = "Liste";
                Liste.LayoutMode = 0;
            }
            panneau.AddChild(Liste);
        }
        AddChild(panneau);
    }

    private static Label CréerLabel(string nom_)
    {
        Label label = new Label();
        {
            label.Name = "Nom";
            label.Text = nom_;
            label.CustomMinimumSize = new(10, 20);
        }
        return label;
    }

    private Control AjouterOption(Control option_)
    {
        Liste.AddChild(option_);
        Options.Add((IOption)option_);
        return option_;
    }

    public Control AjouterOptionNumérique(string nomOption_, MajVariable1 fonctionMaj_, string nomVariable_ = "X", double valeurParDéfaut_ = 0D)
    { return AjouterOption(new OptionNumérique(nomOption_, fonctionMaj_, new string[] { nomVariable_ }, new double[] { valeurParDéfaut_ })); }
    public Control AjouterOptionNumérique(string nomOption_, MajVariable2 fonctionMaj_, string[] nomsVariables_, double[] valeursParDéfaut_)
    { return AjouterOption(new OptionNumérique(nomOption_, fonctionMaj_, nomsVariables_, valeursParDéfaut_)); }
    public Control AjouterOptionNumérique(string nomOption_, MajVariable3 fonctionMaj_, string[] nomsVariables_, double[] valeursParDéfaut_)
    { return AjouterOption(new OptionNumérique(nomOption_, fonctionMaj_, nomsVariables_, valeursParDéfaut_)); }
    public Control AjouterOptionNumérique(string nomOption_, MajVariable4 fonctionMaj_, string[] nomsVariables_, double[] valeursParDéfaut_)
    { return AjouterOption(new OptionNumérique(nomOption_, fonctionMaj_, nomsVariables_, valeursParDéfaut_)); }
    
    public Control AjouterOptionCouleur(string nomOption_, MajVariableCouleur fonctionMaj_, Color couleurParDéfaut_)
    { return AjouterOption(new OptionCouleur(nomOption_, fonctionMaj_, couleurParDéfaut_)); }

    public void Synchroniser()
    {
        // parcourir toutes les options et s'assurer que leur valeurs soit bien assignées aux variables associées
        foreach (IOption option in Options)
        { option.Synchroniser(); }
    }
}
