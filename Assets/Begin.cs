using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Begin : MonoBehaviour
{
    public AudioClip clickClip;

    // Use this for initialization
    void Start()
    {
    }


    public void OnStartGame(string sceneName)
    {
        this.GetComponent<AudioSource>().clip = clickClip;
        this.GetComponent<AudioSource>().Play();
        //Application.LoadLevel(SceneNumber); //Unity4.6及之前版本的写法
        SceneManager.LoadScene(sceneName);
    }


    // Update is called once per frame
    void Update()
    {

    }
}