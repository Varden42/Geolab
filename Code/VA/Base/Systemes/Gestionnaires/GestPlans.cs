using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using VA.Base.Debug;
using VA.Base.Fichiers.JSON;
using VA.Base.Ressources;
using VA.Base.Utiles;
using Json = VA.Base.Ressources.Json;

namespace VA.Base.Systèmes.Gestionnaires;

// TODO: Tout revoir de zéro, les types de plans ne doivent pas être coder en dur.
public class GestPlans
{
    private const float IntervalleMaj = 1f;
    
    public enum TypeDePlan { Train, Module, Créature }
    public enum TypePlanTrain { Chassis, Corps }
    public enum TypePlanModule { Moteur }
    public enum TypePlanCréatures { Petit, Moyen, Gros }
    
    public enum TypeViePlan { UsageUnique, Durée, Condition }

    public readonly struct Id
    {
        public readonly int Catégorie0;
        public readonly int Catégorie1;
        public readonly string Modèle;
    
        public Id(int Catégorie0_, int Catégorie1_, string modèle_)
        {
            Catégorie0 = Catégorie0_;
            Catégorie1 = Catégorie1_;
            Modèle = modèle_;
        }
    }

    public struct ViePlan
    {
        public readonly TypeViePlan TypeDeVie;
        public readonly float DuréeDeVie;
        public readonly Ressource<IPlan>.ConditionSurvie Condition;

        public ViePlan()
        {
            TypeDeVie = TypeViePlan.UsageUnique;
            DuréeDeVie = 0f;
            Condition = null;
        }
        public ViePlan(float duréeDeVie_)
        {
            TypeDeVie = TypeViePlan.Durée;
            DuréeDeVie = duréeDeVie_;
            Condition = null;
        }
        public ViePlan(Ressource<IPlan>.ConditionSurvie condition_)
        {
            TypeDeVie = TypeViePlan.Condition;
            DuréeDeVie = 0f;
            Condition = condition_;
        }
    }
    
    public interface IPlan
    {
        public int Id { get; }
        public string Nom { get; }

        public static Dictionary<string, IndexPlanJson> Indexs(string cheminJson_, ref CompteurInt compteur_)
        {
            Dictionary<string, IndexPlanJson> liste = null;
            if (FileAccess.FileExists(cheminJson_))
            {
                string nom = null;
                ulong début = 0UL, longueur = 0UL;
                
                JArray tableau = Json.Ressource_Tableau(cheminJson_);
                foreach (JToken token in tableau)
                {
                    if (token.Type == JTokenType.Object)
                    {
                        JObject objet = (JObject)token;
                        objet["Id"] = compteur_.Ajouter;
                        nom = objet["Nom"]?.ToString();
                    }
                }
                FileAccess lecteur = FileAccess.Open(cheminJson_, FileAccess.ModeFlags.Write);
                lecteur.StoreString(tableau.ToString());
                
                lecteur = FileAccess.Open(cheminJson_, FileAccess.ModeFlags.Read);
                if (lecteur != null)
                {
                    byte accolade0 = (byte)'{', accolade1 = (byte)'}', guillemet = (byte)'"';
                    int compteurBlocs = 0;
                    while (!lecteur.EofReached())
                    {
                        byte car = lecteur.Get8();
                        if (car == accolade0)
                        {
                            if (compteurBlocs == 0)
                            { début = lecteur.GetPosition() - 1; }
                            ++compteurBlocs;
                        }
                        else if (car == accolade1)
                        {
                            --compteurBlocs;
                            if (compteurBlocs == 0 && nom != null)
                            {
                                longueur = (lecteur.GetPosition() + 1) - début;
                                liste = liste == null ? new() : liste;
                                liste.Add(nom, new(début, longueur));
                            }
                        }
                    }
                
                    lecteur.Close();
                }
            }
            return liste;
        }
        
