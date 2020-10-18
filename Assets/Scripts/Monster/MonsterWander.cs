using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterWander: MonoBehaviour
{

    //定义敌人的Transform
    Transform m_transform;
    //CharacterController m_ch;

    //定义动画组件
    public Animator m_animator;

    //定义寻路组件
    NavMeshAgent m_agent;

    //定义一个主角类的对象
    public PlayController m_player;

    PlayerAttribute playerAttribute;
    //角色移动速度
    float m_moveSpeed = 0.5f;
    //角色旋转速度
    float m_rotSpeed = 120;
    //定义生命值
    int m_life = 15;

    //定义计时器
    float m_timer = 2;
    //定义生成点
    //protected EnemySpawn m_spawn;


    // Use this for initialization
    void Start()
    {
        //初始化m_transform 为物体本身的tranform
        m_transform = this.transform;

        //初始化动画m_ani 为物体的动画组件
        m_animator = this.GetComponent<Animator>();

        //初始化寻路组件m_agent 为物体的寻路组件
        m_agent = GetComponent<NavMeshAgent>();

        //初始化主角
        m_player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayController>();
        playerAttribute= GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerAttribute>();


    }

    // Update is called once per frame
    void Update()
    {
        ////设置敌人的寻路目标
        //m_agent.SetDestination(m_player.m_transform.position);

        ////调用寻路函数实现寻路移动
        //MoveTo();

        //敌人动画的播放与转换
        //如果玩家的生命值小于等于0时,什么都不做 (主角死后 敌人无需再有动作)
        if (playerAttribute.health <= 0)
        {
            return;
        }

        //获取当前动画状态(Idle Run Attack Death 中的一种)
        AnimatorStateInfo stateInfo = m_animator.GetCurrentAnimatorStateInfo(0);

        //Idle   如果角色在等待状态条 并且 没有处于转换状态  (0代表的是Base Layer)
        if (stateInfo.fullPathHash == Animator.StringToHash("Base Layer.idle") && !m_animator.IsInTransition(0))
        {
            //此时把Idle状态设为false  (此时把状态设置为false 一方面Unity 动画设置里面has exit time已经取消  另一方面为了避免和后面的动画冲突 )
            m_animator.SetBool("idle", false);

            //待机一定时间后(Timer)  之所以有这个Timer 是因为在动画播放期间 无需对下面的语句进行判断(判断也没有用) 从而起到优化的作用
            m_timer -= Time.deltaTime;

            //如果计时器Timer大于0  返回 (什么也不干,作用是优化 优化 优化)
            if (m_timer > 0)
            {
                return;
            }

            //如果距离主角小于3米 把攻击动画的Bool值设为true  (激活指向Attack的通道)
            if (Vector3.Distance(m_transform.position, m_player.transform.position) < 3f)
            {
                m_animator.SetBool("IdleToAttack",true);
            }
            //如果距离主角不小于3米
            else
            {
                //那么把计时器重置为1
                m_timer = 1;
                //重新获取自动寻路的位置
                m_agent.SetDestination(m_player.transform.position);
                //激活指向Run的通道
                m_animator.SetBool("IdleToWalk",true);
            }
        }


        //walk   如果角色指向奔跑状态条  并且  没有处于转换状态  (0代表的是Base Layer)
        if (stateInfo.fullPathHash == Animator.StringToHash("Base Layer.walk") && !m_animator.IsInTransition(0))
        {
            //关闭指向Run的通道
            m_animator.SetBool("walk", false);
            //计时器时间随帧减少
            m_timer -= Time.deltaTime;
            //计时器时间小于0时 重新获取自动寻路的位置  重置计时器时间为1
            if (m_timer < 0)
            {
                m_agent.SetDestination(m_player.transform.position);
                m_timer = 1;
            }

            //调用跟随函数
            MoveTo();

            //当角色与主角的距离小于等于5米时
            if (Vector3.Distance(m_transform.position, m_player.transform.position) <= 5f)
            {
                //清楚当前路径 当路径被清除  代理不会开始寻找新路径直到SetDestination 被调用
                m_agent.ResetPath();
                //激活指向Attack的通道
                m_animator.SetBool("WalkToAttack", true);

            }
        }


        //Attack 如果角色指向攻击状态条  并且  没有处于转换状态   (0代表的是Base Layer)
        if (stateInfo.fullPathHash == Animator.StringToHash("Base Layer.attack") && !m_animator.IsInTransition(0))
        {
            //调用转向函数
            RotationTo();

            //关闭指向Attack的通道
            m_animator.SetBool("attack", false);

            //当播放过一次动画后  normalizedTime 实现状态的归1化(1就是整体和全部)  整数部分是时间状态的已循环数  小数部分是当前循环的百分比进程(0-1)
            if (stateInfo.normalizedTime >= 1.0f)
            {
                //激活指向Idle的通道
                m_animator.SetBool("Idle", true);

                //计时器时间重置为2
                m_timer = 2;


                //m_player.OnDamage(1);

            }
        }


        //Death  如果角色指向死亡状态条  并且  没有处于转换状态   (0代表的是Base Layer)
        if (stateInfo.fullPathHash == Animator.StringToHash("Base Layer.Death") && !m_animator.IsInTransition(0))
        {
            //摧毁这个物体的碰撞体
            Destroy(this.GetComponent<Collider>());

            //自动寻路时间被归零  角色不再自动移动
            m_agent.speed = 0;

            //死亡动画播放一遍后 角色死亡
            if (stateInfo.normalizedTime >= 1.0f)
            {
                //OnDeath()
            }


        }
    }


    //敌人的自动寻路函数
    void MoveTo()
    {
        //定义敌人的移动量
        float speed = m_moveSpeed * Time.deltaTime;

        //通过寻路组件的Move()方法实现寻路移动
        m_agent.Move(m_transform.TransformDirection(new Vector3(0, 0, speed)));
    }


    //敌人转向目标点函数
    void RotationTo()
    {
        //定义当前角度
        Vector3 oldAngle = m_transform.eulerAngles;
        //获得面向主角的角度
        m_transform.LookAt(m_player.transform);

        //定义目标的方向  Y轴方向  也就是敌人左右转动面向玩家
        float target = m_transform.eulerAngles.y;
        //转向目标的速度 等于时间乘以旋转角度
        float speed = m_rotSpeed * Time.deltaTime;
        //通过MoveTowardsAngle() 函数获得转的角度
        float angle = Mathf.MoveTowardsAngle(oldAngle.y, target, speed);

        //实现转向
        m_transform.eulerAngles = new Vector3(0, angle, 0);
    }

}
