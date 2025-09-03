using System.Text;
using Godot;
using VA.Base.Systèmes;

namespace VA.Base;

/// <summary>
/// Affiche et controle un texte dans un cadre 2D
/// </summary>
public partial class AffichageTexte2D: PanelContainer
{
    private AffichageTexte2D()
    {
        Name = "AffichageTexte2D_PanelContainer";
        AddThemeStyleboxOverride("panel", ResourceLoader.Load<StyleBoxFlat>(Biblio.Index.RecupIndex(Systèmes.Gestionnaires.Index.CatégorieIndexs.Style, "Debug")));
        //PivotOffset = Size / 2;
        //GUI.Utiles.CentrerControl(this);
        //Resized += MajPivot;
        GrowHorizontal = Control.GrowDirection.Both;
        GrowVertical = Control.GrowDirection.Both;
        AddChild(CréerZoneTexte());
    }
    
    public AffichageTexte2D(string texte_, Color couleur_, Vector2 position_ = default, bool cadre_ = true) : this()
    {
        Position = position_;
        // TODO: Ne pas laisser le texte sortir de l'écran si la position est trop proche du bord en fonction de la taille
        Label texte = GetChild<Label>(0);
        texte.Text = texte_;
        texte.AddThemeColorOverride("font_color", couleur_);
        SetSelfModulate(new Color(1, 1, 1, cadre_ ? 1f : 0f));
    }
    
    // TODO: Spécifier une position et non pas seulement au centre.
    // Par exemple un autre ancrage avec offset, ou directement une position à l'écran et une autre taille minimale.

    // public override void _Ready()
    // {
    //     CallDeferred("Init");
    //     // AddThemeStyleboxOverride("Panel", ResourceLoader.Load<StyleBoxFlat>(Biblio.Index.RecupIndex(Systèmes.Gestionnaires.Index.CatégorieIndexs.Style, "Debug")));
    //     // SetAnchorsPreset(LayoutPreset.Center);
    // }
    //
    // private void Init()
    // {
    //     //AddThemeStyleboxOverride("panel", ResourceLoader.Load<StyleBoxFlat>(Biblio.Index.RecupIndex(Systèmes.Gestionnaires.Index.CatégorieIndexs.Style, "Debug")));
    //     SetAnchorsPreset(LayoutPreset.Center);
    // }

    private Label CréerZoneTexte()
    {
        Label texte = new();
        texte.Name = "Texte";
        texte.Text = "<vide>";
        texte.HorizontalAlignment = HorizontalAlignment.Center;
        texte.VerticalAlignment = VerticalAlignment.Center;
        texte.AutowrapMode = TextServer.AutowrapMode.WordSmart;
        texte.CustomMinimumSize = new(300, 0);
        texte.AddThemeConstantOverride("outline_size", 4);

        return texte;
    }

    private void MajPivot()
    {
        PivotOffset = Size / 2;
        Position = Position;
    }
    
    // TODO: durée de vie, effet d'apparition/disparition/etc , 
}