using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveForwardAndDie : MonoBehaviour
{

    public float speed = 10f;
    public float lifetime = 10f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.position += speed * transform.forward * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }
}
