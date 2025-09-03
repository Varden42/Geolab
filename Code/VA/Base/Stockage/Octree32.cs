using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Godot;

namespace VA.Base.Stockage;

/// <summary>
/// Stocke des objets à position unique dans un Octree sur 10 profondeur
/// Chaque Objet est identifié par un ID sur 32 bits, 3 bit par profondeur
/// Les 2 bits restants ne sont pas utilisés
/// </summary>
public class Octree32<T>
{
    public struct ID
    {
        /// <summary>
        /// Stocke la position d'un index donné sur un axe donné ([0,?] = X, [1,?] = Y, [2,?] = Z)
        /// 0 étant au début et 1 à la fin.
        /// </summary>
        private static readonly int[,] PositionsAxes = { { 0, 1, 1, 0, 0, 1, 1, 0 }, { 0, 0, 0, 0, 1, 1, 1, 1 }, { 1, 1, 0, 0, 1, 1, 0, 0 } };
        
        /// <summary>
        /// Stocke l'index opposé sur un axe donné ([0,?] = X, [1,?] = Y, [2,?] = Z)
        /// </summary>
        private static readonly int[,] PairesParAxes = { { 1, 0, 3, 2, 5, 4, 7, 6 }, { 4, 5, 6, 7, 0, 1, 2, 3 }, { 3, 2, 1, 0, 7, 6, 5, 4 } };

        public static readonly Vector3I PositionMax = new Vector3I(1023, 1023, 1023);
        public static readonly Vector3I PositionMin = Vector3I.Zero;
        
        public Int32 Id { get; private set; }
        public Vector3I Position => VersPosition();
        public string Binaire => Regex.Replace(Convert.ToString(Id, 2).PadLeft(32, '0'), ".{3}", "[$0]");

        public ID(Int32 id_)
        { Id = id_; }

        public ID(Vector3I position_)
        {
            // Vérifier si la position est valide
            if ((position_.X is >= 0 and < (1 << 9)) && (position_.Y is >= 0 and < (1 << 9)) && (position_.Z is >= 0 and < (1 << 9)))
            {
                Id = 0;
                Int32 index = 0, tailleCube = 1 << 9;
                for (int hauteur = 9; hauteur >= 0; --hauteur)
                {
                    // on calcul la position dans le noeud à la hauteur h
                    // et modifie la position pour correspondre au noeud suivant
                    if (position_.X < tailleCube) // 0-3-4-7
                    {
                        if (position_.Y < tailleCube) // 0-3
                        {
                            if (position_.Z < tailleCube)
                            { index = 3; }
                            else
                            {
                                index = 0;
                                position_.Z -= tailleCube;
                            }
                        }
                        else // 4-7
                        {
                            if (position_.Z < tailleCube)
                            { index = 7; }
                            else
                            {
                                index = 4;
                                position_.Z -= tailleCube;
                            }
                            position_.Y -= tailleCube;
                        }
                    }
                    else
                    {
                        if (position_.Y < tailleCube) // 1-2
                        {
                            if (position_.Z < tailleCube)
                            { index = 2; }
                            else
                            {
                                index = 1;
                                position_.Z -= tailleCube;
                            }
                        }
                        else // 5-6
                        {
                            if (position_.Z < tailleCube)
                            { index = 6; }
                            else
                            {
                                index = 5;
                                position_.Z -= tailleCube;
                            }
                            position_.Y -= tailleCube;
                        }
                        position_.X -= tailleCube;
                    }

                    // on enregistre la paire à son emplacement dans l'ID
                    Id |= (index << (hauteur * 3) + 2);
                    tailleCube >>= 1;
                }
            }
            else
            { throw new ArgumentException($"La position fournit est en dehors de l'Octree !!"); }
        }


