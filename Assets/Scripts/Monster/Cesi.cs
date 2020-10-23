using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cesi : Monster
{
    public float speed;
    public float startWaitTime;
    private float waitTime;

    //下次移动位置
    public Transform movePos;
 
    // Start is called before the first frame update
    public void Start()
    {
        base.Start();
        
    }

    // Update is called once per frame
    public void Update()
    {
        base.Update();
    }

}
