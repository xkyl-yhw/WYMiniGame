using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SecondWeapon : MonoBehaviour
{
    public WeaponController weaponController;
    public GameObject secondWeapon;
    public int index;
    public List<GameObject> mList = new List<GameObject>();
    public Image source;
    public Sprite secondWeaponIcon;
    public string iconPath;
    public string second;
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {
        mList = weaponController.mList;
        index = weaponController.weaponIndex;
        if(index<2)
        {
            secondWeapon = mList[index + 1];
        }
        else
        {
            secondWeapon = mList[0];
        }
        second = "small" + secondWeapon.name;
        iconPath = "WeaponIcon/" + second;
        secondWeaponIcon = Resources.Load<Sprite>(iconPath);
        source.sprite = secondWeaponIcon;
        //Debug.Log(secondWeaponIcon.name);
    }
}
