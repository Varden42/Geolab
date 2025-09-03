using System;
using Godot;

namespace VA.Base.Maths.Géom;

public static class Rectangle
{
    #region 2D
    

    /// <summary>
    /// Calcule la surface d'un rectangle
    /// </summary>
    /// <param name="a_"></param>
    /// <param name="b_"></param>
    /// <param name="c_"></param>
    /// <param name="d_"></param>
    /// <param name="verifRect_">true = vérifier que les 4 points représentent un rectangle</param>
    /// <returns></returns>
    public static float Surface(Vector2 a_, Vector2 b_, Vector2 c_, Vector2 d_, bool verifRect_ = true)
    {
        if (verifRect_ && !EstRectangle(a_, b_, c_, d_))
        { return -1f; }
        if (EstAABB(a_, b_, c_, d_))
        { return (d_.X - a_.X) * (b_.Y - a_.Y); }

        return (b_ - a_).Length() * (d_ - a_).Length();
    }
    
    /// <summary>
    /// Calcule la surface d'un rectangle
    /// </summary>
    /// <param name="quad_"></param>
    /// <param name="verifRect_">true = vérifier que la structure représente un rectangle</param>
    /// <returns></returns>
    public static float Surface(QuadStruct2D quad_, bool verifRect_ = true) => Surface(quad_.A, quad_.B, quad_.C, quad_.D, verifRect_);

    /// <summary>
    /// Vérifie si ces 4 points correspondent à un rectangle
    /// </summary>
    /// <param name="a_"></param>
    /// <param name="b_"></param>
    /// <param name="c_"></param>
    /// <param name="d_"></param>
    /// <returns></returns>
    public static bool EstRectangle(Vector2 a_, Vector2 b_, Vector2 c_, Vector2 d_)
    {
        Vector2 ab = (b_ - a_).Normalized();
        Vector2 ad = (d_ - a_).Normalized();
        return EstParallélépipède(a_, b_, c_, d_) && (ad == Vecteurs.RotationVecteur90(ab, true) || ad == Vecteurs.RotationVecteur90(ab, false));
    }
    
    /// <summary>
    /// Vérifie si la structure est bien rectangle
    /// </summary>
    /// <param name="quad_"></param>
    /// <returns></returns>
    public static bool EstRectangle(QuadStruct2D quad_) => EstRectangle(quad_.A, quad_.B, quad_.C, quad_.D);

    /// <summary>
    /// Vérifie si les arrêtes de ce rectangle sont parallèles entre elles
    /// </summary>
    /// <param name="a_"></param>
    /// <param name="b_"></param>
    /// <param name="c_"></param>
    /// <param name="d_"></param>
    /// <returns></returns>
    public static bool EstParallélépipède(Vector2 a_, Vector2 b_, Vector2 c_, Vector2 d_)
    { return (b_ - a_) == (c_ - d_) && (d_ - a_) == (c_ - b_); }

    /// <summary>
    /// Vérifie si les arrêtes de ce rectangle sont parallèles entre elles
    /// </summary>
    /// <param name="quad_"></param>
    /// <returns></returns>
    public static bool EstParallélépipède(QuadStruct2D quad_) => EstParallélépipède(quad_.A, quad_.B, quad_.C, quad_.D);

    /// <summary>
    /// Vérifie si le rectangle1 se trouve dans le rectangle2
    /// </summary>
    /// <param name="a_">rectangle1</param>
    /// <param name="b_">rectangle1</param>
    /// <param name="c_">rectangle1</param>
    /// <param name="d_">rectangle1</param>
    /// <param name="e_">rectangle2</param>
    /// <param name="f_">rectangle2</param>
    /// <param name="g_">rectangle2</param>
    /// <param name="h_">rectangle2</param>
    /// <returns></returns>
    public static bool RectangleDansRectangle(Vector2 a_, Vector2 b_, Vector2 c_, Vector2 d_, Vector2 e_, Vector2 f_, Vector2 g_, Vector2 h_)
    {
        if (EstAABB(e_, f_, g_, h_))
        {
            if (EstAABB(a_, b_, c_, d_))
            { return a_.X >= e_.X && c_.X <= g_.X && a_.Y >= e_.Y && c_.Y <= g_.Y; }
            return PointDansAABB(a_, e_, f_, g_, h_) && PointDansAABB(b_, e_, f_, g_, h_) && PointDansAABB(c_, e_, f_, g_, h_) && PointDansAABB(d_, e_, f_, g_, h_);
        }
        return PointDansRectangle(a_, e_, f_, g_, h_) && PointDansRectangle(b_, e_, f_, g_, h_) && PointDansRectangle(c_, e_, f_, g_, h_) && PointDansRectangle(d_, e_, f_, g_, h_);
    }

