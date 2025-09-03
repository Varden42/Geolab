using System.Collections.Generic;
using Godot;

namespace VA.Base.Maths.Géom;

public static class Cercle
{
    private const float Tms = 1f;

    /// <summary>
    /// Calcul les points formant l'arc entre deux vecteurs BA et BC. Les points ne doivent pas être alignés
    /// </summary>
    /// <param name="a_">La destination du premier vecteur.</param>
    /// <param name="b_">L'origine des deux vecteurs.</param>
    /// <param name="c_">La destination du deuxième vecteur.</param>
    /// <param name="rayon_">Le rayon de l'arc</param>
    /// <param name="division_">Le nombre de segments composant l'arc.</param>
    /// <param name="tailleMinSegment_">La taille minimale d'un segment.</param>
    /// <returns>La liste des points ainsi calculée.</returns>
    public static List<Vector3> CalcPointsArc(Vector3 a_, Vector3 b_, Vector3 c_, float rayon_, int division_,
        float tailleMinSegment_ = Tms)
    {
        List<Vector3> points = new List<Vector3>();

        //Debug.Log($"Cercle.CalcPoints({a_}, {b_}, {c_}, {rayon_}, {division_}, {sens_}, {tailleMinSegment_}, ");
        Vector3 axe = (a_ - b_).Cross(c_ - b_);
        float angle = 0f;
        Vector3 ba = (a_ - b_).Normalized() * rayon_;

        if (axe.Length() != 0)
        {
            angle = (a_ - b_).SignedAngleTo(c_ - b_, axe);
            angle = angle < 0 ? 360f + angle : angle;
        }

        if (angle > 0f)
        {
            // on calcule la taille du Segment avec la division et le rayon spécifié
            float alpha = angle / division_;
            float ad = Mathf.Abs(Mathf.Sin(alpha / 2f) * rayon_);

            var point = b_ + ba;
            points.Add(point);

            //Debug.Log($"alpha [{alpha}] -- AD [{AD}] -- point [{point}]");

            // si le segment est inférieur à la taille minimale, on recalcule la division.
            if (ad < tailleMinSegment_)
            {
                division_ = (int)(angle / Mathf.RadToDeg(Mathf.Asin(tailleMinSegment_ / 2 / rayon_ * 2f)));
                // Evite une division par 0.
                division_ = division_ < 1 ? 1 : division_;

                alpha = angle / division_;
                //Debug.Log($"Correction de la division [{division_}], alpha [{alpha}]");
            }

            // on calcule la position des points en fonction de l'angle de la division.
            Quaternion quat = new(axe, alpha);
            for (int p = 0; p < division_; ++p)
            {
                points.Add(b_ + (quat * point));
                //Debug.Log($"Ajout du point [{point}]");
            }
        }

        return points;
    }

    /// <summary>
    /// Calcule les points tracant le cercle/arc partant d'une origine et tournant autour d'un centre selon un axe précis.
    /// </summary>
    /// <param name="centre_">Le centre de la rotation</param>
    /// <param name="origine_">L'origine du tracé</param>
    /// <param name="division_">Le nombre de segment devant former le tracé</param>
    /// <param name="angle_">L'angle de la rotation</param>
    /// <param name="axe_">L'axe de la rotation</param>
    /// <param name="sensHoraire_">Le sens de rotation</param>
    /// <param name="miseAPlat_">-1: Mise à plat de l'origine parralèle à l'axe, 0: pas de mise à plat de l'origine, 1: mise à plat par rotation</param>
    /// <returns>La liste des points formant le tracé</returns>
    public static List<Vector3> CalcPointsArc(Vector3 centre_, Vector3 origine_, int division_, float angle_ = 360f,
        Vector3 axe_ = new Vector3(), bool sensHoraire_ = true, int miseAPlat_ = 0)
    {
        List<Vector3> points = new List<Vector3>();
        //division_ = division_ < 3 ? 3 : division_;
        float gamma = sensHoraire_ ? angle_ / division_ : -(angle_ / division_);

        Vector3 co = origine_ - centre_;

        if (miseAPlat_ != 0)
        {
            // calc normale plan
            Vector3 normale = co.Normalized().Cross(axe_.Normalized()).Normalized();
            // calc angle ACO
            float alpha = axe_.Normalized().AngleTo(co.Normalized());
            // calculer l'angle OCR
            float beta = alpha - ((int)(alpha / 90f) * 90f);
            // tourner O autour de la normale du plan de l'équivalent de l'angle OCR. en sens antihoraire à gauche et horaire à droite
            co = new Quaternion(normale, beta) * co;

            if (miseAPlat_ < 0)
            {
                float cr = Mathf.Cos(beta) * co.Length();
                co = co.Normalized() * cr;
            }
        }

        // on calcule la position des points en fonction de l'angle de la division.
        Quaternion rot = new(axe_, gamma);
        for (int p = 0; p < division_; ++p)
        {
            points.Add(centre_ + co);
            co = rot * co;
        }

        return points;
    }

