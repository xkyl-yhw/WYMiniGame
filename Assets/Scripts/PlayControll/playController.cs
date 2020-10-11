using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]

public class playController : MonoBehaviour
{
    public float moveSpeed = 0;
    public float roatSpeed = 0;
    public float jumpSpeed = 0;
    public float runSpeed = 0;
    public float gravity = 0;
    public string maskName = "Ground"; //输入地面层的Layer名字
    public float dashDuration;// 控制冲刺时间
    public float dashSpeed;// 冲刺速度
    private bool isDash = false;
    private float dashTime; //临时变量
    private float dashCoefficient = 1;
    public float dashCD;// 冲刺CD
    private float dashCDtime = 0;// 临时变量

    private CharacterController controller;

    private Vector3 moveDirection = Vector3.zero;//角色移动

    private int groundLayerIndex; //地面层

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        groundLayerIndex = LayerMask.GetMask(maskName); //初始化地面layer的序列

        dashTime = dashDuration;//初始化冲刺时间
    }

    // Update is called once per frame
    void Update()
    {
        //Vector3 forward = Input.GetAxis("Vertical") * transform.TransformDirection(Vector3.forward) * moveSpeed;//向前移动
        //Vector3 move = new Vector3(Input.GetAxis("Vertical"), 0, -Input.GetAxis("Horizontal")) * moveSpeed;//移动位置
        //controller.Move(move * Time.deltaTime); //移动
        //transform.Rotate(new Vector3(0, Input.GetAxis("Horizontal") * roatSpeed * Time.deltaTime, 0));//旋转


        this.Rotating(); //角色旋转-朝向鼠标

        if (dashCDtime <= 0 && Input.GetButtonDown("Jump"))
        {
            isDash = true;
            moveDirection = transform.forward;// forward 指向物体当前的前方
            moveDirection.y = 0f;// 只做平面的上下移动和水平移动，不做高度上的上下移动
            dashCDtime = dashCD;
            dashControll();
        }

        if (isDash)
        {
            this.Dash(); //冲刺
        }
        else
        {
            this.Moving(); //角色移动-WASD
        }

        if (dashCDtime>0)
            dashCDtime = TimeCount(dashCDtime);
    }

    //移动
    void Moving()
    {
        float speed = moveSpeed;//速度赋值默认速度

        ////跳跃
        //if (controller.isGrounded) 
        //{
        //    if (Input.GetButton("Jump"))
        //        moveDirection.y = jumpSpeed;
        //}

        //跑步
        if (controller.isGrounded &&(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
        {
            speed = runSpeed;
        }
        //移动
        moveDirection.x = Input.GetAxis("Horizontal") * speed;
        moveDirection.z = Input.GetAxis("Vertical") * speed;
        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);

    }

    //旋转
    void Rotating()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;//存储射线信息
        if (Physics.Raycast(ray, out hitInfo, 200, groundLayerIndex))//生成射线
        {
            Vector3 playerToMouse = hitInfo.point - transform.position;
            playerToMouse.y = 0;
            Quaternion newRotation = Quaternion.LookRotation(playerToMouse);
            this.transform.rotation = newRotation;
        }
    }


    //时间倒计时
    float TimeCount(float timeUse)
    {
        timeUse -= Time.deltaTime;
        return timeUse;
    }

    //冲刺
    private void Dash()
    {
        if (dashTime < 0)// reset
        {
            isDash = false;
            dashTime = dashDuration;
        }
        else
        {
            dashTime -= Time.deltaTime;
            controller.Move(moveDirection * dashSpeed * Time.deltaTime);
        }
    }

    //获取系数
    private void dashControll()
    {
        //dashCoefficient = this.GetComponent<>; //获取人物属性脚本中的人物负重并且进行计算
        dashSpeed *= dashCoefficient;
        dashDuration *= dashCoefficient;
    }
}
