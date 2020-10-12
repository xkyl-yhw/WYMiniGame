using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public GameObject weapon;
    private WeaponObject weaponObject;
    private float strikingDistance;

    //public GameObject bulletPrefab;

    // Start is called before the first frame update
    void Start()
    {
        weaponObject = weapon.GetComponent<WeaponObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if(weaponObject.isShoot)
        {
            Debug.Log("射击");
        }

        strikingDistance = weaponObject.strikingDistance;
    }

    public float getStrikingDistance()
    {
        return strikingDistance;
    }
}
