using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Login : NetworkBehaviour
{
    public GameObject loginCanvas;
    public GameObject player;
    void Start()
    {
        loginCanvas = player.transform.Find("LoginCanvas").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        loginCanvas.SetActive(true);
        
    }
}
