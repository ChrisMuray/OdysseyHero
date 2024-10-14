using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannonball : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Cannonball>(out Cannonball cannonball)) return;
        Destroy(gameObject);
    }
}
