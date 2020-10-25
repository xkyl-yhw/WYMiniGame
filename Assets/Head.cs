using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Head : MonoBehaviour
{
    public Image head;
    public GameObject player;
    public PlayerAttribute playerAttribute;
    void Start()
    {
        playerAttribute = player.GetComponent<PlayerAttribute>();
    }

    // Update is called once per frame
    void Update()
    {
        head.sprite = playerAttribute.head;
    }
}
