using System;
using Godot;
using Newtonsoft.Json.Linq;

namespace VA.Base.Ressources;

public static class Fichier
{
    public static string ChargerFichier(string chemin_)
    {
        string texte = String.Empty;
        if (FileAccess.FileExists(chemin_))
        { texte = FileAccess.GetFileAsString(chemin_); }
        else
        { GD.PrintErr($"Impossible de charger le fichier à l'emplacement {chemin_} !!"); }

        return texte;
    }
    public static string ChargerPartieFichier(string chemin_, ulong Début_, ulong Longueur_)
    {
        string texte = String.Empty;
        if (FileAccess.FileExists(chemin_))
        {
            FileAccess lecteur = FileAccess.Open(chemin_, FileAccess.ModeFlags.Read);
            lecteur.Seek(Début_); 
            byte[] data = lecteur.GetBuffer((long)Longueur_);
            lecteur.Close();
    
            texte = System.Text.Encoding.UTF8.GetString(data);
        }
        else
        { GD.PrintErr($"Impossible de charger le fichier à l'emplacement {chemin_} !!"); }

        return texte;
    }

    public static void LireVersCaractère(ref FileAccess lecteur_, char caractère_)
    {
        byte car = lecteur_.Get8(), cible = (byte)caractère_;
        while (!lecteur_.EofReached() && car != cible)
        { car = lecteur_.Get8(); }
    }

    public static string LireEntreCaratères(ref FileAccess lecteur_, char caractère_)
    {
        string résultat = "";
        byte car = lecteur_.Get8(), cible = (byte)caractère_;;
        
        while (!lecteur_.EofReached() && car != cible)
        { car = lecteur_.Get8(); }
        
        while (!lecteur_.EofReached() && car != cible)
        { résultat += (char)lecteur_.Get8(); }

        return résultat;
    }

    public static void Reculer(ref FileAccess lecteur_, ulong distance_)
    {
        if (lecteur_.GetPosition() >= distance_)
        { lecteur_.Seek(lecteur_.GetPosition() - distance_); }
    }
}