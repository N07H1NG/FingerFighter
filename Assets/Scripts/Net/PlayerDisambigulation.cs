using System;
using UnityEngine;
using Unity.Collections;
using Unity.Netcode;
using System.Linq;

public class PlayerDisambigulation : NetworkBehaviour
{
    int playerIndex;
    void Start(){
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }
    // Start is called before the first frame update
    public override void OnNetworkSpawn(){
        if(!IsServer){
            foreach(Transform child in transform){
                Destroy(child.gameObject);
            }
        }else{
            GetComponentInChildren<MyPlayer>().enabled = true;
            playerIndex = Array.IndexOf(NetworkManager.ConnectedClientsIds.ToArray(),OwnerClientId);
            
            PositionCamera(NetworkManager.GetComponent<NetworkMenuHandler>().splitOrientation);
        }
    }

    public void PositionCamera(bool hor){
        if (hor) GetComponentInChildren<Camera>().rect = new Rect(playerIndex/2f,0,0.5f,1);
        else GetComponentInChildren<Camera>().rect = new Rect(0,playerIndex/2f,1,0.5f);
    }
}