    /// <summary>
    /// Vérifie si le rectangle1 se trouve dans le rectangle2
    /// </summary>
    /// <param name="rectangle1_"></param>
    /// <param name="rectangle2_"></param>
    /// <returns></returns>
    public static bool RectangleDansRectangle(QuadStruct2D rectangle1_, QuadStruct2D rectangle2_)
    { return PointDansRectangle(rectangle1_.A, rectangle2_) && PointDansRectangle(rectangle1_.B, rectangle2_) && PointDansRectangle(rectangle1_.C, rectangle2_) && PointDansRectangle(rectangle1_.D, rectangle2_); }
    
    public static bool RectangleDansRectangle(Vector3 a_, Vector3 b_, Vector3 c_, Vector3 d_, Vector3 e_, Vector3 f_, Vector3 g_, Vector3 h_)
    {
        // TODO: à faire
        // Vérifier que les deux rectangles soient bien dans le même plan
        // Calculer la matrice d'orientation pour retrouver des coordonnées 2D
        // reproduire le code de la version 2D
        
        throw new NotImplementedException();
    }

    public static bool PointDansRectangle(Vector2 point_, Vector2 a_, Vector2 b_, Vector2 c_, Vector2 d_)
    {
        // On calcule les vecteurs depuis le point A vers les autres points du rectangle
        Vector2 ab = b_ - a_;
        Vector2 ad = d_ - a_;
    
        // On calcule le vecteur depuis le point A vers le point à tester
        Vector2 ap = point_ - a_;
    
        // On utilise le produit scalaire pour projeter le point sur les côtés du rectangle
        float dotAB = ap.Dot(ab) / ab.Dot(ab);
        float dotAD = ap.Dot(ad) / ad.Dot(ad);
    
        // Le point est dans le rectangle si les projections sont entre 0 et 1
        return dotAB >= 0 && dotAB <= 1 && dotAD >= 0 && dotAD <= 1;
    }
    
    public static bool PointDansRectangle(Vector2 point_, QuadStruct2D quad_) => PointDansRectangle(point_, quad_.A, quad_.B, quad_.C, quad_.D);


    /// <summary>
    /// Calcule si un rectangle est une AABB
    /// </summary>
    /// <param name="a_"></param>
    /// <param name="b_"></param>
    /// <param name="c_"></param>
    /// <param name="d_"></param>
    /// <returns></returns>
    public static bool EstAABB(Vector2 a_, Vector2 b_, Vector2 c_, Vector2 d_)
    { return Algèbre.CompFloatEgal(a_.X, b_.X) && Algèbre.CompFloatEgal(a_.Y, d_.Y) && Algèbre.CompFloatEgal(c_.X, d_.X) && Algèbre.CompFloatEgal(c_.Y, b_.Y); }
    /// <summary>
    /// Calcule si un rectangle est une AABB
    /// </summary>
    /// <param name="quad_"></param>
    /// <returns></returns>
    public static bool EstAABB(QuadStruct2D quad_) => EstAABB(quad_.A, quad_.B, quad_.C, quad_.D);

    /// <summary>
    /// Calcule si un point se trouve dans une AABB
    /// </summary>
    /// <param name="point_"></param>
    /// <param name="a_"></param>
    /// <param name="b_"></param>
    /// <param name="c_"></param>
    /// <param name="d_"></param>
    /// <param name="verifAABB">true = vérifier que les points fourni correspondent bien à une AABB</param>
    /// <returns></returns>
    public static bool PointDansAABB(Vector2 point_, Vector2 a_, Vector2 b_, Vector2 c_, Vector2 d_, bool verifAABB = false)
    {
        if (verifAABB && !EstAABB(a_, b_, c_, d_))
        { return false; }
        return point_.X >= a_.X && point_.X <= d_.X && point_.Y >= a_.Y && point_.Y <= c_.Y;
    }

    /// <summary>
    /// Calcule si un point se trouve dans une AABB
    /// </summary>
    /// <param name="point_"></param>
    /// <param name="quad_"></param>
    /// <returns></returns>
    public static bool PointDansAABB(Vector2 point_, QuadStruct2D quad_) => PointDansAABB(point_, quad_.A, quad_.B, quad_.C, quad_.D);

