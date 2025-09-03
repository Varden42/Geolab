using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Godot;

namespace VA.Base.Noeuds;

[SuppressMessage("ReSharper", "PossibleInvalidCastExceptionInForeachLoop")]
public static class Utiles
{
    /// <summary>
    /// Attaches a script to a node in the scene.
    /// </summary>
    /// <param name="noeud_">The node to attach the script to.</param>
    /// <param name="script_">The path to the script resource.</param>
    /// <returns>The instance of the attached script.</returns>
    public static GodotObject AttacherScript(Node noeud_, string script_)
    {
        ulong guiNodeId = noeud_.GetInstanceId();
        noeud_.SetScript(GD.Load<Script>(script_));
        return GodotObject.InstanceFromId(guiNodeId);
    }


    public static void ViderNode(Node noeud_)
    {
        foreach (Node enfant in noeud_.GetChildren())
        {
            noeud_.RemoveChild(enfant);
            enfant.QueueFree();
        }
    }
    
    /// <summary>
    /// Trouve le premier enfant du Noeud qui soit du type T
    /// </summary>
    /// <param name="parent_">La Noeud à parcourir</param>
    /// <param name="récursif_">Si l'on doit parcourir également les noeuds enfants</param>
    /// <typeparam name="T">Le type de Noeud à trouver</typeparam>
    /// <returns>Le Noeud de type T trouvé</returns>
    public static T TrouverNodeEnfant<T>(Node parent_, bool récursif_ = true) where T : Node
    {
        if (parent_ != null && parent_.GetChildCount() > 0)
        {
            foreach (Node enfant in parent_.GetChildren())
            {
                if (enfant is T)
                { return (T)enfant; }
            }
            
            if (récursif_)
            {
                foreach (Node enfant in parent_.GetChildren())
                { return TrouverNodeEnfant<T>(enfant); }
            }
        }
        return null;
    }
    
    /// <summary>
    /// Trouve tous les enfants du Noeud qui soit du type T
    /// </summary>
    /// <param name="parent_">La Noeud à parcourir</param>
    /// <param name="récursif_">Si l'on doit parcourir également les noeuds enfants</param>
    /// <typeparam name="T">Le type de Noeuds à trouver</typeparam>
    /// <returns>Une Liste des Noeuds de type T trouvés</returns>
    public static List<T> TrouverNodesEnfant<T>(Node parent_, bool récursif_ = true) where T : Node
    {
        List<T> enfants = new();
        if (parent_ != null && parent_.GetChildCount() > 0)
        {
            foreach (Node enfant in parent_.GetChildren())
            {
                if (enfant is T)
                { enfants.Add((T)enfant); }
            }
            
            if (récursif_)
            {
                foreach (Node enfant in parent_.GetChildren())
                { enfants.AddRange(TrouverNodesEnfant<T>(enfant)); }
            }
        }
        return enfants;
    }

    /// <summary>
    /// Trouve le premier enfant du Noeud qui soit du type T avec le nom spécifié
    /// </summary>
    /// <param name="parent_">La Noeud à parcourir</param>
    /// <param name="nom_">Le nom de l'enfant à trouver</param>
    /// <param name="récursif_">Si l'on doit parcourir également les noeuds enfants</param>
    /// <typeparam name="T">Le type de Noeud à trouver</typeparam>
    /// <returns>Le Noeud de type T trouvé</returns>
    public static T TrouverNodeEnfant<T>(Node parent_, string nom_, bool récursif_ = true) where T : Node
    {
        T résultat = null;
        if (parent_ != null && parent_.GetChildCount() > 0)
        {
            foreach (Node enfant in parent_.GetChildren())
            {
                if (enfant is T && enfant.Name == nom_)
                { return (T)enfant; }
            }
            
            if (récursif_)
            {
                foreach (Node enfant in parent_.GetChildren())
                {
                    résultat = TrouverNodeEnfant<T>(enfant, nom_);
                    if (résultat != null)
                    { return résultat; }
                }
            }
        }

        return résultat;
    }

