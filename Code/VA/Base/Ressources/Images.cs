using System;
using Godot;

namespace VA.Base.Ressources;

public static class Images
{
    public static Texture2D Texture2DParDéfaut()
    {
        Image imageDeRemplacement = Image.Create(8, 8, false, Image.Format.Rgba8);
        imageDeRemplacement.Fill(new(0.8f, 0f, 0.8f, 1f));
        return ImageTexture.CreateFromImage(imageDeRemplacement);
    }
    
    public static Texture3D Texture3DParDéfaut()
    { throw new NotImplementedException(); }
    
    public static TextureLayered TextureLayeredParDéfaut()
    { throw new NotImplementedException(); }
    
    
    public static Texture2D ImageVersTexture2D(string chemin_)
    {
        if (ResourceLoader.Exists(chemin_) && FichierValide(chemin_))
        { return ResourceLoader.Load<Texture2D>(chemin_); }

        return Texture2DParDéfaut();
    }

    public static bool FichierValide(string nom_)
    { return nom_.ToLower().EndsWith(".png") || nom_.ToLower().EndsWith(".jpg"); } // TODO: ajouter d'autres extensions


    public static Texture2D IcôneManquant()
    {
        // TODO: Créer un icône avec un point d'interrogation
        return Texture2DParDéfaut();
    }
}