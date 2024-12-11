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
using UnityEngine.UI;



#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Events;
#endif

public class NetworkMenuHandler : MonoBehaviour
{

    public bool splitOrientation = true;
    
    [SerializeField]
    ExampleNetworkDiscovery m_Discovery;
    
    NetworkManager m_NetworkManager;
    [SerializeField] TMP_Dropdown ServerDropdown;
    [SerializeField] MenuLogic menu;

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


    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        m_NetworkManager.OnClientConnectedCallback += ClientConnectinsChanged;
        m_NetworkManager.OnClientDisconnectCallback += ClientConnectinsChanged;
        m_NetworkManager.OnClientStopped += ClientDisconnect;
    }

    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {
        m_NetworkManager.OnClientConnectedCallback -= ClientConnectinsChanged;
        m_NetworkManager.OnClientDisconnectCallback -= ClientConnectinsChanged;
        m_NetworkManager.OnClientStopped -= ClientDisconnect;
    }
    void Start()
    {
        ServerDropdown.gameObject.SetActive(false);
    }

    void OnServerFound(IPEndPoint sender, DiscoveryResponseData response)
    {
        ServerDropdown.gameObject.SetActive(true);
        
        discoveredServers[sender.Address] = response;
        addresses.Add(sender.Address);
        ServerDropdown.ClearOptions();
        List<string> srvrList = new List<string>();
        foreach(IPAddress address in addresses){
            
            
            srvrList.Add($"{discoveredServers[address].ServerName}\n{address.ToString()}");
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
        
    }

    public void Connect(){
        if (addresses.Count>0){
            DiscoveryResponseData trg = discoveredServers[addresses[ServerDropdown.value]];
            UnityTransport transport = (UnityTransport)m_NetworkManager.NetworkConfig.NetworkTransport;
            transport.SetConnectionData(addresses[ServerDropdown.value].ToString(), trg.Port);
            m_NetworkManager.OnClientStarted += OnClientStarted;
            m_NetworkManager.StartClient();
            
        }
        
        
    }

    public void StartServer(){
        m_NetworkManager.StartServer();
        m_Discovery.StartServer();
        menu.ShowServer();
    }
    
    public void SetServerName(string newName){
        m_Discovery.ServerName = newName;
    }


    void OnClientStarted(){
        menu.ShowClient();
        m_NetworkManager.OnClientStarted -= OnClientStarted;
    }

    void SetOrientation(bool newOrientation){
        if (splitOrientation!=newOrientation){
            splitOrientation = newOrientation;
            foreach(NetworkClient playerClient in m_NetworkManager.ConnectedClientsList){
                playerClient.PlayerObject.GetComponent<PlayerDisambigulation>().PositionCamera(splitOrientation);
            }
            menu.PositionServerSideMenu();
        }
    }

    void ClientConnectinsChanged(ulong clientID){
        if (m_NetworkManager.IsServer){
            menu.PositionServerSideMenu();
        }
    }

    public void SwapOrientation(){
        SetOrientation(!splitOrientation);
    }

    public void StopServer(){
        
        m_Discovery.StopDiscovery();
        m_NetworkManager.Shutdown();
        menu.ShowGeneral();
    }

    public void ClientDisconnect(bool host){
        menu.ShowGeneral();
    }
}
