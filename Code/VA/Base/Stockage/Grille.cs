using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using VA.Base.GUI;
using VA.Base.Maths;
using VA.Base.Utiles;
using VA.Base.Maths.Géom;

namespace VA.Base.Stockage;

public class Grille<T>: IInterface2D
{
    public class BoiteObjet
    {
        public readonly T Objet;
        public int[] Indexs { get; private set; }
        public RectangleI Emplacement { get; }

        public BoiteObjet(T objet_, int[] indexs_, RectangleI emplacement_)
        {
            Objet = objet_;
            Indexs = indexs_;
            Emplacement = emplacement_;
        }

        public void MajEmplacement(RectangleI nouvelEmplacement_, int[] indexs_)
        {
            Emplacement.Modifier(nouvelEmplacement_);
            Indexs = indexs_;
        }
    }
    
    protected BoiteObjet[] Cases;
    protected List<BoiteObjet> Boites;

    private InterfaceGrille<T> Interface;
    public Control Interface2D => Interface ?? CréerInterface();
    
    public delegate void ModifierTaille(Vector2I nouvelleTaille_);
    public event ModifierTaille ModificationTaille;

    public delegate void AjoutObjet(BoiteObjet boite_);
    public event AjoutObjet Ajout;

    public delegate void RetraitObjet(BoiteObjet boite_);
    public event RetraitObjet Retrait;

    public delegate void DéplacementObjet(BoiteObjet boite_);
    public event DéplacementObjet Déplacement;

    public delegate void Mort();
    public event Mort Mourir;

    public Vector2I Taille { get; private set; }
    public int Largeur => Taille.X;
    public int Hauteur => Taille.Y;

    public int NombreObjets => Boites.Count;

    public Grille(Vector2I taille_) : this(taille_.X, taille_.Y) {}

    public Grille(int largeur_, int hauteur_)
    {
        Cases = new BoiteObjet[largeur_ * hauteur_];
        for (int c = 0; c < Cases.Length; ++c)
        { Cases[c] = null; }
        Boites = new();
        Taille = new(largeur_, hauteur_);
        Interface = null;
    }
    
    ~Grille()
    { Mourir?.Invoke(); }

    public void Redimensionner(Vector2I nouvelleTaille_)
    {
        if (Taille != nouvelleTaille_ && nouvelleTaille_.X > 0 && nouvelleTaille_.Y > 0)
        {
            List<BoiteObjet> anciennesBoites = Boites;
            Taille = nouvelleTaille_;
            Cases = new BoiteObjet[nouvelleTaille_.X * nouvelleTaille_.Y];
            
            foreach (BoiteObjet boite in anciennesBoites)
            { Ajouter(boite.Objet, boite.Emplacement); }
            
            ModificationTaille?.Invoke(Taille);
        }
    }

    #region Déplacer
    public bool DéplacerObjet(Vector2I position_, Vector2I vecteurDéplacement_)
    {
        if (vecteurDéplacement_ != Vector2I.Zero)
        { return DéplacerBoite(RécupBoite(position_), vecteurDéplacement_); }
        return false;
    }
    public bool DéplacerObjet(Vector2I position_, RectangleI nouvelEmplacement_) => DéplacerBoite(RécupBoite(position_), nouvelEmplacement_);
    // public bool DéplacerObjet(T objet_, Vector2I vecteurDéplacement_)
    // {
    //     if (vecteurDéplacement_ != Vector2I.Zero)
    //     { return DéplacerBoite(RécupBoite(objet_), vecteurDéplacement_); }
    //     return false;
    // }
    // public bool DéplacerObjet(T objet_, RectangleI nouvelEmplacement_) => DéplacerBoite(RécupBoite(objet_), nouvelEmplacement_);

    private bool DéplacerBoite(BoiteObjet boite_, Vector2I vecteurDéplacement_)
    {
        if (boite_ != null)
        {
            RectangleI nouvelEmplacement = new(boite_.Emplacement);
            nouvelEmplacement.Déplacer(vecteurDéplacement_);
            return DéplacerBoite(boite_, nouvelEmplacement);
        }
        return false;
    }
    private bool DéplacerBoite(BoiteObjet boite_, RectangleI nouvelEmplacement_)
    {
        if (EmplacementDisponible(nouvelEmplacement_, boite_))
        {
            // Retrait
            foreach (int index in boite_.Indexs)
            { Cases[index] = null; }
            // Ajout
            boite_.MajEmplacement(nouvelEmplacement_, IndexsZone(nouvelEmplacement_));
            foreach (int index in boite_.Indexs)
            { Cases[index] = boite_; }
            
            Déplacement?.Invoke(boite_);
            return true;
        }
        return false;
    }
    #endregion
    
    public bool Ajouter(T objet_, RectangleI emplacement_)
    {
        // TODO: Donner la posibilité de faire glisser l'objet vers la position disponible la plus proche
        if (RentreDansLaGrille(emplacement_))
        { return Ajouter(new(objet_, IndexsZone(emplacement_), emplacement_)); }
        return false;
    }

    private bool Ajouter(BoiteObjet boite_)
    {
        if (EstVide(boite_.Indexs))
        {
            foreach (int index in boite_.Indexs)
            { Cases[index] = boite_; }
            Boites.Add(boite_);
            Ajout?.Invoke(boite_);
            return true;
        }
        return false;
    }

    public bool Retirer(Vector2I position_)
    { return Retirer(RécupBoite(position_)); }
    // public bool Retirer(T objet_)
    // { return Retirer(RécupBoite(objet_)); }
    private bool Retirer(BoiteObjet boite_)
    {
        if (boite_ != null)
        {
            foreach (int index in boite_.Indexs)
            { Cases[index] = null; }
            Boites.Remove(boite_);
            boite_.Emplacement.Modifier(-1, -1, -1, -1);
            Retrait?.Invoke(boite_);
            return true;
        }
        return false;
    }

    public bool EmplacementDisponible(RectangleI emplacement_, BoiteObjet filtre_ = null)
    {
        if (RentreDansLaGrille(emplacement_))
        {
            int[] indexs = IndexsZone(emplacement_);
            return filtre_ != null ? EstVide(indexs, filtre_) : EstVide(indexs);
        }
        return false;
    }
    
    private bool RentreDansLaGrille(RectangleI emplacement_)
    { return emplacement_.Gauche >= 0 && emplacement_.Droite < Largeur && emplacement_.Bas >= 0 && emplacement_.Haut < Hauteur; }
    
    public bool EstVide(RectangleI emplacement_, BoiteObjet filtre_)
    { return EstVide(IndexsZone(emplacement_), filtre_); }
    public bool EstVide(RectangleI emplacement_)
    { return EstVide(IndexsZone(emplacement_)); }
    public bool EstVide(int[] indexs_, BoiteObjet filtre_)
    {
        foreach (int index in indexs_)
        {
            if (Cases[index] != null && Cases[index] != filtre_)
            { return false; }
        }
        return true;
    }
    public bool EstVide(int[] indexs_)
    {
        foreach (int index in indexs_)
        {
            if (Cases[index] != null)
            { return false; }
        }
        return true;
    }
    
