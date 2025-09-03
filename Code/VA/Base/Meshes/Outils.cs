using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using VA.Base.Maths;
using GodotArray = Godot.Collections.Array;
//using Triangle = VA.Base.Meshes.Outils_.Triangle;

namespace VA.Base.Meshes;

public static class Outils
{
    //private const float ToléranceNormales = 0.0001f;
    //public static readonly float ToléranceNormalesOutils = ToléranceNormales;
    public class Triangle: IEquatable<Triangle>
    {
        public Vector3[] Sommets { get; private set; }
        public Vector3 A => Sommets[0];
        public Vector3 B => Sommets[1];
        public Vector3 C => Sommets[2];
        public Vector3 Normale { get; private set; }
        public Arrête[] Arrêtes { get; private set; }

        public bool Entouré => Arrêtes[0].Partagée && Arrêtes[1].Partagée && Arrêtes[2].Partagée;

        public Arrête AB => Arrêtes[0];
        public Arrête BC => Arrêtes[1];
        public Arrête CA => Arrêtes[2];
        public Face Face { get; private set; }


        //public Triangle(Vector3 a_, Vector3 b_, Vector3 c_, Face face_) : this(a_, b_, c_) { Face = face_; }
        //public Triangle(Arrête ab_, Arrête bc_, Arrête ca_, Face face_) : this(ab_, bc_, ca_) { Face = face_; }
        public Triangle(Vector3 a_, Vector3 b_, Vector3 c_, Face face_ = null)
        {
            Sommets = new[] { a_, b_, c_ };
            Normale = Maths.Géom.Triangle.Normale(A, B, C);
            // on crée de nouvelles Arrêtes et les doublons seront supprimés lorsque deux triangles compareront leur arrêtes identiques
            Arrêtes = new Arrête[] { new(this, 0), new(this, 1), new(this, 2) };

            if (face_ == null)
            { Face = new(this, Normale); }
            else
            {
                Face = face_;
                Face.AjoutTriangle(this);
            }
            //Face = face_ ?? new(this, Normale);
        }
        
        public Triangle(Arrête ab_, Arrête bc_, Arrête ca_, Face face_ = null)
        {
            if (ab_.A == ca_.B && ab_.B == bc_.A && bc_.B == ca_.A && 
                ab_.A != Vector3.Inf && bc_.A != Vector3.Inf && ca_.A != Vector3.Inf)
            {
                Sommets = new[] { ab_.A, bc_.A, ca_.A };
                Normale = Maths.Géom.Triangle.Normale(A, B, C);
                if (!ab_.AjoutTriangle(this, 0) || !bc_.AjoutTriangle(this, 1) || !ca_.AjoutTriangle(this, 2))
                { throw new ArgumentException($"Erreur, le triangle ne peut pas être ajouté aux Arrêtes [{ab_.A} - {ab_.B}] ; [{bc_.A} - {bc_.B}] ; [{ca_.A} - {ca_.B}]"); }
                Arrêtes = new Arrête[] { ab_, bc_, ca_ };
                
                if (face_ == null)
                { Face = new(this, Normale); }
                else
                {
                    Face = face_;
                    Face.AjoutTriangle(this);
                }
                //Face = face_ ?? new(this, Normale);
            }
            else
            { throw new ArgumentException($"Erreur lors de la création du triangle avec les Arrêtes [{ab_.A} - {ab_.B}] ; [{bc_.A} - {bc_.B}] ; [{ca_.A} - {ca_.B}]"); }
        }

        public override bool Equals(object obj)
        { return obj != null && Equals(obj as Triangle); }

        public bool Equals(Triangle triangle_)
        { return A == triangle_?.A && B == triangle_.B && C == triangle_.C && 
                 AB == triangle_.AB && BC == triangle_.BC && CA == triangle_.CA; }

        public override int GetHashCode()
        { return (A.GetHashCode() * 17 + B.GetHashCode()) * 17 + C.GetHashCode(); }

        public static bool operator ==(Triangle triangle1_, Triangle triangle2_)
        { return triangle1_?.A == triangle2_?.A && triangle1_?.B == triangle2_?.B && triangle1_?.C == triangle2_?.C; }
        
        public static bool operator !=(Triangle triangle1_, Triangle triangle2_)
        { return triangle1_?.A != triangle2_?.A && triangle1_?.B != triangle2_?.B && triangle1_?.C != triangle2_?.C; }

