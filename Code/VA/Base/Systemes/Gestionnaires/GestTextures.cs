using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Godot;

using VA.Base.Debug;
using VA.Base.Ressources;

namespace VA.Base.Systèmes.Gestionnaires;

/// <summary>
/// Gère le chargement et déchargement des Textures.
/// </summary>
public sealed class GestTextures
{
    private static readonly GestTextures Singleton = new();
    private static readonly object Cadenas = new object();
    public static GestTextures Instance => Singleton;

    public enum TypesTextures { Texture2D, Texture3D, TextureLayered }
    private Dictionary<string, Ressource<Texture2D>> Textures2D;
    private Dictionary<string, Ressource<Texture3D>> Textures3D;
    private Dictionary<string, Ressource<TextureLayered>> TexturesLayered;

    // Explicit static constructor to tell C# compiler
    // not to mark type as beforefieldinit
    static GestTextures()
    {    }

    private GestTextures()
    {
        Textures2D = new();
        Textures2D.Add("Défaut", new(Images.Texture2DParDéfaut()));
        
        Textures3D = new();
        Textures3D.Add("Défaut", new(Images.Texture3DParDéfaut()));
        
        TexturesLayered = new();
        TexturesLayered.Add("Défaut", new(Images.TextureLayeredParDéfaut()));
    }
    
    // TODO: créer un fonction pour charger une ressource temporairement
    
    /// <summary>
    /// Charge une texture précise à l'emplacement choisi
    /// </summary>
    /// <param name="cheminRessource_"></param>
    /// <param name="nom_"></param>
    /// <param name="forcer_"></param>
    /// <returns></returns>
    public Ressource<Texture2D> ChargerTexture2D(string cheminRessource_, string nom_ = "", bool forcer_ = false)
    {
        lock (Cadenas)
        {
            nom_ = nom_ == "" ? Path.GetFileName(cheminRessource_).Split('.')[0] : nom_;
            
            if (!Textures2D.ContainsKey(nom_))
            {
                Texture2D texture = Images.ImageVersTexture2D(cheminRessource_);
                Textures2D[nom_] = new(texture);
            }
            else if (Textures2D.ContainsKey(nom_) && forcer_)
            { Textures2D[nom_].MajRess(Images.ImageVersTexture2D(cheminRessource_)); }
            else
            { Journal.Entrée($"Une Texture2D existe déjà avec le nom [{nom_}], chargement impossible"); }

            return Textures2D[nom_];
        }
    }
    
    public void ChargerDossierTextures2D(string cheminDossier_, bool forcer_ = false)
    {
        lock (Cadenas)
        {
            foreach (string ressource in Fichiers.Génériques.RécupListeFichiers(cheminDossier_))
            { ChargerTexture2D(ressource); }
        }
    }
    
    public Ressource<Texture2D> ChargerTexture2D(Texture2D texture_, string nom_, bool forcer_ = false)
    {
        lock (Cadenas)
        {
            if (!Textures2D.ContainsKey(nom_))
            { Textures2D[nom_] = new(texture_); }
            else if (Textures2D.ContainsKey(nom_) && forcer_)
            { Textures2D[nom_].MajRess(texture_); }
            else
            { Journal.Entrée($"Une Texture2D existe déjà avec le nom [{nom_}], chargement impossible"); }

            return Textures2D[nom_];
        }
    }

    public void ChargerTexture3D(string cheminRessource_, string nom_, bool forcer_ = false)
    { throw new NotImplementedException(); }

    public void ChargerTextureLayered(string cheminRessource_, string nom_, bool forcer_ = false)
    { throw new NotImplementedException(); }

    public Ressource<Texture2D> RécupTexture2D(string nom_)
    {
        lock (Cadenas)
        {
            if (Textures2D.TryGetValue(nom_, out Ressource<Texture2D> texture))
            { return texture; }

            Journal.Entrée($"La texture [{nom_}] n'a pas pu être récupérée!");
            return Textures2D["défaut"];
        }
    }
    
    public Ressource<Texture3D> RécupTexture3D(string nom_)
    {
        throw new NotImplementedException();
        // if (Textures3D.TryGetValue(nom_, out Ressource<Texture3D> texture))
        // { return texture; }
        // Journal.Entrée($"La texture [{nom_}] n'a pas pu être récupérée!");
        // return Textures3D["défaut"];
    }
    
    public Ressource<TextureLayered> RécupTextureLayered(string nom_)
    {
        throw new NotImplementedException();
        // if (TexturesLayered.TryGetValue(nom_, out Ressource<TextureLayered> texture))
        // { return texture; }
        // Journal.Entrée($"La texture [{nom_}] n'a pas pu être récupérée!");
        // return TexturesLayered["défaut"];
    }

    // public Texture Récup(Types type_, string nom_)
    // {
    //     switch (type_)
    //     {
    //         case Types.Texture2D:
    //             return RécupTexture2D(nom_).Ress;
    //         case Types.Texture3D:
    //             return RécupTexture3D(nom_);
    //         case Types.TextureLayered:
    //             return RécupTextureLayered(nom_);
    //     }
    //
    //     return null;
    // }


    public bool EstChargé(TypesTextures typeTextures_, string nom_)
    {
        lock (Cadenas)
        {
            switch (typeTextures_)
            {
                case TypesTextures.Texture2D:
                    return Textures2D.ContainsKey(nom_);
                case TypesTextures.Texture3D:
                    return Textures3D.ContainsKey(nom_);
                case TypesTextures.TextureLayered:
                    return TexturesLayered.ContainsKey(nom_);
            }

            return false;
        }
    }


    public bool Jeter(TypesTextures typeTextures_, string nom_)
    {
        lock (Cadenas)
        {
            switch (typeTextures_)
            {
                case TypesTextures.Texture2D:
                    return Textures2D.Remove(nom_);
                case TypesTextures.Texture3D:
                    return Textures3D.Remove(nom_);
                case TypesTextures.TextureLayered:
                    return TexturesLayered.Remove(nom_);
            }

            return false;
        }
    }

    public List<string> Liste(TypesTextures typeTexturesTexture_)
    {
        lock (Cadenas)
        {
            List<string> liste = new();
            switch (typeTexturesTexture_)
            {
                case TypesTextures.Texture2D:
                    liste = Textures2D.Keys.ToList();
                    break;
                case TypesTextures.Texture3D:
                    liste = Textures3D.Keys.ToList();
                    break;
                case TypesTextures.TextureLayered:
                    liste = TexturesLayered.Keys.ToList();
                    break;
            }

            return liste;
        }
    }
}