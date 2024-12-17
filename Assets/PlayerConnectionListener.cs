using UnityEngine;
using UnityEngine.Events;
public class PlayerConnectionListener : MonoBehaviour
{
    [Tooltip("Event to register with.")]
        public PlayerConnectionEvent Event;

        [Tooltip("Response to invoke when Event is raised.")]
        public UnityEvent<int,ulong,Color> Response;

        private void OnEnable()
        {
            Event.RegisterListener(this);
        }

        private void OnDisable()
        {
            Event.UnregisterListener(this);
        }

        public void OnEventRaised(int playerCount,ulong playerID,Color playerColor)
        {
            Response.Invoke(playerCount,playerID,playerColor);
        }
}
