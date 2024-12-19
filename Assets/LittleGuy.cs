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
    [SerializeField] BublikiGameMode gameMode;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        //agent.updateRotation = false;
        agent.updateUpAxis = false;
        animator = GetComponentInChildren<Animator>();
        StartCoroutine(Roam());
        gameMode.RegisterGuy(this);
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Physics.Raycast(transform.position,Vector3.down,out downhit);
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
                yield return new WaitForSeconds(Random.Range(10f,20f));
                
            }
            i = (i+1)%5;
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
            Vector3 trg = transform.position+30f*new Vector3(math.cos(a),0,math.sin(a))+lastdir*5f*farther;
            found = NavMesh.SamplePosition(trg,out hit, 15f,NavMesh.AllAreas);
            i++;
            if (i>100){
                found = true;
                print("NotFound");
            }
            
        }
        lastdir = (hit.position-transform.position).normalized;
        agent.SetDestination(hit.position);
    }

    public void Finish(){
        GetComponentInChildren<Pond>().GetResults(out ulong fav,out int scr, out bool draw);
        gameMode.ReceiveResult(new BublikResult(this,fav,scr,draw));
    }


}
