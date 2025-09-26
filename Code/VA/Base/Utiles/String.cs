using System;
using Godot;

namespace VA.Base.Utiles;

public static class String
{
    /// <summary>
    /// Construit un matricule à partir de ses composants
    /// </summary>
    /// <param name="nom_"></param>
    /// <param name="séparateur_"></param>
    /// <param name="id_"></param>
    /// <returns></returns>
    public static string GénérerMatricule(string nom_ = "_", char séparateur_ = '#', int id_ = -1)
    { return $"{nom_}{séparateur_}{id_}"; }

    /// <summary>
    /// découpe un Matricule en ses deux composantes
    /// </summary>
    /// <param name="matricule_"></param>
    /// <param name="séparateur_"></param>
    /// <returns>retourne un id de -1 si le matricule est incorrect</returns>
    public static (string nom, int id) DéconstruireMatricule(string matricule_, char séparateur_)
    {
        string[] parties = matricule_.Split(séparateur_);
        if (parties.Length == 2)
        { return (parties[0], parties[1].ToInt()); }    // TODO: peut-être vérifier s'il s'agit bien de chiffres et mettre -1 si ce n'est pas le cas

        return ("_", -1);
    }


}