using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;

public class SplitScreenAudioSrcManager : MonoBehaviour
{
    public static SplitScreenAudioSrcManager singleton { get; private set; }
    NetworkManager m_NetworkManager;

    List<GameObject> Players = new List<GameObject>();

    public Vector3 average_location;
    
    // Start is called before the first frame update
    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        m_NetworkManager = GetComponent<NetworkManager>();
    }
    void Start()
    {
        if (SplitScreenAudioSrcManager.singleton == null){
            singleton = this;
        }
        
        if (m_NetworkManager.IsServer){
            foreach (NetworkClient clnt in m_NetworkManager.ConnectedClientsList){
                Players.Add(clnt.PlayerObject.gameObject);
            }
        }
    }
    
    

    // Update is called once per frame
    void Update()
    {
        average_location = Vector3.zero;
        foreach (GameObject plr in Players){
            average_location += plr.transform.position;
        }
    }

    public void RecountPlayer(){
        Players.Clear();
        foreach (NetworkClient clnt in m_NetworkManager.ConnectedClientsList){
            Players.Add(clnt.PlayerObject.gameObject.GetComponentInChildren<MyPlayer>().gameObject);
        }
    }
}
