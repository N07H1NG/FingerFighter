using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class RpcTest : NetworkBehaviour
{

    List<NetworkTouchData> framtouches = new List<NetworkTouchData>();
    MyPlayer plr;
    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        plr = GetComponentInChildren<MyPlayer>();
    }
    void Update()
    {
        
        if (!IsServer && IsOwner) //Only send an RPC to the server from the client that owns the NetworkObject of this NetworkBehaviour instance
        {
            foreach(Touch t in Touch.activeTouches){
                NetworkTouchData td = new NetworkTouchData();
                td.delta = t.delta;
                td.screenPosition = t.startScreenPosition;
                td.touchId = t.touchId;
                td.phase = t.phase;
                ServerOnlyNewTouchRpc(td, NetworkObjectId);
            }
            ServerFinishFrameRpc(NetworkObjectId);
        }
    }
    

    [Rpc(SendTo.ClientsAndHost)]
    void ClientAndHostRpc( ulong sourceNetworkObjectId)
    {
        Debug.Log($"Client Received the RPC  on NetworkObject #{sourceNetworkObjectId}");
        if (IsOwner) //Only send an RPC to the owner of the NetworkObject
        {
            
        }
    }

    [Rpc(SendTo.Server)]
    void ServerOnlyNewTouchRpc(NetworkTouchData td, ulong sourceNetworkObjectId)
    {
        Debug.Log($"Server Received the RPC on NetworkObject #{sourceNetworkObjectId}");
        framtouches.Add(td);
        
        
    }

    [Rpc(SendTo.Server)]
    void ServerFinishFrameRpc(ulong sourceNetworkObjectId)
    {
        Debug.Log($"Server Received the finish RPC on NetworkObject #{sourceNetworkObjectId}");
        plr.networkActiveTouches = framtouches;
        framtouches.Clear();
    }

}