        /// <summary>
        /// Change la Face du Triangle et retire celui-ci de son ancienne Face
        /// </summary>
        /// <param name="face_"></param>
        public void ChangerFace(Face face_)
        {
            if (!ReferenceEquals(face_, null) && !ReferenceEquals(Face, face_))
            {
                if (!ReferenceEquals(Face, null))
                { Face.RetraitTriangle(this); }
                Face = face_;
                Face.AjoutTriangle(this, true);
            }
        }

        /// <summary>
        /// Teste si le triangle a une arrête en commun et la combine si c'est le cas
        /// L'ajoute à la Face si la normale est identique
        /// </summary>
        public bool Adjacent(Triangle triangle_)
        {
            if (!Entouré && !triangle_.Entouré)
            {
                foreach (Arrête arrêteT1 in Arrêtes)
                {
                    for (int s2 = 0; s2 < 3; ++s2)
                    {
                        if (!arrêteT1.Partagée && !triangle_.Arrêtes[s2].Partagée)
                        {
                            if (arrêteT1.AjoutTriangle(triangle_, s2))
                            { return true; }
                        }
                    }
                }
            }
            
            return false;
        }


        public bool EstCoplanaire(Triangle triangle_)
        { return Mathf.Abs(Normale.Dot(triangle_.Normale) - 1) < Face.Tolérance; }


        public bool MêmeFace(Triangle triangle_)
        { return Face.AjoutTriangle(triangle_); }

        /// <summary>
        /// Calcule l'index de l'arrête correspondant aux deux indexs de sommets
        /// </summary>
        public static int IndexArrête(int indexSommet1_, int indexSommet2_)
        {
            if (indexSommet1_ != indexSommet2_ && indexSommet1_ is >= 0 and < 3 && indexSommet2_ is >= 0 and < 3)
            {
                int[] tableConversion = new[] { 0, 2, 1 };
                return tableConversion[indexSommet1_ + indexSommet2_ - 1];
            }
            else
            { throw new ArgumentException($"les deux indexs [{indexSommet1_}][{indexSommet2_}] correspondent au même sommet ou sont invalides"); }
        }

        /// <summary>
        /// convertit un index d'arrête en une paire d'index de sommets
        /// </summary>
        public static (int sommet1, int sommet2) IndexsSommets(int indexArrête_)
        {
            if (indexArrête_ is >= 0 and < 3)
            {
                switch (indexArrête_)
                {
                    case 0:
                        return (0, 1);
                    case 1:
                        return (1, 2);
                    case 2:
                        return (2, 0);
                }
            }
            
            throw new ArgumentException($"l'index [{indexArrête_}] de l'arrête est invalide");
            
        }
    }

    public class Arrête: IEquatable<Arrête>
    {
        private int[] SommetA, SommetB;
        public Vector3 A => SommetA[0] >= 0 ? Triangles[0].Sommets[SommetA[0]] : SommetA[1] >= 0 ? Triangles[1].Sommets[SommetA[1]] : Vector3.Inf;
        public Vector3 B => SommetB[0] >= 0 ? Triangles[0].Sommets[SommetB[0]] : SommetB[1] >= 0 ? Triangles[1].Sommets[SommetB[1]] : Vector3.Inf;
        public Triangle[] Triangles;
        public bool Partagée => !ReferenceEquals(Triangles[0], null) && !ReferenceEquals(Triangles[1], null);
        public bool BordureFace => !Partagée || Triangles[0]?.Face != Triangles[1]?.Face;

        public Arrête(Triangle triangle_, int indexArrête_)
        {
            Triangles = new Triangle[] { null, null };
            SommetA = new []{-1, -1};
            SommetB = new []{-1, -1};
            
            if (!AjoutTriangle(triangle_, indexArrête_))
            { throw new ArgumentException($"Erreur lors de la création de l'arrête avec le triangle [{triangle_}] et l'index d'arrête [{indexArrête_}]"); }
        }
        
        public Arrête(in Triangle triangle1_, in Triangle triangle2_, int indexArrête1_, int indexArrête2_)
        {
            if (!AjoutTriangle(in triangle1_, indexArrête1_) && !AjoutTriangle(in triangle2_, indexArrête2_))
            { throw new ArgumentException($"Erreur lors de la création de l'arrête avec les triangle [{triangle1_}] et [{triangle1_}] associés aux indexs d'arrêtes [{indexArrête1_}] et [{indexArrête2_}]"); }
        }

        public override bool Equals(object obj)
        { return obj != null && Equals(obj as Arrête); }

