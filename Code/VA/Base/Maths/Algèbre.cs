using System.Collections.Generic;
using Godot;

namespace VA.Base.Maths;

public class Algèbre
{
    // Cosinus courants
    public static readonly float Cos0 = 1f;
    public static readonly float Cos30 = Mathf.Sqrt(3f) * 0.5f;
    public static readonly float Cos45 = Mathf.Sqrt(2f) * 0.5f;
    public static readonly float Cos60 = 0.5f;
    public static readonly float Cos90 = 0f;
    //Sinus Courant
    public static readonly float Sin0 = 0f;
    public static readonly float Sin30 = 0.5f;
    public static readonly float Sin45 = Mathf.Sqrt(2f) * 0.5f;
    public static readonly float Sin60 = Mathf.Sqrt(3f) * 0.5f;
    public static readonly float Sin90 = 1f;
        
    public static readonly double PI = 3.14159265358979323846264338327950288d;
    public static readonly double TAU = 6.28318530717958647692528676655900577d;

    // Float comparaisons
    public static bool CompFloatEgal(float a_, float b_, float epsilon_ = float.Epsilon)
    { return Mathf.Abs(Mathf.Abs(a_) - Mathf.Abs(b_)) < epsilon_; }

    public static bool CompFloatInfEgal(float a_, float b_, float epsilon_ = float.Epsilon)
    { return CompFloatEgal(a_, b_, epsilon_) ? true : a_ < b_; }

    public static bool CompFloatSupEgal(float a_, float b_, float epsilon_ = float.Epsilon)
    { return CompFloatEgal(a_, b_, epsilon_) ? true : a_ > b_; }

    public static bool CompFloatInf(float a_, float b_, float epsilon_ = float.Epsilon)
    { return CompFloatEgal(a_, b_, epsilon_) ? false : a_ < b_; }

    public static bool CompFloatSup(float a_, float b_, float epsilon_ = float.Epsilon)
    { return CompFloatEgal(a_, b_, epsilon_) ? false : a_ > b_; }

    // Double comparaisons
    public static bool CompDoubleEgal(double a_, double b_, double epsilon_ = double.Epsilon)
    { return Mathf.Abs(Mathf.Abs(a_) - Mathf.Abs(b_)) < epsilon_; }

    public static bool CompDoubleInfEgal(double a_, double b_, double epsilon_ = double.Epsilon)
    { return CompDoubleEgal(a_, b_, epsilon_) ? true : a_ < b_; }

    public static bool CompDoubleSupEgal(double a_, double b_, double epsilon_ = double.Epsilon)
    { return CompDoubleEgal(a_, b_, epsilon_) ? true : a_ > b_; }

    public static bool CompDoubleInf(double a_, double b_, double epsilon_ = double.Epsilon)
    { return CompDoubleEgal(a_, b_, epsilon_) ? false : a_ < b_; }

    public static bool CompDoubleSup(double a_, double b_, double epsilon_ = double.Epsilon)
    { return CompDoubleEgal(a_, b_, epsilon_) ? false : a_ > b_; }

    // Comparaisons de Vecteurs //
    // Compare deux position 3D et détermine si A_ et égal à B_ dans l'ordre (0, 0, 0) => (1, 1, 1) et Z < X < Y
    public static bool CompVec3DFEgal(Vector3 A_, Vector3 B_, float tolerance_ = float.Epsilon)
    { return CompFloatEgal(A_.Y, B_.Y, tolerance_) ? CompFloatEgal(A_.X, B_.X, tolerance_) ? CompFloatEgal(A_.Z, B_.Z, tolerance_) : false : false; }

    // Compare deux position 3D et détermine si A_ et inférieur à B_ dans l'ordre (0, 0, 0) => (1, 1, 1) et Z < X < Y
    public static bool CompVec3DFInf(Vector3 A_, Vector3 B_, float tolerance_ = float.Epsilon)
    { return CompFloatEgal(A_.Y, B_.Y, tolerance_) ? CompFloatEgal(A_.Z, B_.Z, tolerance_) ? CompFloatInf(A_.X, B_.X, tolerance_) : CompFloatInf(A_.Z, B_.Z, tolerance_) : CompFloatInf(A_.Y, B_.Y, tolerance_); }

    // Compare deux position 3D et détermine si A_ et inférieur ou égal à B_ dans l'ordre (0, 0, 0) => (1, 1, 1) et Z < X < Y
    public static bool CompVec3DFInfEgal(Vector3 A_, Vector3 B_, float tolerance_ = float.Epsilon)
    { return CompFloatEgal(A_.Y, B_.Y, tolerance_) ? CompFloatEgal(A_.Z, B_.Z, tolerance_) ? CompFloatInfEgal(A_.X, B_.X, tolerance_) : CompFloatInf(A_.Z, B_.Z, tolerance_) : CompFloatInf(A_.Y, B_.Y, tolerance_); }

