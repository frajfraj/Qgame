﻿using Microsoft.Xna.Framework;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;

class Agent:BaseAgent {
    [STAThread]
    static void Main() {
        Program.Start(new Agent());
    }
    public Agent() { }
    public override Drag SökNästaDrag(SpelBräde bräde) {
        //Så ni kan kolla om systemet fungerar!
        Spelare jag = bräde.spelare[0];
        Spelare motståndare = bräde.spelare[1];

        Drag drag = new Drag();

        List<Point> möjligamål = new List<Point>();

        for (int i = 0; i < SpelBräde.N; i++)
        {
            möjligamål.Add(new Point(i, 8));
        }

        List<Point> motståndaremål = new List<Point>();

        for(int i = 0; i < SpelBräde.N; i++)
        {
            motståndaremål.Add((new Point(i, 0)));
        }

        List<Point> väg = BFS(bräde, jag.position, möjligamål);
        List<Point> motståndareVäg = BFS(bräde, motståndare.position, motståndaremål);

        if (motståndareVäg.Count < väg.Count && motståndareVäg.Count > 1)
        {
            Point väggPosition = motståndareVäg[0];

            if (motståndare.position.X == väggPosition.X)
            {
                if (!bräde.horisontellaVäggar[väggPosition.X, väggPosition.Y] &&
                !bräde.horisontellaVäggar[väggPosition.X, väggPosition.Y + 1])
                {
                    drag.typ = Typ.Horisontell;
                    drag.point = väggPosition;
                }
                
            }
            else
            {
                if (!bräde.vertikalaVäggar[väggPosition.X, väggPosition.Y] &&
                !bräde.vertikalaVäggar[väggPosition.X + 1, väggPosition.Y])
                {
                    drag.typ = Typ.Vertikal;
                    drag.point = väggPosition;
                }
                
            }

           
        }

        else if (väg.Count > 0)
        {
            Point nästaSteg = väg[0];
            drag.typ = Typ.Flytta;
            drag.point = nästaSteg;
        }
        else 
        { 
            System.Diagnostics.Debugger.Break();
        }

        return drag;
    }
    
    public override Drag GörOmDrag(SpelBräde bräde, Drag drag) {
        //Om draget ni försökte göra var felaktigt så kommer ni hit
        System.Diagnostics.Debugger.Break();    //Brytpunkt
        return SökNästaDrag(bräde);
    }

    private List<Point> BFS(SpelBräde bräde, Point start, List<Point> mål)
    {
        Queue<Point> kö = new Queue<Point>();
        Dictionary<Point, Point> föregångare = new Dictionary<Point, Point>();
        HashSet<Point> besökta = new HashSet<Point>();

        kö.Enqueue(start);
        besökta.Add(start);

        while (kö.Count > 0) 
        {
            Point nuvarande = kö.Dequeue();

            if (mål.Contains(nuvarande)) 
            { 
                return ÅterskapaSökVäg(föregångare, start, nuvarande);
            }

            foreach (Point granne in HittaGiltigaDrag(bräde, nuvarande)) 
            { 
                if (!besökta.Contains(granne))
                {
                    kö.Enqueue(granne);
                    besökta.Add(granne);
                    föregångare[granne] = nuvarande;
                }
            }
        }

        return new List<Point>(); //ingen väg hittad
    }

    private IEnumerable<Point> HittaGiltigaDrag(SpelBräde bräde, Point position)
    {
        List<Point> giltigaDrag = new List<Point>();
        Point[] riktningar = { new Point(0, 1), new Point(0, -1), new Point(1, 0), new Point(-1, 0) };

        foreach (var riktning in riktningar) 
        {
            Point nyPosition = new Point(position.X + riktning.X, position.Y + riktning.Y);

            if(ärGiltigtDrag(bräde, position, nyPosition))
            {
                giltigaDrag.Add(nyPosition);
            }
        }
        return giltigaDrag;
    }

    private bool ärGiltigtDrag(SpelBräde bräde, Point nuvarande, Point nästa)
    {
        if (nästa.X < 0 || nästa.X >= SpelBräde.N || nästa.Y < 0 || nästa.Y >= SpelBräde.N) return false;

        if (nuvarande.X == nästa.X)
        {
            if (nuvarande.Y < nästa.Y)
            {
                return !bräde.horisontellaVäggar[nuvarande.X, nuvarande.Y];
            }
            else
            {
                return !bräde.horisontellaVäggar[nästa.X, nästa.Y];
            }
        }
        else
        {
            if (nuvarande.X < nästa.X)
            {
                return !bräde.vertikalaVäggar[nuvarande.X, nuvarande.Y];
            }
            else
            {
                return !bräde.vertikalaVäggar[nästa.X, nästa.Y];
            }
        }
    }

    private void PlaceraVägg(SpelBräde bräde, Point position, bool ärHorisontell)
    {
        if (ärHorisontell)
        {
            if (!bräde.horisontellaVäggar[position.X, position.Y])
            {
                bräde.horisontellaVäggar[position.X, position.Y] = true;
            }
        }
        else
        {
            if (!bräde.vertikalaVäggar[position.X, position.Y])
            {
                bräde.vertikalaVäggar[position.X, position.Y] = true;
            }
        }
    }

    private List<Point> ÅterskapaSökVäg(Dictionary<Point, Point> föregångare, Point start, Point mål)
    {
        List<Point> väg = new List<Point>();
        Point nuvarande = mål;

        while (!nuvarande.Equals(start)) 
        { 
            väg.Add(nuvarande);
            nuvarande = föregångare[nuvarande];
        }

        väg.Reverse();
        return väg;
    }
}
