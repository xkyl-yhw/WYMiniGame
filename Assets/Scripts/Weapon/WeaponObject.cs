using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class WeaponObject : NetworkBehaviour
{
    private Animator anim;

    public string weaponName; //名字
    public int damage; //伤害值
    //public bool isRanged = false; //是否是远程，是=远程，否=近战
    public int strikingDistance; //攻击距离
    public int maxNumOfAmmo; //弹药数量上限
    private int currentAmmo; //现存弹药数量
    private bool isReload = false; //是否在装填弹药
    public float reloadCD; //装填时间
    private float reloadTime;
    private bool canShoot = true; //是否可以开火
    public bool isShoot = true; //是否正在开火
    public bool isBasy = false; //鼠标左键是否用来点击道具了；是=用了；否=没用，可以开抢
    public float shootCD = 0.2f;// 开火的间隔
    private float lastShootTime;

    public AudioClip reloadAudio; //换弹音效
    public AudioClip fireAudio; //开枪音效
    private AudioSource audioSource;
    private bool isReloadAudioPlay = false;

    public GameObject bulletPrefab;

    // Start is called before the first frame update
    void Start()
    {
        currentAmmo = maxNumOfAmmo;
        reloadTime = reloadCD;
        lastShootTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        //是否在换弹
        if (isReload)
        {
            canShoot = false;
            reloadTime = TimeCount(reloadTime);
            Debug.Log("换弹中");

            if (!isReloadAudioPlay)
            {
                audioSource = GetComponent<AudioSource>();
                audioSource.clip = reloadAudio;
                audioSource.volume = 0.8f;
                audioSource.Play();
                isReloadAudioPlay = true;
            }

            //当换弹时间结束后结束换弹装填，重置cd
            if (reloadTime<=0)
            {
                currentAmmo = maxNumOfAmmo;
                isReload = false;
                reloadTime = reloadCD;
                Debug.Log("换弹结束" + weaponName + "子弹数" + currentAmmo);
                isReloadAudioPlay = false;
                audioSource.volume = 0.4f;
            }
        }
        else
        {
            canShoot = true;
        }

        if (canShoot && !isBasy && Input.GetKeyDown(KeyCode.Mouse0) && Time.time - lastShootTime> shootCD)
        {
            lastShootTime = Time.time;
               currentAmmo -= 1;
            Debug.Log(weaponName + "子弹数" + currentAmmo);
            isShoot = true;
            GameObject grenade = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

            //开枪声
            audioSource = GetComponent<AudioSource>();
            audioSource.clip = fireAudio;
            audioSource.Play();

            //CmdFire();

            hitGround();
        }
        else
        {
            isShoot = false;
        }

        //当子弹为0时换弹
        if (currentAmmo <= 0)
        {
            isReload = true;
        }
    }

    public bool getIsShoot()
    {
        return canShoot;
    }

    public int getCurrentAmmo()
    {
        return currentAmmo;
    }

    float TimeCount(float timeUse)
    {
        timeUse -= Time.deltaTime;
        return timeUse;
    }

    public void hitGround()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray,out hit,200, 1 << LayerMask.NameToLayer("Ground")))
        {
            if (Vector3.Distance(hit.point, transform.parent.transform.position) < strikingDistance)
            {
                StartCoroutine(waitGrass(hit));
            }
        }
    }

    private IEnumerator waitGrass(RaycastHit hit)
    {
        yield return new WaitForSeconds(0.5f);
        hit.collider.gameObject.GetComponentInParent<HexGrid>().InfectCell(hit.point, GetComponentInParent<TeamSetup>().teamColor);

    }

    //[Command]
    //private void CmdFire()
    //{
    //    GameObject grenade = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
    //    NetworkServer.Spawn(grenade);
    //}
}
