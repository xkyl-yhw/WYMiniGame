using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public GameObject weapon;
    private WeaponObject weaponObject;
    private float strikingDistance;

    private int weaponIndex;
    public int weaponNum = 10;

    public float SwitchWeaponCD = 1f;
    public float SwitchWeaponCD2 = 0.5f;
    private float mSwitchWeapon;
    private float mSwitchWeapon2;
    private float AxisCounts;
    private bool isScroll;
    private bool isScrollCD;

    public Vector3 weaponPosition = new Vector3(1.8f, 0f, 1.0f);

    //public GameObject bulletPrefab;

    // Start is called before the first frame update
    void Start()
    {
        weaponObject = weapon.GetComponent<WeaponObject>();
        mSwitchWeapon = 0;
        mSwitchWeapon2 = 0;
        AxisCounts = 0;
        isScroll = false;
        isScrollCD = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (weaponObject.isShoot)
        {
            Debug.Log("射击");
        }

        strikingDistance = weaponObject.strikingDistance;

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
                SwitchWeapon(AxisCounts);
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
    }

    // 提供攻击距离
    public float getStrikingDistance()
    {
        return strikingDistance;
    }

    // 滚轮控制武器切换
    private void SwitchWeapon(float MouseScrollWheel)
    {
        Destroy(weapon);

        AxisCounts = 0;

        if (MouseScrollWheel > 0)
            NextWeapon();
        if (MouseScrollWheel < 0)
            PreviousWeapon();

        GameObject weaponResource = Resources.Load<GameObject>("Weapons/Gun" + weaponIndex);
        if (weaponResource != null)
        {
            weapon = Instantiate(weaponResource);

            weapon.transform.parent = this.transform;
            weapon.transform.localPosition = weaponPosition;
            weapon.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            //weapon.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        }
    }

    public void NextWeapon()//下一个武器
    {
        weaponIndex++;
        if (weaponIndex > weaponNum - 1)
            weaponIndex = 0;
    }

    public void PreviousWeapon()//上一个武器
    {
        weaponIndex--;
        if (weaponIndex < 0)
            weaponIndex = weaponNum - 1;
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
}
