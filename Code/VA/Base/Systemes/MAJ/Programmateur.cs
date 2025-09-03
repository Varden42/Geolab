using System;
using System.Collections.Generic;

namespace VA.Base.Systèmes
{
    /// <summary>
    /// Permet de programmer une liste d'actions à intervalles régulier, un nombre précis d'occurences ou jusqu'à la mort de l'objet
    /// </summary>
    public class Programmateur: MettreAJour
    {
        public readonly struct RésultatExécution
        {
            public readonly int Pourcentage;
            public readonly bool[] Historique;

            public RésultatExécution(int pourcentage_, bool[] historique_)
            {
                Pourcentage = pourcentage_;
                Historique = historique_;
            }
        }
    
        public delegate bool Tache();
        public delegate bool EventExécution(RésultatExécution résultat_);

        private List<Tache> Taches;
        private int Occurences;
        public RésultatExécution DernièreExécution;
        public event EventExécution Exécution;

        public Programmateur(double intervalle_, int occurences_ = -1): this(new List<Tache>(), intervalle_, occurences_) {}
        public Programmateur(Tache[] taches_, double intervalle_, int occurences_ = -1): this(new List<Tache>(taches_), intervalle_, occurences_) {}
        public Programmateur(List<Tache> taches_, double intervalle_, int occurences_ = -1): base(intervalle_)
        {
            Taches = taches_;
            Occurences = occurences_;
        }
    
        protected override void ActionDeMaj()
        {
            if (Occurences > 0)
            { Exécuter(); }
            else
            { Tuer(); }
        }

        private void Exécuter()
        {
            bool[] historique = new bool[Taches.Count];
            int tâchesRéussies = 0;
            for (int t = 0; t < Taches.Count; ++t)
            {
                historique[t] = Taches[t]();
                if (historique[t])
                { ++tâchesRéussies; }
            }

            DernièreExécution = new((tâchesRéussies * 100) / historique.Length, historique);

            Exécution?.Invoke(DernièreExécution);
            
            --Occurences;
            if (--Occurences <= 0)
            { Tuer(); }
        }

        public void Redémarrer(int occurences_ = 1)
        {
            Occurences = occurences_;
            Ranimer();
        }
    
        public void AjoutTache(Tache tache_)
        { Taches.Add(tache_); }
    }
}