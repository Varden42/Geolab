using System;
using System.Diagnostics;
using Godot;

namespace VA.Base.Maths;

public static class Vecteurs
{
    // /// <summary>
    // /// Calcule le vecteur médian.
    // /// </summary>
    // /// <param name="a_"></param>
    // /// <param name="b_"></param>
    // /// <returns></returns>
    // public static Vector2 Médiane(Vector2 a_, Vector2 b_) => Médiane([a_, b_]);
    // { return new Vector2((a_.X + b_.X) / 2, (a_.Y + b_.Y) / 2); }
    
    /// <summary>
    /// Calcule le vecteur médiane entre les vecteurs fournit
    /// </summary>
    /// <param name="vecteurs_">La liste des vecteurs</param>
    /// <returns></returns>
    public static Vector2 Médiane(Vector2[] vecteurs_)
    {
        Vector2 mediane = new Vector2();
        foreach (Vector2 vecteur in vecteurs_)
        { mediane += vecteur; }
        mediane /= vecteurs_.Length;
        return mediane;
    }
    
    // /// <summary>
    // /// Calcule le vecteur médian.
    // /// </summary>
    // /// <param name="a_"></param>
    // /// <param name="b_"></param>
    // /// <param name="c_"></param>
    // /// <returns></returns>
    // public static Vector3 Médiane(Vector3 a_, Vector3 b_, Vector3 c_)
    // { return new Vector3((a_.X + b_.X + c_.X) / 3, (a_.Y + b_.Y + c_.Y) / 3, (a_.Z + b_.Z + c_.Z) / 3); }

    /// <summary>
    /// Calcule le vecteur médiane entre les vecteurs fournit
    /// </summary>
    /// <param name="vecteurs_">La liste des vecteurs</param>
    /// <returns></returns>
    public static Vector3 Médiane(Vector3[] vecteurs_)
    {
        Vector3 mediane = new Vector3();
        foreach (Vector3 vecteur in vecteurs_)
        { mediane += vecteur; }
        mediane /= vecteurs_.Length;
        return mediane;
    }


    public static Vector2 Bissectrice(Vector2 a_, Vector2 b_)
    { return (a_.Normalized() + b_.Normalized()).Normalized(); }
    
    public static Vector3 Bissectrice(Vector3 a_, Vector3 b_)
    { return (a_.Normalized() + b_.Normalized()).Normalized(); }
    
    public static Vector2 OctahedronEncode(Vector3 vecteur_)
    {
        vecteur_ /= Mathf.Abs(vecteur_.X) + Mathf.Abs(vecteur_.Y) + Mathf.Abs(vecteur_.Z);
        Vector2 o;
        if (vecteur_.Z >= 0.0f) {
            o.X = vecteur_.X;
            o.Y = vecteur_.Y;
        } else {
            o.X = (1.0f - Mathf.Abs(vecteur_.Y)) * (vecteur_.X >= 0.0f ? 1.0f : -1.0f);
            o.Y = (1.0f - Mathf.Abs(vecteur_.X)) * (vecteur_.Y >= 0.0f ? 1.0f : -1.0f);
        }
        o.X = o.X * 0.5f + 0.5f;
        o.Y = o.Y * 0.5f + 0.5f;
        return o;
    }

    public static Vector3 OctahedronDecode(in Vector2 vecteurCompressé_)
    {
        Vector2 f = new(vecteurCompressé_.X * 2.0f - 1.0f, vecteurCompressé_.Y * 2.0f - 1.0f);
        Vector3 vecteurDécompressé_ = new(f.X, f.Y, 1.0f - Mathf.Abs(f.X) - Mathf.Abs(f.Y));
        float t = Mathf.Clamp(-vecteurDécompressé_.Z, 0.0f, 1.0f);
        vecteurDécompressé_.X += vecteurDécompressé_.X >= 0 ? -t : t;
        vecteurDécompressé_.Y += vecteurDécompressé_.Y >= 0 ? -t : t;
        return vecteurDécompressé_.Normalized();
    }
    
    public static Vector2 octahedron_tangent_encode(Vector4 tangente_)
    {
        const float bias = 1.0f / 32767.0f;
        Vector2 res = OctahedronEncode(new(tangente_.X, tangente_.Y, tangente_.Z));
        res.Y = Mathf.Max(res.Y, bias);
        res.Y = res.Y * 0.5f + 0.5f;
        res.Y = tangente_.W >= 0.0f ? res.Y : 1 - res.Y;
        return res;
    }