        public bool Equals(Arrête arrête_)
        { return Triangles[0] == arrête_.Triangles[0] && Triangles[1] == arrête_.Triangles[1] && SommetA == arrête_.SommetA && SommetB == arrête_.SommetB; }

        public override int GetHashCode()
        {
            Vector3 a = A, b = B;
            if (A == Vector3.Inf && B == Vector3.Inf)
            { return 0; }
            
            return A.GetHashCode() * 17 + B.GetHashCode();
        }

        public static bool operator ==(Arrête arrête1_, Arrête arrête2_)
        { return !ReferenceEquals(arrête1_, null) && !ReferenceEquals(arrête2_, null) && Algèbre.CompVec3DFEgal(arrête1_.A, arrête2_.A) && Algèbre.CompVec3DFEgal(arrête1_.B, arrête2_.B); }
        
        public static bool operator !=(Arrête arrête1_, Arrête arrête2_)
        { return !ReferenceEquals(arrête1_, null) && !ReferenceEquals(arrête2_, null) && !Algèbre.CompVec3DFEgal(arrête1_.A, arrête2_.A) && !Algèbre.CompVec3DFEgal(arrête1_.B, arrête2_.B); }

        /// <summary>
        /// Ajoute un triangle à l'arrête s'il y a 1 triangles déjà lié à cette arrête et si le triangle partage bien des sommets avec l'arrête
        /// </summary>
        public bool AjoutTriangle(in Triangle triangle_)
        {
            bool réussite = false;
            // on calcule les index des triangles dans l'arrête
            if (!ReferenceEquals(triangle_, null))
            {
                int indexTriangleExistant, indexTriangleAjouté;
                if (ReferenceEquals(Triangles[0], null))
                {
                    indexTriangleAjouté = 0;
                    if (ReferenceEquals(Triangles[1], null))
                    { indexTriangleExistant = -1; }
                    else
                    { indexTriangleExistant = 1; }
                }
                else
                {
                    if (ReferenceEquals(Triangles[1], null))
                    {
                        indexTriangleExistant = 0;
                        indexTriangleAjouté = 1;
                    }
                    else
                    {
                        indexTriangleExistant = 2;
                        indexTriangleAjouté = -1;
                    }
                }

                // on gère les situations où les indexs ne sont pas valides pour un ajout de Triangle
                if (indexTriangleExistant < 0)
                { throw new Exception("L'arrête n'est liée à aucun triangles !!"); }
                else if (indexTriangleExistant > 1) // l'arrête est déjà pleine
                { return false; }

                // on check la correspondance des sommets
                for (int a = 0; a < 3; ++a)
                {
                    var indexsSommets = Triangle.IndexsSommets(a);

                    if (Algèbre.CompVec3DFEgal(triangle_.Sommets[indexsSommets.sommet1], Triangles[indexTriangleExistant].Sommets[SommetA[indexTriangleExistant]]) &&
                        Algèbre.CompVec3DFEgal(triangle_.Sommets[indexsSommets.sommet2], Triangles[indexTriangleExistant].Sommets[SommetB[indexTriangleExistant]]))
                    {
                        SommetA[indexTriangleAjouté] = indexsSommets.sommet1;
                        SommetB[indexTriangleAjouté] = indexsSommets.sommet2;
                        Triangles[indexTriangleAjouté] = triangle_;
                        réussite = true;
                    }
                    else if (Algèbre.CompVec3DFEgal(triangle_.Sommets[indexsSommets.sommet1], Triangles[indexTriangleExistant].Sommets[SommetB[indexTriangleExistant]]) &&
                             Algèbre.CompVec3DFEgal(triangle_.Sommets[indexsSommets.sommet2], Triangles[indexTriangleExistant].Sommets[SommetA[indexTriangleExistant]]))
                    {
                        SommetB[indexTriangleAjouté] = indexsSommets.sommet1;
                        SommetA[indexTriangleAjouté] = indexsSommets.sommet2;
                        Triangles[indexTriangleAjouté] = triangle_;
                        réussite = true;
                    }
                    
                    // Met à jour l'arrête dans le Triangle
                    if (réussite)
                    {
                        triangle_.Arrêtes[a].RetraitTriangle(triangle_);
                        triangle_.Arrêtes[a] = this;
                        break;
                    }
                }
            }

            return réussite;
        }

