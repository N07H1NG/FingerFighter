using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Mathematics;

public class GlobalScore : MonoBehaviour
{
    public Dictionary<ulong,int> score =  new Dictionary<ulong, int>();
    public List<ulong> order = new List<ulong>();
    Color[] colors = new Color[4];
    [SerializeField] public TMP_Text[] text; 

    public int done = 0;
    bool risen = false;

    // Start is called before the first frame update


    // Update is called once per frame
    public void PlayerConnect(int playerCount,ulong ClientID,Color playerColor){
        order.Add(ClientID);
        score[ClientID] = 0;
        Color.RGBToHSV(playerColor,out float hue,out float sat,out float val);
        Color outlinecol = Color.HSVToRGB(hue,1-math.pow(1-sat,3),val+0.4f*math.sign(0.5f-val));
        
        if (playerCount-1==0){
            
            
            text[0].fontSharedMaterial.SetColor("_FaceColor",playerColor);
            text[0].fontSharedMaterial.SetColor("_OutlineColor",outlinecol);
            colors[0] = playerColor;
        }
        else if (playerCount-1==1){
            
            text[1].fontSharedMaterial.SetColor("_FaceColor",playerColor);
            text[1].fontSharedMaterial.SetColor("_OutlineColor",outlinecol);
            colors[1] = playerColor;
        }
    }

    public IEnumerator Rise(){
        Vector3 target = transform.position;
        target.y = 85f;
        Vector3 speed = Vector3.zero;
        while(transform.position.y<80f){
            transform.position = Vector3.SmoothDamp(transform.position,target,ref speed,10f);
            yield return null;
        }
        risen = true;
    }

    public bool InPlace(){
        return risen;
    }
}