        // public static IndexPlanJson Index(string cheminJson_, string nom_)
        // {
        //     IndexPlanJson indexRecherché = null;
        //     if (FileAccess.FileExists(cheminJson_))
        //     {
        //         FileAccess lecteur = FileAccess.Open(cheminJson_, FileAccess.ModeFlags.Read);
        //         if (lecteur != null)
        //         {
        //             byte accolade0 = (byte)'{', accolade1 = (byte)'}', guillemet = (byte)'"';
        //             int compteurBlocs = 0;
        //             bool trouvé = false;
        //             ulong début = 0UL, longueur = 0UL;
        //             while (!lecteur.EofReached())
        //             {
        //                 byte car = lecteur.Get8();
        //                 if (car == accolade0)
        //                 {
        //                     if (compteurBlocs == 0)
        //                     {
        //                         début = lecteur.GetPosition();
        //                         // chercher le nom dans le bloc que l'on viens d'ouvrir
        //                         string nomPropriété = Fichier.LireEntreCaratères(ref lecteur, '"');
        //                         if (nomPropriété == "Nom" && Fichier.LireEntreCaratères(ref lecteur, '"') == nom_)
        //                         { trouvé = true; }
        //                     }
        //
        //                     ++compteurBlocs;
        //                 }
        //                 else if (car == accolade1)
        //                 {
        //                     --compteurBlocs;
        //                     if (compteurBlocs == 0 && trouvé)
        //                     {
        //                         longueur = (lecteur.GetPosition() + 1) - début;
        //                         indexRecherché = new(nom_, début, longueur);
        //                     }
        //                 }
        //             }
        //
        //             lecteur.Close();
        //         }
        //     }
        //     return indexRecherché;
        // }
    }

    public struct IndexPlanJson
    {
        //public string Nom { get; private set; }
        public ulong Début { get; }
        public ulong Longueur { get; }

        public IndexPlanJson(/*string nom_, */ulong début_, ulong longueur_)
        {
            //Nom = nom_;
            Début = début_;
            Longueur = longueur_;
        }
    }
    
    private static GestPlans Singleton = new();
    private static readonly object Cadenas = new object();
    public static GestPlans Instance => Singleton;

    private Dictionary<int, Dictionary<int, Dictionary<string, Ressource<IPlan>>>> Plans;
    private Dictionary<int, Dictionary<int, Dictionary<string, IndexPlanJson>>> Registre;

    static GestPlans()
    {    }

    private GestPlans()
    {
        ChargerIndexs();
    }

    private (string NomCatégorie, int Id)[] ListeSecondeCatégorie(Id id_)
    { return ListeSecondeCatégorie((TypeDePlan)id_.Catégorie0); }
    /// <summary>
    /// crée une liste pour une sous-catégorie de plans précis
    /// </summary>
    /// <param name="type_">le type de la sous-catégorie</param>
    /// <returns></returns>
    private (string NomCatégorie, int Id)[] ListeSecondeCatégorie(TypeDePlan type_)
    {
        List<(string NomCatégorie, int Id)> liste = new();
        switch (type_)
        {
            case TypeDePlan.Train:
                foreach (TypePlanTrain secondeCatégorie in Enum.GetValues(typeof(TypePlanTrain)))
                { liste.Add((secondeCatégorie.ToString(), (int)secondeCatégorie)); }
                break;
            case TypeDePlan.Module:
                foreach (TypePlanModule secondeCatégorie in Enum.GetValues(typeof(TypePlanModule)))
                { liste.Add((secondeCatégorie.ToString(), (int)secondeCatégorie)); }
                break;
            case TypeDePlan.Créature:
                foreach (TypePlanCréatures secondeCatégorie in Enum.GetValues(typeof(TypePlanCréatures)))
                { liste.Add((secondeCatégorie.ToString(), (int)secondeCatégorie)); }
                break;
        }

        return liste.ToArray();
    }

    private bool CheckIdSousCatégorie(Id id_)
    { return CheckIdSousCatégorie((TypeDePlan)id_.Catégorie0, id_.Catégorie1); }
    /// <summary>
    /// vérifie si l'entier fournit correspond bien à la sous-catégorie correspondante au type donné
    /// </summary>
    /// <param name="premièreCatégorie_">le type de plan</param>
    /// <param name="secondeCatégorie_">la sous-catégorie de plans</param>
    /// <returns></returns>
    private bool CheckIdSousCatégorie(TypeDePlan premièreCatégorie_, int secondeCatégorie_)
    {
        switch (premièreCatégorie_)
        {
            case TypeDePlan.Train: return Enum.GetValues(typeof(TypePlanTrain)).Length > secondeCatégorie_;
            case TypeDePlan.Module: return Enum.GetValues(typeof(TypePlanModule)).Length > secondeCatégorie_;
            case TypeDePlan.Créature: return Enum.GetValues(typeof(TypePlanCréatures)).Length > secondeCatégorie_;
        }
        return false;
    }

