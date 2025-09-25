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
    /// La méthode qui construit le prefab, elle sera systématiquement appellé dans le constructeur de cette classe et donc ne devra jamais faire appel à des variables externes à la méthode.
    /// </summary>
    protected abstract void Construire();

    /// <summary>
    /// Le constructeur qui appelle la méthode "Construire" de la classe enfant afin de construire le Contrlà la constuction de l'objet. 
    /// </summary>
    public ControlPrefab()
    {
        Construire();
    }
}