    private int[] IndexsZone(RectangleI emplacement_)
    {
        Vector2I taille = emplacement_.Taille;
        int[] indexs = new int[taille.X * taille.Y];
        int origine = Tableaux.Index2DVers1D(emplacement_.GaucheBas, Largeur);
        int compteur = 0;
        for (int y = 0; y < taille.Y; ++y)
        {
            for (int x = 0; x < taille.X; ++x)
            {
                indexs[compteur] = origine + (y * Largeur) + x;
                ++compteur;
            }
        }
        return indexs;
    }

    private bool IndexValide(int index_)
    { return index_ >= 0 && index_ < Cases.Length; }

    public BoiteObjet RécupBoite(Vector2I position_)
    {
        int index = Tableaux.Index2DVers1D(position_, Largeur);
        if (IndexValide(index))
        { return Cases[index]; }

        throw new ArgumentException($"La position [{position_}] est en dehors de la grille");
    }

    // private BoiteObjet RécupBoite(T objet_)
    // { return Boites.Find(b => b.Objet == objet_); }
    
    public T this[Vector2I position_] => RécupBoite(position_).Objet;

    public (bool Réussite, Vector2I Position) PremièrePositionLibre(bool axe_, Vector2I départ_, bool direction_)
    {
        Vecteurs.GarderDansPlage(ref départ_, new(0, Largeur - 1), new(0, Hauteur - 1));
        int index = 0, décalagePosition = direction_ ? 1 : -1, axe = axe_ ? 0 : 1;
        while (départ_[axe] >= 0 && départ_[axe] < (direction_ ? Largeur : Hauteur))
        {
            if (RécupBoite(départ_) == null)
            { return (true, départ_); }
            départ_[axe] += décalagePosition;
        }
        return (false, new Vector2I(-1, -1));
    }

    public List<BoiteObjet> ListeBoites()
    { return new List<BoiteObjet>(Boites); }

    private Control CréerInterface()
    {
        Interface = (InterfaceGrille<T>)VA.Base.Noeuds.Utiles.AttacherScript(new Control(), "res://VA/Base/GUI/2D/Interfaces/InterfaceGrille.cs");
        Interface.Init(this);
        return Interface;
    }
}

public class GrilleRangés<T>
{
    // private interface IEmplacement
    // {
    //     public Vector2I Position { get; }
    //     public Vector2I Taille { get; }
    // }
    
    private class Objet// : IEmplacement
    {
        public readonly bool Rotation;
        public readonly RectangleI Boite;

        //public Vector2I Position => Boite.Position;
        //public Vector2I Taille => Boite.Taille;

        public Objet(RectangleI boite_, bool rotation_)
        {
            Rotation = rotation_;
            Boite = boite_;
        }
        
        public static bool operator ==(Objet a_, Objet b_) 
        { return a_.Boite == b_.Boite; }

        public static bool operator !=(Objet a, Objet b) => !(a == b);

        public Objet Tourner(Vector2I centreRotation, bool sens_)
        {
            if (Rotation)
            {
                RectangleI rectangleTourné = Boite;
                rectangleTourné.Tourner(centreRotation, sens_);
                return new Objet(rectangleTourné, Rotation);
            }
            return this;
        }
    }

    // private class EmplacementLibre : IEmplacement
    // {
    //     private readonly RectangleI Boite;
    //
    //     public Vector2I Position => Boite.Position;
    //     public Vector2I Taille => Boite.Taille;
    //
    //     public EmplacementLibre(Vector2I position_, Vector2I taille_)
    //     { Boite = new(position_, taille_); }
    // }

    // private readonly struct RectangleI
    // {
    //     // la taille peut être négative et elle représente l'orientation de la boite //
    //     public readonly Vector2I Position;
    //     public readonly Vector2I Taille;
    //
    //     public RectangleI(Vector2I position_, Vector2I taille_)
    //     {
    //         Position = position_;
    //         Taille = taille_;
    //     }
    //
    //     public static bool operator ==(RectangleI a_, RectangleI b_)
    //     { return a_.Position == b_.Position && a_.Taille == b_.Taille; }
    //
    //     public static bool operator !=(RectangleI a, RectangleI b) => !(a == b);
    //
    //     public bool PeutContenir(Vector2I taille_)
    //     { return Mathf.Abs(taille_.X) <= Mathf.Abs(Taille.X) && Mathf.Abs(taille_.Y) <= Mathf.Abs(Taille.Y); }
    //     public bool PeutContenir(RectangleI boite_) => PeutContenir(boite_.Taille);
    //
    //     public RectangleI Tourner()
    //     { return new(Position, new(Taille.Y, Taille.X)); }
    //
    //     public bool Croise(RectangleI boite_)
    //     {
    //         // Calculer si deux boites se croisent
    //         
    //     }
    // }
    
    private class Boite2DComparer : IComparer<RectangleI>
    {
        private GrilleRangés<T> Grille;

        public Boite2DComparer(GrilleRangés<T> grille_)
        { Grille = grille_; }
        
        public int Compare(RectangleI b0, RectangleI b1)
        {
            if (b0 == b1)
            { return 0; }
            
            return Grille.EstPrioritaire(b0, b1) ? 1 : -1;
        }
    }
    
    public class TypeRangement
    {
        public enum DirectionTri { BasGauche, GaucheBas, GaucheHaut, HautGauche, HautDroite, DroiteHaut, DroiteBas, BasDroite }
        public enum OrdreTri { Croissant, Décroissant }

        public readonly DirectionTri Direction;
        public readonly OrdreTri Ordre;

        public TypeRangement(): this(DirectionTri.BasGauche, OrdreTri.Décroissant) {}
        public TypeRangement(DirectionTri direction_, OrdreTri ordre_)
        {
            Direction = direction_;
            Ordre = ordre_;
        }
        
        
    }
    
