namespace VA.Base.GUI;

/// <summary>
/// définit le comportement de base des Elements pouvant controler des Elements enfants
/// </summary>
public abstract partial class Conteneur: Element
{
    // TODO: définir les properties de base
    
    public virtual int NombreElements { get; private set; }
}