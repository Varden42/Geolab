using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

using Godot;
using VA.Base.Utiles;

namespace VA.Base.Systèmes.Gestionnaires;

public class GestComposants
{
    /// <summary>
    /// Stocke un ensemble de Value Types dans un tableau de bytes et fournit les outils pour les récupérer
    /// un composant a pour fonction de stocker des données de tailles variables mais n'est pas fait pour être modifié après sa création.
    /// </summary>
    public class Composant
    {
        /*
        TypeCode enum:
        Empty = 0,       // A null reference.
        Object = 1,      // A general type representing any reference or value type not explicitly represented by another TypeCode.
        DBNull = 2,      // A database null value.
        Boolean = 3,     // A simple type representing Boolean values of true or false.
        Char = 4,        // An integral type representing unsigned 16-bit integers with values between 0 and 65535.
        SByte = 5,       // An integral type representing signed 8-bit integers with values between -128 and 127.
        Byte = 6,        // An integral type representing unsigned 8-bit integers with values between 0 and 255.
        Int16 = 7,       // An integral type representing signed 16-bit integers with values between -32768 and 32767.
        UInt16 = 8,      // An integral type representing unsigned 16-bit integers with values between 0 and 65535.
        Int32 = 9,       // An integral type representing signed 32-bit integers with values between -2147483648 and 2147483647.
        UInt32 = 10,     // An integral type representing unsigned 32-bit integers with values between 0 and 4294967295.
        Int64 = 11,      // An integral type representing signed 64-bit integers with values between -9223372036854775808 and 9223372036854775807.
        UInt64 = 12,     // An integral type representing unsigned 64-bit integers with values between 0 and 18446744073709551615.
        Single = 13,     // A floating point type representing values ranging from approximately 1.5 x 10^-45 to 3.4 x 10^38 with a precision of 7 digits.
        Double = 14,     // A floating point type representing values ranging from approximately 5.0 x 10^-324 to 1.7 x 10^308 with a precision of 15-16 digits.
        Decimal = 15,    // A simple type representing values ranging from 1.0 x 10^-28 to approximately 7.9 x 10^28 with 28-29 significant digits.
        DateTime = 16,   // A type representing a date and time value.
        String = 18      // A sealed class type representing Unicode character strings.
         */
        private enum TypesValeurSpéciales
        { Vector2 = 32, Vector2I, Vector3, Vector3I, Vector4, Vector4I }
        
        public class Index
        {
            public readonly int Départ;
            public readonly int Longueur;

            public Index(int départ_, int longueur_)
            {
                Départ = départ_;
                Longueur = longueur_;
            }
        }

        private static Dictionary<string, int> IdsTypesSpéciaux;
        private byte[] Données;
        private int TailleUtilisée;
        private int IndexByteLibre;
        private Dictionary<int, Dictionary<string, Index>> Registre;

        public int Taille => TailleUtilisée;
        public int EspaceDisponible => Données.Length - TailleUtilisée;

        static Composant()
        {
            IdsTypesSpéciaux = new();
            // parcourir TypesValeurSpéciales et ajouter les valeurs à IdsTypesSpéciaux en commencant après la dernière valeur de TypeCode
            foreach (TypesValeurSpéciales type in Enum.GetValues(typeof(TypesValeurSpéciales)))
            { IdsTypesSpéciaux.Add(type.ToString(), (int)type); }
        }

        public Composant(): this(0) {}
        public Composant(int taille_)
        {
            Données = new byte[taille_];
            TailleUtilisée = 0;
            IndexByteLibre = 0;
            Registre = new();
        }
        
        // TODO: éventuellement rajouter un constructeur qui prend un tableau de bytes et un registre ou une liste de types pour construire le registre
        
