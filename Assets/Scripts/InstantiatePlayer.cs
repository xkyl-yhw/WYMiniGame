using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiatePlayer : MonoBehaviour
{
    public TeamTag precentTag;
    public GameObject playerPrefebs;
    public GameObject OccupyPoint;

    void Update()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
                {
                    GameObject go = Instantiate(playerPrefebs, hit.point + Vector3.up, Quaternion.identity);
                    go.GetComponent<TeamSetup>().teamTag = precentTag;
                }
                if (hit.collider.gameObject.tag == "Player")
                {
                    OccupyPoint.GetComponent<OccupationPos>().delePlayer(hit.collider.gameObject);
                    DestroyImmediate(hit.collider.gameObject);
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            precentTag = TeamTag.red;
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            precentTag = TeamTag.green;
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            precentTag = TeamTag.blue;
        }
    }
}