        /// <summary>
        /// Tente de lier un Triangle par une arrête précise
        /// </summary>
        /// <param name="triangle_"></param>
        /// <param name="indexArrête_">L'index de l'arrête dans le triangle</param>
        /// <returns>La réussite ou non de l'ajout</returns>
        public bool AjoutTriangle(in Triangle triangle_, int indexArrête_)
        {
            bool réussite = false;
            if (!ReferenceEquals(triangle_, null))
            {
                int indexTriangleExistant, indexTriangleAjouté;
                if (ReferenceEquals(Triangles[0], null))
                {
                    indexTriangleAjouté = 0;
                    if (ReferenceEquals(Triangles[1], null))
                    { indexTriangleExistant = -1; }
                    else
                    { indexTriangleExistant = 1; }
                }
                else
                {
                    if (ReferenceEquals(Triangles[1], null))
                    {
                        indexTriangleExistant = 0;
                        indexTriangleAjouté = 1;
                    }
                    else
                    {
                        indexTriangleExistant = 2;
                        indexTriangleAjouté = -1;
                    }
                }

                var indexsSommets = Triangle.IndexsSommets(indexArrête_);
                // on gère les cas particuliers
                if (indexTriangleExistant < 0)
                {
                    SommetA[indexTriangleAjouté] = indexsSommets.sommet1;
                    SommetB[indexTriangleAjouté] = indexsSommets.sommet2;
                    Triangles[indexTriangleAjouté] = triangle_;
                    return true;
                }
                else if (indexTriangleExistant > 1) // l'arrête est déjà pleine
                { return false; }

                // on check la correspondance des sommets
                if (Algèbre.CompVec3DFEgal(triangle_.Sommets[indexsSommets.sommet1], Triangles[indexTriangleExistant].Sommets[SommetA[indexTriangleExistant]]) &&
                    Algèbre.CompVec3DFEgal(triangle_.Sommets[indexsSommets.sommet2], Triangles[indexTriangleExistant].Sommets[SommetB[indexTriangleExistant]]))
                {
                    SommetA[indexTriangleAjouté] = indexsSommets.sommet1;
                    SommetB[indexTriangleAjouté] = indexsSommets.sommet2;
                    Triangles[indexTriangleAjouté] = triangle_;
                    réussite = true;
                }
                else if (Algèbre.CompVec3DFEgal(triangle_.Sommets[indexsSommets.sommet1], Triangles[indexTriangleExistant].Sommets[SommetB[indexTriangleExistant]]) &&
                         Algèbre.CompVec3DFEgal(triangle_.Sommets[indexsSommets.sommet2], Triangles[indexTriangleExistant].Sommets[SommetA[indexTriangleExistant]]))
                {
                    SommetB[indexTriangleAjouté] = indexsSommets.sommet1;
                    SommetA[indexTriangleAjouté] = indexsSommets.sommet2;
                    Triangles[indexTriangleAjouté] = triangle_;
                    réussite = true;
                }
                    
                // Met à jour l'arrête dans le Triangle
                if (réussite)
                {
                    triangle_.Arrêtes[indexArrête_].RetraitTriangle(triangle_);
                    triangle_.Arrêtes[indexArrête_] = this;
                }
            }

            return réussite;
        }


        public bool RetraitTriangle(Triangle triangle_)
        {
            if (triangle_ == Triangles[0])
            {
                Triangles[0] = null;
                SommetA[0] = -1;
                SommetB[0] = -1;
            }
            else if (triangle_ == Triangles[1])
            {
                Triangles[1] = null;
                SommetA[1] = -1;
                SommetB[1] = -1;
            }
            else
            { return false; }

            return true;
        }
    }


    public class Face
    {
        private List<Triangle> Triangles;
        private List<Arrête> Bordure;
        public Vector3 Normale { get; private set; }
        public readonly float Tolérance;

        /// <summary>
        /// Construit une Face vide
        /// </summary>
        /// <param name="tolérance">La tolérance maximale entre la normale de la Face et celle d'un triangle la composant</param>
        /// <param name="normale_">La normale de la Face</param>
        public Face(float tolérance = Communs.ToléranceNormales, Vector3 normale_ = default): this(normale_, tolérance) {    }

        /// <summary>
        /// Construit une Face vide
        /// </summary>
        /// <param name="normale_">La normale de la Face</param>
        /// <param name="tolérance">La tolérance maximale entre la normale de la Face et celle d'un triangle la composant</param>
        public Face(Vector3 normale_ = default, float tolérance = Communs.ToléranceNormales)
        {
            Triangles = new();
            Bordure = new();
            Normale = normale_;
            Tolérance = tolérance;
        }

