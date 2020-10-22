using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Launchar : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    object[] instantiationData;

    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("welcome");

        PhotonNetwork.JoinOrCreateRoom("Room", new Photon.Realtime.RoomOptions() { MaxPlayers = 12 }, default);
        //PhotonNetwork.Instantiate("NetMechanics", new Vector3(142, -20, 167), Quaternion.identity, 0);
    }
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        object[] instantiationData = info.photonView.InstantiationData;
        // ...
    }
    public override void OnJoinedRoom()
    {
        //base.OnJoinedRoom();
        PhotonNetwork.Instantiate("NetPlayer", new Vector3(125, -20, 124), Quaternion.identity, 0, instantiationData);
        //PhotonNetwork.LoadLevel(0);
        // PhotonNetwork.LoadLevel(1);
        //PhotonNetwork.LoadLevel(1);
    }

    // Update is called once per frame

    void Update()
    {
        
    }
}
