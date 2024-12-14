using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FishSpawner : MonoBehaviour
{
    [SerializeField] GameObject fishPerfab;
    // Start is called before the first frame update
    void OnEnable(){
        StartCoroutine(SpawnFish());
    }
    void OnDisable(){
        StopAllCoroutines();
    }

    IEnumerator SpawnFish(){
        
        while(true){
            GetComponent<AudioSource>().Play();
            Vector3 dir = Quaternion.Euler(0,Random.Range(0f,360),0)*Vector3.forward;
            transform.forward = dir;
            float timer = 0f;
            float spawnTimer = 0f;
            while (timer<20f){
                yield return null;
                transform.position = dir*(-500+50*timer)+Vector3.up*100f;
                timer += 5*Time.deltaTime;
                spawnTimer += 5*Time.deltaTime;
                if (spawnTimer>=1f && new Vector2(transform.position.x,transform.position.z).magnitude<200){
                    GameObject newfish = Instantiate(fishPerfab,transform.position,Random.rotation);
                    spawnTimer = 0f;
                }
            }
            GetComponent<AudioSource>().Stop();
            yield return new WaitForSeconds(Random.Range(8f,30f));
        }
    }
}
