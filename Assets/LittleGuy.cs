using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

using Random = UnityEngine.Random;

public class LittleGuy : MonoBehaviour
{
    Animator animator;
    NavMeshAgent agent;
    Vector3 lastdir = Vector3.zero;
    RaycastHit downhit = new RaycastHit();
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        //agent.updateRotation = false;
        agent.updateUpAxis = false;
        animator = GetComponentInChildren<Animator>();
        StartCoroutine(Roam());
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        transform.rotation = Quaternion.FromToRotation(transform.up,downhit.normal)*transform.rotation;        
      
    }

    IEnumerator Roam(){
        int i = 0;
        while(true){
            ChooseTarget(i);
            yield return null;
            animator.SetBool("Running",true);
           
            yield return new WaitUntil(IsPathComplete);
            animator.SetBool("Running",false);
            if (i<3){
                yield return null;
            }
            else{
                yield return new WaitForSeconds(10f);
                
            }
            i = (i+1)%4;
        }
    }

    bool IsPathComplete(){
        return agent.remainingDistance<=agent.stoppingDistance;
    }

    void ChooseTarget(int farther){
        NavMeshHit hit = new NavMeshHit();
        bool found = false;
        int i =0;
        while(!found){
            float a = Random.Range(0f,360f);
            Vector3 trg = transform.position+40f*new Vector3(math.cos(a),0,math.sin(a))+lastdir*10f*farther;
            found = NavMesh.SamplePosition(trg,out hit, 5f,NavMesh.AllAreas);
            i++;
            if (i>100){
                found = true;
                print("NotFound");
            }
            
        }
        lastdir = (hit.position-transform.position).normalized;
        agent.SetDestination(hit.position);
    }
}