    // private class EspaceRangement
    // {
    //     private GrilleObjetsRangés<T> Grille;
    //     private EspaceRangement Parent;
    //     public Vector2I Position { get; private set; }
    //     public Vector2I Taille { get; private set; }
    //     //public bool Sens { get; private set; }
    //     public bool Plein { get; private set; }
    //         
    //     private List<Boite> Boites;
    //     private List<EspaceRangement> SousEspaces;
    //
    //     public EspaceRangement(GrilleObjetsRangés<T> grille_, EspaceRangement parent_, Vector2I position_/*, Vector2I taille_, bool sens_*/)
    //     {
    //         Grille = grille_;
    //         Parent = parent_;
    //         Position = position_;
    //         // Calculer la taille par rapport au type de rangement et la taille de la grille
    //         Taille = Vector2I.Zero;
    //         //Sens = sens_;
    //         Boites = new();
    //     }
    //
    //     public bool Ajouter(Boite boite_)
    //     {
    //         // vérifier que la boite n'est pas plus grande que l'espace
    //         if (boite_.Taille[(int)Grille.Rangement.Direction] <= Taille[(int)Grille.Rangement.Direction])
    //         {
    //             // si la boite fait la même taille que l'espace on tente de l'ajouter
    //             if (TailleIdentique(boite_))
    //             {
    //                 // on trouve l'emplacement où il doit être ajouté
    //                 int indexInsertion = CalculerIndexNouvelleBoite(boite_);
    //                 // on ajoute la boite et on décale les autres boites se trouvant après la nouvelle boite
    //                 Boites.Insert(indexInsertion, boite_);
    //                 for (int b = indexInsertion + 1; b < Boites.Count; ++b)
    //                 { DécalerBoite(b, boite_.Taille[(int)Grille.Rangement.Direction]); }
    //                 // Réduire la taille des sous-espaces, voir les supprimés s'il ne reste pas de place
    //                 // les boites ne tenant plus dans les sous-espaces sont ajoutés à une liste d'attente
    //                 // les sous-espaces devenus vides doivent être vidés et ajouter les boites enfants à la liste d'attente
    //                 // regrouper les sous-espaces vides ensembles
    //                 // essayer de rajouter les boites en file d'attentes dans les sous-espaces restants/recombinés
    //                 // tenter de tourner les boites si l'option est activée
    //                 // Sinon, les garder en attente
    //             }
    //         }
    //         
    //         if (PeutContenir(boite_))
    //         {
    //             // si la boite fait la même taille que l'espace on tente de l'ajouter
    //             if (TailleIdentique(boite_))
    //             {
    //                 // on trouve l'emplacement où il doit être ajouté
    //                 int indexInsertion = CalculerIndexNouvelleBoite(boite_);
    //                 // on ajoute la boite et on décale les autres boites se trouvant après la nouvelle boite
    //                 // Réduire la taille des sous-espaces, voir les supprimés s'il ne reste pas de place
    //                 // les boites netenant plus dans les sous-espaces sont ajoutés à une liste d'attente avant d'être réajouté à la grille
    //             }
    //
    //             // on essaye de l'ajouter dans un sous-espace
    //             if (SousEspaces.Count > 0)
    //             {
    //                 foreach (EspaceRangement sousEspace in SousEspaces)
    //                 {
    //                     if (sousEspace.Ajouter(boite_))
    //                     {
    //                         return true;
    //                     } // on a réussi à ajouter la boite dans le sous-espace
    //                     // il n'y avait plus de place pour la boite dans le sous-espace, on passe au suivant
    //                 }
    //
    //                 // sinon on l'ajoute dans l'espace s'il y a de la place
    //                 if (EspaceLibre() >= boite_.Taille[(int)Grille.Rangement.Direction])
    //                 {
    //                     // on trouve l'emplacement où il doit être ajouté
    //                     int indexInsertion = CalculerIndexNouvelleBoite(boite_);
    //                     // on ajoute la boite et on décale les autres boites se trouvant après la nouvelle boite
    //                     if (PeutRentrer(indexInsertion, boite_))
    //                     {
    //                         // si la boite peut rentrer
    //                     }
    //                     Boites.Insert(indexInsertion, boite_);
    //                     for (int b = indexInsertion + 1; b < Boites.Count; ++b)
    //                     { DécalerBoite(b, boite_.Taille[(int)Grille.Rangement.Direction]); }
    //                     // on réduit la taille des sous-espaces
    //                     for (int se = 0; se < SousEspaces.Count; ++se)
    //                     {
    //                         SousEspaces[se]
    //                     }
    //                 }
    //
    //                 // la boite ne peut pas être ajouté dans cet espace ou les sous-espaces donc on remonte dans l'arborescence
    //                 return false;
    //             }
    //             // si la boite fait la même taille que l'espace, on l'ajoute à l'espace
    //             // si elle est plus petite, créer les deux premiers sous-espaces et ajout la boite au premier
    //         }
    //         return false;
    //     }
    //
    //
    //     private bool DécalerBoite(int index_, int décalage_)
    //     {
    //         // change la position de la boite si elle ne sort pas de l'espace
    //         Boite boite = Boites[index_];
    //     }
    //
    //     /// <summary>
    //     /// Calcule l'espace théoriquement disponible dans l'espace à partir d'une position
    //     /// </summary>
    //     /// <param name="position_">la position dans l'espace dans le sens de rangement</param>
    //     /// <returns></returns>
    //     private int EspaceDisponible(int position_)
    //     { return Taille[(int)Grille.Rangement.Direction] - position_; }
    //
    //     private bool PeutRentrer(int position_, Boite boite_)
    //     { return position_ + boite_.Taille[(int)Grille.Rangement.Direction] <= Taille[(int)Grille.Rangement.Direction]; }
    //
    //     private int CalculerIndexNouvelleBoite(Boite boite_)
    //     {
    //         int index = Boites.BinarySearch(boite_, Comparer<Boite>.Create((b1, b2) => b1.Taille[(int)Grille.Rangement.Direction].CompareTo(b2.Taille[(int)Grille.Rangement.Direction])));
    //         return index < 0 ? ~index : index;
    //     }
    //
    //     private void InitialiserTaille()
    //     {
    //         if (Boites.Count > 0)
    //         {
    //             switch (Grille.Rangement.Direction)
    //             {
    //                 case TypeRangement.DirectionTri.Horizontale:
    //                     Taille = new(Grille.Taille.X - Position.X, Boites[0].Taille.Y);
    //                     break;
    //                 case TypeRangement.DirectionTri.Verticale:
    //                     Taille = new(Boites[0].Taille.X, Grille.Taille.Y - Position.Y);
    //                     break;
    //             }
    //         }
    //     }
    //
    //     public bool PeutContenir(Boite boite_)
    //     {
    //         if (EspaceLibre() >= boite_.Taille[(int)Grille.Rangement.Direction])
    //         {
    //             switch (Grille.Rangement.Direction)
    //             {
    //                 case TypeRangement.DirectionTri.Horizontale:
    //                     return boite_.Taille.Y <= Taille.Y;
    //                 case TypeRangement.DirectionTri.Verticale:
    //                     return boite_.Taille.X <= Taille.X;
    //             }
    //         }
    //         return false;
    //     }
    //
    //     private bool TailleIdentique(Boite boite_)
    //     {
    //         switch (Grille.Rangement.Direction)
    //         {
    //             case TypeRangement.DirectionTri.Horizontale:
    //                 return boite_.Taille.Y == Taille.Y;
    //             case TypeRangement.DirectionTri.Verticale:
    //                 return boite_.Taille.X == Taille.X;
    //         }
    //     }
    //
    //     public int EspaceOccupé()
    //     {
    //         // TODO: éventuellement optimiser en stockant la place occupée dans une variable pour les cas où il y ai beaucoup de boites dans un espace
    //         int mesure = 0;
    //         foreach (Boite boite in Boites)
    //         { mesure += boite.Taille[(int)Grille.Rangement.Direction]; }
    //         return mesure;
    //     }
    //
    //     public int EspaceLibre()
    //     { return Taille[(int)Grille.Rangement.Direction] - EspaceOccupé(); }
    // }
    
