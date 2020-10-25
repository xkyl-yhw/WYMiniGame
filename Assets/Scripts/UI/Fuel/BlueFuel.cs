using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BlueFuel : MonoBehaviour
{
    public Image img;
    // Use this for initialization
    public GameObject machine;
    public RecoveryMachine recoveryMachine;
    public float essenceMax;
    public float currentEssence;
    void Start()
    {
        machine = GameObject.Find("blueRecoveryMachine");
        recoveryMachine = machine.GetComponent<RecoveryMachine>();
        //img = GetComponent<Image>();    //获取Image组件
    }

    // Update is called once per frame
    void Update()
    {
        essenceMax = recoveryMachine.essenceRequired;
        currentEssence = recoveryMachine.currentEssence;
        img.fillAmount = currentEssence / essenceMax;

    }
}
