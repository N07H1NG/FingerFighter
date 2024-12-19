using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;

public class SplitScreenAudioSrcManager : MonoBehaviour
{
    public static SplitScreenAudioSrcManager singleton { get; private set; }
    [SerializeField] NetworkManager m_NetworkManager;

    List<GameObject> Players = new List<GameObject>();

    public Vector3[] locations;
    
    // Start is called before the first frame update
    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {

    }
    void Start()
    {
        if (SplitScreenAudioSrcManager.singleton == null){
            singleton = this;
        }
        

    }
    
    

    // Update is called once per frame
    void Update()
    {
        if (Players.Count>=1){
            Vector3 fw = Vector3.zero;
            for (int i = 0;i<Players.Count;i++){
                locations[i] = Players[i].transform.position;
                fw += Players[i].transform.forward;
            }
            transform.forward = fw/Players.Count;
        }
    }

    public void RecountPlayer(){
        Players.Clear();
        foreach (NetworkClient clnt in m_NetworkManager.ConnectedClientsList){
            Players.Add(clnt.PlayerObject.gameObject.GetComponentInChildren<MyPlayer>().gameObject);
        }
        locations = new Vector3[Players.Count];
    }
}
