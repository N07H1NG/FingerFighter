using System;
using UnityEngine;
using Unity.Collections;
using Unity.Netcode;

using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;
using Unity.VisualScripting;

public class TouchMessageHandler : NetworkBehaviour
{
    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        if(IsClient){
            EnhancedTouchSupport.Enable();
        }
    }
    
    [Tooltip("The name identifier used for this custom message handler.")]
    public string MessageName = "TouchMessage";

    /// <summary>
    /// For most cases, you want to register once your NetworkBehaviour's
    /// NetworkObject (typically in-scene placed) is spawned.
    /// </summary>
    /// 

    public override void OnNetworkSpawn()
    {
        // Both the server-host and client(s) register the custom named message.
        NetworkManager.CustomMessagingManager.RegisterNamedMessageHandler(MessageName, ReceiveMessage);
        
        
    }

    
    void Update()
    {
        if (IsClient) //Only send an RPC to the server from the client that owns the NetworkObject of this NetworkBehaviour instance
        {
            foreach(Touch t in Touch.activeTouches){
                Debug.Log("Sending message");
                NetworkTouchData td = new NetworkTouchData();
                td.delta = t.delta;
                td.screenPosition = t.screenPosition;
                td.touchId = t.touchId;
                td.phase = t.phase;
                SendMessage(NetworkManager.ServerClientId,td);
            }
            
        }
    }
    public override void OnNetworkDespawn()
    {
        // De-register when the associated NetworkObject is despawned.
        NetworkManager.CustomMessagingManager.UnregisterNamedMessageHandler(MessageName);
        // Whether server or not, unregister this.
    }

    /// <summary>
    /// Invoked when a custom message of type <see cref="MessageName"/>
    /// </summary>
    private void ReceiveMessage(ulong senderId, FastBufferReader messagePayload)
    {
        
        var receivedMessageContent = new NetworkTouchData();
        messagePayload.ReadValueSafe(out receivedMessageContent);
        if (IsServer)
        {
            Debug.Log($"Sever received Touch from client ({senderId})");
        }
        else
        {
            Debug.Log($"Client received Touch from the server.");
        }
    }

    /// <summary>
    /// Invoke this with a Guid by a client or server-host to send a
    /// custom named message.
    /// </summary>
    public void SendMessage(ulong Id, NetworkTouchData Data)
    {
        var messageContent = Data;
        var writer = new FastBufferWriter(1100, Allocator.Temp);
        var customMessagingManager = NetworkManager.CustomMessagingManager;
        using (writer)
        {
            writer.WriteValueSafe(messageContent);
            if (IsServer)
            {
                // This is a server-only method that will broadcast the named message.
                // Caution: Invoking this method on a client will throw an exception!
                customMessagingManager.SendNamedMessageToAll(MessageName, writer);
            }
            else
            {
                // This is a client or server method that sends a named message to one target destination
                // (client to server or server to client)
                customMessagingManager.SendNamedMessage(MessageName, Id, writer);
            }
        }
    }
}