using Microsoft.Xna.Framework;
using System;

class Agent:BaseAgent {
    [STAThread]
    static void Main() {
        Program.Start(new Agent());
    }
    public Agent() { }
    public override Drag SökNästaDrag(SpelBräde bräde) {
        //Så ni kan kolla om systemet fungerar!
        Spelare jag = bräde.spelare[0];
        Point playerPos = jag.position;
        Drag drag = new Drag();
        if (jag.position.Y < 4) {
            drag.typ = Typ.Flytta;
            drag.point = playerPos;
            drag.point.Y++;
        } else if (jag.antalVäggar > 7) {

            drag.typ = Typ.Horisontell;
            drag.point = new Point(23-jag.antalVäggar*2, 2);
        } else {
            drag.typ = Typ.Flytta;
            drag.point = playerPos;
            drag.point.Y++;
        }
        return drag;
    }
    public override Drag GörOmDrag(SpelBräde bräde, Drag drag) {
        //Om draget ni försökte göra var felaktigt så kommer ni hit
        System.Diagnostics.Debugger.Break();    //Brytpunkt
        return SökNästaDrag(bräde);
    }
}
//enum Typ { Flytta, Horisontell, Vertikal }
//struct Drag
//{
//    public Typ typ;
//    public Point point;
//}