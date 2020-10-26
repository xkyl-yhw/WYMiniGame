using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Icon : MonoBehaviour
{
    public SpriteRenderer icon;
    public PlayerAttribute playerAttribute;
    void Start()
    {
        icon.sprite = playerAttribute.head;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
