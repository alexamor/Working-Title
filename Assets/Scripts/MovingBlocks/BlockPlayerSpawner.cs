using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockPlayerSpawner : MonoBehaviour {

    bool col = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        col = true;

        Debug.Log("collided");
    }

    private void Start()
    {
        Debug.Log("col " + col);
    }
}