    private IPlan CréerPlan(JObject json_, Id id_)
    { return CréerPlan(json_, (TypeDePlan)id_.Catégorie0, id_.Catégorie1); }
    /// <summary>
    /// en fonction de la sous-catégorie, appelle le constructeur de Plan correspondant
    /// </summary>
    /// <param name="json_">le JObject contenant le plan</param>
    /// <param name="type_">le type de plan</param>
    /// <param name="sousCatégorie_">la sous-catégorie du plan</param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    private IPlan CréerPlan(JObject json_, TypeDePlan type_, int sousCatégorie_)
    {
        IPlan plan = null;
        switch (type_)
        {
            case TypeDePlan.Train:
                switch ((TypePlanTrain)sousCatégorie_)
                {
                    case TypePlanTrain.Chassis:
                        //plan = new Chassis.Plan(json_);
                        break;
                    case TypePlanTrain.Corps:
                        //plan = new Corps.Plan(json_);
                        break;
                }
                break;
            case TypeDePlan.Module:
                switch ((TypePlanModule)sousCatégorie_)
                {
                    case TypePlanModule.Moteur:
                        //plan = new Moteur.Plan(json_);
                        break;
                }
                break;
            case TypeDePlan.Créature:
                switch ((TypePlanCréatures)sousCatégorie_)
                {
                    case TypePlanCréatures.Petit:
                        throw new NotImplementedException();
                }
                break;
        }

        return plan;
    }

    private Index.CatégorieIndexs TypePlanVersIndex(TypeDePlan type_)
    { return (Index.CatégorieIndexs) Enum.Parse(typeof(Index.CatégorieIndexs), type_.ToString(), false); }
    
    /// <summary>
    /// Charge les indexs de tout les plans en fonction de types et sous types présents dans les enums
    /// </summary>
    private void ChargerIndexs()
    {
        // charge tout les plans sous forme d'IndexJson en attendant qu'ils soient demandés, pour pouvoir les charger plus vite.
        lock (Cadenas)
        {
            Registre = new();
            CompteurInt compteur = new();
            foreach (TypeDePlan premièreCatégorie in Enum.GetValues(typeof(TypeDePlan)))
            {
                Dictionary<int, Dictionary<string, IndexPlanJson>> typePlans = new();
                foreach ((string NomCatégorie, int Id) secondeCatégorie in ListeSecondeCatégorie(premièreCatégorie))
                {
                    Dictionary<string, IndexPlanJson> modèles = new();
                    foreach (var index in IPlan.Indexs(Biblio.Index.RecupIndex(TypePlanVersIndex(premièreCatégorie), secondeCatégorie.NomCatégorie), ref compteur))
                    { modèles.Add(index.Key, index.Value); }
                    modèles.Remove("Template");

                    typePlans.Add(secondeCatégorie.Id, modèles);
                }
                Registre.Add((int)premièreCatégorie, typePlans);
            }
        }
    }

    public Ressource<IPlan> ChargerPlan(Id id_, ViePlan viePlan_)
    { return ChargerPlan((TypeDePlan)id_.Catégorie0, id_.Catégorie1, id_.Modèle, viePlan_); }
    public Ressource<IPlan> ChargerPlan(TypeDePlan type_, int sousCatégorie_, string nom_, ViePlan viePlan_)
    {
        if (Registre.ContainsKey((int)type_) && Registre[(int)type_].ContainsKey(sousCatégorie_) && Registre[(int)type_][sousCatégorie_].ContainsKey(nom_))
        {
            IndexPlanJson index = Registre[(int)type_][sousCatégorie_][nom_];
            IPlan plan = CréerPlan(Json.Ressource_Objet(Biblio.Index.RecupIndex(TypePlanVersIndex(type_), sousCatégorie_.ToString()), index.Début, index.Longueur), type_, sousCatégorie_);
            Ressource<IPlan> ressource = null;
            Ressource<IPlan>.ActionDécès décès = () => { Jeter(type_, sousCatégorie_, nom_); };
            switch (viePlan_.TypeDeVie)
            {
                case TypeViePlan.UsageUnique:
                    ressource = new(plan);
                    break;
                
                case TypeViePlan.Durée:
                    if (viePlan_.DuréeDeVie > 0f)
                    {
                        ressource = new Ressource<IPlan>(plan, IntervalleMaj, () => { return Time.GetTicksMsec() - ressource.DateCréation < viePlan_.DuréeDeVie; }, décès, null);
                        Plans[(int)type_][sousCatégorie_].Add(nom_, ressource);
                    }
                    else
                    { throw new ArgumentException($"La durée de vie fournit pour le plan [{(int)type_}][{sousCatégorie_}][{nom_}] n'est pas valide [{viePlan_.DuréeDeVie}] !!"); }
                    break;
                
                case TypeViePlan.Condition:
                    if (viePlan_.Condition != null)
                    {
                        ressource = new Ressource<IPlan>(plan, IntervalleMaj, viePlan_.Condition, décès, null);
                        Plans[(int)type_][sousCatégorie_].Add(nom_, ressource);
                    }
                    else
                    { throw new ArgumentException($"La condition de survie fournit pour le plan [{(int)type_}][{sousCatégorie_}][{nom_}] est null !!"); }
                    break;
            }

            return ressource;
        }
        
        throw new ArgumentException($"Aucun index pour le plan [{(int)type_}][{sousCatégorie_}][{nom_}] n'existe dans le Registre !!");
    }

