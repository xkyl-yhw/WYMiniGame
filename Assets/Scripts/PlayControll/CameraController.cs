using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public bool isMove = false; //可否移动相机

    public float Xboundary = 200;
    public float Zboundary = 200;

    private int SightDistancespeed = 15;
    private bool CameraIsLock = false; //相机是否锁定  
    public float LockDistanceZ = 5f; //相机锁定时距离物体的位置
    public float LockDistanceX = 0f; //相机锁定时距离物体的位置
    private float RectSize = 50f; //矩形大小  
    public float CameraMoveSpeed = 1f; //相机移动速度  
    private Transform Player; //player的Transform  
    private float CamerafieldOfView = 60; //相机的锥形视野范围  
    private Camera camera; //相机  
    private float a= 0.05f ;//倍数

    //屏幕边缘四个矩形  
    private Rect RectUp;
    private Rect RectDown;
    private Rect RectLeft;
    private Rect RectRight;
    void Start()
    {
        //实例化屏幕边缘的四个矩形出来  
        RectUp = new Rect(0, Screen.height - RectSize, Screen.width, Screen.height);
        RectDown = new Rect(0, 0, Screen.width, RectSize);
        RectLeft = new Rect(0, 0, RectSize, Screen.width);
        RectRight = new Rect(Screen.width - RectSize, 0, Screen.width, Screen.height);


        //在场景内查找Tag为Player的物体  
        Player = GameObject.FindGameObjectWithTag("Player").transform;  
           
          
        camera = this.GetComponent<Camera>();

        CameraMoveSpeed = CameraMoveSpeed * a;


    }


void Update()
{

    if (!isMove)
        {
            CameraIsLock = true;
        }
    //如果按下Y键锁定相机再次按下解锁。  
    else if (Input.GetKeyDown(KeyCode.Y))
    {
        CameraIsLock = !CameraIsLock;
    }


    CameraMoveAndLock();//视角移动和锁定  
    //SightDistance();//视距的缩放  

}
//视角移动  
void CameraMoveAndLock()
{
    ////空格回到自己  
    //if (Input.GetKey(KeyCode.Space))
    //{
    //    transform.position = new Vector3(Player.position.x, transform.position.y, Player.position.z - 5f);
    //}
    //如果没有锁定相机（视野）可以移动  
    if (CameraIsLock == false)
    {
        //如果鼠标在屏幕上的位置包含在这个矩形里  
        if (RectUp.Contains(Input.mousePosition))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + CameraMoveSpeed);
        }
        if (RectDown.Contains(Input.mousePosition))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - CameraMoveSpeed);
        }
        if (RectLeft.Contains(Input.mousePosition))
        {
            transform.position = new Vector3(transform.position.x - CameraMoveSpeed, transform.position.y, transform.position.z);
        }
        if (RectRight.Contains(Input.mousePosition))
        {
            transform.position = new Vector3(transform.position.x + CameraMoveSpeed, transform.position.y, transform.position.z);
        }
        //判断相机移动的边缘在哪里，不能 超过设定的最远距离  
        if (transform.position.z >= Zboundary)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, Zboundary);
        }
        if (transform.position.z <= -Zboundary)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, -Zboundary);
        }
        if (transform.position.x >= Xboundary)
        {
            transform.position = new Vector3(Xboundary, transform.position.y, transform.position.z);
        }
        if (transform.position.x <= -Xboundary)
        {
            transform.position = new Vector3(-Xboundary, transform.position.y, transform.position.z);
        }
    }
    else if (CameraIsLock == true)
    {
        //如果锁定视角，相机视野显示主角  
        transform.position = new Vector3(Player.position.x - LockDistanceX, transform.position.y, Player.position.z - LockDistanceZ);
    }

}


//中键滑轮拉远拉近  
void SightDistance()
{
    float MouseScrollWheel = Input.GetAxis("Mouse ScrollWheel");

    CamerafieldOfView += MouseScrollWheel * -SightDistancespeed;
    CamerafieldOfView = Mathf.Clamp(CamerafieldOfView, 30, 60);
    camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, CamerafieldOfView, Time.deltaTime * SightDistancespeed);

}  
} 