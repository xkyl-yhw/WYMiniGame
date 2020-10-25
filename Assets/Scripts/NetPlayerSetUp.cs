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

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        localPosition = transform.position;
        localRotation = transform.rotation;
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
}