        public bool AjoutValeur<T>(string nom_, T valeur_)
        {
            int type = RécupTypeId<T>();
            int premierIdSpécial = Enum.GetValues(typeof(TypeCode)).Length;
            if (!Contient(type, nom_))
            {
                switch (type)
                {
                    case (int)TypeCode.Int32:
                        AjoutValeur(type, nom_, BitConverter.GetBytes((int)(object)valeur_));
                        break;
                    case (int)TypeCode.Single:
                        AjoutValeur(type, nom_, BitConverter.GetBytes((float)(object)valeur_));
                        break;
                    case (int)TypeCode.String:
                        AjoutValeur(type, nom_, Encoding.UTF8.GetBytes((valeur_ as string) ?? string.Empty));
                        break;
                    case (int)TypeCode.Boolean:
                        AjoutValeur(type, nom_, new[] { (byte)((bool)(object)valeur_ ? 1 : 0) });
                        break;
                    case (int)TypeCode.Char:
                        AjoutValeur(type, nom_, new[] { (byte)(char)(object)valeur_ });
                        break;
                    case (int)TypeCode.Byte:
                        AjoutValeur(type, nom_, new[] { (byte)(object)valeur_ });
                        break;
                    case (int)TypeCode.Double:
                        AjoutValeur(type, nom_, BitConverter.GetBytes((double)(object)valeur_));
                        break;
                    case (int)TypesValeurSpéciales.Vector2:
                        AjoutValeur(type, nom_, Conversions.Vector2VersBytes((Vector2)(object)valeur_));
                        break;
                    case (int)TypesValeurSpéciales.Vector3:
                        AjoutValeur(type, nom_, Conversions.Vector3VersBytes((Vector3)(object)valeur_));
                        break;
                    case (int)TypesValeurSpéciales.Vector4:
                        AjoutValeur(type, nom_, Conversions.Vector4VersBytes((Vector4)(object)valeur_));
                        break;
                    case (int)TypesValeurSpéciales.Vector2I:
                        AjoutValeur(type, nom_, Conversions.Vector2IVersBytes((Vector2I)(object)valeur_));
                        break;
                    case (int)TypesValeurSpéciales.Vector3I:
                        AjoutValeur(type, nom_, Conversions.Vector3IVersBytes((Vector3I)(object)valeur_));
                        break;
                    case (int)TypesValeurSpéciales.Vector4I:
                        AjoutValeur(type, nom_, Conversions.Vector4IVersBytes((Vector4I)(object)valeur_));
                        break;
                    //TODO: faire la suite, char, double, long, Vector2, Vector3, etc
                    default:
                        return false;
                    }
                return true;
            }
            return false;
        }

        // public void AjoutInt(string nom_, int valeur_)
        // { AjoutValeur(typeof(int), nom_, BitConverter.GetBytes(valeur_)); }
        //
        // public void AjoutFloat(string nom_, float valeur_)
        // { AjoutValeur(typeof(float), nom_, BitConverter.GetBytes(valeur_)); }
        //
        // public void AjoutString(string nom_, string valeur_)
        // { AjoutValeur(typeof(string), nom_, Encoding.UTF8.GetBytes(valeur_)); }
        

        private void AjoutValeur(int type_, string nom_, byte[] valeur_)
        {
            if (!Contient(type_, nom_))
            {
                int longueur = valeur_.Length;
                if (Données.Length - TailleUtilisée < longueur)
                { Agrandir(longueur - (Données.Length - TailleUtilisée)); }

                Index index = new(IndexByteLibre, longueur);
                AjoutDonnées(valeur_, index);
                AjoutIndex(type_, nom_, index);
                TailleUtilisée += longueur;
                IndexByteLibre += longueur;
            }
        }

        private void AjoutDonnées(byte[] données_, Index index_)
        { Array.Copy(Données, index_.Départ, données_, 0, index_.Longueur); }

        private void AjoutIndex(int type_, string nom_, Index index_)
        {
            if (!Registre.ContainsKey(type_))
            { Registre.Add(type_, new());}
            Registre[type_].Add(nom_, index_);
        }

        private bool Contient(int type_, string nom_)
        { return Registre.ContainsKey(type_) && Registre[type_].ContainsKey(nom_); }

        private void Agrandir(int tailleSuplémentaire_)
        {
            if (tailleSuplémentaire_ > 0)
            {
                byte[] nouveauTableau = new byte[Données.Length + tailleSuplémentaire_];
                Array.Copy(Données, nouveauTableau, Données.Length);
                Données = nouveauTableau;
            }
        }

        public bool Redimensionner(int nouvelleTaille_)
        {
            if (nouvelleTaille_ < TailleUtilisée)
            { return false; }
            
            Agrandir(nouvelleTaille_ - TailleUtilisée);
            return true;
        }
        
        public T RecupValeur<T>(string nom_)
        {
            int type = RécupTypeId<T>();
            if (Contient(type, nom_))
            {
                byte[] données = RécupBytes(Registre[type][nom_]);
                
                switch (type)
                {
                    case (int)TypeCode.Int32:
                        return (T)(object)BitConverter.ToInt32(données, 0);
                    case (int)TypeCode.Single:
                        return (T)(object)BitConverter.ToSingle(données, 0);
                    case (int)TypeCode.String:
                        return (T)(object)BitConverter.ToString(données, 0);
                    case (int)TypeCode.Boolean:
                        return (T)(object)BitConverter.ToBoolean(données, 0);
                    case (int)TypeCode.Char:
                        return (T)(object)BitConverter.ToChar(données, 0);
                    case (int)TypeCode.Byte:
                        return (T)(object)données[0];
                    case (int)TypeCode.Double:
                        return (T)(object)BitConverter.ToDouble(données, 0);
                    case (int)TypesValeurSpéciales.Vector2:
                        return (T)(object)Conversions.BytesVersVector2(données);
                    case (int)TypesValeurSpéciales.Vector3:
                        return (T)(object)Conversions.BytesVersVector3(données);
                    case (int)TypesValeurSpéciales.Vector4:
                        return (T)(object)Conversions.BytesVersVector4(données);
                    case (int)TypesValeurSpéciales.Vector2I:
                        return (T)(object)Conversions.BytesVersVector2I(données);
                    case (int)TypesValeurSpéciales.Vector3I:
                        return (T)(object)Conversions.BytesVersVector3I(données);
                    case (int)TypesValeurSpéciales.Vector4I:
                        return (T)(object)Conversions.BytesVersVector4I(données);
                }
            }
            throw new ArgumentException($"le type [{typeof(T)}] ou le nom [{nom_}] demandés ne sont pas valides");
        }

