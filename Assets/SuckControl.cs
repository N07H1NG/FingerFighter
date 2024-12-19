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
    
    float maxdist;
    
    public ulong playerIndex;
    
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
        Debug.DrawLine(transform.position, transform.position + transform.forward.normalized*-1f*maxdist, Color.magenta);
        print(maxdist);
        
    }

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        maxdist = GetComponent<Collider>().bounds.size.z;
    }

    void OnDisable(){
    }


    /// <summary>
    /// OnTriggerStay is called once per frame for every Collider other
    /// that is touching the trigger.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    void OnTriggerStay(Collider other)
    {
        if (enabled){
            Rigidbody rb;
            Fish f;
            if(other.gameObject.TryGetComponent<Rigidbody>(out rb) && other.gameObject.TryGetComponent<Fish>(out f)){
                f.touched = true;
                Vector3 d = target.position-other.transform.position;
                float p = math.max(maxdist/10f,maxdist-d.magnitude);
                
                rb.AddForce(d.normalized*power*p);
                rb.velocity -= Vector3.ProjectOnPlane(rb.velocity,d)*math.min(Time.fixedDeltaTime*400f,1);
                if (p>f.lastGuyPower||f.LastGuyID ==playerIndex){
                    f.LastGuyID = playerIndex;
                    f.lastGuyPower = p; 
                }
            }
        }
    }
}
