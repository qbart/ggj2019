using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueBooster : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            Debug.Log("Apply booster to player here");
            Destroy(gameObject);
        }
    }
}
