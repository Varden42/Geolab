using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace VA.Base.Maths.Géom;

public static class Utiles
{
    /// <summary>
    /// Calcule le centre d'une liste de points
    /// </summary>
    /// <param name="points_"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">La liste ne doit être ni nulle ni vide</exception>
    public static Vector2 CalcCentre(IEnumerable<Vector2> points_)
    {
        int nbPoints = points_.Count();
        if (points_ == null || nbPoints <= 0)
            throw new ArgumentException("Le tableau de points ne peut pas être vide ou null");
        
        Vector2 somme = Vector2.Zero;
    
        foreach (Vector2 point in points_)
        { somme += point; }
    
        return somme / nbPoints;
    }
    
    /// <summary>
    /// Calcule le centre d'une liste de points
    /// </summary>
    /// <param name="points_"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">La liste ne doit être ni nulle ni vide</exception>
    public static Vector3 CalcCentre(IEnumerable<Vector3> points_)
    {
        int nbPoints = points_.Count();
        if (points_ == null || nbPoints <= 0)
            throw new ArgumentException("Le tableau de points ne peut pas être vide ou null");
        
        Vector3 somme = Vector3.Zero;
    
        foreach (Vector3 point in points_)
        { somme += point; }
    
        return somme / nbPoints;
    }
}