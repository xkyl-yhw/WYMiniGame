using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachetesObject : MonoBehaviour
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

    private float lastAttackTime;

    // Start is called before the first frame update
    void Start()
    {
        anim = this.transform.GetComponentInParent<Animator>(); 
        
        AnimationEvent aniEvt = new AnimationEvent();
        aniEvt.functionName = "SwitchWeaponAnim";
        aniEvt.time = 1f;
        foreach (AnimationClip clip in anim.runtimeAnimatorController.animationClips)
        {
            Debug.Log(clip);
            if (clip.name == "收武器")
            {
                //Debug.Log("Attack长度" + clip.length);
                clip.AddEvent(aniEvt);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDamage && Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (Time.time - lastAttackTime > anim.GetCurrentAnimatorStateInfo(0).length)
            {
                Using();
                isDamage = true;
            }
        }
    }

    public void Using()
    {
        lastAttackTime = Time.time;
        anim.SetTrigger("MachetesAttack");
    }
}