    /// <summary>
    /// Calcule la AABB englobant les 4 points
    /// </summary>
    /// <param name="a_"></param>
    /// <param name="b_"></param>
    /// <param name="c_"></param>
    /// <param name="d_"></param>
    /// <returns></returns>
    public static QuadStruct2D CalcAABBEnglobante(Vector2 a_, Vector2 b_, Vector2 c_, Vector2 d_)
    {
        float xMin = Algèbre.MinF([a_.X, b_.X, c_.X, d_.X]);
        float xMax = Algèbre.MaxF([a_.X, b_.X, c_.X, d_.X]);
        float yMin = Algèbre.MinF([a_.Y, b_.Y, c_.Y, d_.Y]);
        float yMax = Algèbre.MaxF([a_.Y, b_.Y, c_.Y, d_.Y]);
        
        return new QuadStruct2D(new Vector2(xMin, yMin), new Vector2(xMin, yMax), new Vector2(xMax, yMax), new Vector2(xMax, yMin));
    }
    
    /// <summary>
    /// Calcule la AABB englobant les points de la structure
    /// </summary>
    /// <param name="quad_"></param>
    /// <returns></returns>
    public static QuadStruct2D CalcAABBEnglobante(QuadStruct2D quad_) => CalcAABBEnglobante(quad_.A, quad_.B, quad_.C, quad_.D);

    public static QuadStruct2D CalcAABBEnglobante(Vector2[] points_)
    {
        if (points_ == null || points_.Length == 0)
            throw new ArgumentException("Le tableau de points ne peut pas être vide ou null");

        float xMin = points_[0].X, xMax = points_[0].X, yMin = points_[0].Y, yMax = points_[0].Y;

        for (int i = 1; i < points_.Length; i++)
        {
            xMin = Math.Min(xMin, points_[i].X);
            xMax = Math.Max(xMax, points_[i].X);
            yMin = Math.Min(yMin, points_[i].Y);
            yMax = Math.Max(yMax, points_[i].Y);
        }

        return new QuadStruct2D(new Vector2(xMin, yMin), new Vector2(xMin, yMax), new Vector2(xMax, yMax), new Vector2(xMax, yMin));
    }

    /// <summary>
    /// Calcul le centre du rectangle
    /// </summary>
    /// <param name="a_"></param>
    /// <param name="b_"></param>
    /// <param name="c_"></param>
    /// <param name="d_"></param>
    /// <param name="verifRect_">true = vérifier que les 4 points représentent un rectangle</param>
    /// <returns></returns>
    public static Vector2 CalcCentre(Vector2 a_, Vector2 b_, Vector2 c_, Vector2 d_, bool verifRect_ = true)
    {
        if (verifRect_ && !EstRectangle(a_, b_, c_, d_))
        { return Vector2.Inf; }
        if (EstAABB(a_, b_, c_, d_))
        { return new Vector2(a_.X + (d_.X - a_.X) / 2f, a_.Y + (b_.Y - a_.Y) / 2f); }

        return Géom.Utiles.CalcCentre([a_, b_, c_, d_]);
    }
    
    /// <summary>
    /// Calcul le centre du rectangle
    /// </summary>
    /// <param name="quad_"></param>
    /// <returns></returns>
    public static Vector2 CalcCentre(QuadStruct2D quad_) => CalcCentre(quad_.A, quad_.B, quad_.C, quad_.D);

    /// <summary>
    /// Calcule le centre circonscrit au rectangle
    /// </summary>
    /// <param name="a_"></param>
    /// <param name="b_"></param>
    /// <param name="c_"></param>
    /// <param name="d_"></param>
    /// <param name="verifRect_">true = vérifier que les 4 points représentent un rectangle</param>
    /// <returns></returns>
    public static CercleStruct2D CalcCercleCirconscrit(Vector2 a_, Vector2 b_, Vector2 c_, Vector2 d_, bool verifRect_ = true)
    {
        if (verifRect_ && !EstRectangle(a_, b_, c_, d_))
        { return new(Vector2.Inf, 0f); }
        
        Vector2 centre = CalcCentre(a_, b_, c_, d_);
        return new(centre, (centre - a_).Length());
    }
    
    /// <summary>
    /// Calcule le centre circonscrit au rectangle
    /// </summary>
    /// <param name="quad_"></param>
    /// <returns></returns>
    public static CercleStruct2D CalcCercleCirconscrit(QuadStruct2D quad_) => CalcCercleCirconscrit(quad_.A, quad_.B, quad_.C, quad_.D);
    

    #endregion