        public Face(Triangle triangle_, Vector3 normale_ = default, float tolérance = Communs.ToléranceNormales) : this(new List<Triangle>(1){ triangle_ }, normale_, tolérance) {    }
        public Face(Triangle[] triangles_, Vector3 normale_ = default, float tolérance = Communs.ToléranceNormales) : this(new List<Triangle>(triangles_), normale_, tolérance) {    }

        /// <summary>
        /// Construit une Face à partir d'une liste de triangle 
        /// </summary>
        /// <param name="triangles_">La liste de triangles à rajouter</param>
        /// <param name="normale_">La normale de la Face. Si non spécifié, la normale du premier Triangle sera prise</param>
        /// <param name="tolérance_">La tolérance maximale entre la normale de la Face et celle d'un triangle la composant</param>
        public Face(List<Triangle> triangles_, Vector3 normale_ = default, float tolérance_ = Communs.ToléranceNormales)
        {
            Triangles = new();
            Bordure = new();
            Normale = normale_;
            Tolérance = tolérance_;

            AjoutTriangles(triangles_);
        }

        /// <summary>
        /// Ajoute un Triangle à la Face si sa normale correspond et s'il est adjacent avec un Triangle existant
        /// </summary>
        /// <param name="triangle_"></param>
        /// <returns></returns>
        public bool AjoutTriangle(Triangle triangle_, bool forcer_ = false)
        {
            bool réussite = false;
            if (!forcer_)
            {
                if (Normale == default || Mathf.Abs(Normale.Dot(triangle_.Normale) - 1) < Tolérance)
                {
                    if (Triangles.Count > 0)
                    {
                        int compteur = 0;
                        foreach (Triangle triangleFace in Triangles)
                        {
                            if (triangleFace.Adjacent(triangle_))
                            {
                                réussite = true;
                                ++compteur;
                            }

                            if (compteur >= 3)
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        if (Normale == default)
                        {
                            Normale = triangle_.Normale;
                        }

                        réussite = true;
                    }

                    if (réussite)
                    {
                        triangle_.ChangerFace(this);
                        Triangles.Add(triangle_);
                        //MajBordure(triangle_, true);
                    }
                }
            }
            else
            {
                Triangles.Add(triangle_);
                //MajBordure(triangle_, true);
                return true;
            }
            
            return réussite;
        }
        
        // public void AjoutTriangles(Triangle[] triangles_)
        // {
        //     foreach (Triangle triangle in triangles_)
        //     { AjoutTriangle(triangle); }
        // }
        // public void AjoutTriangles(List<Triangle> triangles_)
        // {
        //     foreach (Triangle triangle in triangles_)
        //     { AjoutTriangle(triangle); }
        // }
        public void AjoutTriangles(IEnumerable<Triangle> triangles_)
        {
            foreach (Triangle triangle in triangles_)
            { AjoutTriangle(triangle); }
        }


        public void RetraitTriangle(Triangle triangle_)
        {
            if (Triangles.Contains(triangle_))
            {
                //MajBordure(triangle_, false);
                Triangles.Remove(triangle_);
            }
        }

        /// <summary>
        /// Calcule une Normale en faisant une moyenne de la normale réelle de chaque Triangles
        /// </summary>
        private Vector3 MoyenneNormalesTriangles()
        {
            Vector3 temp = Vector3.Zero;
            foreach (Triangle triangle in Triangles)
            { temp += triangle.Normale; }
            return (temp / Triangles.Count).Normalized();
        }

        /// <summary>
        /// Met à jour la Bordure de la Face en retirant ou enlevant les arrêtes correspondantes au triangle spécifié
        /// </summary>
        /// <param name="action_">true = ajout du triangle - false = retrait du triangle</param>
        private bool MajBordure(Triangle triangle_, bool action_)
        {
            // TODO: Metre à jour uniquement les arrêtes de la bordure qui sont ajoutés ou retirés
            throw new NotImplementedException();
        }

        public List<Arrête> RecalculerBordure()
        {
            Bordure = new();
            foreach (Triangle triangle in Triangles)
            {
                foreach (Arrête nouvelleArrête in triangle.Arrêtes)
                {
                    if (nouvelleArrête.BordureFace)
                    {
                        bool arrêteAjoutée = false;
                        // essayer de trouver une arrête contigu avec la nouvelle arrête dans la liste
                        Arrête maillonGauche = null, maillonDroit = null;
                        if (Bordure.Count <= 0)
                        { Bordure.Add(nouvelleArrête); }
                        else
                        {
                            // on parcours la bordure à la recherche d'une arrête qui touche la nouvelle arrête
                            for (int a = 0; a < Bordure.Count; ++a)
                            {
                                // si on trouve le maillon droit
                                if (Algèbre.CompVec3DFEgal(Bordure[a].A, nouvelleArrête.B))
                                {
                                    maillonDroit = Bordure[a];
                                    Bordure.Insert(a, nouvelleArrête);
                                    arrêteAjoutée = true;

                                    // on cherche le maillon gauche
                                    for (int b = 0; b < Bordure.Count; ++b)
                                    {
                                        if (Bordure[b] != maillonDroit && Bordure[b] != nouvelleArrête && Algèbre.CompVec3DFEgal(Bordure[a].B, nouvelleArrête.A))
                                        {
                                            // récupérer toutes les arrêtes qui soient liés sur la gauche
                                            int indexDébutPlage = b;
                                            while (indexDébutPlage > 0 && Bordure[indexDébutPlage].A == Bordure[indexDébutPlage - 1].B)
                                            { --indexDébutPlage; }

                                            int longueurPlage = b - indexDébutPlage + 1;
                                            Utiles.Tableaux.DéplacerPlageObjets<Arrête>(Bordure, indexDébutPlage, longueurPlage, a - longueurPlage);
                                            break;
                                        }
                                    }
                                    break;
                                }
                                // si on trouve le maillon gauche
                                else if (Algèbre.CompVec3DFEgal(Bordure[a].B, nouvelleArrête.A))
                                {
                                    maillonGauche = Bordure[a];
                                    Bordure.Insert(a + 1, nouvelleArrête);
                                    arrêteAjoutée = true;

                                    // on cherche le maillon droit
                                    for (int b = 0; b < Bordure.Count; ++b)
                                    {
                                        if (Bordure[b] != maillonGauche && Bordure[b] != nouvelleArrête && Algèbre.CompVec3DFEgal(Bordure[a].A, nouvelleArrête.B))
                                        {
                                            // récupérer toutes les arrêtes qui soient liés sur la droite
                                            int indexFinPlage = b;
                                            while (indexFinPlage < Bordure.Count - 1 && Bordure[indexFinPlage].B == Bordure[indexFinPlage + 1].A)
                                            { ++indexFinPlage; }

                                            Utiles.Tableaux.DéplacerPlageObjets<Arrête>(Bordure, b, indexFinPlage - b - 1, a + 2);
                                            break;
                                        }
                                    }
                                    break;
                                }

                                
                            }
                            if (!arrêteAjoutée)
                            { Bordure.Add(nouvelleArrête); }
                        }
                    }
                }
            }

            return Bordure;
        }
    }

