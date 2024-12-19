using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BublikiGameMode : MonoBehaviour
{
    AudioSource audioSource;
    AudioSource bell;
    [SerializeField] MyAudioCue clockCue;
    public UnityEvent ActualStart;
    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        audioSource = GetComponents<AudioSource>()[0];
        bell = GetComponents<AudioSource>()[1];
        
    }
    // Start is called before the first frame update
    public IEnumerator Game(){
        yield return new WaitForSeconds(1f);
        for(int i=0;i<5;i++){
            audioSource.PlayOneShot(clockCue.GetRandomClip());
            yield return new WaitForSeconds(1f);
            
        }
        bell.Play();
        yield return new WaitForSeconds(60f);
        ActualStart.Invoke();
        audioSource.volume = 0.5f;
        for(int i=0;i<12*60-5;i++){
            audioSource.PlayOneShot(clockCue.GetRandomClip());
            yield return new WaitForSeconds(1f);
        }
        audioSource.volume = 1f;
        for(int i=0;i<5;i++){
            audioSource.PlayOneShot(clockCue.GetRandomClip());
            yield return new WaitForSeconds(1f);
        }
        bell.Play();

    }

    public void StartGame(){
        StartCoroutine(Game());
    }
}
