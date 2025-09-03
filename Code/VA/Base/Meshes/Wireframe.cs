using System;
using System.Collections.Generic;
using Godot;
using GodotArray = Godot.Collections.Array;

namespace VA.Base.Meshes;

public static class Wireframe
{
    // TODO: retravailler ce bordel
    public static Mesh WireframeFaces(float épaisseur_, Mesh meshe_)
    { return WireframeFaces(meshe_, épaisseur_, Communs.ToléranceNormales, true); }
    public static Mesh WireframeFaces(Mesh meshe_)
    { return WireframeFaces(meshe_, 0.002f, Communs.ToléranceNormales, true); }
    public static Mesh WireframeFaces(Mesh meshe_, bool recalculer_)
    { return WireframeFaces(meshe_, 0.002f, Communs.ToléranceNormales, recalculer_); }
    public static Mesh WireframeFaces(Mesh meshe_, float tolérance_)
    { return WireframeFaces(meshe_, 0.002f, tolérance_, true); }
    public static Mesh WireframeFaces(Mesh meshe_, float épaisseur_, float tolérance_)
    { return WireframeFaces(meshe_, épaisseur_, tolérance_, true); }
    
    /// <summary>
    /// Crée un maillage 3D correspondant aux faces de la Meshe
    /// </summary>
    /// <param name="meshe_">La meshe à convertir</param>
    /// <param name="tolérance_">La différence de normale minimum entre deux triangles de faces différentes</param>
    /// <param name="recalculer_"></param>
    /// <param name="parent_"></param>
    /// <returns></returns>
    public static Mesh WireframeFaces(Mesh meshe_, float épaisseur_, float tolérance_, bool recalculer_)
    {
        ArrayMesh meshe = new();
        List<GodotArray> surfaces = new();
        
        List<Vector3> vertexs = new();
        List<int> triangles = new();
        List<Vector3> normales = new();
    
        List<List<Outils.Face>> faces = new();
        if (recalculer_)
        { faces = Outils.CalculerFaces(meshe_, tolérance_); }
        else
        {
            // TODO: créer une fonctions qui mesure les faces existantes dans la Mesh plutôt que de les recalculer
            faces = Outils.CalculerFaces(meshe_, tolérance_);
        }
    
        for (int f = 0; f < faces.Count; ++f)
        {
            foreach (Outils.Face face in faces[f])
            {
                foreach (Outils.Arrête arrête in face.RecalculerBordure())
                {
                    // si le nombre de vertexs ou de triangles dépasse 2 000 000, on crée une autre surface
                    if (vertexs.Count > 2000000 || triangles.Count > 2000000)
                    {
                        GodotArray nouvelleSurface = new();
                        nouvelleSurface.Resize((int)Mesh.ArrayType.Max);
                        nouvelleSurface[(int)Mesh.ArrayType.Vertex] = Variant.CreateFrom(vertexs.ToArray());
                        nouvelleSurface[(int)Mesh.ArrayType.Index] = Variant.CreateFrom(triangles.ToArray());
                        nouvelleSurface[(int)Mesh.ArrayType.Normal] = Variant.CreateFrom(normales.ToArray());
                        meshe.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, nouvelleSurface);
    
                        vertexs.Clear();
                        triangles.Clear();
                        normales.Clear();
                    }
    
                    GodotArray géométrieArrête = GéométrieCapsule_Dur(arrête.A, arrête.B, épaisseur_, 6);
    
                    int[] trianglesCapsule = géométrieArrête[(int)Mesh.ArrayType.Index].AsInt32Array();
                    if (vertexs.Count > 0)
                    {
                        for (int index = 0; index < trianglesCapsule.Length; ++index)
                        { trianglesCapsule[index] += vertexs.Count; }
                    }
                    triangles.AddRange(trianglesCapsule);
                    vertexs.AddRange(géométrieArrête[(int)Mesh.ArrayType.Vertex].AsVector3Array());
                    normales.AddRange(géométrieArrête[(int)Mesh.ArrayType.Normal].AsVector3Array());
                }
            }
        }
        GodotArray dernièreSurface = new();
        dernièreSurface.Resize((int)Mesh.ArrayType.Max);
        dernièreSurface[(int)Mesh.ArrayType.Vertex] = Variant.CreateFrom(vertexs.ToArray());
        dernièreSurface[(int)Mesh.ArrayType.Index] = Variant.CreateFrom(triangles.ToArray());
        dernièreSurface[(int)Mesh.ArrayType.Normal] = Variant.CreateFrom(normales.ToArray());
        meshe.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, dernièreSurface);
        
        return meshe;
    }    
    
    private static void CombinerMeshes(ref GodotArray tableau0_, in GodotArray tableau1_)
    {
        List<Vector3> vertexs = new(tableau0_[(int)Mesh.ArrayType.Vertex].AsVector3Array());
        List<int> triangles = new(tableau0_[(int)Mesh.ArrayType.Index].AsInt32Array());
        List<Vector3> normales = new(tableau0_[(int)Mesh.ArrayType.Normal].AsVector3Array());
        int décalage = vertexs.Count;
        
        vertexs.AddRange(tableau1_[(int)Mesh.ArrayType.Vertex].AsVector3Array());
        int[] trianglesCapsule = tableau1_[(int)Mesh.ArrayType.Index].AsInt32Array();
        for (int index = 0; index < trianglesCapsule.Length; ++index)
        { trianglesCapsule[index] += décalage; }
        triangles.AddRange(trianglesCapsule);
        normales.AddRange(tableau1_[(int)Mesh.ArrayType.Normal].AsVector3Array());
        
        tableau0_[(int)Mesh.ArrayType.Vertex] = Variant.CreateFrom(vertexs.ToArray());
        tableau0_[(int)Mesh.ArrayType.Index] = Variant.CreateFrom(triangles.ToArray());
        tableau0_[(int)Mesh.ArrayType.Normal] = Variant.CreateFrom(normales.ToArray());
    }

    // public static Mesh WireframeArrêtes(Mesh meshe_, float minTolérance_, float maxTolérance_)
    // {
    //     // utiliser le dot product
    //     // float différence = tri.Normale.Dot(Maths.Triangle.Normale(vertexs[tris[index++]], vertexs[tris[index++]], vertexs[tris[index++]]));
    // }

    public static Mesh Capsule_Dur(Vector3 a_, Vector3 b_, float rayon_, int division_)
    {
        ArrayMesh capsule = new();
        capsule.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, GéométrieCapsule_Dur(a_, b_, rayon_, division_));
        return capsule;
    }

    public static GodotArray GéométrieCapsule_Dur(Vector3 a_, Vector3 b_, float rayon_, int division_ = 4)
    {
        // Stopwatch chrono = new();
        // chrono.Start();
        division_ = division_ < 4 ? 4 : division_;
        
        List<Vector3> vertexs = new(2 * (4 * (division_ / 4) + 1) * division_);
        List<int> triangles = new(division_ / 4 * 12 * division_);
        List<Vector3> normales = new(vertexs.Capacity);

        float angleRad = Mathf.Tau / division_;
        Vector3 vecY = (a_ - b_).Normalized(), vecX = new Quaternion(Vector3.Up, vecY).Normalized() * Vector3.Right, vecZ = vecX.Cross(vecY).Normalized(), vecRayon = vecX * rayon_, normale = Vector3.Up;
        Quaternion rotLatA = new(vecZ, angleRad), rotLatB = new(vecZ, -angleRad), rotLon = new(vecY, angleRad);
        int latitudeMax = division_ / 4;
        int[,] indexsPointsCylindre = new[,] { { 0, 3, 5, 4 }, { 0, 2, 4, 3 } };

        // on calcule les points d'origines
        Vector3[,] points = { { a_, vecRayon, Vector3.Zero, vecY * rayon_ }, { b_, vecRayon, Vector3.Zero, -vecY * rayon_ } };
        
        // puis on fait les extrémités
        for (int latitude = 0; latitude < latitudeMax; ++latitude)
        {
            points[0,2] = rotLatA * points[0,1];
            points[1,2] = rotLatB * points[1,1];
            // on parcours les anneaux et on construit les quads
            for (int longitude = 0; longitude < division_ ; ++longitude)
            {
                // créer les quatres vertexs du quad en cours
                int indexVertex0;
                for (int p = 0; p < 2; ++p)
                {
                    indexVertex0 = vertexs.Count > 0 ? vertexs.Count : 0;
                    // le sommet
                    if (latitude + 1 == latitudeMax)
                    {
                        // créer des triangles et non des quads
                        if (p == 0)
                        { vertexs.AddRange(new[] { points[p, 0] + points[p,1], points[p, 0] + points[p,3], points[p, 0] + rotLon * points[p,1] }); }
                        if (p == 1)
                        { vertexs.AddRange(new[] { points[p, 0] + points[p,1], points[p, 0] + rotLon * points[p,1], points[p, 0] + points[p,3] }); }
                        
                        normale = Maths.Géom.Triangle.Normale(vertexs[indexVertex0], vertexs[indexVertex0 + 1], vertexs[indexVertex0 + 2]);
                        triangles.AddRange(new[] { indexVertex0, indexVertex0 + 1, indexVertex0 + 2});
                        normales.AddRange(new[] { normale, normale, normale});
                    }
                    else
                    {
                        if (p == 0)
                        { vertexs.AddRange(new[] { points[p, 0] + points[p, 1], points[p, 0] + points[p, 2], points[p, 0] + rotLon * points[p, 2], points[p, 0] + rotLon * points[p, 1] }); }
                        if (p == 1)
                        { vertexs.AddRange(new[] { points[p, 0] + points[p, 1], points[p, 0] + rotLon * points[p, 1], points[p, 0] + rotLon * points[p, 2], points[p, 0] + points[p, 2] }); }
                        
                        normale = Maths.Géom.Triangle.Normale(vertexs[indexVertex0], vertexs[indexVertex0 + 1], vertexs[indexVertex0 + 2]);
                        triangles.AddRange(new[] { indexVertex0, indexVertex0 + 1, indexVertex0 + 2, indexVertex0, indexVertex0 + 2, indexVertex0 + 3 });
                        normales.AddRange(new[] { normale, normale, normale, normale });
                    }
                }
                
                // on construit le cylindre au premier tour
                if (latitude == 0)
                {
                    int décalage = latitudeMax == 1 ? 6 : 8, type = latitudeMax == 1 ? 1 : 0;
                    indexVertex0 = vertexs.Count - décalage;
                    for (int p = 0; p < 4; ++p)
                    { vertexs.Add(vertexs[indexVertex0 + indexsPointsCylindre[type, p]]); }
                    indexVertex0 += décalage;
                    normale = Maths.Géom.Triangle.Normale(vertexs[indexVertex0], vertexs[indexVertex0 + 1], vertexs[indexVertex0 + 2]);
                    triangles.AddRange(new[] { indexVertex0, indexVertex0 + 1, indexVertex0 + 2, indexVertex0, indexVertex0 + 2, indexVertex0 + 3 });
                    normales.AddRange(new[] { normale, normale, normale, normale });
                }
                
                // on tourne l'origine sur la longitude
                points[0,1] = rotLon * points[0,1];
                points[1,1] = rotLon * points[1,1];
                points[0,2] = rotLon * points[0,2];
                points[1,2] = rotLon * points[1,2];
            }
            // on tourne le point sur la latitude
            points[0,1] = points[0,2];
            points[1,1] = points[1,2];
        }
        // GD.Print($"Génération de la capsule en {chrono.Elapsed.TotalMilliseconds:F4} ms et {chrono.Elapsed.Ticks} Ticks");
        // chrono.Stop();

        // GD.Print($"VERTEXS [{vertexs.Count}]/[{vertexs.Capacity}]");
        // GD.Print($"TRIANGLES [{triangles.Count}]/[{triangles.Capacity}]");
        // GD.Print($"NORMALES [{normales.Count}]/[{normales.Capacity}]");
        GodotArray géométrie = new GodotArray();
        géométrie.Resize((int)Mesh.ArrayType.Max);
        géométrie[(int)Mesh.ArrayType.Vertex] = Variant.CreateFrom(vertexs.ToArray());
        géométrie[(int)Mesh.ArrayType.Index] = Variant.CreateFrom(triangles.ToArray());
        géométrie[(int)Mesh.ArrayType.Normal] = Variant.CreateFrom(normales.ToArray());

        return géométrie;
    }
}