    // private class Rangement
    // {
    //     private readonly struct Boite
    //     {
    //         public readonly T Objet;
    //         public readonly Vector2I Position;
    //         public readonly Vector2I Taille;
    //     }
    //
    //     private class Espace
    //     {
    //         private Rangement Intendant;
    //         private Espace Parent;
    //         public Vector2I Position { get; private set; }
    //         public Vector2I Taille { get; private set; }
    //         public DirectionTri Direction { get; private set; }
    //         public bool Sens { get; private set; }
    //         public bool Plein { get; private set; }
    //         
    //         private List<Boite> Boites;
    //         private List<Espace> SousEspaces;
    //
    //         public Espace(Rangement intendant_, Espace parent_, Vector2I position_, Vector2I taille_, DirectionTri direction_, bool sens_)
    //         {
    //             Intendant = intendant_;
    //             Parent = parent_;
    //             Position = position_;
    //             Taille = taille_;
    //             Direction = direction_;
    //             Sens = sens_;
    //             Boites = new();
    //         }
    //
    //         public bool Ajouter(Boite boite_)
    //         {
    //             foreach (Espace sousEspace in SousEspaces)
    //             {
    //                 if (!sousEspace.Plein && sousEspace.PeutContenir(boite_))
    //                 { return sousEspace.Ajouter(boite_); }
    //             }
    //         }
    //
    //         public bool PeutContenir(Boite boite_)
    //         {
    //             switch (Direction)
    //             {
    //                 case DirectionTri.Horizontale:
    //                     return boite_.Taille.Y <= Taille.Y;
    //                 case DirectionTri.Verticale:
    //                     return boite_.Taille.X <= Taille.X;
    //             }
    //         }
    //     }
    //     
    //     
    //     public enum OrigineTri { BasGauche, Gauche, HautGauche, Haut, HautDroite, Droite, BasDroite, Bas }
    //
    //     public enum DirectionTri { Horizontale, Verticale }
    //     public enum OrdreTri { Croissant, Décroissant }
    //     //public enum TriHorizontal { Gauche, Centre, Droite }
    //     //public enum TriVertical { Bas, Centre, Haut }
    //
    //     private OrigineTri Origine;
    //     private DirectionTri Direction;
    //     private OrdreTri Ordre;
    //     //private bool Croissant;
    //     //private bool Horizontal, Vertical, Croissant;
    //     private GrilleObjets<T> Grille;
    //     private Espace Racine;
    //
    //     public Rangement() : this(OrigineTri.BasGauche, DirectionTri.Horizontale, OrdreTri.Croissant) {}
    //     public Rangement(OrigineTri origine_, DirectionTri direction_, OrdreTri ordre_)
    //     {
    //         Grille = null;
    //         Racine = null;
    //         Origine = origine_;
    //         Direction = direction_;
    //         Ordre = ordre_;
    //     }
    //
    //     public void DefGrille(GrilleObjets<T> grille_)
    //     {
    //         Grille = grille_;
    //         Racine = new(this, null, CalculerOrigine(Origine), Grille.Taille, Direction, );
    //     }
    //
    //     private Vector2I CalculerOrigine()
    //     {
    //         switch (Origine)
    //         {
    //             case OrigineTri.Bas:
    //                 return new Vector2I(Grille.Taille.X / 2, 0);
    //             default:
    //             case OrigineTri.BasGauche:
    //                 return new Vector2I(0, 0);
    //             case OrigineTri.Gauche:
    //                 return new Vector2I(0, Grille.Taille.Y / 2);
    //             case OrigineTri.HautGauche:
    //                 return new Vector2I(0, Grille.Taille.Y);
    //             case OrigineTri.Haut:
    //                 return new Vector2I(Grille.Taille.X / 2, Grille.Taille.Y);
    //             case OrigineTri.HautDroite:
    //                 return new Vector2I(Grille.Taille.X, Grille.Taille.Y);
    //             case OrigineTri.Droite:
    //                 return new Vector2I(Grille.Taille.X, Grille.Taille.Y / 2);
    //             case OrigineTri.BasDroite:
    //                 return new Vector2I(Grille.Taille.X, 0);
    //         }
    //     }
    //
    //     public bool Ajouter(T objet_, Vector2I taille_)
    //     {
    //         
    //     }
    // }


    private Grille<Objet> Grille;
    //private List<Objet> Boites;                             // Trier les espaces par leur position, de gauche à droite puis de bas en haut
    private List<RectangleI> EmplacementsLibres;      // Stocke les positions disponibles dans la grille
    private RectangleI EspaceUtilisé;
    private Boite2DComparer TrieurEmplacements;
    private TypeRangement Rangement;
    private readonly float[] FiltrePoids = { 1f, 1f, 0.5f, 0.5f };
    public int Largeur => Grille.Largeur;
    public int Hauteur => Grille.Hauteur;

    public GrilleRangés(Vector2I taille_) : this(taille_.X, taille_.Y, new()) {}
    public GrilleRangés(Vector2I taille_, TypeRangement rangement_) : this(taille_.X, taille_.Y, rangement_) {}

    public GrilleRangés(int largeur_, int hauteur_, TypeRangement rangement_ = null)//: base(largeur_, hauteur_)
    {
        Grille = new Grille<Objet>(largeur_, hauteur_);
        EmplacementsLibres = new();
        EnregistrerEmplacementLibre(new(largeur_, hauteur_));
        EspaceUtilisé = new(0, 0, 0, 0);
        TrieurEmplacements = new(this);
        Rangement = rangement_;
    }

    private Vector2I PositionObjet(Objet objet_) => PositionBoite(objet_.Boite);

    private Vector2I PositionBoite(RectangleI boite_)
    {
        return (Rangement.Direction switch
        {
            TypeRangement.DirectionTri.BasGauche or TypeRangement.DirectionTri.GaucheBas => new(boite_.Gauche, boite_.Bas),
            TypeRangement.DirectionTri.GaucheHaut or TypeRangement.DirectionTri.HautGauche => new(boite_.Gauche, boite_.Haut),
            TypeRangement.DirectionTri.HautDroite or TypeRangement.DirectionTri.DroiteHaut => new(boite_.Droite, boite_.Haut),
            TypeRangement.DirectionTri.DroiteBas or TypeRangement.DirectionTri.BasDroite => new(boite_.Droite, boite_.Bas),
            _ => throw new InvalidOperationException("La direction de Tri n'est pas valide")
        });
    }

