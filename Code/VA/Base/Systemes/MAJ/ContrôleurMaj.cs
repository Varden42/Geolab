using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace VA.Base.Systèmes;

/// <summary>
/// Permet au classes enfants d'être mis à jour à un intervalle donné et pour un nombre d'itération choisi
/// </summary>
public abstract class MettreAJour
{
    protected const double FREQUENCE_MIN = 0.5d;

    protected double Fréquence;
    private double Compteur;
    public  int Itérations { get; private set; }
    public double DuréeDeVie => Fréquence * Itérations;
    //TODO: ajouter un nombre d'occurences
    //protected bool Etat { get; private set; }
    protected MettreAJour(double fréquence_ = FREQUENCE_MIN, int itérations_ = -1)
    {
        Fréquence = fréquence_ >= FREQUENCE_MIN ? fréquence_ : FREQUENCE_MIN;
        Compteur = 0d;
        Itérations = itérations_;
        if (Itérations != 0)
        { Ranimer(); }
    }

    ~MettreAJour()
    { Tuer(); }

    protected void Ranimer()
    { ContrôleurMaj.Enregistrer(this); }

    protected void Tuer()
    { ContrôleurMaj.Retirer(this); }
    
    protected void ResetIntervalle(double fréquence_)
    { Fréquence = fréquence_; }
    
    public void Maj(double delta_)
    {
        if ((Compteur += delta_) >= Fréquence)
        {
            Compteur = 0d;
            ActionDeMaj();
            
            if (Itérations > 0)
            { --Itérations; }
            
            if (Itérations == 0)
            { ContrôleurMaj.Retirer(this); }
        }
    }

    /// <summary>
    /// Modifie le nombre d'itérations restantes, permettant de tuer ou ramener à la vie
    /// </summary>
    /// <param name="itérations_"></param>
    public void MajItérations(int itérations_)
    {
        if (itérations_ != 0 && Itérations == 0)
        { Ranimer(); }
        else if (itérations_ == 0 && Itérations != 0)
        { Tuer(); }

        Itérations = itérations_;
    }

    protected abstract void ActionDeMaj();
}

public class ContrôleurMaj
{
    // Crée l'élément unique à l'initialisation
    private static ContrôleurMaj Singleton = new();
    private static readonly object Cadenas = new object();
    public static ContrôleurMaj Instance => Singleton;

    private List<MettreAJour> Liste;
    
    static ContrôleurMaj() {}
    
    // TODO: regrouper les objets de même fréquences ensembles

    private ContrôleurMaj()
    {
        // Initialiser les variables de l'instance ici
        Liste = new();
    }
    
    /// <summary>
    /// Méthode servant à déclencher l'appel du constructeur static
    /// </summary>
    public static void Démarrer()
    {    }

    /// <summary>
    /// enregistre un nouvel objet afin qu'il soit mis à jour régulièrement
    /// </summary>
    /// <param name="cible_"></param>
    public static void Enregistrer(MettreAJour cible_)
    {
        ArgumentNullException.ThrowIfNull(cible_);
        
        lock (Cadenas)
        {
            if (!Instance.Liste.Contains(cible_))
            { Instance?.Liste.Add(cible_); }
        }
    }

    /// <summary>
    /// Ne plus mettre à jour l'objet ciblé
    /// </summary>
    /// <param name="cible_"></param>
    public static void Retirer(MettreAJour cible_)
    {
        lock (Cadenas)
        { Instance.Liste.Remove(cible_); }
    }

    // TODO: multi-Threading
    /// <summary>
    /// Met à jour les objet de la liste selon leur fréquence de mise à jour
    /// </summary>
    /// <param name="delta_"></param>
    public void Maj(double delta_)
    {
        lock (Cadenas)
        {
            List<MettreAJour> aRetirer = new();
            foreach (MettreAJour item in Liste)
            { item.Maj(delta_); }
            
            foreach (MettreAJour item in aRetirer)
            { Retirer(item); }
        }
    }
}