    // TODO: faire une version qui prend un comparateur pour le nom, afin de fournir plus d'option de recherches
    /// <summary>
    /// Trouve tous les enfants du Noeud qui soit du type T avec le nom spécifié
    /// </summary>
    /// <param name="parent_">La Noeud à parcourir</param>
    /// <param name="nom_">Le nom des Noeuds à trouver</param>
    /// <param name="récursif_">Si l'on doit parcourir également les noeuds enfants</param>
    /// <typeparam name="T">Le type de Noeuds à trouver</typeparam>
    /// <returns>Une Liste des Noeuds de type T trouvés</returns>
    public static List<T> TrouverNodesEnfant<T>(Node parent_, string nom_, bool récursif_ = true) where T : Node
    {
        List<T> enfants = new();
        if (parent_ != null && parent_.GetChildCount() > 0)
        {
            foreach (Node enfant in parent_.GetChildren())
            {
                if (enfant is T && enfant.Name == nom_)
                { enfants.Add((T)enfant); }
            }
            
            if (récursif_)
            {
                foreach (Node enfant in parent_.GetChildren())
                { enfants.AddRange(TrouverNodesEnfant<T>(enfant, nom_)); }
            }
        }
        return enfants;
    }
    
    /// <summary>
    /// Trouve le premier enfant du Noeud avec le nom spécifié
    /// </summary>
    /// <param name="parent_">La Noeud à parcourir</param>
    /// <param name="nom_">Le nom de l'enfant à trouver</param>
    /// <param name="récursif_">Si l'on doit parcourir également les noeuds enfants</param>
    /// <returns>Le Noeud trouvé</returns>
    public static Node TrouverNodeEnfant(Node parent_, string nom_, bool récursif_ = true)
    {
        if (parent_ != null && parent_.GetChildCount() > 0)
        {
            foreach (Node enfant in parent_.GetChildren())
            {
                if (enfant.Name == nom_)
                { return enfant; }
            }
            
            if (récursif_)
            {
                foreach (Node enfant in parent_.GetChildren())
                { return TrouverNodeEnfant(enfant, nom_); }
            }
        }

        return null;
    }
    
    /// <summary>
    /// Trouve tous les enfants du Noeud avec le nom spécifié
    /// </summary>
    /// <param name="parent_">La Noeud à parcourir</param>
    /// <param name="nom_">Le nom des Noeuds à trouver</param>
    /// <param name="récursif_">Si l'on doit parcourir également les noeuds enfants</param>
    /// <returns>Une Liste des Noeuds de type T trouvés</returns>
    public static List<Node> TrouverNodesEnfant(Node parent_, string nom_, bool récursif_ = true)
    {
        List<Node> enfants = new();
        if (parent_ != null && parent_.GetChildCount() > 0)
        {
            foreach (Node enfant in parent_.GetChildren())
            {
                if (enfant.Name == nom_)
                { enfants.Add(enfant); }
            }
            
            if (récursif_)
            {
                foreach (Node enfant in parent_.GetChildren())
                { enfants.AddRange(TrouverNodesEnfant(enfant, nom_)); }
            }
        }
        return enfants;
    }
    
    /// <summary>
    /// Check si un Noeud est valide dans l'instance en cours
    /// </summary>
    /// <param name="noeud_"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static bool EstValide<T>(this T noeud_) where T : GodotObject
    { return noeud_ != null && GodotObject.IsInstanceValid(noeud_) && !noeud_.IsQueuedForDeletion(); }
    
    /// <summary>
    /// Supprime le Noeud de l'arborescence où il se trouve
    /// </summary>
    /// <param name="noeud_"></param>
    public static void Supprimer(this Node noeud_)
    { if (noeud_.EstValide()) noeud_.QueueFree(); }

    /// <summary>
    /// Remplace le parent d'un Noeud
    /// </summary>
    /// <param name="noeud_">le Noeud qui doit changer de parent</param>
    /// <param name="nouveauParent">Le nouveau Parent du Noeud</param>
    /// <returns></returns>
    public static bool EchangeParents(this Node noeud_, Node nouveauParent)
    {
        if (noeud_ is null || nouveauParent is null || noeud_.GetParent() == nouveauParent)
        { return false; }
        noeud_.GetParent().RemoveChild(noeud_);
        nouveauParent.AddChild(noeud_);
        return true;
    }
}