using System;
using System.Collections.Generic;
using Godot;
using VA.Base.Maths.Géom;
using VA.Base.Stockage;

namespace VA.Base.GUI;

public partial class InterfaceGrille<T>: Control
{
    public readonly struct InfosCurseur
    {
        public readonly Vector2I Case;
        public readonly Vector2 Position;
        //private readonly ulong ChronoDernierMouvement;

        public InfosCurseur(Vector2I case_, Vector2 position_)
        {
            Case = case_;
            Position = position_;
            //ChronoDernierMouvement = Time.GetTicksMsec();
        }
        public InfosCurseur()
        {
            Case = new(-1,-1);
            Position = Vector2.Zero;
            //ChronoDernierMouvement = Time.GetTicksMsec();
        }

        // public ulong DuréeDernierMouvement()
        // { return Time.GetTicksMsec() - ChronoDernierMouvement; }
    }

    public struct InfosClic
    {
        public readonly Grille<T>.BoiteObjet Boite;
        public readonly Vector2I Case;
        public readonly MouseButton TypeDeClic;
        public readonly ulong ChronoClic;
        public bool Maintenu { get; private set; }

        public InfosClic(Vector2I case_, Grille<T>.BoiteObjet objet_, MouseButton typeDeClic_ = MouseButton.None)
        {
            Boite = objet_;
            Case = case_;
            TypeDeClic = typeDeClic_;
            ChronoClic = Time.GetTicksMsec();
            Maintenu = false;
        }
        // public InfosClic(InfosClic infosPrécedentes_, double duréeClic_ = 0D)
        // {
        //     Boite = infosPrécedentes_.Boite;
        //     Case = infosPrécedentes_.Case;
        //     TypeDeClic = infosPrécedentes_.TypeDeClic;
        //     DuréeClic = infosPrécedentes_.DuréeClic + duréeClic_;
        //     Maintenu = DuréeClic >= Constantes.DuréeMaintiensClic;
        // }
        public InfosClic()
        {
            Boite = null;
            Case = new(-1,-1);
            TypeDeClic = MouseButton.None;
            ChronoClic = Time.GetTicksMsec();
            Maintenu = false;
        }

        public bool EstValide()
        { return !(TypeDeClic == MouseButton.None || Boite == null || (Case.X < 0 || Case.Y < 0)); }

        public ulong MajDuréeClic()
        {
            ulong durée = Time.GetTicksMsec() - ChronoClic;
            Maintenu = TypeDeClic != MouseButton.None && durée >= Constantes.Gui.DuréeMaintiensClic;
            return durée;
        }
    }
    
    
    private Grille<T> Grille;
    private List<Control> Visuels;

    private Vector2 TC;
    private Color CF, CG;
    private int EL;

    //private Node OptionTaille, OptionTailleCase;
    //private Control Paramètres;

    //private ColorRect Fond;
    //private TracerGrille Grillage;

    //private VisuelRectangle Sélection, SélectionEnMouvement, CréationEnCours;

    //private bool SourisSurGrille;
    //private Vector2I PositionClic;
    private InfosCurseur Curseur;
    private InfosClic ClicEnCours;

    //private (MouseButton typeDeClic, double compteur) ClicEnCours;
    //private bool ClicMaintenu;
    private double CompteurClicMaintenu, DuréeClicMaintenu;
    
    public Vector2I Taille
    {
        get => new(Grille.Largeur, Grille.Hauteur);
        set => RedimensionnerGrille(value);
    }
    public Vector2 TailleCase
    {
        get => TC;
        set => MajOption(ref TC, value);
    }
    public Color CouleurGrille
    {
        get => CG;
        set => MajOption(ref CG, value);
    }
    
    public Color CouleurFond
    {
        get => CF;
        set => MajOption(ref CF, value);
    }
    
    public int EpaisseurLignes
    {
        get => EL;
        set => MajOption(ref EL, value);
    }
    
    private void MajOption<T>(ref T option_, T valeur_) where T : IEquatable<T>
    {
        if (!valeur_.Equals(option_))
        {
            option_ = valeur_;
            QueueRedraw();
        }
    }
    
    // EVENTS //
    
