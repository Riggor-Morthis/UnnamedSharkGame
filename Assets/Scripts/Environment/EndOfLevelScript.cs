using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndOfLevelScript : MonoBehaviour
{
    public string nextLevel;
    public LayerMask playerLayer;
    AudioManagerScript audioManager;

    private void Awake()
    {
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManagerScript>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((playerLayer.value & (1 << collision.gameObject.layer)) > 0)
        {
            audioManager.Play("Triumph");
            SceneManager.LoadScene(nextLevel);
        }
    }
}
