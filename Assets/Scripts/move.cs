using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move : MonoBehaviour
{
    public float moveSpeed = 1f;
    Rigidbody m_Ridigbody;
    private Vector3 velocity;

    private void Start()
    {
        m_Ridigbody = GetComponent<Rigidbody>();
    }
    void Update()
    {
        float horizaotal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        velocity = new Vector3(horizaotal, 0, vertical).normalized;
    }

    private void FixedUpdate()
    {
        m_Ridigbody.velocity = velocity * moveSpeed * Time.fixedDeltaTime;
    }
}