    public class ComparateurNormales : IEqualityComparer<Vector3>
    {
        public bool Equals(Vector3 normale1_, Vector3 normale2_)
        { return Mathf.Abs(normale1_.Dot(normale2_) - 1) < Communs.ToléranceNormales; }

        public int GetHashCode(Vector3 normale_)
        { return normale_.GetHashCode(); }
    }

    public static List<List<Face>> CalculerFaces(Mesh meshe_, float tolérance_ = 0.0001f)
    {
        List<List<Face>> faces = new();
        if (!ReferenceEquals(meshe_, null) && meshe_.GetSurfaceCount() > 0)
        {
            // Parcourir les surfaces de la meshe
            for (int surfaceIndex = 0; surfaceIndex < meshe_.GetSurfaceCount(); ++surfaceIndex)
            {
                List<Face> facesSurface = new();
                faces.Add(facesSurface);

                // Récupère les triangles et vertexs
                GodotArray surfaceMeshe = meshe_.SurfaceGetArrays(surfaceIndex);
                List<int> tris = new(surfaceMeshe[(int)Mesh.ArrayType.Index].AsInt32Array());
                Vector3[] mesheVertexs = surfaceMeshe[(int)Mesh.ArrayType.Vertex].AsVector3Array();

                Dictionary<Vector3, List<Triangle>> trianglesSurface = new();
                
                // si les triangles sont définis
                if (tris.Count % 3 == 0)
                {
                    // créer la liste de triangles
                    for (int t = 0; t < tris.Count;)
                    {
                        Triangle temp = new(mesheVertexs[tris[t++]], mesheVertexs[tris[t++]], mesheVertexs[tris[t++]], new Face(tolérance_));
                        bool groupeExistant = false;
                        foreach (KeyValuePair<Vector3,List<Triangle>> groupe in trianglesSurface)
                        {
                            if (Mathf.Abs(groupe.Key.Dot(temp.Normale) - 1) < Communs.ToléranceNormales)
                            {
                                groupe.Value.Add(temp);
                                groupeExistant = true;
                                break;
                            }
                        }
                        
                        if (!groupeExistant)
                        { trianglesSurface.Add(temp.Normale, new List<Triangle>(new []{ temp })); }
                    }

                    Queue<Triangle> trianglesEnAttente = new();
                    // comparer les triangles ayant la même normale
                    foreach (KeyValuePair<Vector3,List<Triangle>> groupeNormale in trianglesSurface)
                    {
                        while(groupeNormale.Value.Count > 0)
                        {
                            // on récupère le prochain triangle à tester et on le retire de la liste
                            Triangle triangleEnCours;
                            if (trianglesEnAttente.Count > 0)
                            {
                                var temp = trianglesEnAttente.Dequeue();
                                triangleEnCours = temp;
                                groupeNormale.Value.Remove(temp);
                            }
                            else
                            {
                                triangleEnCours = groupeNormale.Value[0];
                                groupeNormale.Value.RemoveAt(0);
                            }

                            // on teste ce triangle contre tout les autres
                            for (int t = 0; t < groupeNormale.Value.Count; ++t)
                            {
                                Triangle triangleATester = groupeNormale.Value[t];
                                if (triangleEnCours.Adjacent(triangleATester))
                                {
                                    triangleATester.ChangerFace(triangleEnCours.Face);
                                    trianglesEnAttente.Enqueue(triangleATester);
                                }
                            }

                            // on ajoute la Face du triangle si elle n'existe pas déjà
                            bool faceExiste = false;
                            if (facesSurface.Count > 0)
                            {
                                foreach (Face face in facesSurface)
                                {
                                    if (ReferenceEquals(face, triangleEnCours.Face))
                                    { faceExiste = true; }
                                }
                            }

                            if (!faceExiste)
                            { facesSurface.Add(triangleEnCours.Face); }
                        }
                    }
                }
            }
        }

        return faces;
    }


