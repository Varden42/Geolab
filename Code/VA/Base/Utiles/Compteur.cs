namespace VA.Base.Utiles;

public class CompteurInt
{
    private int Compte;

    public CompteurInt(int compte_ = 0)
    { Compte = compte_; }

    public int Ajouter => ++Compte;
    public int Retirer => --Compte;
}