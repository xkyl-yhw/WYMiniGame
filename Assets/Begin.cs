﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Begin : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
    }


    public void OnStartGame(int SceneNumber)
    {
        //Application.LoadLevel(SceneNumber); //Unity4.6及之前版本的写法
        SceneManager.LoadScene(SceneNumber);
    }


    // Update is called once per frame
    void Update()
    {

    }
}