using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class SuckControl : MonoBehaviour
{
    [SerializeField] Transform boneBind;
    
    Quaternion rotOffset;
    [SerializeField] Transform target;
    [SerializeField] float power = 20f;
    
    
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
        gameObject.SetActive(false);
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
                Vector3 d = target.position-other.transform.position;
                float p = math.max(2,50-d.magnitude);
                rb.AddForce(d.normalized*power*p);
                rb.velocity -= Vector3.ProjectOnPlane(rb.velocity,d)*math.min(Time.fixedDeltaTime*100f,1);
            }
        }
    }
}
