using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Godot;

namespace VA.Base.Debug;

/// <summary>
/// Permet de faire des entrées dans des journaux pour débuguer
/// </summary>
public static class Journal
{
    private static readonly string Emplacement = "user://Journaux";
    private static readonly int QuantitéeJournauxMax = 5, LongueurMaxMessage = 200; // TODO: changer ces valeurs via un fichier Json de configuration général
    
    // TODO: Plutôt que d'utiliser un enum static, permettre la création de catégories dynamiquement.
    // TODO: regrouper les catégories dans un dossier propre à la session en cours : "Journaux\Session_xxxx\Catégorie"
    public enum Catégories { Principale, Test }
    
    
    /// <summary>
    /// Stocke les journaux actifs de la session [Nom][[chemin du fichier][string du journal]]
    /// </summary>
    private static Dictionary<string, string[]> Journaux;

    static Journal()
    {
        Journaux = new();
        DirAccess.Open("user://").MakeDir("Journaux");
        Init();
    }

    /// <summary>
    /// Méthode servant à déclencher l'appel du constructeur static
    /// </summary>
    public static void Démarrer()
    {    }

    /// <summary>
    /// Scanne le dossier des journaux et crée les nouveaux journaux pour chaque catégories
    /// </summary>
    private static void Init()
    {
        DirAccess catégories = DirAccess.Open(Emplacement);
        if (catégories != null)
        {
            List<string> dossiers = new (catégories.GetDirectories());
            // Pour chaques catégories
            foreach (Catégories cat in (Catégories[])Enum.GetValues(typeof(Catégories)))
            {
                int id = 0;
                string nomCatégorie = Enum.GetName(cat);
                if (dossiers.Contains(nomCatégorie))
                {
                    // on récupère les journaux existants
                    DirAccess catégorie = DirAccess.Open($"{Emplacement}/{nomCatégorie}");
                    string[] journaux = catégorie.GetFiles().Where(j_ => VérifieNomFichier(j_, nomCatégorie)).ToArray();
                    var premierDernier = PremierDernierJournaux(journaux);
                    if (!journaux.IsEmpty())
                    {
                        id = premierDernier[1].id >= 1000000 ? 0 : premierDernier[1].id + 1;
                        // si la quantitée max est atteinte, on supprime le plus ancien
                        if (journaux.Length >= QuantitéeJournauxMax)
                        { catégorie.Remove($"{Emplacement}/{nomCatégorie}/{premierDernier[0].nom}"); }
                    }

                    dossiers.Remove(nomCatégorie);
                }
                else
                {
                    // Sinon on crée le dossier
                    catégories.MakeDir(nomCatégorie);
                }
                
                // On crée le nouveau Journal de la Catégorie pour la session en cours
                FileAccess auteur = FileAccess.Open($"{Emplacement}/{nomCatégorie}/Journal{nomCatégorie}_{id}.jnl", FileAccess.ModeFlags.Write);
                auteur.Close();
                        
                Journaux.Add(nomCatégorie, new []{ $"{Emplacement}/{nomCatégorie}/Journal{nomCatégorie}_{id}.jnl", "" });
            }
        }
    }

    /// <summary>
    /// Récupère le premier et le dernier journal d'une liste par rapport à leur id
    /// corrige le tri par charactère d'un string qui met 10 devant 9
    /// </summary>
    /// <param name="nomsFichiers_"></param>
    /// <returns></returns>
    private static (int id, string nom)[] PremierDernierJournaux(string[] nomsFichiers_)
    {
        (int id, string nom)[] tampon = new (int id, string nom)[]{ (-1, null), (int.MaxValue, null) };
        if (nomsFichiers_.Length > 0)
        {
            for (int f = 0; f < nomsFichiers_.Length; ++f)
            {
                string fichier = nomsFichiers_[f];
                int idFichier = int.Parse(fichier.Split('_', '.')[1]);
                if (f <= 0)
                {
                    tampon[0].id = idFichier;
                    tampon[0].nom = fichier;
                    
                    tampon[1].id = idFichier;
                    tampon[1].nom = fichier;
                }
                else
                {
                    if (idFichier < tampon[0].id)
                    {
                        if (tampon[1].nom == null)
                        { tampon[1] = tampon[0]; }
                        tampon[0].id = idFichier;
                        tampon[0].nom = fichier;
                    }
                    else if (idFichier > tampon[1].id)
                    {
                        tampon[1].id = idFichier;
                        tampon[1].nom = fichier;
                    }
                }
            }
            // foreach (string fichier in nomsFichiers_)
            // {
            //     int idFichier = int.Parse(fichier.Split('_', '.')[1]);
            //     if (tampon[0].nom == null)
            //     {
            //         tampon[0].id = idFichier;
            //         tampon[0].nom = fichier;
            //     }
            //     else
            //     {
            //         if (idFichier < tampon[0].id)
            //         {
            //             if (tampon[1].nom == null)
            //             { tampon[1] = tampon[0]; }
            //             tampon[0].id = idFichier;
            //             tampon[0].nom = fichier;
            //         }
            //         else if (idFichier > tampon[1].id)
            //         {
            //             tampon[1].id = idFichier;
            //             tampon[1].nom = fichier;
            //         }
            //     }
            // }
        }

        return tampon;
    }

