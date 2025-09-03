using Godot;

namespace VA.Base.Outils.Conteneur;

// /// <summary>
// /// Un conteneur qui limite la taille de son enfant
// /// </summary>
// [Tool]
// public partial class ConteneurTailleMax: Container
// {
//     private Vector2 TailleMax_ = Vector2.Inf;
//     [Export]
//     public Vector2 TailleMax
//     {
//         get => TailleMax_;
//         set => Init(value);
//     }
//     
//     public ConteneurTailleMax()
//     {
//         Resized += ControlerTaille;
//     }
//
//     public void Init(Vector2 tailleMax_)
//     {
//         TailleMax_ = tailleMax_;
//         ControlerTaille();
//     }
//
//     private void ControlerTaille()
//     {
//         if (GetChildCount() == 1)
//         {
//             Vector2 position = new (Mathf.Max(0, GetRect().Size.X - TailleMax_.X) / 2f, Mathf.Max(0, GetRect().Size.Y - TailleMax_.Y) / 2f);
//             Vector2 taille = new (Mathf.Min(GetRect().Size.X, TailleMax_.X), Mathf.Min(GetRect().Size.Y, TailleMax_.Y));
//             // Vector2 taille = GetChild<Control>(0).Size;
//             
//             FitChildInRect(GetChild<Control>(0), new Rect2(position, taille));
//             GD.Print($"Controle de taille {position} - {taille}");
//         }
//     }
// }