        /// <summary>
        /// Récupère l'index stocké à la position spécifiée.
        /// </summary>
        /// <param name="indexBloc_">un index entre 0 et 15</param>
        public int this[int indexBloc_]
        {
            get
            {
                int décalage = (9 - indexBloc_) * 3;
                uint masque = 0b_11100000000000000000000000000000 >> indexBloc_ * 3;
                return indexBloc_ is >= 0 and < 10 ? (int)((Id & masque) >> décalage + 2) : throw new OverflowException("L'index fournit est inférieur à 0 ou supérieur à 9 !!");
            }
            set
            {
                if (value is >= 0 and < 8)
                {
                    int décalage = (9 - indexBloc_) * 3;
                    Id = (indexBloc_ is >= 0 and < 10) ? (Id & (~(8 << décalage + 2))) | (value << décalage + 2) : throw new ArgumentException("L'index fournit est inférieur à 0 ou supérieur à 9 !!");
                }
                else
                { throw new ArgumentException("La valeur à assigner est inférieur à 0 ou supérieur à 7 !!"); }
            }
        }

        /// <summary>
        /// Convertit l'ID en position 2D
        /// </summary>
        /// <returns>La position 2D de l'ID dans le QuadTree16</returns>
        private Vector3I VersPosition()
        {
            Vector3I position = new Vector3I();
            UInt32 masque = 0b_11100000000000000000000000000000;
            int index = 0;
            for (int profondeur = 9; profondeur >= 0; --profondeur)
            {
                // on récupère l'index stocké dans la paire
                index = (int)((Id & masque) >> (profondeur * 3) + 2);
                int résolution = 1 << (profondeur);

                // on ajoute à la position le décalage nécessaire en fonction de la position dans le noeud actuel
                position.X += PositionsAxes[0,index] * résolution;
                position.Y += PositionsAxes[1,index] * résolution;
                position.Z += PositionsAxes[2,index] * résolution;
                
                // position.X += index is 1 or 2 or 5 or 6 ? résolution : 0;
                // position.Y += index is 4 or 5 or 6 or 7 ? résolution : 0;
                // position.Z += index is 0 or 1 or 4 or 5 ? résolution : 0;
                masque >>= 3;
            }
            return position;
        }

        // TODO: à faire
        /// <summary>
        /// Ajoute un Vecteur à un ID et calcule l'ID correspondant à la nouvelle position
        /// </summary>
        /// <param name="id_">L'ID d'origine</param>
        /// <param name="vecteur_">Le vecteur de déplacement</param>
        /// <returns>L'ID de la nouvelle position</returns>
        public static ID operator +(ID id_, Vector3I vecteur_)
        {
            Vector3I origine = id_.VersPosition();
            origine += vecteur_;
            return new(origine);
        }

        /// <summary>
        /// Soustrait un Vecteur à un ID et calcule l'ID correspondant à la nouvelle position
        /// </summary>
        /// <param name="id_">L'ID d'origine</param>
        /// <param name="vecteur_">Le vecteur de déplacement</param>
        /// <returns>L'ID de la nouvelle position</returns>
        public static ID operator -(ID id_, Vector3I vecteur_)
        {
            Vector3I origine = id_.VersPosition();
            origine -= vecteur_;
            return new(origine);
        }

        public bool Equals(ID id_)
        { return Id == id_.Id; }

        public bool Equals(ID id0_, ID id1_)
        { return id0_.Equals(id1_); }

        public int GetHashCode(ID t)
        { return Id; }
    }

    /// <summary>
    /// Classe de base abstraite servant 
    /// </summary>
    public abstract class Noeud : IDisposable
    {
        public Noeud Parent { get; set; }

        public Noeud(Noeud parent_)
        { Parent = parent_; }

        /// <summary>
        /// Remonte l'arborescence pour calculer la profondeur du Noeud
        /// </summary>
        /// <returns>la profondeur du Noeud dans le QuadTree</returns>
        public int CalculerProfondeur()
        { return CalculerProfondeur(0); }