    private RectangleI PlacerBoiteDansEmplacement(RectangleI emplacement_, RectangleI boite_)
    {
        return (Rangement.Direction switch
        {
            TypeRangement.DirectionTri.BasGauche or TypeRangement.DirectionTri.GaucheBas
                => new(emplacement_.Bas, emplacement_.Gauche, emplacement_.Bas + boite_.Taille.Y - 1, emplacement_.Gauche + boite_.Taille.X - 1),
            TypeRangement.DirectionTri.GaucheHaut or TypeRangement.DirectionTri.HautGauche
                => new(emplacement_.Haut - boite_.Taille.Y - 1, emplacement_.Gauche, emplacement_.Haut, emplacement_.Gauche + boite_.Taille.X - 1),
            TypeRangement.DirectionTri.HautDroite or TypeRangement.DirectionTri.DroiteHaut
                => new(emplacement_.Haut - boite_.Taille.Y - 1, emplacement_.Droite - boite_.Taille.X - 1, emplacement_.Haut, emplacement_.Droite),
            TypeRangement.DirectionTri.DroiteBas or TypeRangement.DirectionTri.BasDroite
                => new(emplacement_.Bas, emplacement_.Droite - boite_.Taille.X - 1, emplacement_.Bas + boite_.Taille.Y - 1, emplacement_.Droite),
            _ => throw new InvalidOperationException("La direction de Tri n'est pas valide")
        });
    }

    private void TournerBoite(ref RectangleI boite_)
    {
        switch (Rangement.Direction)
        {
            case TypeRangement.DirectionTri.BasGauche or TypeRangement.DirectionTri.GaucheBas:
                boite_.Tourner(boite_.GaucheBas);
                break;
            case TypeRangement.DirectionTri.GaucheHaut or TypeRangement.DirectionTri.HautGauche:
                boite_.Tourner(boite_.GaucheHaut);
                break;
            case TypeRangement.DirectionTri.HautDroite or TypeRangement.DirectionTri.DroiteHaut:
                boite_.Tourner(boite_.DroiteHaut);
                break;
            case TypeRangement.DirectionTri.DroiteBas or TypeRangement.DirectionTri.BasDroite:
                boite_.Tourner(boite_.DroiteBas);
                break;
        }
    }

    public bool Ajouter(T objet_, Vector2I taille_, bool rotation_, bool option_)
    {
        // orienter la taille par rapport à la direction de tri de la grille
        RectangleI boiteObjet = new(taille_.Abs());
        RectangleI boiteATester = new(boiteObjet.Taille), boiteATesterTourné = boiteATester;
        TournerBoite(ref boiteATesterTourné);
        (int indexEmplacement, RectangleI boite) positionIdéale = (-1, null), positionATester0 = (-1, null), positionATester1 = (-1, null);
        bool pos0 = true, pos1 = true;
        
        if (option_)
        {
            // option 1 //
            // Trouver les deux meilleurs emplacements libre correspondants aux deux rotations de l'objet
            TournerBoite(ref boiteATesterTourné);
            for (int e = 0; e < EmplacementsLibres.Count; ++e)
            {
                RectangleI emplacement = EmplacementsLibres[e];
                if (pos0 && emplacement.PeutContenir(boiteATester))
                {
                    positionATester0 = (e, PlacerBoiteDansEmplacement(emplacement, boiteATester));
                    //positionsPossibles.Enqueue((e, PlacerBoiteDansEmplacement(emplacement, boiteATester)));
                    pos0 = false;
                    if (!pos1)
                    { break; }
                }

                if (pos1 && rotation_ && emplacement.PeutContenir(boiteATesterTourné))
                {
                    positionATester1 = (e, PlacerBoiteDansEmplacement(emplacement, boiteATesterTourné));
                    //positionsPossibles.Enqueue((e, PlacerBoiteDansEmplacement(emplacement, boiteATesterTourné)));
                    pos1 = false;
                    if (!pos0)
                    { break; }
                }
                
            }

            // calculer le poids de l'objet dans ces deux emplacements
            // les comparer et choisir le meilleur
            //if (positionIdéale.indexEmplacement < 0 || (!pos1 && EstPrioritaire(positionAComparer.boite, positionIdéale.boite)))
            if (!pos0 && !pos1)
            { positionIdéale = EstPrioritaire(positionATester0.boite, positionATester1.boite) ? positionATester0 : positionATester1; }
        }
        else
        {
            // option 2 //
            // calculer le poids de l'objet dans chacun des emplacement libre et dans chaque rotation
            for (int e = 0; e < EmplacementsLibres.Count; ++e)
            {
                RectangleI emplacement = EmplacementsLibres[e];
                (int indexEmplacement, RectangleI boite) temp = (-1, null);
                if (emplacement.PeutContenir(boiteATester))
                {
                    positionATester0 = (e, PlacerBoiteDansEmplacement(emplacement, boiteATester));
                    pos0 = false;
                }

                if (rotation_ && emplacement.PeutContenir(boiteATesterTourné))
                {
                    positionATester1 = (e, PlacerBoiteDansEmplacement(emplacement, boiteATesterTourné));
                    pos1 = false;
                }

                if (!pos0 && !pos1)
                { temp = EstPrioritaire(positionATester0.boite, positionATester1.boite) ? positionATester0 : positionATester1; }
                else
                { temp = !pos0 ? positionATester0 : !pos1 ? positionATester1 : (-1, null); }
                
                if (temp.indexEmplacement >= 0 && (positionIdéale.indexEmplacement < 0 || EstPrioritaire(temp.boite, positionIdéale.boite)))
                { positionIdéale = temp; }
            }
        }

        // remplacer l'emplacement par l'objet
        RemplacerEmplacementLibre(positionIdéale.indexEmplacement, new(positionIdéale.boite, rotation_));
        // étendre l'espace utilisé par le nouvel objet
        VérifierEspaceUtilisé(positionIdéale.boite);

        return false;
    }

    private void VérifierEspaceUtilisé(RectangleI nouvelleBoite_)
    {
        EspaceUtilisé.Bas = EspaceUtilisé.Bas > nouvelleBoite_.Bas ? nouvelleBoite_.Bas : EspaceUtilisé.Bas;
        EspaceUtilisé.Gauche = EspaceUtilisé.Gauche > nouvelleBoite_.Gauche ? nouvelleBoite_.Gauche : EspaceUtilisé.Gauche;
        EspaceUtilisé.Haut = EspaceUtilisé.Haut < nouvelleBoite_.Haut ? nouvelleBoite_.Haut : EspaceUtilisé.Haut;
        EspaceUtilisé.Droite = EspaceUtilisé.Droite < nouvelleBoite_.Droite ? nouvelleBoite_.Droite : EspaceUtilisé.Droite;
    }

    // private RectangleI OrienterBoite(RectangleI boite_)
    // {
    //     Vector2I inversions = new(1, 1);
    //     switch (Rangement.Direction)
    //     {
    //         case TypeRangement.DirectionTri.BasGauche:
    //         case TypeRangement.DirectionTri.GaucheBas:
    //             inversions.X = boite_.Taille.X > 0 ? 1 : -1;
    //             inversions.Y = boite_.Taille.Y > 0 ? 1 : -1;
    //             break;
    //         case TypeRangement.DirectionTri.BasDroite:
    //         case TypeRangement.DirectionTri.DroiteBas:
    //             inversions.X = boite_.Taille.X > 0 ? -1 : 1;
    //             inversions.Y = boite_.Taille.Y > 0 ? 1 : -1;
    //             break;
    //         case TypeRangement.DirectionTri.HautGauche:
    //         case TypeRangement.DirectionTri.GaucheHaut:
    //             inversions.X = boite_.Taille.X > 0 ? 1 : -1;
    //             inversions.Y = boite_.Taille.Y > 0 ? -1 : 1;
    //             break;
    //         case TypeRangement.DirectionTri.HautDroite:
    //         case TypeRangement.DirectionTri.DroiteHaut:
    //             inversions.X = boite_.Taille.X > 0 ? -1 : 1;
    //             inversions.Y = boite_.Taille.Y > 0 ? -1 : 1;
    //             break;
    //     }
    //     return new(boite_.Position * inversions, boite_.Taille * inversions);
    // }
    
