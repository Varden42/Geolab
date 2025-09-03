using Godot;
using VA.Base.Maths.Géom;

namespace VA.Base.Meshes;

/// <summary>
/// Contient des méthodes pour créer des meshes de différentes formes
/// </summary>
public static partial class Formes
{
    /// <summary>
    /// Crée une Mesh représentant un triangle
    /// </summary>
    /// <param name="a_"></param>
    /// <param name="b_"></param>
    /// <param name="c_"></param>
    /// <param name="couleur_"></param>
    /// <returns></returns>
    public static ArrayMesh Triangle(Vector3 a_, Vector3 b_, Vector3 c_, Color couleur_ = default)
    {
        var surface = new Godot.Collections.Array();
        surface.Resize((int)Mesh.ArrayType.Max);

        var vertexs = new Vector3[]
        { a_, b_, c_ };

        Vector3 normale = Maths.Géom.Triangle.Normale(a_, b_, c_);
        var normales = new Vector3[]
        { normale, normale, normale };

        var couleurs = new Color[]
        { couleur_, couleur_, couleur_ };

        surface[(int)Mesh.ArrayType.Vertex] = vertexs;
        surface[(int)Mesh.ArrayType.Normal] = normales;
        surface[(int)Mesh.ArrayType.Color] = couleurs;

        var meshe = new ArrayMesh();
        meshe.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, surface);
        return meshe;
    }
    
    /// <summary>
    /// Crée une Mesh représentant un triangle
    /// </summary>
    /// <param name="triangle_"></param>
    /// <param name="couleur_"></param>
    /// <returns></returns>
    public static ArrayMesh Triangle(TriangleStruct3D triangle_, Color couleur_) => Triangle(triangle_.A, triangle_.B, triangle_.C, couleur_);
    
    /// <summary>
    /// Crée une Mesh représentant un triangle en utilisant le SurfaceTool
    /// </summary>
    /// <param name="a_"></param>
    /// <param name="b_"></param>
    /// <param name="c_"></param>
    /// <param name="couleur_"></param>
    /// <returns></returns>
    public static ArrayMesh TriangleST(Vector3 a_, Vector3 b_, Vector3 c_, Color couleur_ = default)
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

        // Generate normals and create the mesh
        surfaceTool.GenerateNormals();
        return surfaceTool.Commit();
    }
    
    /// <summary>
    /// Crée une Mesh représentant un triangle en utilisant le SurfaceTool
    /// </summary>
    /// <param name="triangle_"></param>
    /// <param name="couleur_"></param>
    /// <returns></returns>
    public static ArrayMesh TriangleSurfaceTool(TriangleStruct3D triangle_, Color couleur_) => TriangleST(triangle_.A, triangle_.B, triangle_.C, couleur_);
}
