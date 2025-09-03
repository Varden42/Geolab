using System;
using Godot;
using VA.Base.GUI;
using VA.Base.Stockage;

namespace VA.Base.Systemes.Inventaires;

public class InventaireGrille<T>: IInterface2D
{
    private Grille<T> Stockage;
    private InterfaceGrille<T> Interface;

    public Control Interface2D => RécupInterface2D();

    private Control CréerInterface()
    { return new InterfaceGrille<T>(); }

    private Control RécupInterface2D()
    { throw new NotImplementedException(); }
    // { return Interface != null ? Interface.Interface2D : CréerInterface(); }

}