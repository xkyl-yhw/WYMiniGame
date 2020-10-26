using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoragePlayerMsg : MonoBehaviour
{
    public Sprite head;
    public string playerName;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