        private int CalculerProfondeur(int profondeur_)
        { return Parent != null ? Parent.CalculerProfondeur(++profondeur_) : profondeur_; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public (ID Id, int profondeur) CalculerID()
        {
            ID id = new(0);
            int profondeur = CalculerProfondeur() - 1;

            return (id, ++profondeur);
        }

        public void CalculerID(ref ID id_, int profondeur_)
        {
            if (Parent != null)
            {
                for (int i = 0; i < 8; ++i)
                {
                    if (ReferenceEquals(((Branche)Parent)[i], this))
                    { id_[profondeur_] = i; }
                }
                Parent.CalculerID(ref id_, --profondeur_);
            }
        }

        public void Dispose()
        { Parent?.Dispose(); }
    }

    /// <summary>
    /// Un Noeud divisé en Quad sous forme de Branches ou de fruits
    /// </summary>
    public class Branche : Noeud
    {
        private Noeud[] Branches;

        /// <summary>
        /// Constructeur d'une Branche du quadTree16
        /// </summary>
        /// <param name="parent_">Le Noeud parent</param>
        public Branche(Noeud parent_) : base(parent_)
        { Branches = new Noeud[8] { null, null, null, null, null, null, null, null }; }

        /// <summary>
        /// Ajouter un objet dans le QuadTree
        /// </summary>
        /// <param name="id_">L'ID de l'objet à ajouter</param>
        /// <param name="objet_">L'objet (ou sa référence) à ajouter</param>
        /// <returns>Le Fruit contenant l'objet nouvellement ajouté</returns>
        public void Ajout(ID id_, T objet_)
        { Ajout(id_, objet_, CalculerProfondeur()); }

        /// <summary>
        /// Ajouter un objet dans le QuadTree
        /// </summary>
        /// <param name="id_">L'ID de l'objet à ajouter</param>
        /// <param name="objet_">L'objet (ou sa référence) à ajouter</param>
        /// <param name="profondeur_">La profondeur du Noeud actuel (méthode récursive)</param>
        /// <returns>Le Fruit contenant l'objet nouvellement ajouté</returns>
        private void Ajout(ID id_, T objet_, int profondeur_)
        {
            int index = id_[profondeur_];

            // Si le Noeud n'existe pas, on y dépose le Fruit sans aller plus loin
            if (Branches[index] == null)
            {
                Branches[index] = profondeur_ < 9 ? new Fleur(this, objet_, id_) : new Fruit(this, objet_);
                return;
            }

            // si le Noeud visé est une Branche, on continue
            if (Branches[index] is Branche)
            {
                ((Branche)Branches[index]).Ajout(id_, objet_, ++profondeur_);
                return;
            }

            if (Branches[index] is Fleur)
            {
                Branche bourgeon = new Branche(this);
                bourgeon.Ajout(id_, objet_, (Fleur)Branches[index], ++profondeur_);
                Branches[index] = bourgeon;
            }
        }

        /// <summary>
        /// Ajoute un Objet et un Fruit déjà existant, dans une nouvelle Branche
        /// </summary>
        /// <param name="id_">l'ID de l'objet à ajouter</param>
        /// <param name="objet_">L'objet à ajouter</param>
        /// <param name="fleur_">La Fleur à descendre dans l'arborescence</param>
        /// <param name="profondeur_">La profondeur du Noeud</param>
        /// <returns>Le Fruit contenant l'objet nouvellement ajouté</returns>
        private void Ajout(ID id_, T objet_, Fleur fleur_, int profondeur_)
        {
            // tant que les deux fruits ont le même index à cette profondeur, ajouter la branche avant et appeler cette méthode récursivement
            // Sinon, les ajouter séparément car ils se retrouveront dans des Noeuds différents.
            int index0 = id_[profondeur_],
                index1 = fleur_.Id[profondeur_];

            // si les deux objets vont à nouveau dans le même Noeud, on crée une Branche et recomence.
            if (index0 == index1)
            {
                Branche branche = new Branche(this);
                branche.Ajout(id_, objet_, fleur_, ++profondeur_);
                Branches[index0] = branche;
            }
            else
            {
                // sinon, on place les Fruit dans les Noeuds Appropriés
                Branches[index0] = profondeur_ < 9 ? new Fleur(this, objet_, id_) : new Fruit(this, objet_);
                Branches[index1] = profondeur_ < 9 ? new Fleur(this, fleur_.Objet, fleur_.Id) : new Fruit(this, fleur_.Objet);
            }
        }

