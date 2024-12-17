using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class Pond : MonoBehaviour
{
    Dictionary<ulong,int> score =  new Dictionary<ulong, int>();
    ulong[] order = new ulong[4];
    Color[] colors = new Color[4];
    [SerializeField]TMP_Text text; 
    [SerializeField]TMP_Text text2; 
    [SerializeField]SkinnedMeshRenderer body;
    Vector3 correction = Vector3.zero;
    Vector3 speed = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// OnTriggerEnter is called when the Collider other enters the trigger.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    void OnTriggerEnter(Collider other)
    {
        Fish f;
        if (other.gameObject.TryGetComponent<Fish>(out f)){
            score[f.LastGuyID]+=1;
            UpdateScore();
            Destroy(other.gameObject);
        }

    }

    void UpdateScore(){
        
        text.text = score[order[0]].ToString();
        text2.text = score[order[1]].ToString();
        if (score[order[0]]>score[order[1]]){
            SetColor(colors[0]);
        }else if(score[order[0]]<score[order[1]]){
            SetColor(colors[1]);
        }
        else{
            SetColor(Color.white);
        }
    }

    public void SetColor(Color newColor){
        body.material.SetColor("_Outline", newColor);
    }

    public void PlayerConnect(int playerCount,ulong ClientID,Color playerColor){
        order[playerCount-1]=ClientID;
        score[ClientID] = 0;
        
        if (playerCount-1==0){
            //text.fontSharedMaterial.SetColor("_FaceColor",playerColor);
            text.fontSharedMaterial.SetColor("_OutlineColor",playerColor);
            colors[0] = playerColor;
        }
        else if (playerCount-1==1){
            //text.fontSharedMaterial.SetColor("_FaceColor",playerColor);
            text2.fontSharedMaterial.SetColor("_OutlineColor",playerColor);
            colors[1] = playerColor;
        }
    }
}