    // Compare deux position 3D et détermine si A_ et supérieur à B_ dans l'ordre (0, 0, 0) => (1, 1, 1) et Z < X < Y
    public static bool CompVec3DFSup(Vector3 A_, Vector3 B_, float tolerance_ = float.Epsilon)
    { return CompFloatEgal(A_.Y, B_.Y, tolerance_) ? CompFloatEgal(A_.Z, B_.Z, tolerance_) ? CompFloatInf(A_.X, B_.X, tolerance_) : CompFloatSup(A_.Z, B_.Z, tolerance_) : CompFloatSup(A_.Y, B_.Y, tolerance_); }

    // Compare deux position 3D et détermine si A_ et supérieur ou égal à B_ dans l'ordre (0, 0, 0) => (1, 1, 1) et Z < X < Y
    public static bool CompVec3DFSupEgal(Vector3 A_, Vector3 B_, float tolerance_ = float.Epsilon)
    { return CompFloatEgal(A_.Y, B_.Y, tolerance_) ? CompFloatEgal(A_.Z, B_.Z, tolerance_) ? CompFloatSupEgal(A_.X, B_.X, tolerance_) : CompFloatSup(A_.Z, B_.Z, tolerance_) : CompFloatSup(A_.Y, B_.Y, tolerance_); }

    // Compare deux position 3D et détermine si A_ et inférieur à B_ dans l'ordre (0, 0, 0) => (1, 1, 1) et Z < X < Y
    public static bool CompVec3DIInf(Vector3I A_, Vector3I B_, float tolerance_ = float.Epsilon)
    { return A_.Y == B_.Y ? A_.Z == B_.Z ? A_.X < B_.X : A_.Z < B_.Z : A_.Y < B_.Y; }

    // Compare deux position 3D et détermine si A_ et inférieur ou égal à B_ dans l'ordre (0, 0, 0) => (1, 1, 1) et Z < X < Y
    public static bool CompVec3DIInfEgal(Vector3 A_, Vector3 B_, float tolerance_ = float.Epsilon)
    { return A_.Y == B_.Y ? A_.Z == B_.Z ? A_.X <= B_.X : A_.Z < B_.Z : A_.Y < B_.Y; }

    // Compare deux position 3D et détermine si A_ et inférieur à B_ dans l'ordre (0, 0, 0) => (1, 1, 1) et Z < X < Y
    public static bool CompVec3DISup(Vector3I A_, Vector3I B_, float tolerance_ = float.Epsilon)
    { return A_.Y == B_.Y ? A_.Z == B_.Z ? A_.X > B_.X : A_.Z > B_.Z : A_.Y > B_.Y; }

    // Compare deux position 3D et détermine si A_ et inférieur ou égal à B_ dans l'ordre (0, 0, 0) => (1, 1, 1) et Z < X < Y
    public static bool CompVec3DISupEgal(Vector3 A_, Vector3 B_, float tolerance_ = float.Epsilon)
    { return A_.Y == B_.Y ? A_.Z == B_.Z ? A_.X >= B_.X : A_.Z > B_.Z : A_.Y > B_.Y; }
    
    /// <summary>
    /// Trouve la valeur minimale
    /// </summary>
    /// <param name="liste_"></param>
    /// <returns></returns>
    public static float MinF(IEnumerable<float> liste_)
    {
        float min = float.MaxValue;
        foreach (float f in liste_)
        { if (CompFloatInf(f, min)) min = f; }
        return min;
    }

    /// <summary>
    /// Trouve la valeur minimale
    /// </summary>
    /// <param name="liste_"></param>
    /// <returns></returns>
    public static int MinI(IEnumerable<int> liste_)
    {
        int min = int.MaxValue;
        foreach (int i in liste_)
        { if (i < min) min = i; }
        return min;   
    }
    
    /// <summary>
    /// Trouve la valeur maximale
    /// </summary>
    /// <param name="liste_"></param>
    /// <returns></returns>
    public static float MaxF(IEnumerable<float> liste_)
    {
        float max = float.MinValue;
        foreach (float f in liste_)
        { if (CompFloatSup(f, max)) max = f; }
        return max;
    }

    /// <summary>
    /// Trouve la valeur maximale
    /// </summary>
    /// <param name="liste_"></param>
    /// <returns></returns>
    public static int MaxI(IEnumerable<int> liste_)
    {
        int max = int.MinValue;
        foreach (int i in liste_)
        { if (i > max) max = i; }
        return max;  
    }

    /// <summary>
    /// Calcul la somme des n premier entiers.
    /// </summary>
    /// <param name="n">la valeur de "n".</param>
    /// <returns>la somme des n premiers entiers.</returns>
    public static int SomPremEntier(int n)
    { return (n * (n + 1) / 2); }
}