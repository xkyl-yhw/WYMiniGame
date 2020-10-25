using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetPlayerSetUp : MonoBehaviour
{
    public GameObject player_camera;
    public Vector3 offset;
    public Vector3 camera_rotate;
    private void Start()
    {
        GameObject go = Instantiate(player_camera);
        go.GetComponent<CameraController>().Player = this.transform;
        GetComponent<PlayController>().player_Camera = go.GetComponent<Camera>();
        go.GetComponent<Transform>().position = GetComponent<Transform>().position + offset;
        go.GetComponent<Transform>().rotation = Quaternion.Euler(camera_rotate);
    }
}