    private Vector2I VecteurDirection()
    {
        return (Rangement.Direction switch
        {
            TypeRangement.DirectionTri.BasGauche or TypeRangement.DirectionTri.GaucheBas => new(1, 1),
            TypeRangement.DirectionTri.BasDroite or TypeRangement.DirectionTri.DroiteBas => new(-1, 1),
            TypeRangement.DirectionTri.HautGauche or TypeRangement.DirectionTri.GaucheHaut => new(1, -1),
            TypeRangement.DirectionTri.HautDroite or TypeRangement.DirectionTri.DroiteHaut => new(-1, -1),
            _ => throw new InvalidOperationException("La direction de Tri n'est pas valide")
        });
    }

    // private RectangleI CalculerEmplacementLibre(Vector2I position_)
    // {
    //     // Calcule la taille de l'emplacement libre
    //     Vector2I tailleEmplacement = Vector2I.One;
    //     Vector2I décalage = new(1, 0);
    //     while (Grille[position_ + décalage] == null)
    //     { ++décalage.X; }
    //     tailleEmplacement.X = décalage.X;
    //     
    //     décalage = new(0, 1);
    //     while (Grille[position_ + décalage] == null)
    //     { ++décalage.Y; }
    //     tailleEmplacement.Y = décalage.Y;
    //
    //     return new(position_.Y, position_.X, position_.Y + tailleEmplacement.Y - 1, position_.X + tailleEmplacement.X - 1);
    // }

    private void EnregistrerEmplacementLibre(RectangleI nouvelEmplacement_)
    {
        // positionner l'emplacement dans la liste
        if (!FusionnerEmplacements(nouvelEmplacement_))
        {
            int index = EmplacementsLibres.BinarySearch(nouvelEmplacement_, TrieurEmplacements);
            index = index < 0 ? ~index : index;
            EmplacementsLibres.Insert(index, nouvelEmplacement_);
        }
    }

    /// <summary>
    /// Vérifie si le nouvel emplacement ne peut pas être fusionner avec un existant
    /// </summary>
    /// <param name="nouvelEmplacement_"></param>
    private bool FusionnerEmplacements(RectangleI nouvelEmplacement_)
    {
        bool fusion = false;
        for (int e = 0; e < EmplacementsLibres.Count; ++e)
        {
            if (EmplacementsLibres[e].Bas == nouvelEmplacement_.Bas && EmplacementsLibres[e].Haut == nouvelEmplacement_.Haut)
            {
                if (nouvelEmplacement_.Gauche <= EmplacementsLibres[e].Droite + 1 && nouvelEmplacement_.Droite > EmplacementsLibres[e].Droite) // étendre à droite
                {
                    EmplacementsLibres[e].Droite = nouvelEmplacement_.Droite;
                    fusion = true;
                }
                else if (nouvelEmplacement_.Gauche < EmplacementsLibres[e].Gauche && nouvelEmplacement_.Droite >= EmplacementsLibres[e].Gauche - 1) // étendre à gauche
                {
                    EmplacementsLibres[e].Gauche = nouvelEmplacement_.Gauche;
                    fusion = true;
                }
            }
            else if (EmplacementsLibres[e].Bas == nouvelEmplacement_.Bas && EmplacementsLibres[e].Haut == nouvelEmplacement_.Haut)
            {
                if (nouvelEmplacement_.Bas <= EmplacementsLibres[e].Haut + 1 && nouvelEmplacement_.Haut > EmplacementsLibres[e].Haut) // étendre à droite
                {
                    EmplacementsLibres[e].Haut = nouvelEmplacement_.Haut;
                    fusion = true;
                }
                else if (nouvelEmplacement_.Bas < EmplacementsLibres[e].Bas && nouvelEmplacement_.Haut >= EmplacementsLibres[e].Bas - 1) // étendre à gauche
                {
                    EmplacementsLibres[e].Bas = nouvelEmplacement_.Bas;
                    fusion = true;
                }
            }
        }
        return fusion;
    }

    private bool RetirerEmplacementLibre(RectangleI emplacement_)
    {
        int index = EmplacementsLibres.BinarySearch(emplacement_, TrieurEmplacements);
        if (index >= 0)
        {
            EmplacementsLibres.RemoveAt(index);
            return true;
        }
        return false;
    }

    private void RemplacerEmplacementLibre(int indexEmplacement_, Objet objet_)
    {
        // Remplace un emplacement libre par un nouvel Objet
        //RectangleI ancienEmplacement = EmplacementsLibres[indexEmplacement_];
        
        if (Grille.Ajouter(objet_, objet_.Boite))
        {
            EmplacementsLibres.RemoveAt(indexEmplacement_);
            // Trouver les emplacements croisés par la nouvelle boite et tronque les
            foreach (RectangleI emplacement in EmplacementsLibres.FindAll(e => objet_.Boite.Croise(e)))
            { DiviserEmplacement(emplacement, objet_.Boite); }
            // Calcule les emplacements disponibles créés par l'ajout du rectangle
            foreach (Vector2I positionEmplacement in PositionsLibresCôtés(objet_))
            {
                foreach (RectangleI nouvelEmplacement in MesurerEmplacements(positionEmplacement))
                { EnregistrerEmplacementLibre(nouvelEmplacement); }
            }
        }

        throw new Exception($"La grille ne peut pas contenir l'objet à cet emlacement, il y a une erreur!!");
    }

    private bool RentreDansLaGrille(Vector2I position_, Vector2I taille_)
    { return position_.X >= 0 && position_.X + taille_.X <= Largeur && position_.Y >= 0 && position_.Y + taille_.Y <= Hauteur; }

    private RectangleI Comparer(RectangleI boite0_, RectangleI boite1_)
    { return SommePoids(CalculerPoids(boite0_)) >= SommePoids(CalculerPoids(boite1_)) ? boite0_ : boite1_; }
    
    private bool EstPrioritaire(RectangleI boite0_, RectangleI boite1_)
    { return SommePoids(CalculerPoids(boite0_)) >= SommePoids(CalculerPoids(boite1_)); }

    private float SommePoids(float[] poids_)
    {
        float somme = 0f;
        for (int p = 0; p < poids_.Length; ++p)
        { somme += poids_[p] * FiltrePoids[p]; }
        return somme;
    }

