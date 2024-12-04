using System;
using UnityEngine;
using Unity.Collections;
using Unity.Netcode;
using System.Linq;

public class PlayerDisambigulation : NetworkBehaviour
{
    // Start is called before the first frame update
    public override void OnNetworkSpawn(){
        if(IsClient){
            foreach(Transform child in transform){
                Destroy(child.gameObject);
            }
        }else{
            GetComponentInChildren<MyPlayer>().enabled = true;
            float ind = Array.IndexOf(NetworkManager.ConnectedClientsIds.ToArray(),OwnerClientId);
            print(NetworkManager.ConnectedClientsIds+ " " + OwnerClientId);
            GetComponentInChildren<Camera>().rect = new Rect(ind/2f,0,0.5f,1);
        }
    }
}
