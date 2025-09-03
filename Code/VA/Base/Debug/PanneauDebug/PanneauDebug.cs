using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using VA.Base.GUI;
using VA.Base.Systèmes;
using Index = VA.Base.Systèmes.Gestionnaires.Index;

namespace VA.Base.Debug;

/// <summary>
/// Un panneau affichant des informations customisées et mises à jour à un intervalle précis.
/// </summary>
public partial class PanneauDebug : Control
{
    // TODO: ne charger le style qu'une fois
    
    /// <summary>
    /// Affiche une Ligne avec son nom et sa valeur
    /// </summary>
    public partial class Ligne : HBoxContainer
    {
        public readonly Label Nom;
        private Label Valeur;
        private readonly MajLigne RécupValeur;
        private Actualiseur Actualiseur;

        /// <summary>
        /// Constructeur par défaut
        /// </summary>
        public Ligne(): this("<vide>", () => "<vide>") { }
        // {
        //     Name = "<vide>";
        //     StyleBoxFlat style = ResourceLoader.Load<StyleBoxFlat>("res://VA/Base/Debug/PanneauDebug/PanneauDebugStyle.tres");
        //
        //     Nom = new Label();
        //     Nom.Name = "Nom";
        //     Nom.Text = "<vide>";
        //     Nom.AddThemeStyleboxOverride("normal", style);
        //     AddChild(Nom);
        //
        //     Valeur = new Label();
        //     Valeur.Name = "Valeur";
        //     Valeur.Text = "<vide>";
        //     Valeur.AddThemeStyleboxOverride("normal", style);
        //     AddChild(Valeur);
        //
        //     RécupValeur = () => "<vide>";
        // }
        
        /// <summary>
        /// Construit une ligne avec un nom et une valeur
        /// </summary>
        /// <param name="nom_">Le nom de la valeur associée à la ligne</param>
        /// <param name="récupValeur_">La delegate utilisée pour récupérer dynamiquement la valeur associée à la ligne</param>
        public Ligne(string nom_, MajLigne récupValeur_)
        {
            Name = nom_;
            //StyleBoxFlat style = ResourceLoader.Load<StyleBoxFlat>("res://VA/Base/Debug/PanneauDebug/PanneauDebugStyle.tres");

            Nom = new GUI.Etiquette(nom_, "Nom", Style);
            // Nom.AddThemeStyleboxOverride("normal", style);
            AddChild(Nom);

            Valeur = new GUI.Etiquette(récupValeur_(), "Valeur", Style);
            AddChild(Valeur);

            RécupValeur = récupValeur_;

            Actualiseur = null;
        }
        
        ~Ligne()
        { Actualiseur?.RetraitLigne(this); }

        public void Maj()
        { Valeur.Text = RécupValeur(); }

        public void ModifActualiseur(Actualiseur actualiseur_)
        {
            if (actualiseur_ != Actualiseur)
            {
                if (Actualiseur != null)
                { Actualiseur.RetraitLigne(this); }

                Actualiseur = actualiseur_;
                Actualiseur?.AjoutLigne(this);
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is not Ligne autre)
            { return false; }

            return Nom.Text == autre.Nom.Text && RécupValeur() == autre.RécupValeur();
        }

        public override int GetHashCode()
        { return Nom.Text.GetHashCode(); }

        public static bool operator ==(Ligne gauche, Ligne droite)
        {
            if (ReferenceEquals(gauche, null))
            { return ReferenceEquals(droite, null); }
            return gauche.Equals(droite);
        }

        public static bool operator !=(Ligne gauche, Ligne droite)
        { return !(gauche == droite); }
    }

