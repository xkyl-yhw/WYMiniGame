using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ThirdWeapon : MonoBehaviour
{
    public WeaponController weaponController;
    public GameObject thirdWeapon;
    public int index;
    public List<GameObject> mList = new List<GameObject>();
    public Image source;
    public Sprite thirdWeaponIcon;
    public string iconPath;
    public string third;
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {
        mList = weaponController.mList;
        index = weaponController.weaponIndex;
        thirdWeapon = mList[(index + 2)%3];
        third = "small" + thirdWeapon.name;
        iconPath = "WeaponIcon/" + third;
        thirdWeaponIcon = Resources.Load<Sprite>(iconPath);
        source.sprite = thirdWeaponIcon;
        Debug.Log(thirdWeaponIcon.name);
    }
}
