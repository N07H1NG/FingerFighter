using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuckControl : MonoBehaviour
{
    [SerializeField] Transform boneBind;
    
    Quaternion rotOffset;
    float power = 50f;
    
    
    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
           
    }
    // Start is called before the first frame update
    void Start()
    {
        
        rotOffset = Quaternion.Inverse(boneBind.rotation)*transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        
        transform.rotation = boneBind.rotation*rotOffset;
        transform.position = boneBind.position;

        
    }

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        
        Debug.Log("Enabled");
    }

    void OnDisable(){
        Debug.Log("Disabled");
    }


    /// <summary>
    /// OnTriggerStay is called once per frame for every Collider other
    /// that is touching the trigger.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    void OnTriggerStay(Collider other)
    {
        if (enabled){
            Rigidbody rb = new Rigidbody();
            if(other.gameObject.TryGetComponent<Rigidbody>(out rb)){
                rb.AddForce((transform.position-other.transform.position).normalized*power);
            }
        }
    }
}
