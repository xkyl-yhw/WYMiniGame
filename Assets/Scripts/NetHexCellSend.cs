using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NetHexCellSend : MonoBehaviourPun, IPunObservable
{
    [SerializeField]
    public Dictionary<int, GameObject> grassCastDict = new Dictionary<int, GameObject>();
    public Color playerColor;

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
            stream.SendNext(grassCastDict);
            stream.SendNext(playerColor);
        }
        else
        {
            grassCastDict = (Dictionary<int, GameObject>)stream.ReceiveNext();
            playerColor = (Color)stream.ReceiveNext();
        }
    }
}
