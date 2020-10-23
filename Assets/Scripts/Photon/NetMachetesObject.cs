using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class NetMachetesObject : MonoBehaviourPun
{
    private Animator anim;

    public string weaponName; //名字
    public int damage; //伤害值
    //public bool isRanged = false; //是否是远程，是=远程，否=近战
    public int strikingDistance; //攻击距离
    private bool canShoot = true; //是否可以开火
    public bool isShoot = true; //是否正在开火
    public bool isBasy = false; //鼠标左键是否用来点击道具了；是=用了；否=没用，可以开抢

    public bool isDamage = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (canShoot && !isDamage && Input.GetKeyDown(KeyCode.Mouse0))
        {
            isDamage = true;
            canShoot = false; // 当动画结束的时候设置这个值为true
        }
    }

    public void Using()
    {
        //共计动画
        anim.SetTrigger("Attack");
    }
}