    // /// <summary>
    // /// Contient une Ligne et se met à jour de manière autonome
    // /// </summary>
    // public partial class LigneAutonome : Control
    // {
    //     private Ligne Ligne;
    //     private double Fréquence;
    //     private double chrono;
    //
    //     /// <summary>
    //     /// Constructeur sans paramètres, pour le fonctionnement interne de Godot.
    //     /// </summary>
    //     public LigneAutonome()
    //     {
    //         Ligne = new Ligne();
    //         Fréquence = 0d;
    //         chrono = 0d;
    //     }
    //
    //     /// <summary>
    //     /// Représente une ligne du panneau qui se met à jour automatiquement à une fréquence donnée.
    //     /// </summary>
    //     /// <param name="ligne_">La ligne à rendre autonome</param>
    //     /// <param name="fréquence_">La fréquence de rafraichissement de la ligne</param>
    //     public LigneAutonome(Ligne ligne_, double fréquence_)
    //     {
    //         Ligne = ligne_;
    //         Fréquence = fréquence_;
    //         chrono = 0d;
    //     }
    //
    //     /// <summary>
    //     /// Représente une ligne du panneau qui se met à jour automatiquement à une fréquence donnée.
    //     /// </summary>
    //     /// <param name="nom_">Le nom de la valeur associée à la ligne.</param>
    //     /// <param name="récupValeur_">La delegate utilisée pour récupérer dynamiquement la valeur associée à la ligne.</param>
    //     /// <param name="fréquence_">La fréquence de rafraichissement de la ligne</param>
    //     public LigneAutonome(string nom_, MajLigne récupValeur_, double fréquence_)
    //     {
    //         Ligne = new(nom_, récupValeur_);
    //         Fréquence = fréquence_;
    //         chrono = 0d;
    //     }
    //     
    //     public override void _Process(double delta_)
    //     {
    //         chrono += delta_;
    //         if (chrono >= Fréquence)
    //         {
    //             chrono = 0d;
    //             Ligne.Maj();
    //         }
    //     }
    // }

    /// <summary>
    /// Regroupe plusieurs lignes dans une même catégorie
    /// </summary>
    private partial class Catégorie : VBoxContainer
    {
        //public double Fréquence { get; private set; }
        private double Chrono;

        /// <summary>
        /// Constructeur par défaut
        /// </summary>
        public Catégorie()
        {
            Chrono = 0d;
            AddChild(new GUI.Etiquette("<vide>", "TitreCatégorie", Style));
        }

        /// <summary>
        /// Contruit une catégorie avec un titre
        /// </summary>
        /// <param name="titre_">Le titre de la catégorie</param>
        /// <param name="fréquence_">La fréquence de rafraichissement des valeurs</param>
        public Catégorie(string titre_)
        {
            Chrono = 0d;
            Etiquette etiquette = new GUI.Etiquette(titre_, "TitreCatégorie", Style);
            etiquette.Alignement(AlignementHorizontal.Gauche, AlignementVertical.Centre);
            AddChild(etiquette);
            //AddChild(GUI.Templates.EtiquetteGaucheCentre(titre_, "TitreCatégorie", Style));
        }
        
