using System;
using Godot;
using VA.Base.GUI;

namespace VA.Base.Maths.Géom;

// permet de manipuler un Rectangle 2d suivant les axes X et Y (AABB)
public class RectangleI: IInterface2D
{
    private int[] Côtés;
    private Interface2DRectangleI Interface;
    public Control Interface2D => Interface != null ? Interface : CréerInterface();
    public int Bas { get => Côtés[0]; set => ModifierCôté(0, value); }
    public static int CôtéBas => 0;
    public int Gauche { get => Côtés[1]; set => ModifierCôté(1, value); }
    public static int CôtéGauche => 1;
    public int Haut { get => Côtés[2]; set => ModifierCôté(2, value); }
    public static int CôtéHaut => 2;
    public int Droite { get => Côtés[3]; set => ModifierCôté(3, value); }
    public static int CôtéDroit => 3;
    public Vector2I GaucheBas => new(Gauche, Bas);
    public Vector2I GaucheHaut => new(Gauche, Haut);
    public Vector2I DroiteHaut => new(Droite, Haut);
    public Vector2I DroiteBas => new(Droite, Bas);

    public Vector2I Taille => new(Droite - Gauche + 1, Haut - Bas + 1);
    
    public delegate void ActionModification();
    public event ActionModification Modification;

    
    public RectangleI(): this(1, 1) {}
    public RectangleI(Vector2I taille_): this(taille_.X, taille_.Y) {}
    public RectangleI(Vector2I position_, Vector2I taille_)
    {
        if (taille_.X == 0 && taille_.Y == 0)
        { throw new ArgumentException($"Les dimensions [{taille_.X}][{taille_.Y}] ne sont pas valides"); }

        Côtés = new int[4];
        Interface = null;
        
        Côtés[0] = taille_.Y > 0 ? position_.Y : position_.Y + taille_.Y - 1;
        Côtés[1] = taille_.X > 0 ? position_.X : position_.X + taille_.X - 1;
        Côtés[2] = taille_.Y > 0 ? position_.Y + taille_.Y - 1 : position_.Y;
        Côtés[3] = taille_.X > 0 ? position_.X + taille_.X - 1 : position_.X;
    }
    public RectangleI(int tailleX_, int tailleY_)
    {
        if (tailleX_ == 0 && tailleY_ == 0)
        { throw new ArgumentException($"Les dimensions [{tailleX_}][{tailleY_}] ne sont pas valides"); }

        Côtés = new int[4];
        
        Côtés[0] = tailleY_ > 0 ? 0 : tailleY_ - 1;
        Côtés[1] = tailleX_ > 0 ? 0 : tailleX_ - 1;
        Côtés[2] = tailleY_ > 0 ? tailleY_ - 1 : 0;
        Côtés[3] = tailleX_ > 0 ? tailleX_ - 1 : 0;
    }
    public RectangleI(int bas_, int gauche_, int haut_, int droite_)
    {
        if (bas_ > haut_ || gauche_ > droite_)
        { throw new ArgumentException($"Les valeurs [{bas_}][{gauche_}][{haut_}][{droite_}] données au rectangle ne sont pas valides"); }
        Côtés = new[] { bas_, gauche_, haut_, droite_ };
    }

    public RectangleI(RectangleI rectangle_)
    { Côtés = new int[] { rectangle_.Bas, rectangle_.Gauche, rectangle_.Haut, rectangle_.Droite }; }

    public static bool operator ==(RectangleI a_, RectangleI b_)
    { return a_.Bas == b_.Bas && a_.Gauche == b_.Gauche && a_.Haut == b_.Haut && a_.Droite == b_.Droite; }

    public static bool operator !=(RectangleI a, RectangleI b) => !(a == b);

    public int this[int côté_]
    {
        get
        {
            if (côté_ is >= 0 and < 4)
            { return Côtés[côté_]; }
            throw new IndexOutOfRangeException("L'index du côté doit être entre 0 et 4");
        }
        set
        {
            if (côté_ is >= 0 and < 4)
            { Côtés[côté_] = value; }
            throw new IndexOutOfRangeException("L'index du côté doit être entre 0 et 4");
        }
    }

    public bool PeutContenir(Vector2I taille_)
    { return taille_.X <= Taille.X && taille_.Y <= Taille.Y; }
    public bool PeutContenir(RectangleI rectangle_)
    { return Gauche <= rectangle_.Gauche && Droite >= rectangle_.Droite && Bas <= rectangle_.Bas && Haut >= rectangle_.Haut; }

    public bool Croise(RectangleI rectangle_)
    {
        // Calculer si deux boites se croisent
        return !(rectangle_.Gauche > Droite || rectangle_.Droite < Gauche || rectangle_.Bas > Haut || rectangle_.Haut < Bas);
    }
    
