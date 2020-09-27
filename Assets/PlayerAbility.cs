using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbility : MonoBehaviour
{
    public Color infectColor;
    public float rayDis = 3f;

    private void Update()
    {
        RaycastHit hit;
        Debug.DrawRay(transform.position, -transform.up * rayDis, Color.red);
        if (Physics.Raycast(transform.position, -transform.up * rayDis, out hit, rayDis))
        {
            hit.collider.transform.parent.GetComponent<HexGrid>().InfectCell(hit.point, infectColor);
        }
    }
}
