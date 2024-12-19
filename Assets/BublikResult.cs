using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct BublikResult 
{
    LittleGuy guy;
    ulong ID;
    int score;
    bool draw;

    public BublikResult(LittleGuy g,ulong favID,int scr, bool isDraw){
        guy = g;
        ID = favID;
        score = scr;
        draw = isDraw;

    }
}
