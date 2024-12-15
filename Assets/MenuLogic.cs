using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Bson;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class MenuLogic : MonoBehaviour
{
    GameObject bg;
    GameObject serverSetup;
    GameObject clientMenu;
    GameObject serverSide;
    [SerializeField] TMP_Text swapText;
    RectTransform serverSideRect;
    [SerializeField] NetworkMenuHandler m_NetworkMenuHandler;
    [SerializeField] NetworkManager m_NetworkManger;
    // Start is called before the first frame update
    void Awake(){
        bg = transform.GetChild(0).gameObject;
        serverSetup = transform.GetChild(1).gameObject;
        clientMenu = transform.GetChild(2).gameObject;
        serverSide = transform.GetChild(3).gameObject;
        serverSideRect = serverSide.GetComponent<RectTransform>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    
    public void ShowServer(){
        serverSetup.SetActive(false);
        clientMenu.SetActive(false);
        serverSide.SetActive(true);
        PositionServerSideMenu();
    }

    public void ShowClient(){
        serverSetup.SetActive(false);
        clientMenu.SetActive(false);
    }

    public void PositionServerSideMenu(){
        int count = m_NetworkManger.ConnectedClients.Count;
        if (m_NetworkMenuHandler.splitOrientation){
            swapText.text = "layout:\nhorizontal";
        }
        else{
            swapText.text = "layout:\nvertical";
        }
        if (count==0){
            //serverSideRect.offsetMax = Vector2.zero;
            //serverSideRect.offsetMax = Vector2.zero;
            serverSideRect.anchorMax = new Vector2(0.72f,0.72f);
            serverSideRect.anchorMin = new Vector2(0.28f,0.28f);
            serverSideRect.anchoredPosition = Vector2.zero;
        }
        else if (count==1){
            if (m_NetworkMenuHandler.splitOrientation){
                serverSideRect.anchorMax = new Vector2(0.97f,0.72f);
                serverSideRect.anchorMin = new Vector2(0.53f,0.28f);
                
                //serverSideRect.anchoredPosition = new Vector2(-1*Screen.width/4f,0);
            }else{
                serverSideRect.anchorMax = new Vector2(0.72f,0.97f);
                serverSideRect.anchorMin = new Vector2(0.28f,0.53f);
                
                //serverSideRect.anchoredPosition = new Vector2(0,-1*Screen.height/4f);
            }
        }
    }

    public void ShowGeneral(){
        serverSetup.SetActive(true);
        clientMenu.SetActive(true);
        serverSide.SetActive(false);
    }
}