    public static byte[] EncodeNormalTangente(Vector3 normale_, Vector3 tangente_, float w_)
    { return EncodeNormalTangente(normale_, new (tangente_.X, tangente_.Y, tangente_.Z, w_)); }
    public static byte[] EncodeNormalTangente(Vector3 normale_, Vector4 tangente_)
    {
        //Stopwatch chrono = Stopwatch.StartNew();
        
        byte[] datasNormaleTangente = new byte[sizeof(UInt16) * 4];
        int tailleComposant = sizeof(UInt16);
        Vector2 normaleOptimisée = OctahedronEncode(normale_), tangenteOptimisée = octahedron_tangent_encode(tangente_);
        // comme la normale est normalisée, on peut réduire le nombre de bits qu'elle occupe
        UInt16[] normaleTangenteCompressée =
        {
            (UInt16)Mathf.Clamp(normaleOptimisée.X * 65535, 0, 65535), 
            (UInt16)Mathf.Clamp(normaleOptimisée.Y * 65535, 0, 65535),
            (UInt16)Mathf.Clamp(tangenteOptimisée.X * 65535, 0, 65535), 
            (UInt16)Mathf.Clamp(tangenteOptimisée.Y * 65535, 0, 65535)
        };
        
        Buffer.BlockCopy(BitConverter.GetBytes(normaleTangenteCompressée[0]), 0, datasNormaleTangente, 0, tailleComposant);
        Buffer.BlockCopy(BitConverter.GetBytes(normaleTangenteCompressée[1]), 0, datasNormaleTangente, tailleComposant, tailleComposant);
        Buffer.BlockCopy(BitConverter.GetBytes(normaleTangenteCompressée[2]), 0, datasNormaleTangente, tailleComposant * 2, tailleComposant);
        Buffer.BlockCopy(BitConverter.GetBytes(normaleTangenteCompressée[3]), 0, datasNormaleTangente, tailleComposant * 3, tailleComposant);

        //chrono.Stop();
            
        //GD.Print($"EncodeNormalTangente : {chrono.Elapsed.TotalMilliseconds} ms");
        
        
        return datasNormaleTangente;
    }

    /// <summary>
    /// Tourne le vecteur de 90°
    /// </summary>
    /// <param name="vecteur_"></param>
    /// <param name="sens_">true = sens horaire</param>
    /// <returns></returns>
    public static Vector2 RotationVecteur90(Vector2 vecteur_, bool sens_ = true)
    { return sens_ ? new Vector2(vecteur_.Y, -vecteur_.X) : new Vector2(-vecteur_.Y, vecteur_.X); }
    /// <summary>
    /// Tourne le vecteur de 90°
    /// </summary>
    /// <param name="vecteur_"></param>
    /// <param name="sens_">true = sens horaire</param>
    /// <returns></returns>
    public static Vector2I RotationVecteur90(Vector2I vecteur_, bool sens_ = true)
    { return sens_ ? new Vector2I(vecteur_.Y, -vecteur_.X) : new Vector2I(-vecteur_.Y, vecteur_.X); }
    
    public static void GarderDansPlage(ref Vector2 vec_, Vector2 plageX_, Vector2 plageY_)
    {
        vec_.X = Mathf.Clamp(vec_.X, plageX_.X, plageX_.Y);
        vec_.Y = Mathf.Clamp(vec_.Y, plageY_.X, plageY_.Y);
    }
    public static void GarderDansPlage(ref Vector2I vec_, Vector2I plageX_, Vector2I plageY_)
    {
        vec_.X = Mathf.Clamp(vec_.X, plageX_.X, plageX_.Y);
        vec_.Y = Mathf.Clamp(vec_.Y, plageY_.X, plageY_.Y);
    }
    public static void GarderDansPlage(ref Vector3 vec_, Vector3 plageX_, Vector3 plageY_, Vector3 plageZ_)
    {
        vec_.X = Mathf.Clamp(vec_.X, plageX_.X, plageX_.Y);
        vec_.Y = Mathf.Clamp(vec_.Y, plageY_.X, plageY_.Y);
        vec_.Z = Mathf.Clamp(vec_.Z, plageZ_.X, plageY_.Y);
    }
    public static void GarderDansPlage(ref Vector3I vec_, Vector3I plageX_, Vector3I plageY_, Vector3I plageZ_)
    {
        vec_.X = Mathf.Clamp(vec_.X, plageX_.X, plageX_.Y);
        vec_.Y = Mathf.Clamp(vec_.Y, plageY_.X, plageY_.Y);
        vec_.Z = Mathf.Clamp(vec_.Z, plageZ_.X, plageY_.Y);
    }
}