using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class NetPlayerAttribute : MonoBehaviourPun, IPunObservable
{sss
    [SerializeField]
    public float health;
    [SerializeField]
    public float healthMax;
    [SerializeField]
    public float endurance;
    [SerializeField]
    public int enduranceMax;
    [SerializeField]
    public int essencePickNum;
    [SerializeField]
    public int essenceMax;
    [SerializeField]
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
            stream.SendNext(healthMax);
            stream.SendNext(endurance);
            stream.SendNext(enduranceMax);
            stream.SendNext(essenceRate);
        }
        else
        {
            health = (float)stream.ReceiveNext();
            essencePickNum = (int)stream.ReceiveNext();
            healthMax = (float)stream.ReceiveNext();
            endurance = (float)stream.ReceiveNext();
            enduranceMax = (int)stream.ReceiveNext();
            essenceRate = (float)stream.ReceiveNext();
        }
    }
}