    /// <summary>
    /// Calcule les points tracant le cercle/arc de cercle partant d'une origine et tournant autour d'un centre selon un axe précis, en posant l'origine sur le plan perpendiculaire à l'axe.
    /// </summary>
    /// <param name="centre_">Le centre de la rotation</param>
    /// <param name="origine_">L'origine du tracé</param>
    /// <param name="division_">Le nombre de segment devant former le tracé</param>
    /// <param name="rayon_">Le rayon du cercle/arc de cercle à tracer</param>
    /// <param name="angle_">L'angle de la rotation</param>
    /// <param name="axe_">L'axe de la rotation</param>
    /// <param name="sensHoraire_">Le sens de rotation</param>
    /// <param name="miseAPlat_">Précise si la mise à plat est nécessaire</param>
    /// <returns>La liste des points formant le tracé</returns>
    public static List<Vector3> CalcPoints(Vector3 centre_, Vector3 origine_, int division_, float rayon_,
        Vector3 axe_ = new Vector3(), float angle_ = 360f, bool sensHoraire_ = true, bool miseAPlat_ = false)
    {
        List<Vector3> points = new List<Vector3>();
        division_ = division_ < 3 ? 3 : division_;
        float gamma = sensHoraire_ ? angle_ / division_ : -(angle_ / division_);

        Vector3 co = origine_ - centre_;

        if (miseAPlat_)
        {
            // calc normale plan
            Vector3 normale = co.Normalized().Cross(axe_.Normalized()).Normalized();
            // calc angle ACO
            float alpha = axe_.Normalized().AngleTo(co.Normalized());
            // calculer l'angle OCR
            float beta = alpha - ((int)(alpha / 90f) * 90f);
            // tourner O autour de la normale du plan de l'équivalent de l'angle OCR. en sens antihoraire à gauche et horaire à droite
            co = new Quaternion(normale, beta) * co;
        }

        co = co.Normalized() * rayon_;

        // on calcule la position des points en fonction de l'angle de la division.
        Quaternion rot = new(axe_, gamma);
        for (int p = 0; p < division_; ++p)
        {
            points.Add(centre_ + co);
            co = rot * co;
        }

        return points;
    }

    /// <summary>
    /// Trace un Cercle orienté selon un axe précis
    /// </summary>
    /// <param name="centre_">Le centre de la rotation</param>
    /// <param name="rayon_">Le rayon du cercle à tracer</param>
    /// <param name="division_">Le nombre de segment devant former le tracé</param>
    /// <param name="axe_">La normale du cercle</param>
    /// <param name="sensHoraire_"></param>
    /// <returns></returns>
    public static List<Vector3> CalcPoints(Vector3 centre_, float rayon_, int division_, Vector3 axe_,
        bool sensHoraire_ = true)
    {
        List<Vector3> points = new List<Vector3>();
        division_ = division_ < 3 ? 3 : division_;
        float angle = sensHoraire_ ? 360f / division_ : -(360f / division_);

        Vector3 centreVersPoint = (Vector3.Forward * rayon_) * new Quaternion(Vector3.Up, axe_.Normalized());

        // on calcule la position des points en fonction de l'angle de la division.
        Quaternion rot = new(axe_, angle);
        for (int p = 0; p < division_; ++p)
        {
            points.Add(centre_ + centreVersPoint);
            centreVersPoint = rot * centreVersPoint;
        }

        return points;
    }

    /// <summary>
    /// Calcul les points formant un Cercle de Centre, Rayon et Division précis.
    /// </summary>
    /// <param name="centre_">Le Centre du Cercle.</param>
    /// <param name="rayon_">Le rayon du Cercle.</param>
    /// <param name="division_">Le nombre de segments composant le Cercle.</param>
    /// <param name="sensHoraire_">faut-il tourner en sens horaire ou non</param>
    /// <returns>La liste des points ainsi calculée.</returns>
    public static List<Vector3> CalcPoints(Vector3 centre_, float rayon_, int division_, bool sensHoraire_ = true)
    {
        List<Vector3> points = new List<Vector3>();

        // on calcule la taille du Segment avec la division et le rayon spécifié
        division_ = division_ < 3 ? 3 : division_;
        float alpha = sensHoraire_ ? 360f / division_ : -(360f / division_);

        Vector3 point = new Vector3(0f, rayon_, 0f);

        // on calcule la position des points en fonction de l'angle de la division.
        Quaternion quat = Quaternion.FromEuler(new(0f, alpha, 0f));
        for (int p = 0; p < division_ - 1; ++p)
        {
            points.Add(centre_ + point);
            point = quat * point;
        }

        return points;
    }
    
