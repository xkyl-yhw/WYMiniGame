using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbility : MonoBehaviour
{
    public HexGrid hexGrid;
    public Color infectColor;
    public float rayLength = 3f;
    public float downLength = 2;

    private void Start()
    {
        infectColor = TeamSetup.returnColor(GetComponent<TeamSetup>().teamTag);
    }

    private void Update()
    {
        RaycastHit hit;
        Vector3 dir = Vector3.forward;
        for (int i = 0; i < 6; i++)
        {
            if (Physics.Raycast(transform.position, (dir + Vector3.down * downLength).normalized * rayLength, out hit))
            {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
                    hexGrid.InfectCell(hit.point, infectColor);
            }
            dir = Quaternion.Euler(new Vector3(0, 60, 0)) * dir;
            Debug.DrawRay(transform.position, (dir + Vector3.down).normalized * rayLength);
        }
        if (Physics.Raycast(transform.position, Vector3.down * rayLength, out hit))
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
                hexGrid.InfectCell(hit.point, infectColor);
        }
        Debug.DrawRay(transform.position, Vector3.down * rayLength);
    }
}
