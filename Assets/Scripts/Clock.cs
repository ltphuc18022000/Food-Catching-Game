using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clock : MonoBehaviour
{
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        //rb.AddForce(transform.right * 10f * Time.deltaTime, ForceMode.Impulse);
        rb.velocity = new Vector3(Random.Range(-20f, -15f), rb.velocity.y, 0f);
    }

    void Awake()
    {
        //GetComponent<Rigidbody>().AddForce(new Vector3(0, -10f, 0), ForceMode.Force);
    }

    void Update()
    {
        rb.velocity = new Vector3(Random.Range(-6f, -5f), rb.velocity.y, 0f);
    }
}