    /// <summary>
    /// Calcul les points formant un Cercle de Centre, Rayon et Division précis.
    /// </summary>
    /// <param name="cercle_">La structure contenant les infos sur le cercle</param>
    /// <param name="sensHoraire_">faut-il tourner en sens horaire ou non</param>
    /// <returns></returns>
    public static List<Vector3> CalcPoints(CercleStruct3D cercle_, int division_, bool sensHoraire_ = true)
        => CalcPoints(cercle_.Centre, cercle_.Rayon, division_, sensHoraire_);

    /// <summary>
    /// Calcul les points formant un Cercle de Centre, Rayon, Division et taille de segment minimum précis.
    /// </summary>
    /// <param name="centre_">Le Centre du Cercle.</param>
    /// <param name="rayon_">Le rayon du Cercle.</param>
    /// <param name="division_">Le nombre de segments composant le Cercle.</param>
    /// <param name="tailleMinSegment_">La taille minimale d'un segment.</param>
    /// <param name="sensHoraire_"></param>
    /// <returns>La liste des points ainsi calculée.</returns>
    public static List<Vector3> CalcPoints(Vector3 centre_, float rayon_, int division_, float tailleMinSegment_, bool sensHoraire_ = true)
    {
        List<Vector3> points = new List<Vector3>();

        // on calcule la taille du Segment avec la division et le rayon spécifié
        //division_ = division_ < 3 ? 3 : division_;
        float alpha = sensHoraire_ ? 360f / division_ : -(360f / division_);
        float ad = Mathf.Abs(Mathf.Sin(alpha / 2f) * rayon_);

        Vector3 point = new Vector3(0f, rayon_, 0f);

        // si le segment est inférieur à la taille minimale, on recalcule la division.
        if (ad < tailleMinSegment_)
        {
            division_ = (int)(360f / Mathf.RadToDeg(Mathf.Asin(tailleMinSegment_ / 2 / rayon_ * 2f)));

            alpha = sensHoraire_ ? 360f / division_ : -(360f / division_);
        }

        // on calcule la position des points en fonction de l'angle de la division.
        Quaternion quat = Quaternion.FromEuler(new(0f, alpha, 0f));
        for (int p = 0; p < division_ - 1; ++p)
        {
            points.Add(centre_ + point);
            point = quat * point;
        }

        return points;
    }
    
    public static List<Vector3> CalcPoints(CercleStruct3D cercle_, int division_, float tailleSegments_ = Tms, bool sensHoraire_ = true)
        => CalcPoints(cercle_.Centre, cercle_.Rayon, division_, tailleSegments_, sensHoraire_);

    /// <summary>
    /// Calcul les points formant un Cercle de Centre, Division et taille de segment minimum précis.
    /// </summary>
    /// <param name="centre_">Le Centre du Cercle.</param>
    /// <param name="division_">Le nombre de segments composant le Cercle.</param>
    /// <param name="tailleSegments_">La taille des segments dessinant le Cercle.</param>
    /// <param name="sensHoraire_"></param>
    /// <returns>La liste des points ainsi calculée.</returns>
    public static List<Vector3> CalcPoints(Vector3 centre_, int division_, float tailleSegments_ = Tms, bool sensHoraire_ = true)
    {
        List<Vector3> points = new List<Vector3>();

        //division_ = division_ < 3 ? 3 : division_;
        float alpha = sensHoraire_ ? 360f / division_ : -(360f / division_);
        float rayon = (tailleSegments_ / 2) / (Mathf.Sin(alpha / 2));

        Vector3 point = new Vector3(0f, rayon, 0f);

        // on calcule la position des points en fonction de l'angle de la division.
        Quaternion quat = Quaternion.FromEuler(new(0f, alpha, 0f));
        for (int p = 0; p < division_ - 1; ++p)
        {
            points.Add(centre_ + point);
            point = quat * point;
        }

        return points;
    }
}

// TODO: Calculer la normale du cercle

public struct CercleStruct3D(Vector3 centre_, float rayon_)
{
    public Vector3 Centre { get; private set; } = centre_;
    public float Rayon { get; private set; } = rayon_;
}

public struct CercleStruct2D(Vector2 centre_, float rayon_)
{
    public Vector2 Centre { get; private set; } = centre_;
    public float Rayon { get; private set; } = rayon_;
}