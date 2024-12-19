using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;

public class Pond : MonoBehaviour
{
    Dictionary<ulong,int> score =  new Dictionary<ulong, int>();
    List<ulong> order = new List<ulong>();
    Color[] colors = new Color[4];
    [SerializeField]TMP_Text[] text; 
    [SerializeField]SkinnedMeshRenderer body;
    Vector3 correction = Vector3.zero;
    Vector3 speed = Vector3.zero;
    int maxScore = 0;
    bool draw = true;

    ulong favorite;

    SplitScreenAudioSrcManager audioManager;
    // Start is called before the first frame update
    void Start()
    {
        audioManager = SplitScreenAudioSrcManager.singleton;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (audioManager.locations.Length>=1){
            Vector3 dirtext = audioManager.locations[0]-transform.position;
            for(int i=1;i<audioManager.locations.Length;i++){
                if ((audioManager.locations[i]-transform.position).magnitude<dirtext.magnitude){
                    dirtext = audioManager.locations[i]-transform.position;
                }
            }
            foreach(TMP_Text tComp in text){
                tComp.transform.forward = dirtext;
            }
        }
    }

    /// <summary>
    /// OnTriggerEnter is called when the Collider other enters the trigger.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    void OnTriggerEnter(Collider other)
    {
        Fish f;
        if (other.gameObject.TryGetComponent<Fish>(out f)){
            if (f.touched){
                
                ChangeScore(f.LastGuyID,1);
            }
            Destroy(other.gameObject);
        }

    }

    void ChangeScore(ulong who,int howmuch){
        score[who]+=howmuch;
        text[order.IndexOf(who)].text = score[who].ToString();
        if (score[who]>maxScore)
        {
            maxScore = score[who];
            favorite = who;
            draw = false;
        }
        else{
            maxScore = score[who];
            favorite = who;
            draw = false;
            foreach(ulong guy in order){
                if(score[guy]>maxScore){
                    maxScore = score[guy];
                    favorite  =guy;
                    draw = false;
                }else if(score[guy]==maxScore){
                    draw = true;
                }
            }
            
        }
        if (draw){
                SetColor(Color.white);
        }else{
            SetColor(colors[order.IndexOf(favorite)]);
        }
    }


    public void SetColor(Color newColor){
        body.material.SetColor("_Outline", newColor);
    }

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

    public void TimeOver(){
        GetComponent<Collider>().enabled = false;
    }


}
