using System.Diagnostics;
using Godot;

namespace VA.Base.Debug;

public static class Mesurer
{
    public delegate void MethodeAMesurer();

    public static long TempsExécutionMS(MethodeAMesurer methode_)
    {
        var chrono = new Stopwatch();
        chrono.Start();
        methode_();
        chrono.Stop();
        return chrono.ElapsedMilliseconds;
    }
}