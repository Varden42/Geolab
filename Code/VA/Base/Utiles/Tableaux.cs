using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Godot;

using VA.Base.Maths;

namespace VA.Base.Utiles;

public static class Tableaux
    {
        // fournit les fonctions nécessaire au Tri d'un Objet par sa position
        public interface IPos3DTriableF
        { Vector3 Position { get; } }

        public interface IPos3DTriableI
        { Vector3I Position { get; } }

        public interface IPos1DTriableI
        { int Index { get; } }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="objets_"></param>
        /// <param name="cible_"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static (int index, bool existant) TrouveIndexPos3DF<T>(List<T> objets_, Vector3 cible_) where T : IPos3DTriableF
        {
            float tolerance = 0.001f;
            //Debug.Log("############################## TrouveIndexPos3DF ##############################");
            if (objets_.Count > 0)
            {
                // Définit le début et la fin de la plage.
                int debutPlageTest = 0, finPlageTest = objets_.Count - 1;

                while (finPlageTest > debutPlageTest)
                {
                    // calcule l'index situé au milieu de la plage
                    int index = ((finPlageTest - debutPlageTest) / 2) + debutPlageTest;

                    // si l'objet situé au milieu de la plage est égal à la cible, on le renvois,
                    // sinon on regarde si on est inférieur ou supérieur, afin de savoir dans quel sens diriger la recherche.
                    //Debug.Log("CompPos3DEgal(" + objets_[index].RecupPosition() + ", " + cible_ + ") => " + FoncUtiles.CompPos3DEgal(objets_[index].RecupPosition(), cible_));
                    if (Algèbre.CompVec3DFEgal(objets_[index].Position, cible_, tolerance))
                    { return (index, true); }
                    else
                    {
                        // on décale le début ou la fin de la plage en fonction.
                        if (Algèbre.CompVec3DFInf(objets_[index].Position, cible_))
                        { debutPlageTest = index + 1; }
                        else
                        { finPlageTest = index - 1; }
                    }
                }

                //Debug.Log("CompPos3DEgal(" + objets_[debutPlageTest].RecupPosition() + ", " + cible_ + ") => " + FoncUtiles.CompPos3DEgal(objets_[debutPlageTest].RecupPosition(), cible_));
                if (Algèbre.CompVec3DFEgal(objets_[debutPlageTest].Position, cible_, tolerance))
                { return (debutPlageTest, true); }
                else
                { return (Algèbre.CompVec3DFInf(objets_[debutPlageTest].Position, cible_, tolerance) ? debutPlageTest + 1 : debutPlageTest, false); }
            }
            else
            { return (0, false); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="objets_"></param>
        /// <param name="cible_"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static (int index, bool existant) TrouveIndexPos3DI<T>(List<T> objets_, Vector3I cible_) where T : IPos3DTriableI
        {
            //Debug.Log("############################## TrouveIndexPos3DI ##############################");
            if (objets_.Count > 0)
            {
                // Définit le début et la fin de la plage.
                int debutPlageTest = 0, finPlageTest = objets_.Count - 1;

                while (finPlageTest > debutPlageTest)
                {
                    // calcule l'index situé au milieu de la plage
                    int index = ((finPlageTest - debutPlageTest) / 2) + debutPlageTest;

                    // si l'objet situé au milieu de la plage est égal à la cible, on le renvois,
                    // sinon on regarde si on est inférieur ou supérieur, afin de savoir dans quel sens diriger la recherche.
                    //Debug.Log($"objets_[index].Position == cible_ => {objets_[index].Position} == {cible_}) => {objets_[index].Position == cible_}");
                    if (objets_[index].Position == cible_)
                    { return (index, true); }
                    else
                    {
                        // on décale le début ou la fin de la plage en fonction.
                        if (Algèbre.CompVec3DIInf(objets_[index].Position, cible_))
                        { debutPlageTest = index + 1; }
                        else
                        { finPlageTest = index - 1; }
                    }
                }

                //Debug.Log($"objets_[debutPlageTest].RecupPosition() == cible_ => {objets_[debutPlageTest].Position} == {cible_} => {objets_[debutPlageTest].Position == cible_}");
                if (objets_[debutPlageTest].Position == cible_)
                { return (debutPlageTest, true); }
                else
                { return (Algèbre.CompVec3DIInf(objets_[debutPlageTest].Position, cible_) ? debutPlageTest + 1 : debutPlageTest, false); }
            }
            else
            { return (0, false); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="objets_"></param>
        /// <param name="cible_"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static (int index, bool existant) TrouverIndexPos1DI<T>(List<T> objets_, int cible_) where T : IPos1DTriableI
        {
            //Debug.Log("############################## TrouveIndexPos3DI ##############################");
            if (objets_.Count > 0)
            {
                // Définit le début et la fin de la plage.
                int debutPlageTest = 0, finPlageTest = objets_.Count - 1;

                while (finPlageTest > debutPlageTest)
                {
                    // calcule l'index situé au milieu de la plage
                    int index = ((finPlageTest - debutPlageTest) / 2) + debutPlageTest;

                    // si l'objet situé au milieu de la plage est égal à la cible, on le renvois,
                    // sinon on regarde si on est inférieur ou supérieur, afin de savoir dans quel sens diriger la recherche.
                    //Debug.Log("CompPos3DEgal(" + objets_[index].RecupPosition() + ", " + cible_ + ") => " + FoncUtiles.CompPos3DEgal(objets_[index].RecupPosition(), cible_));
                    if (objets_[index].Index == cible_)
                    { return (index, true); }
                    else
                    {
                        // on décale le début ou la fin de la plage en fonction.
                        if (objets_[index].Index < cible_)
                        { debutPlageTest = index + 1; }
                        else
                        { finPlageTest = index - 1; }
                    }
                }

                //Debug.Log("CompPos3DEgal(" + objets_[debutPlageTest].RecupPosition() + ", " + cible_ + ") => " + FoncUtiles.CompPos3DEgal(objets_[debutPlageTest].RecupPosition(), cible_));
                if (objets_[debutPlageTest].Index == cible_)
                { return (debutPlageTest, true); }
                else
                { return ((objets_[debutPlageTest].Index < cible_) ? debutPlageTest + 1 : debutPlageTest, false); }
            }
            else
            { return (0, false); }
        }

        /// <summary>
        /// Convertit une position 2D en position 1D ou x sont les colonnes et y les lignes
        /// </summary>
        /// <param name="x_">l'index de la colonne</param>
        /// <param name="y_">l'index de la ligne</param>
        /// <param name="largeur_">la largeur du tableau</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Index2DVers1D(int x_, int y_, int largeur_)
        { return y_ * largeur_ + x_; }
        
        /// <summary>
        /// Convertit une position 2D en position 1D ou x sont les colonnes et y les lignes
        /// </summary>
        /// <param name="index_">l'index 2D</param>
        /// <param name="largeur_">la largeur du tableau</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Index2DVers1D(Vector2I index_, int largeur_)
        { return index_.Y * largeur_ + index_.X; }

        /// <summary>
        /// Convertir un index 1D en index 2D ou x sont les colonnes et y les lignes
        /// </summary>
        /// <param name="index_"></param>
        /// <param name="largeur_">la largeur du tableau 2D</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2I Index1Dvers2D(int index_, int largeur_)
        { return new Vector2I(index_ / largeur_, index_ % largeur_); }

        /// <summary>
        /// Convertit une Liste en String.
        /// </summary>
        /// <typeparam name="T">Le type d'éléments de contenus dans la List.</typeparam>
        /// <param name="liste_">La Liste à convertir.</param>
        /// <param name="retourLigne_">Si oui ou non il faut faire un retour à la ligne.</param>
        /// <param name="nbElem_">Le Nombre d'éléments de chaque lignes.</param>
        /// <returns></returns>
        public static string List_String<T>(List<T> liste_, bool retourLigne_, int nbElem_ = 0)
        {
            string listeStr = "";
            int compteur = 0;
            foreach (var val in liste_)
            {
                ++compteur;
                listeStr += "[" + Convert.ToString(val) + "]";
                if (retourLigne_)
                {
                    if (compteur >= nbElem_)
                    {
                        listeStr += "\n";
                        compteur = 0;
                    }
                }
            }
            return listeStr;
            // TODO: créer une surcharge qui saute à la ligne lorsqu'un caractère spécifique est rencontré
        }

        /// <summary>
        /// Swap deux objets au sein d'une Liste
        /// </summary>
        public static void EchangerObjets<T>(List<T> liste_, int indexA_, int indexB_)
        {
            T tampon = liste_[indexA_];
            liste_[indexA_] = liste_[indexB_];
            liste_[indexB_] = tampon;
        }
        
        /// <summary>
        /// Swap deux plages d'objets au sein d'une Liste
        /// </summary>
        public static void EchangerPlageObjets<T>(List<T> liste_, int indexA_, int indexB_, int quantité_)
        {
            // on vérifie que les plages ne se croisent pas
            if (indexA_ + quantité_ <= indexB_ || indexB_ + quantité_ <= indexA_)
            {
                List<T> tampon = liste_.GetRange(indexA_, quantité_);
                liste_.RemoveRange(indexA_, quantité_);

                int indexDestination = indexA_ < indexB_ ? indexB_ - quantité_ : indexB_;
                liste_.InsertRange(indexA_, liste_.GetRange(indexDestination, quantité_));
                liste_.RemoveRange(indexB_, quantité_);
                liste_.InsertRange(indexB_, tampon);
                
            }
        }

        /// <summary>
        /// Déplace un objet dans une liste
        /// </summary>
        public static void DéplacerObjet<T>(List<T> liste_, int indexOrigine_, int indexDestination_)
        {
            T tampon = liste_[indexOrigine_];
            liste_.RemoveAt(indexOrigine_);
            indexDestination_ = indexOrigine_ < indexDestination_ ? indexDestination_ - 1 : indexDestination_;
            liste_.Insert(indexDestination_, tampon);
        }

        /// <summary>
        /// Déplae une plage d'objets dans une liste
        /// </summary>
        public static void DéplacerPlageObjets<T>(List<T> liste_, int indexOrigine_, int quantité_, int indexDestination_)
        {
            List<T> tampon = liste_.GetRange(indexOrigine_, quantité_);
            liste_.RemoveRange(indexOrigine_, quantité_);
            indexDestination_ = indexOrigine_ < indexDestination_ ? indexDestination_ - quantité_ : indexDestination_;
            liste_.InsertRange(indexDestination_, tampon);
        }
        
        /// <summary>
        /// Découpe un tableau en plusieurs tableaux
        /// </summary>
        /// <param name="source_">lLe tableau d'origine</param>
        /// <param name="tailleDécoupe_">la taille voulue pour chaque sous tableaux (le dernier tableau pourra être plus petit)</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T[]> Découper<T>(this T[] source_, int tailleDécoupe_)
        {
            for (var i = 0; i < source_.Length; i += tailleDécoupe_)
            { yield return source_.Skip(i).Take(tailleDécoupe_).ToArray(); }
        }
    }