        /// <summary>
        /// Accède à l'un des 8 enfants de la branche via son index.
        /// </summary>
        /// <param name="index_">un index entre 0 et 3</param>
        public Noeud this[int index_]
        {
            get => (index_ >= 0 && index_ < 8) ? Branches[index_] : null;
            set
            {
                if (index_ >= 0 && index_ < 8)
                { Branches[index_] = value; }
            }
        }

        /// <summary>
        /// Parcours l'arborescence récursivement afin de récupérer un objet à partir de son ID
        /// </summary>
        /// <param name="id_">L'ID de l'objet à récupérer</param>
        /// <returns>L'objet trouvé</returns>
        public T RécupObjet(ID id_)
        { return RécupObjet(id_, CalculerProfondeur()); }

        /// <summary>
        /// Parcours l'arborescence récursivement afin de récupérer un objet à partir de son ID
        /// </summary>
        /// <param name="id_">L'ID de l'objet à récupérer</param>
        /// <param name="profondeur_">La profondeur actuelle de la récursion</param>
        /// <returns>L'objet trouvé</returns>
        /// <exception cref="ArgumentException"></exception>
        private T RécupObjet(ID id_, int profondeur_)
        {
            if (profondeur_ is >= 0 and < 10)
            {
                // Récupère le Noeud sur lequel le Fruit doit se trouver
                Noeud cible = Branches[id_[profondeur_]];

                if (cible != null)
                {
                    if (cible is Branche)
                    { return ((Branche)cible).RécupObjet(id_, ++profondeur_); }

                    if (cible is Fruit)
                    { return ((Fruit)cible).Objet; }

                    if (cible is Fleur)
                    { return ((Fleur)cible).Objet; }
                }
            }

            throw new ArgumentException($"L'ID [{id_}] ne pointe vers rien dans l'Octree32 !!");
        }

        /// <summary>
        /// Parcours l'arborescence récursivement afin de récupérer un objet à partir de son ID
        /// </summary>
        /// <param name="id_">L'ID de l'objet à récupérer</param>
        /// <returns>L'objet trouvé</returns>
        public bool Retrait(ID id_)
        { return Retrait(id_, CalculerProfondeur()); }

        /// <summary>
        /// Parcours l'arborescence récursivement afin de récupérer un objet à partir de son ID
        /// </summary>
        /// <param name="id_">L'ID de l'objet à récupérer</param>
        /// <param name="profondeur_">La profondeur actuelle de la récursion</param>
        /// <returns>L'objet trouvé</returns>
        private bool Retrait(ID id_, int profondeur_)
        {
            if (profondeur_ is >= 0 and < 10)
            {
                int index = id_[profondeur_];
                Noeud cible = Branches[index];

                if (cible != null)
                {
                    if (cible is Branche)
                    {
                        return ((Branche)cible).Retrait(id_, ++profondeur_);
                    }

                    if (cible is Fruit or Fleur)
                    {
                        Branches[index] = null;
                        return true;
                    }
                }
            }

            return false;
        }
    }

    public class Fruit : Noeud
    {
        public T Objet { get; private set; }

        public Fruit(Noeud parent_, T objet_) : base(parent_)
        { Objet = objet_; }
    }

    public class Fleur : Noeud
    {
        public ID Id { get; private set; }
        public T Objet { get; private set; }

        public Fleur(Noeud parent_, T objet_, ID id_) : base(parent_)
        {
            Objet = objet_;
            Id = id_;
        }

