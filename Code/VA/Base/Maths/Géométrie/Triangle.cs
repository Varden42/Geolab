using Godot;

namespace VA.Base.Maths.Géom;

public static class Triangle
{
    /// <summary>
    /// Vérifie si les trois points d'un Triangle sont alignés.
    /// </summary>
    public static bool EstPlat(Vector2 a_, Vector2 b_, Vector2 c_)
    {
        return (Algèbre.CompFloatEgal(a_.X, b_.X) && Algèbre.CompFloatEgal(b_.X, c_.X) 
                || (Algèbre.CompFloatEgal(a_.Y, b_.Y) && Algèbre.CompFloatEgal(b_.Y, c_.Y))
                || a_ == c_ || a_ == b_ || b_ == c_
                || Algèbre.CompFloatEgal((b_.X - a_.X) / (c_.X - a_.X), (b_.Y - a_.Y) / (c_.Y - a_.Y)));
    }
    
    /// <summary>
    /// Vérifie si les trois points d'un Triangle sont alignés.
    /// </summary>
    public static bool EstPlat(TriangleStruct2D triangle_) => EstPlat(triangle_.A, triangle_.B, triangle_.C);
    
    /// <summary>
    /// Vérifie si les trois points d'un Triangle sont alignés.
    /// </summary>
    public static bool EstPlat(TriangleStruct3D triangle_) => EstPlat(triangle_.A, triangle_.B, triangle_.C);

    /// <summary>
    /// Vérifie si les trois points d'un Triangle sont alignés.
    /// </summary>
    public static bool EstPlat(Vector3 a_, Vector3 b_, Vector3 c_)
    { return Algèbre.CompFloatEgal((b_ - a_).Cross(c_ - a_).Length(), 0f); }

    // TODO: faire la version 3D
    /// <summary>
    /// Calcul le Centre du Cercle Circonscrit à un Triangle.
    /// Le Triangle ne doit pas être plat
    /// </summary>
    /// <param name="a_">Le point A du Triangle</param>
    /// <param name="b_">Le point B du Triangle</param>
    /// <param name="c_">Le point C du Triangle</param>
    /// <returns>Le Centre du Cercle Circonscrit au Triangle</returns>
    public static Vector2 CentreCercleCirconscrit(Vector2 a_, Vector2 b_, Vector2 c_)
    {
        // Calcule les points médiants de deux des trois côtés.
        Vector2 A_C = Vecteurs.Médiane([a_, c_]), B_C = Vecteurs.Médiane([b_, c_]);

        Vector2 u = c_.Y == b_.Y ? new Vector2(0f, 1f) : new Vector2(1, (b_.X - c_.X) / (c_.Y - b_.Y));
        Vector2 v = c_.Y == a_.Y ? new Vector2(0f, 1f) : new Vector2(1, (a_.X - c_.X) / (c_.Y - a_.Y));

        Vector2 o = new Vector2();
        if (u.X == 0)
        {
            o.X = A_C.X + (B_C.X - A_C.X) * v.X;
            o.Y = A_C.X + (B_C.X - A_C.X) * v.Y;
        }
        else if (v.X == 0)
        {
            o.X = B_C.X + (A_C.X - B_C.X) * u.X;
            o.Y = B_C.X + (A_C.X - B_C.X) * u.Y;
        }
        else
        {
            float temp = (A_C.Y - B_C.Y + (B_C.X - A_C.X) * v.Y) / (u.Y - v.Y);
            o.X = B_C.X + temp * u.X;
            o.Y = B_C.Y + temp * u.Y;
        }

        return o;
    }
    
    /// <summary>
    /// Calcul le Centre du Cercle Circonscrit à un Triangle.
    /// Le Triangle ne doit pas être plat.
    /// </summary>
    /// <param name="triangle_">Le triangle</param>
    /// <returns>Le Centre du Cercle Circonscrit au Triangle</returns>
    public static Vector2 CentreCercleCirconscrit(TriangleStruct2D triangle_) => CentreCercleCirconscrit(triangle_.A, triangle_.B, triangle_.C);
    
    /// <summary>
    /// Calcul la normale d'un triangle dont les points sont listés dans le sens horaire
    /// </summary>
    public static Vector3 Normale(Vector3 a_, Vector3 b_, Vector3 c_)
    { return (a_ - b_).Normalized().Cross((c_ - b_).Normalized()).Normalized(); }
    
    /// <summary>
    /// Calcul la normale d'un triangle dont les points sont listés dans le sens horaire
    /// </summary>
    public static Vector3 Normale(TriangleStruct3D triangle_) => Normale(triangle_.A, triangle_.B, triangle_.C);
}

public struct TriangleStruct3D(Vector3 a_, Vector3 b_, Vector3 c_)
{
    public Vector3 A { get; private set; } = a_;
    public Vector3 B { get; private set; } = b_;
    public Vector3 C { get; private set; } = c_;
}

public struct TriangleStruct2D(Vector2 a_, Vector2 b_, Vector2 c_)
{
    public Vector2 A { get; private set; } = a_;
    public Vector2 B { get; private set; } = b_;
    public Vector2 C { get; private set; } = c_;
}