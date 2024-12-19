using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct BublikResult 
{
    public LittleGuy guy;
    public ulong ID;
    public int score;
    public bool draw;

    public BublikResult(LittleGuy g,ulong favID,int scr, bool isDraw){
        guy = g;
        ID = favID;
        score = scr;
        draw = isDraw;

    }
}