        public Fruit VersFruit()
        { return new Fruit(Parent, Objet); }
    }
    
    private HashSet<ID> Registre;
    private Branche Racine; 
    public int Compte => Registre.Count;
    public static readonly int TailleMin = 0;
    public static readonly int TailleMax = 1024;
    
    public Octree32() 
    {
        Registre = new HashSet<ID>();
        Racine = new Branche(null);
    }

    /// <summary>
    /// Vérifie si un objet se trouve à l'ID spécifié
    /// </summary>
    /// <param name="id_">L'ID à vérifier</param>
    /// <returns></returns>
    public bool Contient(ID id_)
    { return Registre.Contains(id_); }
    /// <summary>
    /// Vérifie si un objet se trouve à la position spécifiée
    /// </summary>
    /// <param name="position_">La position de l'objet dont l'on veut vérifier l'existence</param>
    /// <returns></returns>
    public bool Contient(Vector3I position_)
    { return Registre.Contains(new ID(position_)); }
    
    /// <summary>
    /// Ajoute un Objet T dans le Quadtree à un ID donné
    /// </summary>
    /// <param name="id_">L'ID 32 bits unique désignant l'emplacement de l'objet dans le Quadtree</param>
    /// <param name="objet_">L'objet à ajouter</param>
    /// <returns>L'objet ajouté dans le QuadTree</returns>
    public void Ajout(ID id_, T objet_)
    {
        // Parcourir le registre pour voir si l'ID n'est pas déjà utilisé
        if (!Registre.Contains(id_))
        {
            // L'emplacement est libre, on peut commencer à parcourir le quadTree jusqu'à trouver un Noeud Vide ou son emplacement final.
            Registre.Add(id_);
            Racine.Ajout(id_, objet_);
        }
        else
        { GD.PrintErr("L'ID fournit est déjà présent dans le registre"); }
    }

    /// <summary>
    /// Ajoute un Objet T dans le Quadtree à une position donnée
    /// </summary>
    /// <param name="position_">La position 2D de l'objet dans le Quadtree</param>
    /// <param name="objet_">L'objet à ajouter</param>
    /// <returns>L'objet ajouté dans le QuadTree</returns>
    public ID Ajout(Vector3I position_, T objet_)
    {
        ID id = new ID(position_);
        Ajout(new ID(position_), objet_);
        return id;
    }

    /// <summary>
    /// Récupère la référence stockée dans le QuadTree par son ID
    /// </summary>
    /// <param name="id_">L'ID de l'objet à récupérer</param>
    /// <returns>L'objet correspondant à l'ID</returns>
    public T this[ID id_]
    { get => Racine.RécupObjet(id_); }

    /// <summary>
    /// Récupère la référence stockée dans le QuadTree par sa position
    /// </summary>
    /// <param name="position_">La position 2D de l'objet à récupérer</param>
    /// <returns>L'objet correspondant à la position</returns>
    public T this[Vector3I position_]
    { get => this[new ID(position_)]; }

    /// <summary>
    /// Accède à l'objet stocké sans vérifier si l'ID est valide
    /// </summary>
    /// <param name="id_">L'ID de l'objet à récupérer</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">Si l'ID n'est pas valide</exception>
    public T RécupObjet(ID id_)
    {
        if (Registre.Contains(id_))
        {
            uint masque = 0b_11100000000000000000000000000000;
            Noeud noeudActuel = Racine;
            for (int i = 0; i < 10; ++i)
            {
                if (noeudActuel != null)
                {
                    if (noeudActuel is Branche)
                    {
                        int décalage = (9 - i) * 3 + 2;
                        noeudActuel = ((Branche)noeudActuel)[(id_.Id & (int)masque) >> décalage];
                        masque >>= 3;
                    }

                    if (noeudActuel is Fruit)
                    { return ((Fruit)noeudActuel).Objet; }

                    if (noeudActuel is Fleur)
                    { return ((Fleur)noeudActuel).Objet; }
                }
            }
        }
        else
        { GD.PrintErr("L'ID n'est pas présent dans le registre"); }

        return default;
    }
                    
