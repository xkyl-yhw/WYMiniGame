using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetWork : MonoBehaviour
{
    public GameObject gunPrefab;
    public GameObject maPrefab;
    public GameObject bombPrefab;
    public GameObject bulletPrefab;
    //public GameObject test;
    // Register prefab and connect to the server  
    public void ClientConnect()
    {
        ClientScene.RegisterPrefab(gunPrefab);
        ClientScene.RegisterPrefab(maPrefab);
        ClientScene.RegisterPrefab(bombPrefab);
        ClientScene.RegisterPrefab(bulletPrefab);
        //ClientScene.RegisterPrefab(test);
        NetworkClient.RegisterHandler<ConnectMessage>(OnClientConnect);
        NetworkClient.Connect("localhost");
    }

    void OnClientConnect(NetworkConnection conn, ConnectMessage msg)
    {
        Debug.Log("Connected to server: " + conn);
    }

    public void ServerListen()
    {
        NetworkServer.RegisterHandler<ConnectMessage>(OnServerConnect);
        NetworkServer.RegisterHandler<ReadyMessage>(OnClientReady);

        // start listening, and allow up to 4 connections
        NetworkServer.Listen(4);
    }

    // When client is ready spawn a few trees  
    void OnClientReady(NetworkConnection conn, ReadyMessage msg)
    {
        Debug.Log("Client is ready to start: " + conn);
        NetworkServer.SetClientReady(conn);
        //SpawnTrees();
    }

    //void SpawnTrees()
    //{
    //    int x = 0;
    //    for (int i = 0; i < 5; ++i)
    //    {
    //        GameObject treeGo = Instantiate(treePrefab, new Vector3(x++, 0, 0), Quaternion.identity);
    //        NetworkServer.Spawn(treeGo);
    //    }
    //}

    void OnServerConnect(NetworkConnection conn, ConnectMessage msg)
    {
        Debug.Log("New client connected: " + conn);
    }
}