    private int DimensionDirection(RectangleI boite_)
    {
        switch (Rangement.Direction)
        {
            case TypeRangement.DirectionTri.BasGauche:
            case TypeRangement.DirectionTri.BasDroite:
            case TypeRangement.DirectionTri.HautGauche:
            case TypeRangement.DirectionTri.HautDroite:
                return boite_.Taille.Y;
            case TypeRangement.DirectionTri.GaucheHaut:
            case TypeRangement.DirectionTri.GaucheBas:
            case TypeRangement.DirectionTri.DroiteBas:
            case TypeRangement.DirectionTri.DroiteHaut:
                return boite_.Taille.X;
        }

        throw new Exception($"Enum incorrect...");
    }

    private float[] CalculerPoids(RectangleI boite_)
    {
        float[] poids = new float[4];
        switch (Rangement.Direction)
        {
            case TypeRangement.DirectionTri.BasGauche:
                Array.Copy(CalculerPoids(boite_, 0), 0, poids, 0, 2);
                Array.Copy(CalculerPoids(boite_, 1), 0, poids, 2, 2);
                break;
            case TypeRangement.DirectionTri.BasDroite:
                Array.Copy(CalculerPoids(boite_, 0), 0, poids, 0, 2);
                Array.Copy(CalculerPoids(boite_, 3), 0, poids, 2, 2);
                break;
            case TypeRangement.DirectionTri.HautGauche:
                Array.Copy(CalculerPoids(boite_, 2), 0, poids, 0, 2);
                Array.Copy(CalculerPoids(boite_, 1), 0, poids, 2, 2);
                break;
            case TypeRangement.DirectionTri.HautDroite:
                Array.Copy(CalculerPoids(boite_, 2), 0, poids, 0, 2);
                Array.Copy(CalculerPoids(boite_, 3), 0, poids, 2, 2);
                break;
            case TypeRangement.DirectionTri.GaucheHaut:
                Array.Copy(CalculerPoids(boite_, 1), 0, poids, 0, 2);
                Array.Copy(CalculerPoids(boite_, 2), 0, poids, 2, 2);
                break;
            case TypeRangement.DirectionTri.GaucheBas:
                Array.Copy(CalculerPoids(boite_, 1), 0, poids, 0, 2);
                Array.Copy(CalculerPoids(boite_, 0), 0, poids, 2, 2);
                break;
            case TypeRangement.DirectionTri.DroiteBas:
                Array.Copy(CalculerPoids(boite_, 3), 0, poids, 0, 2);
                Array.Copy(CalculerPoids(boite_, 0), 0, poids, 2, 2);
                break;
            case TypeRangement.DirectionTri.DroiteHaut:
                Array.Copy(CalculerPoids(boite_, 3), 0, poids, 0, 2);
                Array.Copy(CalculerPoids(boite_, 2), 0, poids, 2, 2);
                break;
        }
        return poids;
    }
    
    private float[] CalculerPoids(RectangleI boite_, int direction_)
    {
        switch (direction_)
        {
            case 0: // Bas
                return new[]
                    {
                        1f - (float)(boite_.Bas / Hauteur),
                        1f - (float)((boite_.Haut + 1) / Hauteur)
                    };
            case 1: // Gauche
                return new[]
                    {
                        1f - (float)(boite_.Gauche / Largeur), 
                        1f - (float)((boite_.Droite + 1) / Largeur)
                    };
            case 2: // Haut
                return new[]
                    {
                        (float)((boite_.Haut + 1) / Hauteur),
                        (float)(boite_.Bas / Hauteur)
                    };
            case 3: // Droite
                return new[]
                    {
                        (float)((boite_.Droite + 1) / Largeur),
                        (float)(boite_.Gauche / Largeur)
                    };
        }
        throw new ArgumentException($"la direction fournie [{direction_}] n'est pas valide");
    }
    
    // /// <summary>
    // /// Cherche la première case libre dans une direction
    // /// </summary>
    // /// <param name="départ_">La case de départ</param>
    // /// <param name="axe_">L'axe X ou Y</param>
    // /// <param name="direction_">La direction, droite/gauche ou haut/bas</param>
    // /// <returns></returns>
    // private (bool Réussite, Vector2I Position) PremièrePositionLibre(RectangleI boite_, int côté_)
    // {
    //     // TODO: réécrire en utilisant un Rectangle
    //     
    //     // Trouver la case libre de chaque côté du rectangle.
    //     // éliminer les cases déjà comprises dans un emplacement libre
    //     // calculer la largeur ou hauteur du nouvel espace depuis ce point et en fonction de l'ordre de tri (Bas = X, Gauche = Y)
    //     // décaler la position sur l'axe opposé dans la direction correspondant à l'ordre de tri, tant que la taille ne réduit pas
    //     
    //     
    //     
    //     // Vecteurs.GarderDansPlage(ref départ_, new(0, Largeur - 1), new(0, Hauteur - 1));
    //     // int axe = axe_ ? 0 : 1;
    //     // int côté = axe_ ? direction_ ? 3 : 1 : direction_ ? 2 : 0;
    //     // Vector2I positionATester = départ_;
    //     // bool bordure = false;
    //     //
    //     // while (!bordure)
    //     // {
    //     //     Objet caseATester = Grille[positionATester];
    //     //     if (caseATester == null)
    //     //     { return (true, départ_); }
    //     //
    //     //     positionATester[axe] += caseATester.Boite.Taille[axe];
    //     //     
    //     //     bordure = positionATester[axe] >= Grille.Taille[axe];
    //     // }
    //     // return (false, new Vector2I(-1, -1));
    // }

    /// <summary>
    /// Cherche la première case libre sur le côté d'un objet
    /// </summary>
    /// <param name="objet_">L'objet autour du quel chercher</param>
    /// <param name="axe_">L'axe X ou Y</param>
    /// <param name="direction_">La direction, gauche/droite ou haut/bas</param>
    /// <returns></returns>
    private List<Vector2I> PositionsLibresCôtés(Objet objet_)
    {
        List<Vector2I> emplacementsLibres = new();
        Vector2I[] positionsATester = (Rangement.Direction switch
        {
            TypeRangement.DirectionTri.BasGauche or TypeRangement.DirectionTri.GaucheBas =>
                new[]
                {
                    new Vector2I(objet_.Boite.Gauche, objet_.Boite.Haut + 1),
                    new Vector2I(objet_.Boite.Droite + 1, objet_.Boite.Bas)
                },
            TypeRangement.DirectionTri.BasDroite or TypeRangement.DirectionTri.DroiteBas =>
                new[]
                {
                    new Vector2I(objet_.Boite.Droite, objet_.Boite.Haut + 1),
                    new Vector2I(objet_.Boite.Gauche - 1, objet_.Boite.Bas)
                },
            TypeRangement.DirectionTri.HautGauche or TypeRangement.DirectionTri.GaucheHaut =>
                new[]
                {
                    new Vector2I(objet_.Boite.Gauche, objet_.Boite.Bas - 1),
                    new Vector2I(objet_.Boite.Droite + 1, objet_.Boite.Haut)
                },
            TypeRangement.DirectionTri.HautDroite or TypeRangement.DirectionTri.DroiteHaut =>
                new[]
                {
                    new Vector2I(objet_.Boite.Droite, objet_.Boite.Bas - 1),
                    new Vector2I(objet_.Boite.Gauche - 1, objet_.Boite.Haut)
                },
            _ => throw new InvalidOperationException("La direction de Tri n'est pas valide")
        });

        Vector2I vecDir = VecteurDirection();
        for (int i = 0; i < 2; ++i)
        {
            for (int p = 0; p < Mathf.Abs(objet_.Boite.Taille[i]); ++p)
            {
                if (Grille[positionsATester[i]] == null)
                {
                    emplacementsLibres.Add(positionsATester[i]);
                    break;
                }
                positionsATester[i][i] += vecDir[i];
            }
        }
        return emplacementsLibres;
    }

