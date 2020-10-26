using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(CharacterController))]

public class PlayController : NetworkBehaviour
{
    private float time = 0;
    public bool isOpen;//是否点开大地图
    public GameObject bigMap;
    private Animator thisAnimator;
    public GameObject player;
    public GameObject HUDCanvas;
    public float moveSpeed = 0;
    public float roatSpeed = 0;
    public float jumpSpeed = 0;
    public float runSpeed = 0;
    public float gravity = 0;
    public string maskName = "Ground"; //输入地面层的Layer名字

    public float dashDuration = 0.5f;// 控制冲刺时间
    public float dashSpeed;// 冲刺速度
    private bool isDash = false; //是否在冲刺
    private float dashTime; //临时变量
    public float dashCD;// 冲刺CD
    private float dashCDtime = 0;// 临时变量

    private float endurance; //耐力值临时变量
    private float enduranceMAX; //耐力值最大值
    public float enduranceCoefficient = 5; //耐力系数
    public float enduranceCoefficient2 = 10f; //耐力恢复系数
    public float enduranceDashConsume = 30f; //翻滚消耗耐力

    private float essenceRate; //负重比率
    private float dashCoefficient = 1; //负重影响冲刺系数

    public bool isDefended = false; //是否无敌

    private CharacterController controller;
    private PlayerAttribute playerAttribute;

    private Vector3 moveDirection = Vector3.zero;//角色移动

    private int groundLayerIndex; //地面层
    void Start()
    {
        player = gameObject;
        HUDCanvas = player.transform.Find("HUDCanvas").gameObject;
        bigMap = player.transform.Find("bigMap").gameObject;
        controller = GetComponent<CharacterController>();
        groundLayerIndex = LayerMask.GetMask(maskName); //初始化地面layer的序列

        dashTime = dashDuration;//初始化冲刺时间

        playerAttribute = GetComponent<PlayerAttribute>();
        enduranceMAX = playerAttribute.enduranceMax; //初始化耐力值
        endurance = enduranceMAX;
        thisAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer) return;
        OpenBigMap();
        //HUDCanvas.SetActive(true);
        this.Rotating(); //角色旋转-朝向鼠标

        thisAnimator.SetBool("isWalking", false);

        if (dashCDtime <= 0 && Input.GetButtonDown("Jump") && endurance > enduranceDashConsume)
        {
            isDash = true;
            moveDirection = transform.forward;// forward 指向物体当前的前方
            moveDirection.y = 0f;// 只做平面的上下移动和水平移动，不做高度上的上下移动
            dashCDtime = dashCD;
            dashControll();
            endurance -= enduranceDashConsume;
        }

        if (isDash)
        {
            this.Dash(); //冲刺
        }
        else
        {
            this.Moving(); //角色移动-WASD
        }

        if (dashCDtime > 0)
            dashCDtime = TimeCount(dashCDtime);


        SetPlayerAttributeValue();

        time = TimeCount(time);
        if (time <= 0)
        {
            time = 1;
        }

    }

    //移动
    void Moving()
    {
        float speed = moveSpeed;//速度赋值默认速度

        //跑步
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            thisAnimator.SetBool("isWalking", true);
            thisAnimator.SetBool("isIdle", false);
        }else
        {
            thisAnimator.SetBool("isIdle", true);
        }
        if ((Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) &&
               controller.isGrounded && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
               && (endurance > enduranceCoefficient))
        {
            speed = runSpeed;
            enduranceController(true);
            thisAnimator.SetBool("isWalking", false);
            thisAnimator.SetBool("isRunning", true);
        }
        else
        {
            enduranceController(false);
            thisAnimator.SetBool("isRunning", false);
        }



        //移动

        //moveDirection.x = Input.GetAxis("Horizontal") * speed;
        //moveDirection.z = Input.GetAxis("Vertical") * speed;
        //moveDirection.y -= gravity * Time.deltaTime;
        moveDirection = Input.GetAxis("Horizontal") * Vector3.ProjectOnPlane(Camera.main.transform.right, Vector3.up).normalized * speed + Input.GetAxis("Vertical") * Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up).normalized * speed - Vector3.up * gravity * Time.deltaTime;

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
        thisAnimator.SetBool("isDashing", true);
        Debug.Log(thisAnimator.GetBool("isDashing"));
        if (dashTime < 0)// reset
        {
            isDash = false;
            dashTime = dashDuration;
            thisAnimator.SetBool("isDashing", false);
        }
        else
        {
            dashTime -= Time.deltaTime;
            controller.Move(moveDirection * dashSpeed * Time.deltaTime * dashCoefficient);
        }

    }

    //获取负重系数
    private void dashControll()
    {
        essenceRate = playerAttribute.essenceRate;
        float a = (1 - essenceRate) / 0.25f;
        dashCoefficient = Mathf.Floor(a) * 0.25f; //获取人物属性脚本中的人物负重并且进行计算
    }

    //耐力增减
    public void enduranceController(bool isDecrease)
    {
        if (isDecrease && endurance > 0)
            endurance -= Time.deltaTime * enduranceCoefficient;
        if (endurance < 0)
            endurance = 0;
        if (!isDecrease && endurance <= enduranceMAX)
            endurance += Time.deltaTime * enduranceCoefficient2;
        if (endurance > enduranceMAX)
            endurance = enduranceMAX;
    }

    private void SetPlayerAttributeValue()
    {
        playerAttribute.endurance = endurance;
    }

    public void OpenBigMap()
    {
        HUDCanvas.SetActive(!isOpen);
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            //一直开关背包
            isOpen = !isOpen;
           bigMap.SetActive(isOpen);
           HUDCanvas.SetActive(!isOpen);
        }
    }
}