    public static void ModifierVertex(ref ArrayMesh meshe_, int indexSurface_, int indexVertex_, Vector3 vertex_)
    {
        if (indexVertex_  >= 0 && indexVertex_ < meshe_.SurfaceGetArrayLen(indexSurface_))
        {
            int tailleFloat = sizeof(float);
            int décalage = tailleFloat * 3 * indexVertex_;
            //int offsetVertex = (int)RenderingServer.MeshSurfaceGetFormatVertexStride((RenderingServer.ArrayFormat)meshe_.SurfaceGetFormat(indexSurface_), meshe_.SurfaceGetArrayLen(indexSurface_)) * indexVertex_;

            byte[] régionVertex = new byte[tailleFloat * 3];
            Buffer.BlockCopy(BitConverter.GetBytes(vertex_.X), 0, régionVertex, 0, tailleFloat);
            Buffer.BlockCopy(BitConverter.GetBytes(vertex_.Y), 0, régionVertex, tailleFloat, tailleFloat);
            Buffer.BlockCopy(BitConverter.GetBytes(vertex_.Z), 0, régionVertex, tailleFloat * 2, tailleFloat);

            meshe_.SurfaceUpdateVertexRegion(indexSurface_, décalage, régionVertex);
        }
    }
    public static void ModifierVertex(ArrayMesh meshe_, int indexSurface_, int indexVertex_, Vector3 vertex_, Vector3 normale_)
    {
        
    }
    
