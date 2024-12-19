using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class KillZ : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /// <summary>
    /// OnTriggerEnter is called when the Collider other enters the trigger.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<Fish>(out Fish f)){
            Destroy(other.gameObject);
        }
        else if (other.gameObject.layer==3){
            print("LOH");
            other.transform.parent.position = Vector3.zero;
            other.transform.parent.GetComponentInChildren<MyPlayer>().gameObject.transform.position = Vector3.up*3f;
            other.transform.parent.GetComponentInChildren<MyPlayer>().UpdateFeet();

        }
        else if (other.gameObject.TryGetComponent<LittleGuy>(out LittleGuy lt)){
            lt.GetComponent<NavMeshAgent>().Warp(Vector3.zero);
        }
    }
}