        /// <summary>
        /// Ajoute une ligne dans la catégorie
        /// </summary>
        /// <param name="nom_">Le nom de la valeur</param>
        /// <param name="récupValeur_">La delegate mettant à jour la valeur</param>
        /// <param name="position_">la position de la ligne dans la catégorie</param>
        public bool AjoutLigne(string nom_, MajLigne récupValeur_, int position_ = -1) => AjoutLigne(new Ligne(nom_, récupValeur_), position_);
        /// <summary>
        /// Ajoute une ligne dans la catégorie
        /// </summary>
        /// <param name="ligne_">La ligne à ajouter</param>
        /// <param name="position_">la position de la ligne dans la catégorie</param>
        public bool AjoutLigne(Ligne ligne_, int position_ = -1)
        {
            // vérifier si une ligne de même nom existe déjà
            if (FindChild(ligne_.Nom.Text, false) == null)
            {
                AddChild(ligne_);
                if (position_ < GetChildCount() || position_ >= 0)
                { MoveChild(ligne_, position_); }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Retire une ligne de la catégorie
        /// </summary>
        /// <param name="position_">La position de la ligne dans la catégorie</param>
        /// <returns>un booléen représentant la réussite de l'action</returns>
        public bool RetraitLigne(int position_)
        {
            if (position_ < 0 || position_ >= GetChildCount())
            { return false; }

            GetChild(position_).QueueFree();
            return true;
        }
        /// <summary>
        /// Retire une ligne de la catégorie
        /// </summary>
        /// <param name="nom_">Le nom de la valeur présente dans la ligne</param>
        /// <returns>un booléen représentant la réussite de l'action</returns>
        public bool RetraitLigne(string nom_)
        {
            Node ligne = FindChild(nom_, false);
            if (ligne != null)
            {
                ligne.QueueFree();
                return true;
            }
            return false;
        }
        /// <summary>
        /// Retire une ligne de la catégorie
        /// </summary>
        /// <param name="ligne_">LA ligne à retirer</param>
        /// <returns>un booléen représentant la réussite de l'action</returns>
        public bool RetraitLigne(Ligne ligne_)
        {
            // TODO: faire un vrai test d'égalité
            Node ligne = FindChild(ligne_.Name, false);
            if (ligne != null)
            {
                ligne.QueueFree();
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// Regroupe des lignes devant être mis à jour à la même fréquence
    /// </summary>
    public class Actualiseur
    {
        public double Fréquence { get; private set; }
        private double Chrono;
        private List<Ligne> Lignes;

        /// <summary>
        /// Constructeur par défaut
        /// </summary>
        /// <param name="fréquence_">La fréquence d'actualisation des lignes regroupées</param>
        public Actualiseur(double fréquence_ = 0.5d)
        {
            Fréquence = fréquence_;
            Lignes = new();
        }
        /// <summary>
        /// Constructeur prenant une liste existante de lignes
        /// </summary>
        /// <param name="lignes_">Les lignes que cet actualiseur doit gérer</param>
        /// <param name="fréquence_">La fréquence d'actualisation des lignes regroupées</param>
        public Actualiseur(List<Ligne> lignes_, double fréquence_ = 0.5d)
        {
            Fréquence = fréquence_;
            Lignes = lignes_;
        }

        // public Actualiseur(Ligne ligne_, List<Ligne> lignes_, double fréquence_ = 0.5d)
        // {
        //     Fréquence = fréquence_;
        //     Lignes = lignes_;
        //     Lignes.Add(ligne_);
        // }

        /// <summary>
        /// Ajoute une ligne
        /// </summary>
        /// <param name="ligne_">La ligne à ajouter</param>
        public void AjoutLigne(Ligne ligne_)
        { Lignes.Add(ligne_); }
        /// <summary>
        /// Ajoute plusieurs lignes
        /// </summary>
        /// <param name="ligne_">Les lignes à ajouter</param>
        public void AjoutLigne(IEnumerable<Ligne> ligne_)
        { Lignes.AddRange(ligne_); }
        
        /// <summary>
        /// Retire une ligne
        /// </summary>
        /// <param name="ligne_">La ligne à retirer</param>
        /// <returns>un booléen représentant la réussite de l'action</returns>
        public bool RetraitLigne(Ligne ligne_)
        { return Lignes.Remove(ligne_); }
        /// <summary>
        /// Retire une ligne
        /// </summary>
        /// <param name="nom_">Le nom de la valeur de la ligne à retirer</param>
        /// <returns>un booléen représentant la réussite de l'action</returns>
        public bool RetraitLigne(string nom_)
        {
            Ligne ligne = Lignes.Find(l => l.Nom.Text == nom_);
            return RetraitLigne(ligne);
        }

        /// <summary>
        /// Met à jour le chrono et déclenche la mise à jour des lignes
        /// </summary>
        /// <param name="delta_">Le delta à ajouter au chrono</param>
        /// <returns>true si la mise à jour a été déclenchée</returns>
        public bool Maj(double delta_)
        {
            Chrono += delta_;
            if (Chrono >= Fréquence)
            {
                Chrono = 0d;
                foreach (Ligne ligne in Lignes)
                { ligne.Maj(); }

                return true;
            }

            return false;
        }
    }
    
    // TODO: récupérer le style depuis le gestionnaire de ressources
    protected static StyleBoxFlat Style = ResourceLoader.Load<StyleBoxFlat>(Biblio.Index.RecupIndex(Index.CatégorieIndexs.Style, "PanneauDebug"));
    public delegate string MajLigne();
    private VBoxContainer Liste;
    private Dictionary<string, Catégorie> Catégories;
    private List<Actualiseur> Actualiseurs;

    /// <summary>
    /// Constructeur par défaut
    /// </summary>
    public PanneauDebug()
    {
        Catégories = new();
        Actualiseurs = new();

        Liste = new();
        Liste.Name = "Liste";
        AddChild(Liste);
    }
    
    public override void _Process(double delta_)
    {
        foreach (Actualiseur chrono in Actualiseurs)
        { chrono.Maj(delta_); }
    }

    private Catégorie NouvelleCatégorie(string nom_)
    {
        // TODO: ne stocker les catégories que dans la Liste
        Catégorie nouvelleCatégorie = new Catégorie(nom_);
        Catégories.Add(nom_, nouvelleCatégorie);
        Liste.AddChild(nouvelleCatégorie);
        return nouvelleCatégorie;
    }

    /// <summary>
    /// Ajoute une nouvelle ligne au Panneau
    /// </summary>
    /// <param name="nom_">Le nom de la valeur</param>
    /// <param name="recupValeur_">La delegate permettant de récupérer la valeur à jour</param>
    /// <param name="fréquence_">La fréquence de mise à jour de la ligne</param>
    /// <param name="catégorie_">LA catégorie dans laquelle ranger la ligne</param>
    /// <returns>La réussite de l'ajout de la ligne</returns>
    public bool AjoutLigne(string nom_, MajLigne recupValeur_, double fréquence_ = 0.5d, string catégorie_ = "Divers")
    {
        Ligne ligne = new(nom_, recupValeur_);
        bool réussite = false;
        
        // Trouver ou créer la catégorie correspondante
        if (Catégories.ContainsKey(catégorie_))
        { réussite = Catégories[catégorie_].AjoutLigne(ligne); }
        else
        { réussite = NouvelleCatégorie(catégorie_).AjoutLigne(ligne); }
        
        // Trouver ou créer l'actualiseur correspondant
        Actualiseur actualiseurCorrespondant = Actualiseurs.Find(a => a.Fréquence == fréquence_);
        if (actualiseurCorrespondant == null)
        {
            actualiseurCorrespondant = new(fréquence_);
            actualiseurCorrespondant.AjoutLigne(ligne);
            Actualiseurs.Add(actualiseurCorrespondant);
        }
        else
        { actualiseurCorrespondant.AjoutLigne(ligne); }

        return réussite;
    }

    /// <summary>
    /// Retirer une ligne du panneau
    /// </summary>
    /// <param name="nom_">Le nom de la valeur contenue dans la ligne</param>
    /// <param name="catégorie_">La catégorie contenant la ligne</param>
    /// <returns>la réussite du retrait de la ligne</returns>
    public bool RetraitLigne(string nom_, string catégorie_)
    {
        bool réussite = false;
        Ligne ligneARetirer = null;
        // Trouver la ligne dans la catégorie puis la retirer de l'actualiseur
        if (Catégories.ContainsKey(catégorie_))
        {
            ligneARetirer = Catégories[catégorie_].FindChild(nom_, false) as Ligne;
            if (ligneARetirer != null)
            { ligneARetirer.ModifActualiseur(null); }
            Catégories[catégorie_].RetraitLigne(ligneARetirer);
            réussite = true;
        }
        return réussite;
    }
    /// <summary>
    /// Retirer une ligne du panneau
    /// </summary>
    /// <param name="ligne_">La ligne à retirer</param>
    /// <returns>la réussite du retrait de la ligne</returns>
    public bool RetraitLigne(Ligne ligne_)
    {
        bool réussite = false;
        // on retire de la catégorie
        foreach (var catégorie in Catégories)
        { réussite = catégorie.Value.RetraitLigne(ligne_); }
        for (int c = 0; c < Catégories.Count; c++)
        // on retire de l'actualiseur
        ligne_.ModifActualiseur(null);
        return réussite;
    }
}
