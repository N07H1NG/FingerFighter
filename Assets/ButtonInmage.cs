using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonInmage : MonoBehaviour
{
    [SerializeField] Sprite[] soundSprites;
    [SerializeField] AudioSource src;
    bool on = true;

    public void ChangeSprite(){
        on = !on;
        src.mute = !on;
        transform.GetChild(0).GetComponent<Image>().sprite = soundSprites[on?0:1];
    }
}