    public delegate void Clic(InfosClic infos_);
    public event Clic ClicGauche;
    public event Clic ClicDroit;
    public event Clic ClicMolette;
    
    public delegate void RelacherClic(InfosClic infos_);
    public event RelacherClic RelacherClicGauche;
    public event RelacherClic RelacherClicDroit;
    public event RelacherClic RelacherClicMolette;
    
    public delegate void Glisser(InfosCurseur infos_, Vector2 déplacement_);
    public event Glisser GlisserClicGauche;
    public event Glisser GlisserClicDroit;
    public event Glisser GlisserClicMolette;

    public delegate void Défilement(Vector2 direction_);
    public event Défilement Molette;

    public delegate void NouvelleCaseCiblée(Vector2I case_);
    public event NouvelleCaseCiblée CiblerCase;

    // l'interface retrouve ou crée un visuel pour chaque objet stocker dans la Grille en fonction de s'ils implémentent une interface compatible
    
    public InterfaceGrille()
    {
        Grille = null;
        Visuels = new();
        
        TailleCase = Vector2.One;
        Curseur = new();
        ClicEnCours = new();
        //SourisSurGrille = false;
        //PositionClic = new(-1, -1);
        //ClicEnCours = (MouseButton.None, 0D);
        //Sélection = null;
        //ClicMaintenu = false;
        CompteurClicMaintenu = 0D;
        DuréeClicMaintenu = 0.15D;
        EpaisseurLignes = 1;
        GuiInput += Entrées;
    }

    ~InterfaceGrille()
    { QueueFree(); }

    public void Init(Grille<T> grille_)
    {
        Grille = grille_;

        Grille.ModificationTaille += RedimensionnerGrille;
        Grille.Ajout += Ajouter;
        // Grille.Retrait += ;
        // Grille.Déplacement += ;
        Grille.Mourir += QueueFree;
    }

    private delegate void ActionClic(InputEventMouseButton event_);
    private void Test(InputEvent event_)
    {
        Dictionary<MouseButton, ActionClic> actionsClic = new();
        List<MouseButton> indexs = new();
        if (event_ is InputEventMouseButton clicSouris && actionsClic.Count > 0)
        {
            if (actionsClic.ContainsKey(clicSouris.ButtonIndex))
            { actionsClic[clicSouris.ButtonIndex]?.Invoke(clicSouris); }
        }
        
        // TODO: revoir ce code
        
        // actionsClic.Add(MouseButton.Left, ClicGauche);
        // actionsClic.Add(MouseButton.Right, ClicDroit);
        // actionsClic.Add(MouseButton.Middle, ClicMolette);
    }

    private void Entrées(InputEvent event_)
    {
        if (event_ is InputEventMouse)
        {
            if (event_ is InputEventMouseButton clicSouris)
            {
                if (clicSouris.Pressed)
                {
                    GD.Print("Clic");
                    //Vector2I posCase = PositionSourisDansGrille(clicSouris.Position);
                    ClicEnCours = new(Curseur.Case, Grille.RécupBoite(Curseur.Case), clicSouris.ButtonIndex);

                    switch (ClicEnCours.TypeDeClic)
                    {
                        case MouseButton.Left:
                            ClicGauche?.Invoke(ClicEnCours);
                            break;
                        case MouseButton.Right:
                            ClicDroit?.Invoke(ClicEnCours);
                            break;
                        case MouseButton.Middle:
                            ClicMolette?.Invoke(ClicEnCours);
                            break;
                    }
                }
                else
                {
                    GD.Print("Pas Clic");
                    // terminer les éventuelles actions en cours
                    if (!ClicEnCours.Maintenu)
                    {
                        switch (ClicEnCours.TypeDeClic)
                        {
                            case MouseButton.Left:
                                RelacherClicGauche?.Invoke(ClicEnCours);
                                break;
                            case MouseButton.Right:
                                RelacherClicDroit?.Invoke(ClicEnCours);
                                break;
                            case MouseButton.Middle:
                                RelacherClicMolette?.Invoke(ClicEnCours);
                                break;
                            case MouseButton.WheelLeft:
                                Molette?.Invoke(new(-1f, 0f));
                                break;
                            case MouseButton.WheelRight:
                                Molette?.Invoke(new(1f, 0f));
                                break;
                            case MouseButton.WheelDown:
                                Molette?.Invoke(new(0f, -1f));
                                break;
                            case MouseButton.WheelUp:
                                Molette?.Invoke(new(0f, 1f));
                                break;
                        }
                    }

                    ClicEnCours = new();
                }
            }
            else if (event_ is InputEventMouseMotion eventMouseMotion)
            {
                Vector2I ancienneCase = Curseur.Case;
                Curseur = new(PositionSourisDansGrille(eventMouseMotion.Position), eventMouseMotion.Position);
                ClicEnCours.MajDuréeClic();
                
                if (ancienneCase != Curseur.Case)
                { CiblerCase?.Invoke(Curseur.Case); }
                
                if (ClicEnCours.Maintenu)
                {
                    switch (ClicEnCours.TypeDeClic)
                    {
                        case MouseButton.Left:
                            GlisserClicGauche?.Invoke(Curseur, eventMouseMotion.Relative);
                            break;
                        case MouseButton.Right:
                            GlisserClicDroit?.Invoke(Curseur, eventMouseMotion.Relative);
                            break;
                        case MouseButton.Middle:
                            GlisserClicMolette?.Invoke(Curseur, eventMouseMotion.Relative);
                            break;
                    }
                }
            }
        }
    }

