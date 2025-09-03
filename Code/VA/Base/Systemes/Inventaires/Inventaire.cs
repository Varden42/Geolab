using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Godot;

using VA.Base.GUI;

namespace VA.Base.Systèmes.Inventaires;

// public interface IStockable: IEquatable<IStockable>, IInterface2D
// {
//     // TODO: A faire
// }

//public enum TypeRangement { Libre, Croissant, Decroissant }

public class Inventaire<T>: IInterface2D, IEnumerable<T>// where T : IStockable
{
    #region Rangements

    public interface IRangement
    {
        public enum Configuration { Libre, Croissant, Decroissant }
        
        public Configuration Config { get; }

        public bool Ajouter(T objet_);
        public bool Retirer(T objet_);
    }

    public class RangementLibre : IRangement
    {
        private Inventaire<T> Inventaire;
        public IRangement.Configuration Config { get; }

        public RangementLibre(Inventaire<T> inventaire_)
        {
            Inventaire = inventaire_;
            Config = IRangement.Configuration.Libre;
        }

        public bool Ajouter(T objet_)
        {
            if (Inventaire.EspaceDisponible > 0)
            {
                for (int o = 0; o < Inventaire.Objets.Length; ++o)
                {
                    if (Inventaire.Objets[o] == null)
                    {
                        Inventaire.Objets[o] = objet_;
                        --Inventaire.EspaceDisponible;
                        return true;
                    }
                }
            }
            return false;
        }
        
        public bool Retirer(T objet_)
        {
            for (int o = 0; o < Inventaire.Objets.Length; ++o)
            {
                if (Inventaire.Objets[o] != null && Inventaire.Objets[o].Equals(objet_))
                {
                    Inventaire.Objets[o] = default;
                    ++Inventaire.EspaceDisponible;
                    return true;
                }
            }
            return false;
        }
    }

    public class RangementCroissant : IRangement
    {
        private Inventaire<T> Inventaire;
        public IRangement.Configuration Config { get; }

        public RangementCroissant(Inventaire<T> inventaire_)
        {
            Inventaire = inventaire_;
            Config = IRangement.Configuration.Croissant;
        }

        public bool Ajouter(T objet_)
        {
            if (Inventaire.EspaceDisponible > 0)
            {
                int emplacement = Inventaire.Objets.Length - Inventaire.EspaceDisponible;
                for (; emplacement < Inventaire.Objets.Length; ++emplacement)
                {
                    if (Inventaire.Objets[emplacement] == null)
                    {
                        Inventaire.Objets[emplacement] = objet_;
                        --Inventaire.EspaceDisponible;
                        return true;
                    }
                }
            }
            return false;
        }

        public bool Retirer(T objet_)
        {
            for (int o = 0; o < Inventaire.Objets.Length; ++o)
            {
                if (Inventaire.Objets[o] != null && Inventaire.Objets[o].Equals(objet_))
                {
                    while (Inventaire.Objets[o + 1] != null)
                    {
                        Inventaire.Objets[o] = Inventaire.Objets[o + 1];
                        ++o;
                    }
                    Inventaire.Objets[o] = default;
                    ++Inventaire.EspaceDisponible;
                    return true;
                }
            }
            return false;
        }
    }

    public class RangementDécroissant : IRangement
    {
        private Inventaire<T> Inventaire;
        public IRangement.Configuration Config { get; }

        public RangementDécroissant(Inventaire<T> inventaire_)
        {
            Inventaire = inventaire_;
            Config = IRangement.Configuration.Decroissant;
        }
        
        public bool Ajouter(T objet_)
        {
            if (Inventaire.EspaceDisponible > 0)
            {
                int emplacement = Inventaire.EspaceDisponible - 1;
                for (; emplacement >= 0; --emplacement)
                {
                    if (Inventaire.Objets[emplacement] == null)
                    {
                        Inventaire.Objets[emplacement] = objet_;
                        --Inventaire.EspaceDisponible;
                        return true;
                    }
                }
            }
            return false;
        }

        public bool Retirer(T objet_)
        {
            for (int o = 0; o < Inventaire.Objets.Length; ++o)
            {
                if (Inventaire.Objets[o] != null && Inventaire.Objets[o].Equals(objet_))
                {
                    while (Inventaire.Objets[o - 1] != null)
                    {
                        Inventaire.Objets[o] = Inventaire.Objets[o - 1];
                        --o;
                    }
                    Inventaire.Objets[o] = default;
                    ++Inventaire.EspaceDisponible;
                    return true;
                }
            }
            return false;
        }
    }

    #endregion
    
    private T[] Objets;
    public int EspaceDisponible { get; private set; }
    private IRangement Magasinier;
    
    public Control Interface2D { get; } // TODO: à faire
    
    public Inventaire(int taille_, IRangement rangement_ = null)
    {
        Objets = new T[taille_];
        Magasinier = rangement_ != null ? rangement_ : new RangementLibre(this);
        EspaceDisponible = taille_;
    }

    public bool Ajouter(T objet_)
    { return Magasinier.Ajouter(objet_); }
    
    public bool Retirer(T objet_)
    { return Magasinier.Retirer(objet_); }
    
    private int EmplacementsLibre()
    { return Objets.Count(item => item == null); }

    private int EmplacementsOccupés()
    { return Objets.Count(item => item != null); }

    public T this[int index]
    {
        get
        {
            if (index >= 0 && index < Objets.Length)
            { return Objets[index]; }
            throw new ArgumentOutOfRangeException(nameof(index), "Index is out of the range of the inventory size.");
        }
        set
        {
            if (index >= 0 && index < Objets.Length)
            { Objets[index] = value; }
            else
            { throw new ArgumentOutOfRangeException(nameof(index), "Index is out of the range of the inventory size."); }
        }
    }
    public IEnumerator<T> GetEnumerator()
    { return ((IEnumerable<T>)Objets).GetEnumerator(); }

    IEnumerator IEnumerable.GetEnumerator()
    { return GetEnumerator(); }
}