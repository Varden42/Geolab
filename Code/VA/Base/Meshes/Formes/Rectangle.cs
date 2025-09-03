using System;
using Godot;
using VA.Base.Maths.Géom;

namespace VA.Base.Meshes;

public static partial class Formes
{
    /// <summary>
    /// Crée une Mesh représentant un quad en utilisant le SurfaceTool
    /// </summary>
    /// <param name="a_"></param>
    /// <param name="b_"></param>
    /// <param name="c_"></param>
    /// <param name="d_"></param>
    /// <param name="couleur_"></param>
    /// <returns></returns>
    public static ArrayMesh QuadST(Vector3 a_, Vector3 b_, Vector3 c_, Vector3 d_, Color couleur_ = default)
    {
        var surfaceTool = new SurfaceTool();
        surfaceTool.Begin(Mesh.PrimitiveType.Triangles);

        // Add vertices
        surfaceTool.SetColor(couleur_);
        surfaceTool.AddVertex(a_);
        
        surfaceTool.SetColor(couleur_);
        surfaceTool.AddVertex(b_);
        
        surfaceTool.SetColor(couleur_);
        surfaceTool.AddVertex(c_);
        
        surfaceTool.SetColor(couleur_);
        surfaceTool.AddVertex(c_);
        
        surfaceTool.SetColor(couleur_);
        surfaceTool.AddVertex(d_);
        
        surfaceTool.SetColor(couleur_);
        surfaceTool.AddVertex(a_);

        // Generate normals and create the mesh
        surfaceTool.GenerateNormals();
        return surfaceTool.Commit();
    }
    
    /// <summary>
    /// Crée une Mesh représentant un quad en utilisant le SurfaceTool
    /// </summary>
    /// <param name="quad_"></param>
    /// <param name="couleur_"></param>
    /// <returns></returns>
    public static Mesh QuadST(QuadStruct3D quad_, Color couleur_ = default) => QuadST(quad_.A, quad_.B, quad_.C, quad_.D, couleur_);


    public static Mesh CadreST(QuadStruct3D quad_, Color couleur_ = default, int épaisseur_ = 1)
    {
        // TODO: Créer un rectangle dont le centre est vide, un cadre quoi, afin de pouvoir tester les formules Rectangles
        
        // Calculer La médiane des vecteurs se croisant à chaque sommet.
        // Calculer les sommets du cadre à partir de la médiane et de l'épaisseur
        // Créer ensuite vertexs, couleurs et triangles à partir de ces points
        
        Vector3 ab = quad_.B - quad_.A, bc = quad_.C - quad_.B, cd = quad_.D - quad_.C, da = quad_.A - quad_.D;
        
        throw new NotImplementedException();
    }
}






















