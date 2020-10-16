using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoveryMachine : MonoBehaviour
{
    public int essenceRequired; //复苏需要的量
    public int currentEssence; //现在的量
    public float transferRadius; //传输需要范围
    public bool canTransfer;//能够传输
    public bool canRecovery;//能够复苏
    public Transform player;//检测玩家
    public Transform skillPosition;//复苏机器本身
    void Start()
    {

    }

    void Update()
    {
        if (currentEssence >= essenceRequired)
        {
            canTransfer = false;
        }
        else
        {
            canTransfer = CanTransfer(player, skillPosition, transferRadius);
        }

    }

    //判断玩家能够进行传输的范围
    public bool CanTransfer(Transform player, Transform skillPosition, float radius)
    {
        float distance = Vector3.Distance(player.position, skillPosition.position);
        if (distance <= radius)
        {
            return true;
        }
        return false;
    }
    //范围可视化
    void OnDrawGizmosSelected()
    {
        if (skillPosition == null)
            skillPosition = transform;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(skillPosition.position, transferRadius);
    }
}
