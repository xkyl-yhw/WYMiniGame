using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NowWeapon : MonoBehaviour
{
    public WeaponController weaponController;
    public Image source;
    public Sprite nowWeapon;
    public string iconPath;
    public string now;
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {
        now = "Big" + weaponController.weaponType;
        iconPath = "WeaponIcon/"+now;
        nowWeapon = Resources.Load<Sprite>(iconPath);
        source.sprite = nowWeapon;
        //Debug.Log(nowWeapon.name);
    }
}
