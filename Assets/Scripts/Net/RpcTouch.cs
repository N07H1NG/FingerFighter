using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class RpcTouch : NetworkBehaviour
{

    List<NetworkTouchData> framtouches = new List<NetworkTouchData>();
    MyPlayer plr;
    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>


    public override void OnNetworkSpawn()
    {
        if (IsOwner){
            EnhancedTouchSupport.Enable();
            ServerStartInfoRpc(new Vector2(Screen.width,Screen.height),NetworkObjectId);
        }
        base.OnNetworkSpawn();
        
    }

    public override void OnNetworkDespawn()
    {
        if (IsOwner) EnhancedTouchSupport.Disable();
        base.OnNetworkDespawn();
    }
    void Awake()
    {
        plr = GetComponentInChildren<MyPlayer>();
        
    }
    
    void Update()
    {
        
        if (!IsHost && IsOwner && EnhancedTouchSupport.enabled) //Only send an RPC to the server from the client that owns the NetworkObject of this NetworkBehaviour instance
        {
            foreach(Touch t in Touch.activeTouches){
                NetworkTouchData td = new NetworkTouchData(t);
                
                ServerOnlyNewTouchRpc(td, NetworkObjectId);
            }
            ServerFinishFrameRpc(NetworkObjectId);
            print("YASOSALKA");
        }
        else if(IsHost){
            foreach(Touch t in Touch.activeTouches){
                NetworkTouchData td = new NetworkTouchData(t);
                framtouches.Add(td);
                
            }
            plr.SingleFrameOfTouches(new List<NetworkTouchData>(framtouches));
            framtouches.Clear();
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
        
        plr.SingleFrameOfTouches(new List<NetworkTouchData>(framtouches));
        framtouches.Clear();
    }

    [Rpc(SendTo.Server)]
    void ServerStartInfoRpc(Vector2 screenSize, ulong sourceNetworkObjectId)
    {
        Debug.Log($"Server Received info RPC on NetworkObject #{sourceNetworkObjectId}");
        plr.screenSize = screenSize;
    }

}