    public static void ModifierPlageVertexs(ref ArrayMesh meshe_, int indexSurface_, int indexPremierVertex_, Vector3[] vertexs_)
    {
        if (indexPremierVertex_ >= 0 && indexPremierVertex_ + vertexs_.Length <= meshe_.SurfaceGetArrayLen(indexSurface_))
        {
            int tailleFloat = sizeof(float);

            byte[] régionVertex = new byte[tailleFloat * 3 * vertexs_.Length];
            for (int v = 0; v < vertexs_.Length; ++v)
            {
                Vector3 vertex = vertexs_[v];
                int offset = v * tailleFloat * 3;
                Buffer.BlockCopy(BitConverter.GetBytes(vertex.X), 0, régionVertex, offset, tailleFloat);
                Buffer.BlockCopy(BitConverter.GetBytes(vertex.Y), 0, régionVertex, offset + tailleFloat, tailleFloat);
                Buffer.BlockCopy(BitConverter.GetBytes(vertex.Z), 0, régionVertex, offset + tailleFloat * 2, tailleFloat);
            }
            
            meshe_.SurfaceUpdateVertexRegion(indexSurface_, tailleFloat * 3 * indexPremierVertex_, régionVertex);
        }
    }
    public static void ModifierPlageVertexs(ref ArrayMesh meshe_, int indexSurface_, int indexPremierVertex_, Vector3[] vertexs_, Vector3[] normales_)
    {
        if (indexPremierVertex_ >= 0 && indexPremierVertex_ + vertexs_.Length <= meshe_.SurfaceGetArrayLen(indexSurface_) && normales_.Length == vertexs_.Length)
        {
            int tailleComposantVertex = sizeof(float), tailleComposantNormale = sizeof(UInt16);
            int décalageVertexs = tailleComposantVertex * 3 * indexPremierVertex_, décalageNormales = meshe_.SurfaceGetArrayLen(indexSurface_) * tailleComposantVertex + tailleComposantNormale * 4 * indexPremierVertex_;

            // on calcule le buffer des vertexs et normales/tangentes à modifier
            byte[] régionVertexs = new byte[tailleComposantVertex * 3 * vertexs_.Length];
            byte[] régionNormalesTangentes = new byte[tailleComposantNormale * 4 * normales_.Length];
            Vector3[] normales = meshe_.SurfaceGetArrays(indexSurface_)[(int)Mesh.ArrayType.Normal].AsVector3Array();
            float[] tangentes = meshe_.SurfaceGetArrays(indexSurface_)[(int)Mesh.ArrayType.Tangent].AsFloat32Array();
            
            for (int v = 0; v < vertexs_.Length; ++v)
            {
                Vector3 vertex = vertexs_[v];
                int offset = v * tailleComposantVertex * 3, indexTangente = v * 4;
                Buffer.BlockCopy(BitConverter.GetBytes(vertex.X), 0, régionVertexs, offset, tailleComposantVertex);
                Buffer.BlockCopy(BitConverter.GetBytes(vertex.Y), 0, régionVertexs, offset + tailleComposantVertex, tailleComposantVertex);
                Buffer.BlockCopy(BitConverter.GetBytes(vertex.Z), 0, régionVertexs, offset + tailleComposantVertex * 2, tailleComposantVertex);
                
                Quaternion rotationNormale = new Quaternion(normales[indexPremierVertex_ + v], normales_[v]).Normalized();
                Vector3 tangente = rotationNormale * new Vector3(tangentes[indexTangente], tangentes[indexTangente + 1], tangentes[indexTangente + 2]);
                byte[] normaleTangenteOptimisée = Vecteurs.EncodeNormalTangente(normales_[v], tangente, tangentes[indexTangente + 3]);
                Buffer.BlockCopy(normaleTangenteOptimisée, 0, régionNormalesTangentes, v * normaleTangenteOptimisée.Length, normaleTangenteOptimisée.Length);
            }

            // si l'ont modifier tout les vertexs de la mesh, on modifie les deux buffers d'un coup
            if (vertexs_.Length == meshe_.SurfaceGetArrayLen(indexSurface_))
            {
                byte[] datasCombinées = new byte[régionVertexs.Length + régionNormalesTangentes.Length];
                Buffer.BlockCopy(régionVertexs, 0, datasCombinées, 0, régionVertexs.Length);
                Buffer.BlockCopy(régionNormalesTangentes, 0, datasCombinées, régionVertexs.Length, régionNormalesTangentes.Length);
                meshe_.SurfaceUpdateVertexRegion(indexSurface_, 0, datasCombinées);
            }
            else
            {
                meshe_.SurfaceUpdateVertexRegion(indexSurface_, décalageVertexs, régionVertexs);
                meshe_.SurfaceUpdateVertexRegion(indexSurface_, décalageNormales, régionNormalesTangentes);
            }
        }
    }

    public static void ModifierPlageCouleurST(ref Mesh meshe_, int indexSurface_, int indexPremierVertex_, Color[] couleurs_)
    {
        // TODO: Modifier la couleur d'une mesh faire des tests de géométrie rapide
        throw new NotImplementedException();
    }
}