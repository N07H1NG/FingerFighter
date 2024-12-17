using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class Pond : MonoBehaviour
{
    int score = 0;
    [SerializeField] int pondNumber;
    [SerializeField]TMP_Text text; 
    MeshRenderer water;
    Vector3 correction = Vector3.zero;
    Vector3 speed = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        water = GetComponent<MeshRenderer>();
        UpdateScore(score);
    }

    // Update is called once per frame
    void Update()
    {
        //correction = Vector3.SmoothDamp(correction,Vector3.up-transform.forward,ref speed,0.2f);
        //transform.rotation = Quaternion.FromToRotation(transform.forward,transform.forward+12*Time.deltaTime*correction)*transform.rotation;
        //transform.up = transform.parent.up;
    }

    /// <summary>
    /// OnTriggerEnter is called when the Collider other enters the trigger.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Fish")){
            UpdateScore(score+1);
            Destroy(other.gameObject);
        }

    }

    void UpdateScore(int newScore){
        score = newScore;
        text.text = score.ToString();
    }

    public void SetColor(Color newColor){
        water.material.color = newColor;
    }

    public void PlayerConnect(int playerCount,ulong ClientID,Color playerColor){
        if(playerCount == pondNumber+1){
            SetColor(playerColor);
        }
    }
}
