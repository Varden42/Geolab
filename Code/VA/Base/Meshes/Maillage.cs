using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using VA.Base.Maths;

namespace VA.Base.Meshes.GenProc;

//TODO: A revoir

// public class Maillage
//     {
//         public const float DifférenceNormales = 0.0001f;
//         public class Sommet : IEquatable<Sommet>
//         {
//             public struct Liaison
//             {
//                 //public Sommet Source , Destination;
//                 public int[] IndexsSommets;
//                 public Arrête Arrête;
//
//                 public Liaison(Sommet source_, Sommet destination_)
//                 {
//                     // Si l'arrête n'existe pas, la créer.
//                     // Sinon récupérer la position des sommets dedans.
//
//                     IndexsSommets = new int[2];
//                     Arrête = null;
//                 }
//             }
//             
//             private Maillage Parent;
//             //public List<(int IndexSurface, int IndexVertex)> Vertexs;
//             private Dictionary<int, List<int>> Vertexs;
//             public Dictionary<int, List<Liaison>> Liaisons;
//             //public Dictionary<int, List<Triangle>> Triangles;
//
//             public Vector3 Position;
//
//             //public readonly int IndexSurface;
//             //public readonly int IndexVertex;
//             //public List<Arrête> Arrêtes { get; }
//
//             public Sommet(Maillage parent_, int indexSurface_, int indexVertex_)
//             {
//                 Parent = parent_;
//                 
//                 if ((indexSurface_ < 0 || indexSurface_ >= Parent.Vertexs.Count) && (indexVertex_ < 0 || indexVertex_ >= Parent.Vertexs[indexSurface_].Count))
//                 { throw new ArgumentException($"Les arguments inderSurface_ [{indexSurface_}] et indexVertex_ [{indexVertex_}] ne sont pas valides!!"); }
//                 
//                 Vertexs = new(1);
//                 List<int> vertexs = new(1);
//                 vertexs.Add(indexVertex_);
//                 Vertexs.Add(indexSurface_, vertexs);
//                 
//                 //Arrêtes = new();
//                 //Triangles = new();
//                 Position = Parent.Vertexs[indexSurface_][indexVertex_];
//                 Liaisons = new();
//                 //IndexSurface = indexSurface_;
//                 //IndexVertex = indexVertex_;
//                 //Arrêtes = new();
//             }
//
//             public int RécupVertex(int surface_, int index_)
//             { return Vertexs[surface_][index_]; }
//             
//             //???public bool SupprimerVertex(int surface_, int index_)
//
//             public override int GetHashCode()
//             { return Position.GetHashCode(); }
//
//             public override bool Equals(object obj)
//             { return Equals(obj as Sommet); }
//
//             public bool Equals(Sommet sommet_)
//             { return sommet_ != null && sommet_.Position == Position; }
//         }
//
//         /// <summary>
//         /// Primitive regroupant deux Sommets et une liste de Triangles
//         /// </summary>
//         public class Arrête : IEquatable<Arrête>
//         {
//             public readonly Sommet[] Sommets;
//             public List<Triangle> Triangles;
//
//             /// <summary>
//             /// Crée une Arrête à partir de ses Sommets
//             /// </summary>
//             /// <param name="i_"></param>
//             /// <param name="j_"></param>
//             public Arrête(Sommet i_, Sommet j_)
//             {
//                 Sommets = new[] { i_, j_ };
//
//                 Triangles = new();
//             }
//
//             // /// <summary>
//             // /// Crée une Arrête à partir de la position de ses Sommets
//             // /// </summary>
//             // /// <param name="i_"></param>
//             // /// <param name="j_"></param>
//             // public Arrête(Vector3 i_, Vector3 j_)
//             // {
//             //     I = new Sommet(i_);
//             //     J = new Sommet(j_);
//             //     I.Arrêtes.Add(this);
//             //     J.Arrêtes.Add(this);
//             //
//             //     Triangles = new();
//             // }
//
//             public List<Triangle> TrianglesAdjacents(Triangle triangle_)
//             { return new List<Triangle>(Triangles.Where(t => !t.Equals(triangle_))); }
//
//             public int SommetFaitPartie(Sommet sommet_)
//             { return sommet_.Equals(Sommets[0]) ? 1 : sommet_.Equals(Sommets[1]) ? 2 : -1; }
//
//             public override int GetHashCode()
//             { return (Sommets[0], Sommets[1]).GetHashCode(); }
//
//             public override bool Equals(object obj)
//             { return Equals(obj as Arrête); }
//
//             public bool Equals(Arrête arrête_)
//             { return arrête_ != null && (arrête_.Sommets[0].Equals(Sommets[0]) && arrête_.Sommets[1].Equals(Sommets[1]) || arrête_.Sommets[0].Equals(Sommets[1]) && arrête_.Sommets[1].Equals(Sommets[0])); }
//         }
//
//         public class Triangle : IEquatable<Triangle>
//         {
//             private Arrête[] Arrêtes;
//             public readonly int Surface;
//
//             public readonly (Sommet s, int v) A;
//             public readonly (Sommet s, int v) B;
//             public readonly (Sommet s, int v) C;
//
//             //public Arrête AB => Arrêtes[0];
//             //public Arrête BC => Arrêtes[1];
//             //public Arrête CA => Arrêtes[2];
//
//             public Face Face;
//
//             public Vector3 Normale => Maths.Triangle.Normale(A.s.Position, B.s.Position, C.s.Position).Normalized();
//
//             /// <summary>
//             /// Crée un Triangle à partir de ses Arrêtes
//             /// </summary>
//             /// <param name="ab_"></param>
//             /// <param name="bc_"></param>
//             /// <param name="ca_"></param>
//             public Triangle(Arrête ab_, Arrête bc_, Arrête ca_)
//             {
//                 // Ajouter le Triangle aux arrêtes
//                 Arrêtes = new Arrête[3] { ab_, bc_, ca_ };
//                 ab_.Triangles.Add(this);
//                 bc_.Triangles.Add(this);
//                 ca_.Triangles.Add(this);
//                 Face = null;
//                 
//                 // TODO: Vérifier que les arrêtes fournits sont bien liées ensemble formant un triangle.
//
//                 // List<Triangle> voisins = Adjacents();
//                 // List<Triangle> voisinsAlignés = new();
//                 //
//                 // foreach (Triangle voisin in voisins)
//                 // {
//                 //     if (Face == null)
//                 //     {
//                 //         if (voisin.Face == null)
//                 //         {
//                 //             
//                 //         }
//                 //     }
//                 //     if (voisin.Face != null)
//                 //     {
//                 //         if (Algèbre.CompVec3DFEgal(Normale, voisin.Face.Normale, DifférenceNormales))
//                 //         {
//                 //             voisin.Face.Ajout(this);
//                 //         }
//                 //     }
//                 //     else
//                 //     {
//                 //         if (Algèbre.CompVec3DFEgal(Normale, voisin.Normale, DifférenceNormales))
//                 //         {
//                 //         }
//                 //     }
//                 // }
//             }
//
//             /// <summary>
//             /// Crée un Triangle à partir de ses Sommets
//             /// </summary>
//             /// <param name="a_"></param>
//             /// <param name="b_"></param>
//             /// <param name="c_"></param>
//             public Triangle(Sommet a_, Sommet b_, Sommet c_)
//             {
//                 // Ajouter le Triangle aux arrêtes
//                 Arrêtes = new Arrête[3];
//             
//                 Arrête ab_ = new Arrête(a_, b_);
//                 ab_.Triangles.Add(this);
//                 Arrêtes[0] = ab_;
//             
//                 Arrête bc_ = new Arrête(b_, c_);
//                 bc_.Triangles.Add(this);
//                 Arrêtes[1] = bc_;
//             
//                 Arrête ca_ = new Arrête(c_, a_);
//                 ca_.Triangles.Add(this);
//                 Arrêtes[2] = ca_;
//                 
//                 Face = null;
//             }
//             
//             /// <summary>
//             /// Crée un Triangle à partir de la position de ses sommets
//             /// </summary>
//             /// <param name="a_"></param>
//             /// <param name="b_"></param>
//             /// <param name="c_"></param>
//             public Triangle(Vector3 a_, Vector3 b_, Vector3 c_)
//             {
//                 // Ajouter le Triangle aux arrêtes
//                 Arrêtes = new Arrête[3];
//             
//                 Arrête ab_ = new Arrête(new Sommet(a_), new Sommet(b_));
//                 ab_.Triangles.Add(this);
//                 Arrêtes[0] = ab_;
//             
//                 Arrête bc_ = new Arrête(new Sommet(b_), new Sommet(c_));
//                 bc_.Triangles.Add(this);
//                 Arrêtes[1] = bc_;
//             
//                 Arrête ca_ = new Arrête(new Sommet(c_), new Sommet(a_));
//                 ca_.Triangles.Add(this);
//                 Arrêtes[2] = ca_;
//                 
//                 Face = null;
//             }
//
//
//             public Sommet[] RécupSommets()
//             {
//                 Sommet[] sommets = new Sommet[3];
//                 
//                 if (CA.SommetFaitPartie(AB.I) > 0 && BC.SommetFaitPartie(AB.J) > 0)
//                 { sommets[0] = AB.I; }
//                 else
//                 { sommets[0] = AB.J; }
//                 
//                 if (AB.SommetFaitPartie(BC.I) > 0 && CA.SommetFaitPartie(BC.J) > 0)
//                 { sommets[1] = BC.I; }
//                 else
//                 { sommets[1] = BC.J; }
//                 
//                 if (BC.SommetFaitPartie(CA.I) > 0 && AB.SommetFaitPartie(CA.J) > 0)
//                 { sommets[2] = CA.I; }
//                 else
//                 { sommets[2] = CA.J; }
//             
//                 return sommets;
//             }
//             public List<Triangle> Adjacents()
//             {
//                 List<Triangle> adjacents = new();
//                 adjacents.AddRange(AB.TrianglesAdjacents(this));
//                 adjacents.AddRange(BC.TrianglesAdjacents(this));
//                 adjacents.AddRange(CA.TrianglesAdjacents(this));
//             
//                 return adjacents;
//             }
//
//             public override int GetHashCode()
//             { return (A, B, C).GetHashCode(); }
//
//             public override bool Equals(object obj)
//             { return Equals(obj as Triangle); }
//
//             public bool Equals(Triangle triangle_)
//             { return triangle_ != null && triangle_.A.Equals(A) && triangle_.B.Equals(B) && triangle_.C.Equals(C); }
//         }
//
//         public class Face
//         {
//             public  List<Triangle> Triangles;
//             public  Vector3 Normale;
//
//             public Face()
//             {
//                 Triangles = new();
//                 Normale = Vector3.Zero;
//             }
//
//             public Face(Triangle triangle_)
//             {
//                 Triangles = new();
//                 Triangles.Add(triangle_);
//                 triangle_.Face = this;
//                 Normale = triangle_.Normale;
//             }
//
//             public void Ajout(Triangle triangle_)
//             {
//                 Triangles.Add(triangle_);
//                 triangle_.Face = this;
//                 MajNormale();
//             }
//
//             public void Combiner(Face face_)
//             {
//                 foreach (Triangle triangle in face_.Triangles)
//                 {
//                     triangle.Face = this;
//                     Triangles.Add(triangle);
//                 }
//                 
//                 MajNormale();
//             }
//
//             private void MajNormale()
//             {
//                 Normale = Vector3.Zero;
//                 foreach (Triangle triangle in Triangles)
//                 { Normale += triangle.Normale; }
//                 Normale /= Triangles.Count;
//             }
//
//             public bool Equals(Face face_)
//             { return face_ != null && face_.Normale == Normale; }
//             
//             // TODO: Recalculer la face avec le minimum de Triangles et tous avec la même normale.
//         }
//
//
//         private List<HashSet<Sommet>> Sommets;
//         private List<HashSet<Arrête>> Arrêtes;
//         private List<HashSet<Triangle>> Triangles;
//         private List<Face> Faces;
//
//         private List<List<Vector3>> Vertexs;
//
//         public Maillage()
//         {
//             Sommets = new();
//             Arrêtes = new();
//             Triangles = new();
//             Faces = new();
//
//             Vertexs = new();
//         }
//         
//         public Maillage(Mesh meshe_)
//         {
//             Sommets = new();
//             Arrêtes = new();
//             Triangles = new();
//             Faces = new();
//             
//             Vertexs = new();
//
//             MesheVersMaillage(meshe_);
//             CalculerFaces();
//         }
//
//         private void Vider()
//         {
//             Sommets.Clear();
//             Arrêtes.Clear();
//             Triangles.Clear();
//             Faces.Clear();
//             Vertexs.Clear();
//         }
//
//         private void MesheVersMaillage(Mesh meshe_)
//         {
//             if (!ReferenceEquals(meshe_, null) && meshe_.GetSurfaceCount() > 0)
//             {
//                 Vider();
//                 // Parcourir les surfaces de la meshe
//                 for (int surfaceIndex = 0; surfaceIndex < meshe_.GetSurfaceCount(); ++surfaceIndex)
//                 {
//                     Sommets.Add(new());
//                     Arrêtes.Add(new());
//                     Triangles.Add(new());
//                     
//                     // les différentes valeurs
//                     GodotArray surface = meshe_.SurfaceGetArrays(surfaceIndex);
//                     // construire le maillage de la surface
//                     int[] triangles = surface[(int)Mesh.ArrayType.Index].AsInt32Array();
//                     Vertexs.Add(new(surface[(int)Mesh.ArrayType.Vertex].AsVector3Array()));
//                     //Vector3[] vertexs = surface[(int)Mesh.ArrayType.Vertex].AsVector3Array();
//
//                     // si les triangles sont définis
//                     if (triangles.Length % 3 == 0)
//                     {
//                         // peuple le maillage de Sommets, Arrêtes et Triangles en évitant les doublons
//                         for (int index = 0; index < triangles.Length;)
//                         {
//                             Sommet a = new(this, surfaceIndex, triangles[index++]);
//                             a = Sommets[surfaceIndex].TryGetValue(a, out Sommet a_) ? a_ : a;
//                             Sommets[surfaceIndex].Add(a);
//
//                             Sommet b = new(this, surfaceIndex, triangles[index++]);
//                             b = Sommets[surfaceIndex].TryGetValue(b, out Sommet b_) ? b_ : b;
//                             Sommets[surfaceIndex].Add(b);
//
//                             Sommet c = new(this, surfaceIndex, triangles[index++]);
//                             c = Sommets[surfaceIndex].TryGetValue(c, out Sommet c_) ? c_ : c;
//                             Sommets[surfaceIndex].Add(c);
//
//                             Arrête ab = new(a, b);
//                             ab = Arrêtes[surfaceIndex].TryGetValue(ab, out Arrête ab_) ? ab_ : ab;
//                             Arrêtes[surfaceIndex].Add(ab);
//
//                             Arrête bc = new(b, c);
//                             bc = Arrêtes[surfaceIndex].TryGetValue(bc, out Arrête bc_) ? bc_ : bc;
//                             Arrêtes[surfaceIndex].Add(bc);
//
//                             Arrête ca = new(c, a);
//                             ca = Arrêtes[surfaceIndex].TryGetValue(ca, out Arrête ca_) ? ca_ : ca;
//                             Arrêtes[surfaceIndex].Add(ca);
//
//                             Triangle abc = new(ab, bc, ca);
//                             Triangles[surfaceIndex].Add(abc);
//                         }
//                     }
//                 }
//             }
//         }
//
//         public Maillage Dupliquer()
//         {
//             Maillage duplicat = new();
//
//             // à faire pour chaques surfaces
//             foreach (Sommet sommet in Sommets)
//             { duplicat.Sommets.Add(new(this, sommet.IndexSurface, sommet.IndexVertex)); }
//
//             foreach (Arrête arrête in Arrêtes)
//             {
//                 if (duplicat.Sommets.TryGetValue(arrête.I, out Sommet a_) &&
//                     duplicat.Sommets.TryGetValue(arrête.J, out Sommet b_))
//                 { duplicat.Arrêtes.Add(new(a_, b_)); }
//             }
//
//             foreach (Triangle triangle in Triangles)
//             {
//                 if (duplicat.Arrêtes.TryGetValue(triangle.AB, out Arrête ab_) &&
//                     duplicat.Arrêtes.TryGetValue(triangle.BC, out Arrête bc_) &&
//                     duplicat.Arrêtes.TryGetValue(triangle.CA, out Arrête ca_))
//                 { duplicat.Triangles.Add(new(ab_, bc_, ca_)); }
//             }
//
//             return duplicat;
//         }
//
//         private void CalculerFaces()
//         {
//             // TODO: A faire pour chaques surfaces
//             List<Triangle> trianglesATester = new(Triangles);
//             List<Face> faces = new();
//             while (trianglesATester.Count > 0)
//             {
//                 Triangle triangleEnCours = trianglesATester[0];
//                 Face faceEnCours = new Face(triangleEnCours);
//                 faces.Add(faceEnCours);
//                 //List<Triangle> adjacents = triangles[t].Adjacents();
//                 foreach (Triangle adjacent in triangleEnCours.Adjacents())
//                 {
//                     if (!ReferenceEquals(adjacent.Face, null))
//                     {
//                         if (Algèbre.CompVec3DFEgal(faceEnCours.Normale, adjacent.Face.Normale, DifférenceNormales))
//                         {
//                             adjacent.Face.Combiner(faceEnCours);
//                             faces.Remove(faceEnCours);
//                             faceEnCours = triangleEnCours.Face;
//                         }
//                     }
//                     else if (Algèbre.CompVec3DFEgal(faceEnCours.Normale, adjacent.Normale, DifférenceNormales))
//                     {
//                         faceEnCours.Ajout(adjacent);
//                         trianglesATester.Remove(adjacent);
//                     }
//                 }
//                 
//                 trianglesATester.RemoveAt(0);
//             }
//
//             while (faces.Count > 0)
//             {
//                 for (int f = 1; f < faces.Count; ++f)
//                 {
//                     if (Algèbre.CompVec3DFEgal(faces[0].Normale, faces[f].Normale, DifférenceNormales))
//                     {
//                         faces[0].Combiner(faces[f]);
//                         faces.Remove(faces[f]);
//                     }
//                     Faces.Add(faces[0]);
//                     faces.Remove(faces[0]);
//                 }
//             }
//         }
//
//         public Mesh MaillageVersMeshe(bool facesDissociées_)
//         { return CalculerMesh(facesDissociées_); }
//
//         private Mesh CalculerMesh(bool facesDissociées_)
//         {
//             // TODO: A faire pour chaque surfaces
//             for (int indeSurface = 0; indeSurface < Vertexs.Count; ++indeSurface)
//             {
//                 // boucler ici...
//             }
//             foreach (HashSet<Triangle> triangles in Triangles)
//             {
//                 List<Vector3> vertexs = new();
//                 int[] tris = new int[triangles.Count * 3];
//                 int indexTriangle = 0;
//
//                 foreach (Triangle triangle in triangles)
//                 {
//                     Sommet[] abc = triangle.RécupSommets();
//                     vertexs.AddRange(new []
//                     {
//                         Vertexs[abc[0].IndexSurface][abc[0].IndexVertex],
//                         Vertexs[abc[1].IndexSurface][abc[1].IndexVertex],
//                         Vertexs[abc[2].IndexSurface][abc[2].IndexVertex]
//                     });
//                     
//                     tris[indexTriangle++] = 
//                 }
//             }
//             
//             
//             GodotArray surfaceArray = new GodotArray();
//             surfaceArray.Resize((int)Mesh.ArrayType.Max);
//
//             surfaceArray[(int)Mesh.ArrayType.Vertex] = Vertexs;
//             surfaceArray[(int)Mesh.ArrayType.TexUV] = UVs;
//             surfaceArray[(int)Mesh.ArrayType.Index] = TrianglesVersArray();
//             surfaceArray[(int)Mesh.ArrayType.Normal] = CalculerNormales();
//             
//             ArrayMesh meshe = new ArrayMesh();
//             meshe.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, surfaceArray);
//             return meshe;
//         }
//     }