        // public (bool Existe, int Valeur) RécupInt(string nom_)
        // { return Contient(RécupTypeId<int>(), nom_) ? (true, BitConverter.ToInt32(RécupBytes(Registre[RécupTypeId<int>()][nom_]), 0)) : (false, default); }
        //     
        // public (bool Existe, float Valeur) RécupFloat(string nom_)
        // { return Contient(RécupTypeId<float>(), nom_) ? (true, BitConverter.ToSingle(RécupBytes(Registre[RécupTypeId<float>()][nom_]), 0)) : (false, default); }
        //     
        // public (bool Existe, string Valeur) RécupString(string nom_)
        // { return Contient(RécupTypeId<string>(), nom_) ? (true, BitConverter.ToString(RécupBytes(Registre[RécupTypeId<string>()][nom_]), 0)) : (false, default); }
        //     
        // // TODO: faire la suite, char, double, long, Vector2, Vector3, etc

        private byte[] RécupBytes(Index index_)
        {
            byte[] valeur = new byte[index_.Longueur];
            System.Array.Copy(Données, index_.Départ, valeur, 0, index_.Longueur);
            return valeur;
        }

        private int RécupTypeId<T>()
        {
            TypeCode typeSystème = Type.GetTypeCode(typeof(T));

            if (typeSystème != TypeCode.Object)
            { return (int)typeSystème; }
            
            if (IdsTypesSpéciaux.TryGetValue(typeof(T).ToString(), out int idType))
            { return idType; }
            
            { throw new ArgumentException($"le type [{typeof(T)}] fournit n'est pas valide comme valeur d'un Composant"); }
        }
    }
    
    //public enum TypesComposants { Nom, Masse,  }
    
    // Crée l'élément unique à l'initialisation
    private static GestComposants Singleton = new();
    private static readonly object Cadenas = new object();
    public static GestComposants Instance => Singleton;

    private Dictionary<string, Dictionary<int, Composant>> Composants;

    // Explicit static constructor to tell C# compiler
    // not to mark type as beforefieldinit
    static GestComposants() {}

    private GestComposants()
    {
        Composants = new();
    }

    public static (bool Réussite, Composant Composant) Ajout(string type_, int id_, Composant composant_)
    {
        // les méthodes du Singleton doivent se lock pour s'assurer de ne pas être appelées simultanément à plusieurs emplacements.
        lock (Cadenas)
        {
            if (Instance.Composants.TryGetValue(type_, out Dictionary<int, Composant> dictComps))
            {
                if (dictComps.TryGetValue(id_, out Composant composant))
                { return (false, composant); }
                
                dictComps.Add(id_, composant_);
                return (true, composant_);
            }
            
            Instance.Composants.Add(type_, new Dictionary<int, Composant> { { id_, composant_ } });
            return (true, composant_);
        }
    }

    public static Composant Récup(string type_, int id_)
    {
        lock (Cadenas)
        {
            if (Instance.Composants.TryGetValue(type_, out Dictionary<int, Composant> dictComps))
            {
                if (dictComps.TryGetValue(id_, out Composant composant))
                {
                    return composant;
                }
            }

            return null;
        }
    }

    public static bool Contient(string nom_, int id_)
    { return Instance.Composants.ContainsKey(nom_) && Instance.Composants[nom_].ContainsKey(id_); }

    /// <summary>
    /// Combine 3 id de 16, 8 et 8 bits en un seul sur 32 bits
    /// </summary>
    /// <param name="id_">entre </param>
    /// <param name="sousId1_"></param>
    /// <param name="sousId2_"></param>
    /// <returns></returns>
    public static int CréerId(int id_, int sousId1_, int sousId2_)
    {
        if (id_ < 0 || id_ > 0xFFFF)
            throw new ArgumentOutOfRangeException(nameof(id_), "Value must fit within 16 bits.");
        if (sousId1_ < 0 || sousId1_ > 0xFF)
            throw new ArgumentOutOfRangeException(nameof(sousId1_), "Value must fit within 8 bits.");
        if (sousId2_ < 0 || sousId2_ > 0xFF)
            throw new ArgumentOutOfRangeException(nameof(sousId2_), "Value must fit within 8 bits.");

        int IdFinal = (ushort)id_;
        IdFinal |= (byte)sousId1_ << 16;
        IdFinal |= (byte)sousId2_ << 24;
        return IdFinal;
    }
}