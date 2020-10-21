using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
public class BloodToFront : MonoBehaviourPun, IPunObservable
{
    public NetPlayerAttribute playerAttribute;
    public GameObject player;
    public Image img;
    [SerializeField]
    public float currentHealth;
    [SerializeField]
    public float healthMax;
    void Start()
    {
        currentHealth = healthMax;
        playerAttribute = player.GetComponent<NetPlayerAttribute>();
        //img = GetComponent<Image>();    //获取Image组件
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Camera.main.transform.rotation;
        currentHealth = playerAttribute.health;
        img.fillAmount = currentHealth / healthMax;
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(currentHealth);
            stream.SendNext(healthMax);
            //stream.SendNext(img.fillAmount);
        }
        else
        {
            // Network player, receive data
            currentHealth = (float)stream.ReceiveNext();
            healthMax = (float)stream.ReceiveNext();
            //img.fillAmount = (float)stream.ReceiveNext();
        }
    }
}
