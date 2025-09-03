using Godot;
using VA.Base.Systèmes;

namespace VA.Base.Systèmes.Gestionnaires
{
    /// <summary>
    /// Encapsule les ressources dans une classe partagée et permet de gérer leur durée de vie.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Ressource<T>: MettreAJour
    {
        public delegate void RessourceModifiée(T nouvelleRessource_);
        public delegate bool ConditionSurvie();
        public delegate void ActionDécès();

        /// <summary>
        /// Le compteur 
        /// </summary>
        public class CompteurRessource
        {
            public int Compteur;

            public CompteurRessource()
            { Compteur = 1; }
        }

        // /// <summary>
        // /// gère la survie de la Ressource
        // /// </summary>
        // public class Coeur
        // {
        //     private ConditionSurvie EstVivant;
        //     private ActionDécès Mourir;
        // }
        
        public T Ress { get; private set; }
        private readonly CompteurRessource Compteur;
        public int Quantité => Compteur.Compteur;
        public readonly ulong DateCréation;
        private ConditionSurvie EstVivant;
        private event ActionDécès Mourir;
        public event RessourceModifiée MajRessource;

        public Ressource(T ressource_) : this(ressource_, new CompteurRessource()) {}
        public Ressource(T ressource_, double intervalleMaj_, ConditionSurvie conditionsurvie_, ActionDécès actionDécès_, RessourceModifiée majRessource_) : 
            this(ressource_, new CompteurRessource(), intervalleMaj_, conditionsurvie_, actionDécès_, majRessource_) {}

        private Ressource(T ressource_, CompteurRessource compteur_, double intervalleMaj_ = -1d, ConditionSurvie conditionsurvie_ = null, ActionDécès actionDécès_ = null, RessourceModifiée majRessource_ = null) : 
            base(intervalleMaj_, intervalleMaj_ < 0 ? 0 : -1)
        {
            Ress = ressource_;
            Compteur = compteur_;
            ++Compteur.Compteur;
            DateCréation = Time.GetTicksMsec();
            EstVivant = conditionsurvie_ ?? (() => true);
            Mourir += actionDécès_;
            MajRessource += majRessource_;
        }

        ~Ressource()
        {
            --Compteur.Compteur;
            Tuer();
        }

        public void ActiverMaj(float intervalle_, ConditionSurvie conditionsurvie_, ActionDécès actionDécès_, RessourceModifiée majRessource_)
        {
            ResetIntervalle(intervalle_);
            EstVivant = conditionsurvie_;
            Mourir += actionDécès_;
            MajRessource += majRessource_;
            Ranimer();
        }

        public Ressource<T> Dupliquer()
        {return new Ressource<T>(Ress, Compteur, Fréquence, EstVivant, Mourir, MajRessource); }

        public void MajRess(T nouvelleRessource_)
        {
            Ress = nouvelleRessource_;
            MajRessource?.Invoke(Ress);
        }

        protected override void ActionDeMaj()
        {
            if (EstVivant == null || !EstVivant())
            { Mourir?.Invoke(); }
        }
    }
}