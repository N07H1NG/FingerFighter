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
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = true;
        agent.updateUpAxis = true;
        animator = GetComponentInChildren<Animator>();
        StartCoroutine(Roam());
        
    }

    // Update is called once per frame
    void Update()
    {
        agent.updateRotation =true;
    }

    IEnumerator Roam(){
        while(true){
            ChooseTarget();
            //yield return null;
            //animator.SetBool("Running",true);
           
            yield return new WaitUntil(IsPathComplete);
            //animator.SetBool("Running",false);
            yield return new WaitForSeconds(3f);
        }
    }

    bool IsPathComplete(){
        return agent.remainingDistance<=agent.stoppingDistance;
    }

    void ChooseTarget(){
        NavMeshHit hit = new NavMeshHit();
        bool found = false;
        int i =0;
        while(!found){
            float a = Random.Range(0f,360f);
            Vector3 trg = transform.position+50*new Vector3(math.cos(a),0,math.sin(a));
            found = NavMesh.SamplePosition(trg,out hit, 10f,NavMesh.AllAreas);
            i++;
            if (i>100){
                found = true;
                print("NotFound");
            }
            
        }
        agent.SetDestination(hit.position);
    }
}
