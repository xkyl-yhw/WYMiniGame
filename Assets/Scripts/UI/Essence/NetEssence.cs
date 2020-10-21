using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
public class NetEssence : MonoBehaviourPun, IPunObservable
{
    public Image img;
    // Use this for initialization
    [SerializeField]
    public Text essenceText;
    public GameObject player;
    [SerializeField]
    public NetPlayerAttribute playerAttribute;
    public float upEssence;
    [SerializeField]
    public float currentEssence;
    void Start()
    {
        playerAttribute = player.GetComponent<NetPlayerAttribute>();
        img = GetComponentInChildren<Image>();    //获取Image组件
        essenceText = GetComponentInChildren<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        currentEssence = playerAttribute.essencePickNum;
        img.fillAmount = currentEssence / upEssence;
        essenceText.text = currentEssence.ToString() + "/" + upEssence.ToString();

    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(currentEssence);
            //stream.SendNext(essenceText);
            stream.SendNext(img.fillAmount);
        }
        else
        {
            // Network player, receive data
            currentEssence = (float)stream.ReceiveNext();
            img.fillAmount = (float)stream.ReceiveNext();
            //essenceText = (Text)stream.ReceiveNext();
        }
    }
}