    // public override void _Process(double delta_)
    // {
    //     
    // }

    public void Ajouter(Grille<T>.BoiteObjet boite_)
    {
        // créer un visuel en fonction du type d'objet
        // Créer un BoxContainer de la talle de l'objet dans la grille
        if (typeof(T).GetInterface(nameof(IInterface2D)) != null)
        {
            // récupérer l'interface et l'ajouter
        }
        else
        {
            // créer une interface rectangle et l'ajouter
        }
    }

    public void ModifierCouleurFond(Color couleur_)
    {
        CouleurFond = couleur_;
        QueueRedraw();
    }
    
    public void ModifierCouleurGrille(Color couleur_)
    {
        CouleurGrille = couleur_;
        QueueRedraw();
    }

    private void RedimensionnerGrille(Vector2I nouvelleTaille_)
    {
        CustomMinimumSize = Grille.Taille * TailleCase;
        QueueRedraw();
    }

    public override void _Draw()
    {
        // Fond
        DrawRect(new(Position, Size), CouleurFond);
        // Grille
        for (int y = 0; y <= Taille.Y; ++y)
        {
            for (int x = 0; x <= Taille.X; ++x)
            { DrawLine(new Vector2(Position.X + (x * TailleCase.X), Position.Y), new Vector2(Position.X + (x * TailleCase.X), Position.Y + Size.Y), CouleurGrille, EpaisseurLignes); }
            DrawLine(new Vector2(Position.X, y * TailleCase.Y), new Vector2(Position.X + Size.X, y * TailleCase.Y), CouleurGrille, EpaisseurLignes);
        }
    }

    private Vector2I PositionSourisDansGrille(Vector2 positionSouris_)
    {
        Vector2 positionF = positionSouris_ - GlobalPosition;
        positionF.X /= TailleCase.X;
        //pos = new((int)(positionSouris_.X - Fond.GlobalPosition.X) / TailleCaseGrille.X, (int)(positionSouris_.Y - Fond.GlobalPosition.Y) / TailleCaseGrille.Y);
        Vector2I positionI = new((int)positionF.X, Mathf.Abs((int)(positionF.Y / TailleCase.Y) - (Taille.Y - 1)));
        return positionI;
    }

    public Vector2 PositionCaseEcran(Vector2I position_)
    { return new Vector2(Position.X + position_.X * TailleCase.X, (Taille.Y - position_.Y - 1) * TailleCase.Y); }
    
    // private void ChangerCouleurFond()
    // { CouleurFond = ((ColorPickerButton)Utiles.TrouverNodeEnfant<SpinBox>(OptionCouleurFond, "Couleur")).Color; }
    //
    // private void ChangerCouleurGrille()
    // { CouleurGrille = ((ColorPickerButton)Utiles.TrouverNodeEnfant<SpinBox>(OptionCouleurGrille, "Couleur")).Color; }
    
    
}
