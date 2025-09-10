using Godot;
using VA.Base.GUI.Outils.Barre;

namespace Geolab.Code.Tests;

public partial class TestsBarres: Node
{
    private BarreMultiDir Barre;
    public TestsBarres()
    {
        Barre = new(BarreMultiDir.EnumBord.Haut);
    }
    
    public override void _Ready()
    {
        AddChild(Barre);
    }
}