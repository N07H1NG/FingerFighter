using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Net;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Object = UnityEngine.Object;
using TMPro;
using UnityEngine.SceneManagement;







#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Events;
#endif

public class NetworkMenuHandler : MonoBehaviour
{

    public GameEvent startGameEvent;
    public GameEvent playercountEvent;

    public PlayerConnectionEvent connectionEvent;
    public PlayerConnectionEvent disconnectEvent;
    public bool splitOrientation = true;
    
    [SerializeField]
    ExampleNetworkDiscovery m_Discovery;
    
    NetworkManager m_NetworkManager;
    [SerializeField] TMP_Dropdown ServerDropdown;
    [SerializeField] MenuLogic menu;

    [SerializeField] ColorPicker m_ColorPicker;

    int calibrated = 0;
    [SerializeField] int player_count = 2;

    public Dictionary<ulong,Color> playerColors = new Dictionary<ulong, Color>();
    List<IPAddress> addresses = new List<IPAddress>();
    Dictionary<IPAddress, DiscoveryResponseData> discoveredServers = new Dictionary<IPAddress, DiscoveryResponseData>();
    // Start is called before the first frame update

    void OnApplicationQuit()
    {
        m_NetworkManager.Shutdown();
    }

    public void Quit(){
        if (m_NetworkManager.IsServer){
            StopServer();

        }
        else if (m_NetworkManager.IsClient){
            m_Discovery.StopDiscovery();
            m_NetworkManager.Shutdown();
            menu.ShowGeneral();
        }
        else Application.Quit();
        
    }
    void Awake()
    {
        m_Discovery = GetComponent<ExampleNetworkDiscovery>();
        m_NetworkManager = GetComponent<NetworkManager>();
        m_NetworkManager.NetworkConfig.ConnectionApproval = true;
        
    }

    


    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        m_NetworkManager.OnClientConnectedCallback += ClientConnected;
        m_NetworkManager.OnClientDisconnectCallback += ClientDisconnect;
        m_NetworkManager.OnClientStopped += ClientStopped;
    }

    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {
        
        m_NetworkManager.OnClientConnectedCallback -= ClientConnected;
        m_NetworkManager.OnClientDisconnectCallback -= ClientDisconnect;
        m_NetworkManager.OnClientStopped -= ClientStopped;
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
        print("added option");

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
        print("refresh");
    }

    public void Discover(){
        ServerDropdown.gameObject.SetActive(false);
        discoveredServers.Clear();
        addresses.Clear();
        ServerDropdown.ClearOptions();
        m_Discovery.StartClient();
        m_Discovery.ClientBroadcast(new DiscoveryBroadcastData());
        print("Discovering");
        
    }

    public void Connect(){
        if (addresses.Count>0){
            GetComponent<AudioSource>().Stop();
            Color c = m_ColorPicker.color;
            NetworkManager.Singleton.NetworkConfig.ConnectionData = new byte[3]{(byte)(c.r*255f),(byte)(c.g*255f),(byte)(c.b*255f)};
            DiscoveryResponseData trg = discoveredServers[addresses[ServerDropdown.value]];
            UnityTransport transport = (UnityTransport)m_NetworkManager.NetworkConfig.NetworkTransport;
            transport.SetConnectionData(addresses[ServerDropdown.value].ToString(), trg.Port);
            m_NetworkManager.OnClientStarted += OnClientStarted;
            m_NetworkManager.StartClient();
            
        }
        
        
    }

    public void StartServer(){
        print("Startedserver");
        m_NetworkManager.ConnectionApprovalCallback = CheckApproval;
        m_NetworkManager.StartServer();
        m_Discovery.StartServer();
        menu.ShowServer();
        calibrated = 0;
        SceneManager.LoadSceneAsync("Playground",mode:LoadSceneMode.Additive);
        
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
            foreach(NetworkClient playerClient in m_NetworkManager.ConnectedClientsList){
                playerClient.PlayerObject.GetComponent<PlayerDisambigulation>().RecalculateIndex();
            }
            menu.PositionServerSideMenu();
            playercountEvent.Raise();
        }
    }

    public void SwapOrientation(){
        SetOrientation(!splitOrientation);
    }

    public void StopServer(){
        m_NetworkManager.Shutdown();
        m_Discovery.StopDiscovery();
        menu.ShowGeneral();
        SceneManager.UnloadSceneAsync("Playground");
    }

    public void ClientStopped(bool host){
        menu.ShowGeneral();
    }

    private void CheckApproval(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        if (m_NetworkManager.ConnectedClients.Count>=2){
            response.Approved = false;
        }else{
            response.Approved = true;
        }
        //Debug.Log("Approving client");
        // The client identifier to be authenticated
        var clientId = request.ClientNetworkId;

        // Additional connection data defined by user code
        var connectionData = request.Payload;
        playerColors[clientId] = new Color(connectionData[0]/255f,connectionData[1]/255f,connectionData[2]/255f);

        // Your approval logic determines the following values
        
        response.CreatePlayerObject = true;

        // The Prefab hash value of the NetworkPrefab, if null the default NetworkManager player Prefab is used
        response.PlayerPrefabHash = null;

        // Position to spawn the player object (if null it uses default of Vector3.zero)
        response.Position =100f*(0.5f-(m_NetworkManager.ConnectedClients.Count))* Vector3.forward;
        print("Location set to " + response.Position);

        // Rotation to spawn the player object (if null it uses the default of Quaternion.identity)
        response.Rotation = Quaternion.identity;

        // If response.Approved is false, you can provide a message that explains the reason why via ConnectionApprovalResponse.Reason
        // On the client-side, NetworkManager.DisconnectReason will be populated with this message via DisconnectReasonMessage
        //response.Reason = "Some reason for not approving the client";

        // If additional approval steps are needed, set this to true until the additional steps are complete
        // once it transitions from true to false the connection approval response will be processed.
        response.Pending = false;
    }

    void ClientConnected(ulong clientID){
        if(m_NetworkManager.IsServer){
            m_NetworkManager.ConnectedClients[clientID].PlayerObject.GetComponent<PlayerDisambigulation>().SetColor(playerColors[clientID]);
            connectionEvent.Raise(m_NetworkManager.ConnectedClients.Count,clientID,playerColors[clientID]);
            if (m_NetworkManager.ConnectedClients.Count==player_count){
                StartCoroutine(StartGame());
                
            }
        }
        ClientConnectinsChanged(clientID);
        
    }

    void ClientDisconnect(ulong clientID){
        disconnectEvent.Raise(m_NetworkManager.ConnectedClientsList.Count,clientID,playerColors[clientID]);
        ClientConnectinsChanged(clientID);
    }

    IEnumerator StartGame(){
        while (calibrated!=player_count){
            yield return null;
        }
        startGameEvent.Raise();
    }

    public void Calibrated(){
        calibrated += 1;
    }
    
}
