using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using Mirror;

public class WeaponController : NetworkBehaviour
{
    Animator anim;
    public GameObject weapon;

    public int weaponIndex;
    public string weaponType; //Gun,Machete,Bomb
    //public int gunNum = 10;
    //public int macheteNum = 10;
    //public int bombNum = 10;

    public float SwitchWeaponCD = 1f;
    public float SwitchWeaponCD2 = 0.5f;
    private float mSwitchWeapon;
    private float mSwitchWeapon2;
    private float AxisCounts;
    private bool isScroll;
    private bool isScrollCD;

    public Vector3 weaponPosition = new Vector3(1.8f, 0f, 1.0f);
    public Vector3 weaponRotationGun = new Vector3(1.8f, 0f, 1.0f);
    public Vector3 weaponRotationMachete = new Vector3(1.8f, 0f, 1.0f);

    public List<GameObject> mList = new List<GameObject>(); //武器列表

    // Start is called before the first frame update
    void Start()
    {
        mSwitchWeapon = 0;
        mSwitchWeapon2 = 0;
        AxisCounts = 0;
        isScroll = false;
        isScrollCD = false;

        weaponType = "Gun";
        weaponIndex = 0;
        anim = this.GetComponent<Animator>();

        AnimationEvent aniEvt = new AnimationEvent();
        aniEvt.functionName = "SwitchWeaponAnim";
        aniEvt.time = 0.8f;
        foreach (AnimationClip clip in anim.runtimeAnimatorController.animationClips)
        {
            Debug.Log(clip);
            if (clip.name == "收武器")
            {
                //Debug.Log("Attack长度" + clip.length);
                clip.AddEvent(aniEvt);
            }
        }
        AnimationEvent aniEvt2 = new AnimationEvent();
        aniEvt2.functionName = "SwitchWeaponAnim2";
        aniEvt2.time = 0.7f;
        foreach (AnimationClip clip in anim.runtimeAnimatorController.animationClips)
        {
            Debug.Log(clip);
            if (clip.name == "拿武器")
            {
                //Debug.Log("Attack长度" + clip.length);
                clip.AddEvent(aniEvt2);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer) return;
        float MouseScrollWheel = Input.GetAxis("Mouse ScrollWheel"); // 滚轮角度
        if (Input.GetAxis("Mouse ScrollWheel") != 0 && !isScroll && !isScrollCD)
        {
            isScroll = true;
        }
        if (isScroll)
        {
            MouseScrollWheelAxisCounts(MouseScrollWheel);
            if (AxisCounts > 0.3 || AxisCounts < -0.3)
            {
                isScroll = false;
                isScrollCD = true;
                anim.SetTrigger("isSwitch2");
                this.GetComponent<PlayController>().enabled = false;
                //anim.SetFloat("WalkX", 0.5f);
                //anim.SetFloat("WalkY", 1f);
            }
        }
        if (isScrollCD)
        {
            mSwitchWeapon2 += Time.deltaTime;
            if (mSwitchWeapon2 > SwitchWeaponCD2)
            {
                isScrollCD = false;
                mSwitchWeapon2 = 0;
            }
        }

        //当炸弹投出去的时候切换武器
        if (weaponType == "Bomb")
        {
            anim.SetFloat("RunX", 1);
            BombObject bombObject = weapon.GetComponent<BombObject>();
            if (bombObject.isSwitchWeapon)
            {
                SwitchWeapon(1);
                anim.SetTrigger("isSwitch1");
                this.GetComponent<PlayController>().enabled = false;
            }
        }
        else if (weaponType == "Gun")
        {
            anim.SetFloat("RunX", 0);
        }
        else if (weaponType == "Machete")
        {
            anim.SetFloat("RunX", 0.5f);
        }
    }

    void SwitchWeaponAnim()
    {
        anim.SetBool("isIdle",true);
        Destroy(weapon);
        SwitchWeapon(AxisCounts);

        anim.SetTrigger("isSwitch1");
        //anim.SetFloat("WalkX", 0.5f);
        //anim.SetFloat("WalkY", 0.75f);
    }

    void SwitchWeaponAnim2()
    {
        anim.SetBool("isIdle", true);
        this.GetComponent<PlayController>().enabled = true;
        if (weaponType == "Gun")
        {
            weapon.transform.localRotation = Quaternion.Euler(weaponRotationGun);
        }
        if (weaponType == "Machete")
        {
            weapon.transform.localRotation = Quaternion.Euler(weaponRotationMachete);
        }
    }


    // 滚轮控制武器切换
    private void SwitchWeapon(float MouseScrollWheel)
    {
        AxisCounts = 0;

        if (MouseScrollWheel > 0)
            NextWeapon();
        if (MouseScrollWheel < 0)
            PreviousWeapon();

        CmdWeapon();
    }

    [Command]
    private void CmdWeapon()
    {
        weapon = Instantiate(mList[weaponIndex]);
        if (weaponIndex == 2) weapon.GetComponent<BombObject>().playerTeam = GetComponentInParent<TeamSetup>();
        NetworkServer.Spawn(weapon);
        weaponType = GetWeaponType(weaponIndex);

        weapon.transform.parent = GetChild(this.transform,"Bip001 R Hand");
        weapon.transform.localPosition = weaponPosition;
        weapon.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
    }

    public void NextWeapon()//下一个武器
    {
        weaponIndex++;
        if (weaponIndex > mList.Count - 1)
        {
            weaponIndex = 0;
        }
    }

    public void PreviousWeapon()//上一个武器
    {
        weaponIndex--;
        if (weaponIndex < 0)
        {
            weaponIndex = mList.Count - 1;
        }
    }

    // 计算每次开始滚滚轮，一秒内鼠标滚轮的角度
    private void MouseScrollWheelAxisCounts(float MouseScrollWheel)
    {
        if (mSwitchWeapon > SwitchWeaponCD)
        {
            mSwitchWeapon = 0;
            AxisCounts = 0;
            isScroll = false;
        }
        else
        {
            mSwitchWeapon += Time.deltaTime;
            AxisCounts += MouseScrollWheel;
        }
    }

    private string GetWeaponType(int i)
    {
        // 获取GameObject名称
        string name = mList[i].name;
        Regex re = new Regex(@"[a-zA-Z]+");
        Match m = re.Match(name);
        string fixtype = m.Value;

        //Debug.Log(fixtype);
        return fixtype;
    }

    public static Transform GetChild(Transform parentTF, string childName)
    {
        //在子物体中查找
        Transform childTF = parentTF.Find(childName);

        if (childTF != null)
        {
            return childTF;
        }
        //将问题交由子物体
        int count = parentTF.childCount;
        for (int i = 0; i < count; i++)
        {
            childTF = GetChild(parentTF.GetChild(i), childName);
            if (childTF != null)
            {
                return childTF;
            }
        }
        return null;
    }

    //检测是否完成了一次砍刀攻击
    public void SetMachetesIsDamage()
    {
        weapon.GetComponent<MachetesObject>().isDamage = false;
        anim.SetBool("isIdle", true);
    }

    //检测砍刀动画中途是否打中人
    public void CheckDamage()
    {
        Vector3 a = weapon.transform.GetChild(0).transform.position;
        foreach (Collider b in Physics.OverlapSphere(a,weapon.GetComponent<MachetesObject>().strikingDistance))
        {
            if(b.gameObject.tag == "Player" && b.gameObject != this.gameObject) // 对玩家造成伤害
            {
                PlayerHealth playerHealth = b.gameObject.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    Debug.Log("砍刀正在对玩家造成伤害");
                    playerHealth.DamagePlayer(weapon.GetComponent<MachetesObject>().damage);
                }
            }
            else if (b.gameObject.tag == "Monster") // 对怪物造成伤害
            {
                if (b.gameObject.GetComponent<Monster>().health > 0)
                {
                    Debug.Log("砍刀正在对怪物造成伤害");
                    b.gameObject.GetComponent<Monster>().TakeDamage(weapon.GetComponent<MachetesObject>().damage);
                }
            }
        }

    }
}
