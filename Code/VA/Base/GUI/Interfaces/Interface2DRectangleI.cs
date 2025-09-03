using Godot;

using VA.Base.Maths.Géom;

namespace VA.Base.GUI;

public partial class Interface2DRectangleI : Control
{
    private RectangleI Rectangle;
    private Rect2 Cadre;
    private bool CadreVisible { get; set; }
    private Vector2 _TailleUnité;
    private Color _CouleurFond, _CouleurCadre;
    private Texture2D _Texture;

    public Vector2 TailleUnité
    {
        get => _TailleUnité;
        set
        {
            _TailleUnité = value;
            MajTaille();
        }
    }
    
    public Color CouleurFond
    {
        get => _CouleurFond;
        set
        {
            _CouleurFond = value;
            QueueRedraw();
        }
    }
    
    public Color CouleurCadre
    {
        get => _CouleurCadre;
        set
        {
            _CouleurCadre = value;
            QueueRedraw();
        }
    }
    
    public Texture2D Texture
    {
        get => _Texture;
        set
        {
            _Texture = value;
            QueueRedraw();
        }
    }
    
    // TODO: revoir ce code
    
    // EVENT //
    // public delegate void Clic(InfosClic infos_);
    // public event Clic ClicGauche;
    // public event Clic ClicDroit;
    //
    // public delegate void RelacherClic(InfosClic infos_);
    // public event RelacherClic RelacherClicGauche;
    // public event RelacherClic RelacherClicDroit;
    //
    // public delegate void Glisser(InfosCurseur infos_, Vector2 déplacement_);
    // public event Glisser GlisserClicGauche;
    // public event Glisser GlisserClicDroit;

    public delegate void Défilement(Vector2 direction_);
    public event Défilement Molette;

    public delegate void NouvelleCaseCiblée(Vector2I case_);
    public event NouvelleCaseCiblée CiblerCase;

    public Interface2DRectangleI()
    {
        Rectangle = null;
        _TailleUnité = Vector2.One;
        CustomMinimumSize = _TailleUnité;
        Cadre = new Rect2(Vector2.Zero, Size);
        _CouleurFond = Colors.Fuchsia;
        _CouleurCadre = Colors.Black;
        _Texture = null;
        CadreVisible = false;

        GuiInput += Entrées;
    }

    ~Interface2DRectangleI()
    { QueueFree(); }

    public void Init(RectangleI rectangleI_)
    {
        Rectangle = rectangleI_;
        MajTaille();
    }

    private void MajTaille()
    {
        if (Rectangle != null)
        {
            CustomMinimumSize = new Vector2(Rectangle.Taille.X * TailleUnité.X, Rectangle.Taille.Y * _TailleUnité.Y);
            Position = Rectangle.GaucheHaut * _TailleUnité;
            MajCadre();
        }
    }
    
    private void MajCadre()
    {
        Cadre = new Rect2(Vector2.Zero, Size);
        QueueRedraw();
    }

    public override void _Draw()
    {
        if (_Texture != null)
        { DrawTextureRect(_Texture, Cadre, false); }
        else
        { DrawRect(Cadre, _CouleurFond); }
        
        if (CadreVisible)
        { DrawRect(Cadre, _CouleurCadre, false, 2f); }
    }

    private void Entrées(InputEvent event_)
    {
        
    }
}