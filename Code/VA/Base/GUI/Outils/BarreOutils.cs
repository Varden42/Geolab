using System;
using System.Collections.Generic;
using Godot;
using VA.Base.Utiles;

namespace VA.Base.GUI.Outils;

/// <summary>
/// Une barre contenant des outils et pouvant être placé sur les bords du rect dans lequel il se trouve
/// </summary>
public partial class BarreOutils: Prefab.ControlPrefab
{
    public const float TAILLE_ELEMENT = 20f;
    public enum EnumBord { Haut, Bas, Gauche, Droite }

    private EnumBord Bord;
    private PanelContainer Panneau;
    private FlowContainer Lignes;
    // private List<IElement> Elements;
    private Dictionary<string, Elements.Groupe> Elements;
    private CompteurInt Compteur;
    private int Zoom_;
    
    public int Zoom { get => Zoom_; set => ChangerZoom(value); }
    public bool Vertical => Bord != EnumBord.Haut && Bord != EnumBord.Bas;

    public event Action<bool> ChangementDeBord;

    private void Init(EnumBord bord_ = EnumBord.Haut)
    {
        Bord = bord_;

        Elements = new();
        Compteur = new();

        Panneau = new();
        {
            Panneau.ClipContents = true;

            Lignes = new();
            {
                Lignes.Name = "Lignes";
                Lignes.ClipContents = true;
                Lignes.AddThemeConstantOverride("h_separation", 1);
                Lignes.AddThemeConstantOverride("v_separation", 1);
            }
            Panneau.AddChild(Lignes);
        }
        AddChild(Panneau);
        
        
        
        ChangerBord(Bord);
        
        Resized += MajTailleActions;
    }

    protected override void Construire()
    {
        Panneau = new();
        {
            Panneau.ClipContents = true;

            Lignes = new();
            {
                Lignes.Name = "Lignes";
                Lignes.ClipContents = true;
                Lignes.AddThemeConstantOverride("h_separation", 1);
                Lignes.AddThemeConstantOverride("v_separation", 1);
            }
            Panneau.AddChild(Lignes);
        }
        AddChild(Panneau);
    }
    
    public BarreOutils()
    {
        Init();
    }

    public BarreOutils(EnumBord bord_)
    {
        Init(bord_);
    }

    /// <summary>
    /// Modifie la barre en fonction du bord sur lequel elle se trouve.
    /// </summary>
    /// <param name="bord_"></param>
    private void ChangerBord(EnumBord bord_)
    {
        Bord = bord_;
        ChangementDeBord?.Invoke(Vertical);
        
        // reconfigurer les controles en fonction du bord choisit
        switch (Bord)
        {
            case EnumBord.Haut:
                AnchorsPreset = (int)LayoutPreset.TopWide;
                Lignes.Vertical = false;
                break;
            case EnumBord.Bas:
                AnchorsPreset = (int)LayoutPreset.BottomWide;
                Lignes.Vertical = false;
                break;
            case EnumBord.Gauche:
                AnchorsPreset = (int)LayoutPreset.LeftWide;
                Lignes.Vertical = true;
                break;
            case EnumBord.Droite:
                AnchorsPreset = (int)LayoutPreset.RightWide;
                Lignes.Vertical = true;
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Calcule La taille des actions en fonction de la largeur/hauteur de la barre de manière à ce qu'ils prennent toute la longueur en gardant un ratio correct
    /// Les actions devront toujours se rapprocher d'une taille de 20x20 multiplier par le ratio de zoom
    /// </summary>
    private void MajTailleActions()
    {
        // TODO: prendre en compte l'espace entre les éléments ainsi que les bordures.
        // parcourir les éléments et calculer le cumul de toutes les zones de vide pour les retirer de la longueur de la barre.
        // faut-il ajouter un séparateur entre les éléments ou bien l'intégrer dans chaque éléments ?
        
        // Redimensionner toutes les actions en fonction du zoom et si ca dépasse, ca débordera sur la ligne du dessous
        float tailleEléments = TAILLE_ELEMENT * Zoom_;
        // float longueurBarre = Bord == EnumBord.Bas || Bord == EnumBord.Haut ? GetViewportRect().Size.X - 2 : GetViewportRect().Size.Y - 2;
        //
        // float espaceVide = 0;
        // foreach (KeyValuePair<string, Groupe> groupe in Elements)
        // { espaceVide += groupe.Value.NombreElements + 1; }
        //
        // float séparations = Elements.Count > 1 ? Elements.Count - 1 : 0;
        //
        // float longueurUtile = longueurBarre - (espaceVide + séparations);
        //
        // tailleEléments = longueurUtile / Mathf.Round(longueurUtile / tailleEléments);
        foreach (KeyValuePair<string, Groupe> groupe in Elements)
        { groupe.Value.RedimensionnerElements(tailleEléments); }
    }
    
    
    private void ChangerZoom(int zoom_)
    {
        if (zoom_ != Zoom_)
        { MajTailleActions(); }
    }

    public void AjouterElement(Element element_, int index_ = -1)
    {
        Groupe groupe = new();
        groupe.AjouterElement(element_);
        
        Elements.Add(Compteur.Ajouter.ToString(), groupe);
        Noeuds.Utiles.AjouterNode(Lignes, groupe, index_);
        ChangementDeBord += groupe.Réorienter;
    }
    
    public void AjouterElement(string nomGroupe_, Element element_, int index_ = -1)
    {
        if (Elements.TryGetValue(nomGroupe_, out Groupe groupe))
        { groupe.AjouterElement(element_, index_); }
        else
        {
            groupe = new();
            groupe.AjouterElement(element_);
            Elements.Add(nomGroupe_, groupe);
            Noeuds.Utiles.AjouterNode(Lignes, groupe);
            ChangementDeBord += groupe.Réorienter;
        }
    }
    
    public void AjouterElements(string nomGroupe_, IEnumerable<Element> elements_, int index_ = -1)
    {
        if (Elements.TryGetValue(nomGroupe_, out Groupe groupe))
        { groupe.AjouterElements(elements_, index_); }
        else
        {
            groupe = new();
            groupe.AjouterElements(elements_);
            Elements.Add(nomGroupe_, groupe);
            Noeuds.Utiles.AjouterNode(Lignes, groupe);
            ChangementDeBord += groupe.Réorienter;
        }
    }
    
    // faire les méthodes de retrait
    
    public void AjouterGroupe(string nomGroupe_, Groupe groupe_, int index_ = -1)
    {
        if (!Elements.ContainsKey(nomGroupe_))
        {
            Elements.Add(nomGroupe_, groupe_);
            Noeuds.Utiles.AjouterNode(Lignes, groupe_, index_);
            ChangementDeBord += groupe_.Réorienter;
        }
    }

    public bool RetraitElement(Element element_)
    {
        foreach (KeyValuePair<string, Groupe> groupe in Elements)
        {
            Element element = groupe.Find(e => e.Value == element_);
        }


        Element element = Elements.Find(e => e.Value == element_);
        bool réussite = Elements.Remove(element_);
        (element as Control)?.QueueFree();
        return réussite;
    }

    public void Redimensionner(float taille_)
    {
        throw new NotImplementedException();
    }

    public void Réorienter(bool vertical_ = true)
    {
        throw new NotImplementedException();
    }

}