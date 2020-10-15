using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoveryMachine : MonoBehaviour
{
    public int essenceRequired; //复苏需要的量
    public float radius; //复苏需要范围
    public bool canRecovery;
    public Transform player;
    public Transform skillPosition;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        canRecovery = CircleAttack( player, skillPosition, radius);
    }

    public bool CircleAttack(Transform player, Transform skillPosition, float radius)
    {
        float distance = Vector3.Distance(player.position, skillPosition.position);
        if (distance < radius)
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
        Gizmos.DrawWireSphere(skillPosition.position, radius);
    }
}
