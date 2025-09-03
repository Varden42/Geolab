using Godot;

namespace VA.Base.GUI;

/// <summary>
/// Définit une interface 2d entre un objet et le joueur
/// </summary>
public interface IInterface2D
{
    public Control Interface2D { get; }
}

public interface IInterface3D
{
    public Node3D Interface3D { get; }
}