    /// <summary>
    /// Retirer un Objet du QuadTree
    /// </summary>
    /// <param name="id_">L'ID de l'objet à retirer</param>
    /// <returns>La réussite du retrait</returns>
    public bool Retrait(ID id_)
    {
        bool réussite = false;
        if (Registre.Contains(id_))
        {
            réussite = Racine.Retrait(id_);
            if (réussite)
            { Registre.Remove(id_); }
            else
            { GD.PrintErr("L'ID fournit est présent dans le registre mais ne point vers aucun Objet"); }
        }
        else
        { GD.PrintErr("L'ID n'est pas présent dans le registre"); }

        return réussite;
    }

    /// <summary>
    /// Vide le QuadTree
    /// </summary>
    public void Vider()
    {
        // éventuellement optimiser en supprimant manuellement tout les Noeuds de l'arbre plutot que laisser le Garbage Collector le faire/
        Racine = new(null);
        Registre.Clear();
    }

    /// <summary>
    /// Vérifie la totalité de l'arborescence à la recherche d'erreur (DEBUG)
    /// </summary>
    /// <returns></returns>
    public (bool, string) EstValide()
    {
        int pasDansRegistre = 0, Valide = 0, pasDansArbre = 0;
        HashSet<ID> copie = new(Registre);
        List<ID> ListeIDs = new();

        ValiderNoeud(Racine, new ID(0), 0, ref ListeIDs);

        string IDsPasDansRegistre = "", IDSPasDansArbre = "";
        foreach (var id in ListeIDs)
        {
            if (copie.Contains(id))
            {
                ++Valide;
                copie.Remove(id);
            }
            else
            {
                ++pasDansRegistre;
                IDsPasDansRegistre += $"[{id.Binaire}] ";
            }
        }

        foreach (var id in copie)
        {
            ++pasDansArbre;
            IDSPasDansArbre += $"[{id.Binaire}] ";
        }
                        
        string debug = $"Nombre d'ID récupérés [{ListeIDs.Count}]\n" +
                       $"Nombre d'ID présent dans l'Arborescence mais pas dans le registre [{pasDansRegistre}] => {IDsPasDansRegistre}\n" +
                       $"Nombre d'ID présent dans le registre mais pas dans l'Aborescence [{pasDansArbre}] => {IDSPasDansArbre}\n" +
                       $"Nombre d'ID valide [{Valide}]";

        return (pasDansArbre == 0 && pasDansRegistre == 0 && Valide == Registre.Count, debug);
    }

    public void ValiderNoeud(Branche branche_, ID id_, int profondeur_, ref List<ID> ids_)
    {
        //GD.Print($"Validation du Noeud à la position {id_.Binaire}");
        for (int n = 0; n < 8; n++)
        {
            if (branche_[n] != null)
            {
                if (branche_[n] is Branche)
                {
                    id_[profondeur_] = n;
                    ValiderNoeud(branche_[n] as Branche, id_, profondeur_ + 1, ref ids_);
                }
                else if (branche_[n] is Fleur)
                {
                    id_[profondeur_] = n;
                    //var debug = branche_[n].CalculerID();
                    //GD.Print($"Une Fleur a été trouvée à {id_.Binaire}({profondeur_})\nCalcul de l'ID [{debug.Id.Binaire}({debug.profondeur})\nID enregistré dans la Fleur {((Fleur)branche_[n]).Id.Binaire}");
                    ids_.Add(((Fleur)branche_[n]).Id);
                }
                else
                {
                    id_[profondeur_] = n;
                    //var debug = branche_[n].CalculerID();
                    //GD.Print($"Un Fruit a été trouvée à {id_.Binaire}({profondeur_})\nCalcul de l'ID [{debug.Id.Binaire}[{debug.profondeur}]");
                    ids_.Add(id_);
                }
            }
        }
    }
}