    /// <summary>
    /// Ajoute une entrée dans un journal précis.
    /// </summary>
    /// <param name="message_">L'entrée à ajouter</param>
    /// <param name="journal_">Le journal dans lequel l'ajouter, par défaut, le journal principal</param>
    public static void Entrée(string message_, Catégories journal_ = Catégories.Principale)
    { EnregistrerEntrée(message_, Enum.GetName(journal_), new StackTrace()); }
    
    /// <summary>
    /// Ajoute une entrée dans un journal précis.
    /// </summary>
    /// <param name="entrée_">L'entrée à ajouter</param>
    /// <param name="journal_">Le journal dans lequel ajouter l'entrée. le crée s'il n'existe pas encore</param>
    public static void Entrée(string message_, string journal_)
    {
        if (!Journaux.ContainsKey(journal_))
        {
            int id = 0;
            DirAccess dossierJournal = DirAccess.Open($"{Emplacement}/{journal_}");
            if (dossierJournal != null)
            {
                string[] journaux = dossierJournal.GetFiles().Where(j_ => VérifieNomFichier(j_, journal_)).ToArray();
                var premierDernier = PremierDernierJournaux(journaux);
                if (!journaux.IsEmpty())
                {
                    id = premierDernier[1].id >= 1000000 ? 0 : premierDernier[1].id + 1;
                    // si la quantitée max est atteinte, on supprime le plus ancien
                    if (journaux.Length >= QuantitéeJournauxMax)
                    { dossierJournal.Remove($"{Emplacement}/{journal_}/{journaux.First()}"); }
                }
                // Journaux.Add(journal_, new []{ $"{Emplacement}/{journal_}/Journal{journal_}_{id}.jnl", "" });
                // FileAccess auteur = FileAccess.Open(Journaux[journal_][0], FileAccess.ModeFlags.Write);
                // auteur.Close();
            }
            else
            {
                // Créer le dossier
                DirAccess.Open(Emplacement).MakeDir(journal_);
                // Journaux.Add(journal_, new []{ $"{Emplacement}/{journal_}/Journal{journal_}_{id}.jnl", "" });
                // FileAccess auteur = FileAccess.Open(Journaux[journal_][0], FileAccess.ModeFlags.Write);
                // auteur.Close();
            }
            // on crée le journal
            Journaux.Add(journal_, new []{ $"{Emplacement}/{journal_}/Journal{journal_}_{id}.jnl", "" });
            FileAccess auteur = FileAccess.Open(Journaux[journal_][0], FileAccess.ModeFlags.Write);
            auteur.Close();
        }

        EnregistrerEntrée(message_, journal_, new StackTrace());
    }


    private static void EnregistrerEntrée(string message_, string journal_, StackTrace stack_)
    {
        string entrée = CréerEntrée(message_, new StackTrace(true));
        FileAccess auteur = FileAccess.Open(Journaux[journal_][0], FileAccess.ModeFlags.ReadWrite);
        auteur.SeekEnd();
        auteur.StoreLine(entrée);
        Journaux[journal_][1] += $"{entrée}\n";
        auteur.Close();
    }


    private static string CréerEntrée(string message_, StackTrace stack_)
    {
        StackFrame[] stackFrames = stack_.GetFrames();
        char séparateur = message_.Length >= LongueurMaxMessage ? '\n' : '-';
        string entrée = $"{DateTime.Now.ToString("[dd/MM/yyyy][HH:mm:ss]")}-[{stackFrames[2].GetFileName().Split('\\').Last().Split('.')[0]}|{stackFrames[2].GetFileLineNumber()}]{séparateur}{message_}";

        return entrée;
    }

    /// <summary>
    /// Vérifie si un fichier est nommé correctement et peut donc être considéré comme un journal.
    /// </summary>
    /// <param name="nom_">Le nom du fichier à vérifier</param>
    /// <param name="catégorie_">La catégorie à laquelle le fichier doit correspondre</param>
    private static bool VérifieNomFichier(string nom_, string catégorie_)
    {
        string[] parties = nom_.Split('_', '.');
        //GD.Print($"[{parties[0]}][{parties[1]}][{parties[2]}]");
        return parties.Length == 3 && parties[0] == $"Journal{catégorie_}" && int.TryParse(parties[1], out int _) && parties[2] == "jnl";
    }
}