    public Ressource<IPlan> RécupPlan(Id id_)
    { return RécupPlan((TypeDePlan)id_.Catégorie0, id_.Catégorie1, id_.Modèle, new ViePlan()); }
    public Ressource<IPlan> RécupPlan(Id id_, ViePlan viePlan_)
    { return RécupPlan((TypeDePlan)id_.Catégorie0, id_.Catégorie1, id_.Modèle, viePlan_); }
    public Ressource<IPlan> RécupPlan(TypeDePlan type_, int sousCatégorie_, string nom_)
    { return RécupPlan(type_, sousCatégorie_, nom_, new ViePlan()); }
    /// <summary>
    /// retourne le Plan demandé s'il existe dans le registre
    /// </summary>
    /// <param name="type_">le type de plan</param>
    /// <param name="sousCatégorie_">la sous-catégorie du plan</param>
    /// <param name="nom_">le nom du plan</param>
    /// <returns>une implémentation de IPlan correspondant à la sous-catégorie</returns>
    /// <exception cref="Exception">le plan n'existe pas dans le registre</exception>
    public Ressource<IPlan> RécupPlan(TypeDePlan type_, int sousCatégorie_, string nom_, ViePlan viePlan_)
    {
        lock (Cadenas)
        {
            int etat = EtatDeChargement(type_, sousCatégorie_, nom_);
            if (etat != 1)
            {
                if (etat == 0)
                { ChargerPlan(type_, sousCatégorie_, nom_, viePlan_); }
                else
                { throw new Exception("Le plan demandé n'existe pas."); }
            }
            return Plans[(int)type_][sousCatégorie_][nom_].Dupliquer();
        }
    }

    private int EtatDeChargement(Id id_)
    { return EtatDeChargement((TypeDePlan)id_.Catégorie0, id_.Catégorie1, id_.Modèle); }
    private int EtatDeChargement(TypeDePlan type_, int sousCatégorie_, string nom_)
    {
        lock (Cadenas)
        {
            if (Plans.ContainsKey((int)type_) && Plans[(int)type_].ContainsKey(sousCatégorie_) && Plans[(int)type_][sousCatégorie_].ContainsKey(nom_))
            { return Plans[(int)type_][sousCatégorie_][nom_].Ress is IndexPlanJson ? 0 : 1; }
            return -1;
        }
    }

    public bool EstChargé(Id id_)
    { return EstChargé((TypeDePlan)id_.Catégorie0, id_.Catégorie1, id_.Modèle); }
    public bool EstChargé(TypeDePlan type_, int sousCatégorie_, string nom_)
    {
        lock (Cadenas)
        { return EtatDeChargement(type_, sousCatégorie_, nom_) == 1; }
    }

    private bool Jeter(Id id_)
    { return Jeter((TypeDePlan)id_.Catégorie0, id_.Catégorie1, id_.Modèle); }
    private bool Jeter(TypeDePlan type_, int sousCatégorie_, string nom_)
    {
        lock (Cadenas)
        { return Plans.ContainsKey((int)type_) && Plans[(int)type_].ContainsKey(sousCatégorie_) && Plans[(int)type_][sousCatégorie_].Remove(nom_); }
    }

    public List<string> Liste(Id id_)
    { return Liste((TypeDePlan)id_.Catégorie0, id_.Catégorie1); }
    public List<string> Liste(TypeDePlan type_, int sousCatégorie_)
    {
        lock (Cadenas)
        {
            if (Plans.ContainsKey((int)type_) && Plans[(int)type_].ContainsKey(sousCatégorie_))
            { return Plans[(int)type_][sousCatégorie_].Keys.ToList(); }
            return new();
        }
    }
    
    //TODO: gérer la durée de vie des Ressourc<IPlan> 
}