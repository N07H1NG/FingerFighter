using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuckControl : MonoBehaviour
{
    [SerializeField] Transform boneBind;
    
    Quaternion rotOffset;
    
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
}
