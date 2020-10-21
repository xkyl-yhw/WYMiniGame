using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class NetPlayerAttribute : MonoBehaviourPun, IPunObservable
{
    [SerializeField]
    public float health;

    public float endurance;

    public int enduranceMax;
    [SerializeField]
    public int essencePickNum;

    public float essenceRate;


    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine && PhotonNetwork.IsConnected)//如果观察不是当前角色以及网络连接上
        {
            return;
        }


    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(health);
            stream.SendNext(essencePickNum);
        }
        else
        {
            health = (float)stream.ReceiveNext();
            essencePickNum = (int)stream.ReceiveNext();
        }
    }
}