    #region 3D
    
    
    /// <summary>
    /// Calcule la surface d'un rectangle
    /// </summary>
    /// <param name="a_"></param>
    /// <param name="b_"></param>
    /// <param name="c_"></param>
    /// <param name="d_"></param>
    /// <param name="verifRect_">true = vérifier que les 4 points représentent un rectangle</param>
    /// <returns></returns>
    public static float Surface(Vector3 a_, Vector3 b_, Vector3 c_, Vector3 d_, bool verifRect_ = true)
    {
        if (verifRect_ && !EstRectangle(a_, b_, c_, d_))
        { return -1f; }

        return (b_ - a_).Length() * (d_ - a_).Length();
    }
    
    /// <summary>
    /// Calcule la surface d'un rectangle
    /// </summary>
    /// <param name="quad_"></param>
    /// <param name="verifRect_">true = vérifier que la structure représente un rectangle</param>
    /// <returns></returns>
    public static float Surface(QuadStruct3D quad_, bool verifRect_ = true) => Surface(quad_.A, quad_.B, quad_.C, quad_.D, verifRect_);
    
    /// <summary>
    /// Vérifie si ces 4 points correspondent à un rectangle
    /// </summary>
    /// <param name="a_"></param>
    /// <param name="b_"></param>
    /// <param name="c_"></param>
    /// <param name="d_"></param>
    /// <returns></returns>
    public static bool EstRectangle(Vector3 a_, Vector3 b_, Vector3 c_, Vector3 d_)
    {
        Vector3 ab = (b_ - a_).Normalized();
        Vector3 ad = (d_ - a_).Normalized();
        return EstParallélépipède(a_, b_, c_, d_) && Algèbre.CompFloatEgal(Mathf.RadToDeg(ad.Normalized().AngleTo(ab)), 90f, 0.001f);
    }
    
    /// <summary>
    /// Vérifie si la structure est bien rectangle
    /// </summary>
    /// <param name="quad_"></param>
    /// <returns></returns>
    public static bool EstRectangle(QuadStruct3D quad_) => EstRectangle(quad_.A, quad_.B, quad_.C, quad_.D);
    
    /// <summary>
    /// Vérifie si les arrêtes de ce rectangle sont parallèles entre elles
    /// </summary>
    /// <param name="a_"></param>
    /// <param name="b_"></param>
    /// <param name="c_"></param>
    /// <param name="d_"></param>
    /// <returns></returns>
    public static bool EstParallélépipède(Vector3 a_, Vector3 b_, Vector3 c_, Vector3 d_)
    { return (b_ - a_) == (c_ - d_) && (d_ - a_) == (c_ - b_); }

    /// <summary>
    /// Vérifie si les arrêtes de ce rectangle sont parallèles entre elles
    /// </summary>
    /// <param name="quad_"></param>
    /// <returns></returns>
    public static bool EstParallélépipède(QuadStruct3D quad_) => EstParallélépipède(quad_.A, quad_.B, quad_.C, quad_.D);
    
    /// <summary>
    /// Vérifie si un point se trouve dans un rectangle (donc sur le même plan)
    /// </summary>
    /// <param name="point_"></param>
    /// <param name="a_"></param>
    /// <param name="b_"></param>
    /// <param name="c_"></param>
    /// <param name="d_"></param>
    /// <returns></returns>
    public static bool PointDansRectangle(Vector3 point_, Vector3 a_, Vector3 b_, Vector3 c_, Vector3 d_)
    {
        // Premièrement, on vérifie si le point est dans le même plan que le rectangle
        Vector3 ab = b_ - a_;
        Vector3 ac = c_ - a_;
        Vector3 normal = ab.Cross(ac); // Vecteur normal au plan du rectangle
        Vector3 ap = point_ - a_;
    
        // Vérifier si le point est dans le plan (avec une petite marge d'erreur)
        float distanceAuPlan = normal.Dot(ap);
        if (Mathf.Abs(distanceAuPlan) > 1e-6f)
        { return false; } // Le point n'est pas dans le plan du rectangle

        // Maintenant, on utilise les coordonnées barycentriques pour vérifier si le point est dans le rectangle
        // On projette le point sur les vecteurs du rectangle
        float dotABAB = ab.Dot(ab);
        float dotABAC = ab.Dot(ac);
        float dotACAC = ac.Dot(ac);
        float dotABAP = ab.Dot(ap);
        float dotACAP = ac.Dot(ap);
    
        // Calcul des coordonnées barycentriques
        float denom = dotABAB * dotACAC - dotABAC * dotABAC;
        float u = (dotACAC * dotABAP - dotABAC * dotACAP) / denom;
        float v = (dotABAB * dotACAP - dotABAC * dotABAP) / denom;
    
        // Le point est dans le rectangle si les coordonnées barycentriques sont entre 0 et 1
        return u >= 0 && u <= 1 && v >= 0 && v <= 1;
    }
    
