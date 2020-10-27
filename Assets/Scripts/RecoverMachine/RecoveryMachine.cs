using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoveryMachine : MonoBehaviour
{
    public TeamTag teamTag;//对应出生点
    public int essenceRequired; //复苏需要的量
    public int currentEssence; //现在的量
    public float transferRadius; //传输需要范围
    public float recoveryRadius; //复苏以及处死怪物范围
    public bool canTransfer;//能够传输
    public bool canRecovery;//能够复苏
    public float timer;
    public float recoveryTimer;//复苏需要时间
    public bool isInSphere; //是否在范围
    public GameObject machine;//复苏机器本身位置
    public Monster[] monsters;//被杀死的怪物
    public Material M1, M2;
    void Start()
    {
        machine = gameObject;
        machine.name = teamTag.ToString() + "RecoveryMachine";

    }

    void Update()
    {
        if (currentEssence >= essenceRequired)
        {
            canTransfer = false;
            timer += Time.deltaTime;
            this.transform.GetComponent<Renderer>().material = M1;
            if (timer >= recoveryTimer)
            {
                canRecovery = true;
                timer = 0;
                currentEssence = 0;
                GameObject hexgrid = GameObject.FindGameObjectWithTag("HexGrid");
                for (int i = 0; i < hexgrid.GetComponent<HexGrid>().cells.Length; i++)
                {
                    if (Vector3.Distance(hexgrid.GetComponent<HexGrid>().cells[i].transform.position, machine.transform.position) < recoveryRadius)
                    {
                        hexgrid.GetComponent<HexGrid>().InfectCell(hexgrid.GetComponent<HexGrid>().cells[i].transform.position, TeamSetup.returnColor(teamTag));
                    }
                }
                canRecovery = false;
            }
            return;
        }
        canTransfer = true;

        //monsters = (Monster[])GameObject.FindObjectsOfType(typeof(Monster));

        //for (int i = 0; i < monsters.Length; i++)
        //{
        //    isInSphere = InRecoverySphere(monsters[i].transform, machine.transform, recoveryRadius);
        //    monsters[i].inRecoverySphere = isInSphere;
        //    if (isInSphere)
        //    {
        //        monsters[i].objectMachine = machine;
        //    }
        //    else
        //    {
        //        monsters[i].objectMachine = null;
        //    }
        //}

    }


    ////判断玩家能够进行传输的范围
    //public bool CanTransfer(Transform player, Transform machine, float radius)
    //{
    //    float distance = Vector3.Distance(player.position, machine.position);
    //    if (distance <= radius)
    //    {
    //        return true;
    //    }
    //    return false;
    //}
    public bool InRecoverySphere(Transform monster, Transform machine, float radius)
    {
        float distance = Vector3.Distance(monster.position, machine.position);
        if (distance <= radius)
        {
            return true;
        }
        return false;
    }
    //范围可视化
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(machine.transform.position, transferRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(machine.transform.position, recoveryRadius);
    }
}
