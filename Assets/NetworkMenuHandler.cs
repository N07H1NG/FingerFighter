using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;
using System.Net;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using Object = UnityEngine.Object;
using TMPro;


#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Events;
#endif

public class NetworkMenuHandler : MonoBehaviour
{
    [SerializeField]
    ExampleNetworkDiscovery m_Discovery;
    
    NetworkManager m_NetworkManager;
    [SerializeField] TMP_Dropdown ServerDropdown;

    List<IPAddress> addresses = new List<IPAddress>();
    Dictionary<IPAddress, DiscoveryResponseData> discoveredServers = new Dictionary<IPAddress, DiscoveryResponseData>();
    // Start is called before the first frame update
    void Awake()
    {
        m_Discovery = GetComponent<ExampleNetworkDiscovery>();
        m_NetworkManager = GetComponent<NetworkManager>();
    }

    #if UNITY_EDITOR
    void OnValidate()
    {
        if (m_Discovery == null) // This will only happen once because m_Discovery is a serialize field
        {
            m_Discovery = GetComponent<ExampleNetworkDiscovery>();
            UnityEventTools.AddPersistentListener(m_Discovery.OnServerFound, OnServerFound);
            Undo.RecordObjects(new Object[] { this, m_Discovery}, "Set NetworkDiscovery");
        }
    }
    #endif
    void Start()
    {
        ServerDropdown.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnServerFound(IPEndPoint sender, DiscoveryResponseData response)
    {
        ServerDropdown.gameObject.SetActive(true);
        print("found");
        discoveredServers[sender.Address] = response;
        addresses.Add(sender.Address);
        ServerDropdown.ClearOptions();
        List<string> srvrList = new List<string>();
        foreach(IPAddress address in addresses){
            
            
            srvrList.Add($"{discoveredServers[address].ServerName}[{address.ToString()}]");
        }
        ServerDropdown.AddOptions(srvrList);

    }

    public void StopDiscovery(){
        ServerDropdown.gameObject.SetActive(false);
        m_Discovery.StopDiscovery();
        discoveredServers.Clear();
        addresses.Clear();
        ServerDropdown.ClearOptions();
    }

    public void Refresh(){
        ServerDropdown.gameObject.SetActive(false);
        discoveredServers.Clear();
        addresses.Clear();
        ServerDropdown.ClearOptions();
        m_Discovery.ClientBroadcast(new DiscoveryBroadcastData());
    }

    public void Discover(){
        ServerDropdown.gameObject.SetActive(false);
        discoveredServers.Clear();
        addresses.Clear();
        ServerDropdown.ClearOptions();
        m_Discovery.StartClient();
        m_Discovery.ClientBroadcast(new DiscoveryBroadcastData());
        Debug.Log("Discovery Started");
    }

    public void Connect(){
        if (addresses.Count>0){
            DiscoveryResponseData trg = discoveredServers[addresses[ServerDropdown.value]];
            UnityTransport transport = (UnityTransport)m_NetworkManager.NetworkConfig.NetworkTransport;
            transport.SetConnectionData(addresses[ServerDropdown.value].ToString(), trg.Port);
            m_NetworkManager.StartClient();
        }
        
        
    }

    public void StartServer(){
        m_NetworkManager.StartServer();
        m_Discovery.StartServer();
    }
}
