using System;
using System.Linq;

using Godot;

namespace VA.Base.Utiles;

public static class Conversions
{
    private static T[] DécouperBinaires<T>(byte[] binaires_, int nombreValeurs_, int taille_, Func<byte[], int, T> conversion_)
    {
        if (binaires_.Length == taille_ * nombreValeurs_)
        {
            byte[][] parties = binaires_.Découper(taille_).ToArray();
            return parties.Select(bytes => conversion_(bytes, 0)).ToArray();
        }

        throw new ArgumentException($"Les données binaires fournies ne permettent d'être découpé en {nombreValeurs_} parties: [{binaires_.Length} bits]");
    }
    
    private static float[] BytesVersFloats(byte[] binaires_, int nombreValeurs_)
    { return DécouperBinaires(binaires_, nombreValeurs_, sizeof(float), BitConverter.ToSingle); }
    
    private static int[] BytesVersEntiers(byte[] binaires_, int nombreValeurs_)
    { return DécouperBinaires(binaires_, nombreValeurs_, sizeof(float), BitConverter.ToInt32); }

    private static byte[] PrimitifsVersBytes<T>(T[] valeurs_)
    {
        if (!typeof(T).IsPrimitive)
        { throw new ArgumentException($"La liste de valeurs fournies n'est pas d'un type valide: [{typeof(T)}]"); }
        
        int sizeOfT = System.Runtime.InteropServices.Marshal.SizeOf(typeof(T));
        byte[] résultat = new byte[valeurs_.Length * sizeof(int)];
        for (int i = 0; i < valeurs_.Length; i++)
        {
            byte[] bytes = BitConverter.GetBytes((dynamic)valeurs_[i]);
            Array.Copy(bytes, 0, résultat, i * sizeOfT, sizeOfT);
        }
        return résultat;
    }

    public static float[] VersTab(this Vector2 vecteur_)
    { return new[] { vecteur_.X, vecteur_.Y }; }
    public static int[] VersTab(this Vector2I vecteur_)
    { return new[] { vecteur_.X, vecteur_.Y }; }
    public static float[] VersTab(this Vector3 vecteur_)
    { return new[] { vecteur_.X, vecteur_.Y, vecteur_.Z }; }
    public static int[] VersTab(this Vector3I vecteur_)
    { return new[] { vecteur_.X, vecteur_.Y, vecteur_.Z }; }
    public static float[] VersTab(this Vector4 vecteur_)
    { return new[] { vecteur_.X, vecteur_.Y, vecteur_.Z, vecteur_.W }; }
    public static int[] VersTab(this Vector4I vecteur_)
    { return new[] { vecteur_.X, vecteur_.Y, vecteur_.Z, vecteur_.W }; }

    public static byte[] Vector2VersBytes(Vector2 vecteur_)
    { return PrimitifsVersBytes(vecteur_.VersTab()); }

    public static Vector2 BytesVersVector2(byte[] binaires_)
    {
        float[] parties = BytesVersFloats(binaires_, 2);
        return new(parties[0], parties[1]);
    }
    
    public static byte[] Vector2IVersBytes(Vector2I vecteur_)
    { return PrimitifsVersBytes(vecteur_.VersTab()); }

    public static Vector2I BytesVersVector2I(byte[] binaires_)
    {
        int[] parties = BytesVersEntiers(binaires_, 2);
        return new(parties[0], parties[1]);
    }
    
    public static byte[] Vector3VersBytes(Vector3 vecteur_)
    { return PrimitifsVersBytes(vecteur_.VersTab()); }

    public static Vector3 BytesVersVector3(byte[] binaires_)
    {
        float[] parties = BytesVersFloats(binaires_, 3);
        return new(parties[0], parties[1], parties[2]);
    }
    
    public static byte[] Vector3IVersBytes(Vector3I vecteur_)
    { return PrimitifsVersBytes(vecteur_.VersTab()); }

    public static Vector3I BytesVersVector3I(byte[] binaires_)
    {
        int[] parties = BytesVersEntiers(binaires_, 3);
        return new(parties[0], parties[1], parties[2]);
    }
    
    public static byte[] Vector4VersBytes(Vector4 vecteur_)
    { return PrimitifsVersBytes(vecteur_.VersTab()); }

    public static Vector4 BytesVersVector4(byte[] binaires_)
    {
        float[] parties = BytesVersFloats(binaires_, 4);
        return new(parties[0], parties[1], parties[3], parties[4]);
    }
    
    public static byte[] Vector4IVersBytes(Vector4I vecteur_)
    { return PrimitifsVersBytes(vecteur_.VersTab()); }

    public static Vector4I BytesVersVector4I(byte[] binaires_)
    {
        int[] parties = BytesVersEntiers(binaires_, 4);
        return new(parties[0], parties[1], parties[3], parties[4]);
    }
}