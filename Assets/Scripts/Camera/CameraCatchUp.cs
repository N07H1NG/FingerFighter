using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCatchUp : MonoBehaviour
{
    Vector3 speed = Vector3.zero;
    Vector3 rotspeed = Vector3.zero;
    [SerializeField] Transform target;
    // Start is called before the first frame update
    

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.SmoothDamp(transform.position,target.position,ref speed,0.3f);
        transform.forward = Vector3.SmoothDamp(transform.forward,target.forward,ref rotspeed,0.3f);
    }
}
