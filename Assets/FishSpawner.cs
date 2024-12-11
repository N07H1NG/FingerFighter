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
            float timer = 0f;
            float spawnTimer = 0f;
            while (timer<20f){
                transform.position = Vector3.forward*(-100+10*timer)+Vector3.up*200f;
                timer += Time.deltaTime;
                spawnTimer += Time.deltaTime;
                if (spawnTimer>=1f){
                    GameObject newfish = Instantiate(fishPerfab,transform.position,Random.rotation);
                    spawnTimer = 0f;
                }
            }
            yield return new WaitForSeconds(Random.Range(8f,30f));
        }
    }
}