    /// <summary>
    /// Vérifie si un point se trouve dans un rectangle (donc sur le même plan)
    /// </summary>
    /// <param name="point_"></param>
    /// <param name="quad_"></param>
    /// <returns></returns>
    public static bool PointDansRectangle(Vector3 point_, QuadStruct3D quad_) => PointDansRectangle(point_, quad_.A, quad_.B, quad_.C, quad_.D);

    /// <summary>
    /// Vérifie que les 4 points d'un rectangle sont dans le même plan
    /// </summary>
    /// <param name="a_"></param>
    /// <param name="b_"></param>
    /// <param name="c_"></param>
    /// <param name="d_"></param>
    /// <returns></returns>
    public static bool PointsDansPlan(Vector3 a_, Vector3 b_, Vector3 c_, Vector3 d_)
    {
        // Calcule les vecteurs depuis le point a_ vers les autres points
        Vector3 ab = b_ - a_;
        Vector3 ac = c_ - a_;
        Vector3 ad = d_ - a_;

        // Calcule le vecteur normal en utilisant le produit vectoriel
        Vector3 normal = ab.Cross(ac);

        // Si les points sont coplanaires, le produit scalaire du vecteur normal et du vecteur ad doit être zéro
        float dotProduct = normal.Dot(ad);

        // Permet une petite marge d'erreur pour les nombres à virgule flottante
        return Mathf.Abs(dotProduct) < 1e-6f;
    }
    
    /// <summary>
    /// Vérifie que les 4 points d'un rectangle sont dans le même plan
    /// </summary>
    /// <param name="quad_"></param>
    /// <returns></returns>
    public static bool PointsDansPlan(QuadStruct3D quad_) => PointsDansPlan(quad_.A, quad_.B, quad_.C, quad_.D);
    
    public static Vector3[] CalcPoints(Vector3 a_, Vector3 b_, Vector3 c_)
    { return [a_, b_, c_, a_ + (c_ - b_)]; }

    // public static Vector3[] CalcPoints(Vector3 a_, Vector3 diagonale_, float orientation_)
    // {
    //     // TODO : Refaire
    //     Vector3 b_ = a_ + diagonale_ * new Vector3(Mathf.Cos(orientation_), Mathf.Sin(orientation_), 0);
    //     Vector3 c_ = a_ + diagonale_ * new Vector3(Mathf.Cos(orientation_ + Mathf.Pi / 2), Mathf.Sin(orientation_ + Mathf.Pi / 2), 0);
    //     return CalcPoints(a_, b_, c_);
    // }
    
    
    /// <summary>
    /// Calcul le centre du rectangle
    /// </summary>
    /// <param name="a_"></param>
    /// <param name="b_"></param>
    /// <param name="c_"></param>
    /// <param name="d_"></param>
    /// <param name="verifRect_">true = vérifier que les 4 points représentent un rectangle</param>
    /// <returns></returns>
    public static Vector3 CalcCentre(Vector3 a_, Vector3 b_, Vector3 c_, Vector3 d_, bool verifRect_ = true)
    {
        if (verifRect_ && !EstRectangle(a_, b_, c_, d_))
        { return Vector3.Inf; }

        return Géom.Utiles.CalcCentre([a_, b_, c_, d_]);
    }
    
    /// <summary>
    /// Calcul le centre du rectangle
    /// </summary>
    /// <param name="quad_"></param>
    /// <returns></returns>
    public static Vector3 CalcCentre(QuadStruct3D quad_) => CalcCentre(quad_.A, quad_.B, quad_.C, quad_.D);

    #endregion
}

public struct QuadStruct3D(Vector3 a_, Vector3 b_, Vector3 c_, Vector3 d_)
{
    public Vector3 A { get; private set; } = a_;
    public Vector3 B { get; private set; } = b_;
    public Vector3 C { get; private set; } = c_;
    public Vector3 D { get; private set; } = d_;
}

public struct QuadStruct2D(Vector2 a_, Vector2 b_, Vector2 c_, Vector2 d_)
{
    public Vector2 A { get; private set; } = a_;
    public Vector2 B { get; private set; } = b_;
    public Vector2 C { get; private set; } = c_;
    public Vector2 D { get; private set; } = d_;
}

public struct RectangleStruct3DSimplifié
{
    public Vector2 A { get; private set; }
    public Vector2 AC { get; private set; }
}