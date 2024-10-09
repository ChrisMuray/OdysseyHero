using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleEnemy : MonoBehaviour
{

    private void Awake()
    {
        // Destroy on death
        GetComponent<Health>().OnDeath += (context, e) => Destroy(gameObject);
    }
}
