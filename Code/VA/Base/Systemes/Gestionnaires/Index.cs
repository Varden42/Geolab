using System;
using System.Collections.Generic;

namespace  VA.Base.Systèmes.Gestionnaires;

/// <summary>
/// Un emplacement commun pour tout les chemins de ressources et autres.
/// Facilite la recherche et simplifie la gestion du code lorsqu'une ressource est déplacée dans l'arborescence du projet.
/// </summary>
public class Index
{
    public enum CatégorieIndexs { Scène, Créature, Image, Style, Scripts, Matériaux }
    
    private static Index Singleton = new();
    private static readonly object Cadenas = new object();
    public static Index Instance => Singleton;

    private Dictionary<CatégorieIndexs, Dictionary<string, string>> Indexs;

    // Explicit static constructor to tell C# compiler not to mark type as beforefieldinit
    static Index()
    {    }

    private Index()
    {
        Indexs = new();
        {
            #region Scènes
            Dictionary<string, string> scènes = new();
            {
                scènes.Add("MenuPrincipal", "res://Menu/MenuPrincipal.cs");
                scènes.Add("Trajet", "res://Trajet/Trajet.cs");
            }
            Indexs.Add(CatégorieIndexs.Scène, scènes);
            #endregion
            
            #region Images
            Dictionary<string, string> images = new();
            {
                images.Add("IcônesGui", "res://Ressources/GUI/Icones");
            }
            Indexs.Add(CatégorieIndexs.Image, images);
            #endregion
            
            #region Créatures
            Dictionary<string, string> créatures = new();
            {
                créatures.Add("Petite", "");
                créatures.Add("Moyenne", "");
                créatures.Add("Grande", "");
            }
            Indexs.Add(CatégorieIndexs.Créature, créatures);
            #endregion
            
            #region Styles
            Dictionary<string, string> styles = new();
            {
                styles.Add("Debug", "res://VA/Styles/DebugStyle.tres");
                styles.Add("PanneauDebug", "res://VA/Styles/PanneauDebugStyle.tres");
            }
            Indexs.Add(CatégorieIndexs.Style, styles);
            #endregion
            
            #region Scripts
            Dictionary<string, string> scripts = new();
            {
                scripts.Add("TestsGéométrie", "res://Scripts/Tests/TestsGéométrie.cs");
            }
            Indexs.Add(CatégorieIndexs.Scripts, scripts);
            #endregion
            
            #region Matériaux
            Dictionary<string, string> matériaux = new();
            {
                matériaux.Add("TestMaterial_I", "res://Matériaux/TestMaterial_I.tres");
            }
            Indexs.Add(CatégorieIndexs.Matériaux, matériaux);
            #endregion
        }

        // TODO: Lorsque le MODING sera implémenté, charger les données relatives aux mods
        // gérer les conflits lorsque deux mods ajoutes des éléments de même nom
    }
    
    public string RecupIndex(CatégorieIndexs catégorieIndex_, string nom_)
    {
        lock (Cadenas)
        {
            if (Instance.Indexs.ContainsKey(catégorieIndex_) && Instance.Indexs[catégorieIndex_].ContainsKey(nom_))
            { return Instance.Indexs[catégorieIndex_][nom_]; }
            
            throw new ArgumentException($"L'index [{nom_}], dans la catégorie [{catégorieIndex_.ToString()}] n'existe pas !!");
        }
    }
}