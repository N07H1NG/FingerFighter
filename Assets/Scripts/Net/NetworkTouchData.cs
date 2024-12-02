using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

using System.Linq;
using Unity.Netcode;

using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;


public struct NetworkTouchData : INetworkSerializable
{
    // Start is called before the first frame update
    public Vector2 delta;
    public TouchPhase phase;
    public Vector2 screenPosition;
    public int touchId;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref delta);
        serializer.SerializeValue(ref phase);
        serializer.SerializeValue(ref screenPosition);
        serializer.SerializeValue(ref touchId);
    }
}