    private Vector4I CalculerCroisement(RectangleI rectangle_)
    {
        Vector4I croisement = Vector4I.Zero;
        if (Croise(rectangle_))
        {
            croisement.X = Mathf.Max(0, rectangle_.Bas - Bas);
            croisement.Y = Mathf.Max(0, rectangle_.Gauche - Gauche);
            croisement.Z = Mathf.Max(0, rectangle_.Haut - Haut);
            croisement.W = Mathf.Max(0, rectangle_.Droite - Droite);
        }

        return croisement;

        //return configuration >= 0 ? (true, configuration, croisement) : (false, configuration, croisement);
    }

    public bool Contient(Vector2I point_)
    { return point_.X >= Gauche && point_.X <= Droite + Taille.X && point_.Y >= Bas && point_.Y <= Haut + Taille.Y; }

    /// <summary>
    /// Tourne le rectangle autour d'un point
    /// </summary>
    /// <param name="centreRotation"></param>
    /// <param name="sens_"></param>
    public void Tourner(Vector2I centreRotation, bool sens_ = true)
    {
        // Vector2I gh = Vecteurs.Rotation90(new(Gauche - centreRotation.X, Bas - centreRotation.Y), sens_),
        //         db = Vecteurs.Rotation90(new(Droite - centreRotation.X, Haut - centreRotation.Y), sens_);
        
        if (sens_)
        {
            Côtés[0] = -(Droite - centreRotation.X) + centreRotation.Y;
            Côtés[1] = (Bas - centreRotation.Y) + centreRotation.X;
            Côtés[2] = -(Gauche - centreRotation.X) + centreRotation.Y;
            Côtés[3] = (Haut - centreRotation.Y) + centreRotation.X;
        }
        else
        {
            Côtés[0] = (Gauche - centreRotation.X) + centreRotation.Y;
            Côtés[1] = -(Haut - centreRotation.Y) + centreRotation.X;
            Côtés[2] = (Droite - centreRotation.X) + centreRotation.Y;
            Côtés[3] = -(Bas - centreRotation.Y) + centreRotation.X;
        }
        Modification?.Invoke();
    }

    public void Déplacer(Vector2I vecteurDéplacement_)
    {
        Côtés[0] += vecteurDéplacement_.Y;
        Côtés[1] += vecteurDéplacement_.X;
        Côtés[2] += vecteurDéplacement_.Y;
        Côtés[3] += vecteurDéplacement_.X;
        Modification?.Invoke();
    }

    public void Déplacer(Vector2I vecteurDéplacement_, Vector4I limites_)
        => Déplacer(vecteurDéplacement_, new RectangleI(limites_.X, limites_.Y, limites_.Z, limites_.W));
    public void Déplacer(Vector2I vecteurDéplacement_, RectangleI limites_)
    {
        RectangleI nouvelEmplacement = new(this);
        nouvelEmplacement.Déplacer(vecteurDéplacement_);
        if (limites_.PeutContenir(nouvelEmplacement))
        {
            Côtés = nouvelEmplacement.Côtés;
            Modification?.Invoke();
        }
    }

    public void Modifier(RectangleI nouveauRectangle_)
    {
        for (int c = 0; c < 4; ++c)
        { Côtés[c] = nouveauRectangle_.Côtés[c]; }
        Modification?.Invoke();
    }
    public void Modifier(int bas_, int gauche_, int haut_, int droite_)
    {
        Bas = bas_;
        Gauche = gauche_;
        Haut = haut_;
        Droite = droite_;
        Modification?.Invoke();
    }

    public void ModifierCôté(int côté_, int nouvelleValeur_)
    {
        if (côté_ is >= 0 and < 4)
        {
            Côtés[côté_] = nouvelleValeur_;
            Modification?.Invoke();
        }
    }

    public void Etirer(Vector2I direction_)
    {
        if (direction_ != Vector2I.Zero)
        {
            if (direction_.X > 0)
            { Côtés[3] += direction_.X; }
            else if (direction_.X < 0)
            { Côtés[1] += direction_.X; }

            if (direction_.Y > 0)
            { Côtés[2] += direction_.Y; }
            else if (direction_.Y < 0)
            { Côtés[0] += direction_.Y; }

            Modification?.Invoke();
        }
    }

    // public Rect2I CadreI()
    // { return new Rect2I(Vector2I.Zero, Taille); }
    //
    // public Rect2 Cadre()
    // { return new Rect2(Vector2.Zero, Taille); }
    // public Rect2 Cadre(Vector2 tailleUnité_)
    // { return new Rect2(Vector2.Zero, Taille * tailleUnité_); }

    private Control CréerInterface()
    {
        Interface = (Interface2DRectangleI)Noeuds.Utiles.AttacherScript(new Control(), "res://VA/Base/GUI/2D/Interfaces/InterfaceRectangleI.cs");
        Interface.Init(this);
        return Interface;
    }
}