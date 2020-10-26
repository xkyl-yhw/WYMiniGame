using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetPlayerSetUp : NetworkBehaviour
{
    [SyncVar]
    private Vector3 localPosition;
    [SyncVar]
    private Quaternion localRotation;

    private GameObject myCamera;
    public Vector3 offset;
    public Vector3 angle;

    [SyncVar(hook = nameof(SetColor))]
    private Color playerColor;

    private Color[] temp = { Color.red, Color.blue, Color.green };
    public Material[] mat;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        localPosition = transform.position;
        localRotation = transform.rotation;
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        int playerNum = GameObject.FindGameObjectsWithTag("Player").Length;
        Debug.Log(playerNum);
        playerNum %= 3;
        playerColor = temp[playerNum];
    }

    private void Start()
    {
        if (isLocalPlayer)
        {
            myCamera = GameObject.FindGameObjectWithTag("MainCamera");
            myCamera.transform.position = localPosition;
            myCamera.transform.rotation = localRotation;
            myCamera.transform.position = offset.x * transform.forward + offset.y * transform.up + offset.z * transform.right;
            myCamera.transform.rotation *= Quaternion.Euler(angle);
            myCamera.GetComponent<CameraController>().Player = transform;

        }
    }

    private void SetColor(Color oldColor, Color newColor)
    {
        GetComponent<TeamSetup>().teamColor = newColor;
    }
}

