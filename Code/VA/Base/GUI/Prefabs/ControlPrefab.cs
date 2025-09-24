using Godot;

namespace VA.Base.GUI.Prefab;


/*public interface IControlPrefab
{
    /// <summary>
    /// La classe dans laquelle construire la hiérarchie du Control et l'ajouter au Control dont doit hériter la classe implémentant cette interface
    /// </summary>
    public Control CréerControl();
}*/

/// <summary>
/// Classe de base de tout les prefabriqués de GUI
/// </summary>
public abstract partial class ControlPrefab : Control
{
    /// <summary>
    /// La méthode qui construit le prefab
    /// </summary>
    protected abstract void Construire();

    public ControlPrefab()
    {
        Construire();
    }
}