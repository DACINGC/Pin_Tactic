using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopGlass : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Rope"))
            return;

        Destroy(collision.gameObject);
    }
}
