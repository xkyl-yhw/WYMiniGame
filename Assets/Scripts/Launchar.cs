using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Launchar : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("welcome");
        PhotonNetwork.JoinOrCreateRoom("Room", new Photon.Realtime.RoomOptions() { MaxPlayers = 12 }, default);
    }

    public override void OnJoinedRoom()
    {
        //base.OnJoinedRoom();
        PhotonNetwork.Instantiate("Player", new Vector3(125, -20, 124), Quaternion.identity, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
