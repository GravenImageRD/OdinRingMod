using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PursueTarget : MonoBehaviour
{
    bool seeking = true;
    float startTime;
    public GameObject target;
    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
        rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.up;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == target)
        {
            seeking = false;
            startTime = Time.time;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!seeking)
        {
            if ((Time.time - startTime) > 1f)
            {
                GameObject.Destroy(this);
            }
            return;
        }

        float t = Time.time - startTime;
        Vector3 vectorToTarget = target.transform.position - transform.position;
        rb.velocity += vectorToTarget * t * t;
    }
}