    private RectangleI[] MesurerEmplacements(Vector2I origine)
    {
        List<RectangleI> emplacementsDisponibles = new();
        // RectangleI nouvelEmplacement = new(origine.Y, origine.X, origine.Y, origine.X);
        Vector2I vecDir = VecteurDirection(), vec = origine, côtésAModifier = new(vecDir.X > 0 ? 3 : 1, vecDir.Y > 0 ? 2 : 0);
        int[] ordresAxes = OrdreAxesMesureEmplacement();
        RectangleI zoneACalculer = new(
            Mathf.Min(origine.Y, Grille.Hauteur),
            Mathf.Min(origine.X, Grille.Largeur),
            Mathf.Max(origine.Y, Grille.Hauteur),
            Mathf.Max(origine.X, Grille.Largeur));
        List<Vector2I> limites = new();
        
        
        // en fonction de la direction, calculer la largeur ou hauteur de l'emplacement
        for (; vec[ordresAxes[1]] != zoneACalculer[côtésAModifier[ordresAxes[1]]]; vec[ordresAxes[1]] += vecDir[ordresAxes[1]])
        {
            Vector2I limite = new(zoneACalculer[côtésAModifier.X], zoneACalculer[côtésAModifier.Y]);
            if (SortieEspaceUtilisé(vec))
            {
                limite[ordresAxes[1]] = vec[ordresAxes[1]];
                limites.Add(limite);
                break;
            }
            
            for (vec[ordresAxes[0]] = origine[ordresAxes[1]]; vec[ordresAxes[0]] != zoneACalculer[côtésAModifier[ordresAxes[1]]]; vec[ordresAxes[0]] += vecDir[ordresAxes[1]])
            {
                if (SortieEspaceUtilisé(vec))
                {
                    limite[ordresAxes[1]] = vec[ordresAxes[1]];
                    break;
                }
                
                if (Grille[vec] != null)
                {
                    limite = vec;
                    break;
                }
            }

            if (limites.Count < 0 || limite[ordresAxes[1]] != limites[limites.Count - 1][ordresAxes[1]])
            {
                limites.Add(limite);
                if (limite[ordresAxes[0]] - origine[ordresAxes[0]] < zoneACalculer.Taille[ordresAxes[0]])
                { zoneACalculer[côtésAModifier[ordresAxes[0]]] = limite[ordresAxes[0]] - vecDir[ordresAxes[0]]; }
            }

            if (Grille[vec] != null)
            { break; }
        }
        limites.Add(vec); // dernière limite
        
        // Calculer les différents espaces avec les limites calculées
        // prendre la première limite
        while (limites.Count > 1)
        {
            Vector2I taille = Vector2I.One, diagonale = Vector2I.One;
            diagonale[ordresAxes[0]] = limites[0][ordresAxes[0]] - vecDir[ordresAxes[0]];
            limites.RemoveAt(0);
            // calculer sa distance sur l'axe 1 avec la prochaine limite dont la distance avec l'origine sur l'axe 0 est plus petit
            diagonale[ordresAxes[1]] = limites.First(l => Mathf.Abs(l[ordresAxes[0]] - origine[ordresAxes[0]]) < Mathf.Abs(limites[0][ordresAxes[0]] - origine[ordresAxes[0]]))[ordresAxes[1]] - vecDir[ordresAxes[1]];;
            // calculer la taille de l'emplacement
            emplacementsDisponibles.Add(new(
                Mathf.Min(origine.Y, diagonale.Y),
                Mathf.Min(origine.X, diagonale.X),
                Mathf.Max(origine.Y, diagonale.Y),
                Mathf.Max(origine.X, diagonale.X)));
        }
        return emplacementsDisponibles.ToArray();
    }

    /// <summary>
    /// Vérifie si la position sur un axe précis sors de l'espace utilisé dans la grille, dans une direction précise
    /// </summary>
    /// <param name="position_">la position sur l'axe</param>
    /// <param name="axe_">0 = X, 1 = Y</param>
    /// <param name="sens_">positif ou négatif</param>
    /// <returns>si la position est sortie de l'espace utilisé</returns>
    private bool SortieEspaceUtilisé(Vector2I position_)
    { return EspaceUtilisé.Contient(position_); }
    
    /// <summary>
    /// trie les axes X et Y par rapport au sens de calcule de la dimensions d'un emplacement dans la grille
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    private int[] OrdreAxesMesureEmplacement()
    {
        return (Rangement.Direction switch
        {
            TypeRangement.DirectionTri.BasGauche or TypeRangement.DirectionTri.BasDroite or TypeRangement.DirectionTri.HautGauche or TypeRangement.DirectionTri.HautDroite => new []{ 0, 1 },
            TypeRangement.DirectionTri.GaucheHaut or TypeRangement.DirectionTri.GaucheBas or TypeRangement.DirectionTri.DroiteHaut or TypeRangement.DirectionTri.DroiteBas => new []{ 1, 0 },
            _ => throw new InvalidOperationException("La direction de Tri n'est pas valide")
        });
    }

    private void DiviserEmplacement(RectangleI emplacement_, RectangleI boite_)
    {
        RetirerEmplacementLibre(emplacement_);
        
        if (boite_.Bas > emplacement_.Bas) // tronquer le haut
        { EnregistrerEmplacementLibre(new(boite_.Bas, boite_.Gauche, boite_.Bas - 1, boite_.Droite)); }
        
        if (boite_.Haut < emplacement_.Haut) // tronquer le bas
        { EnregistrerEmplacementLibre(new(boite_.Haut + 1, boite_.Gauche, boite_.Haut, boite_.Droite)); }
        
        if (boite_.Gauche > emplacement_.Gauche) // tronquer la droite
        { EnregistrerEmplacementLibre(new(boite_.Bas, boite_.Gauche, boite_.Haut, boite_.Gauche - 1)); }
        
        if (boite_.Droite < emplacement_.Droite) // tronquer la gauche
        { EnregistrerEmplacementLibre(new(boite_.Bas, boite_.Droite + 1, boite_.Haut, boite_.Droite)); }
    }
}


