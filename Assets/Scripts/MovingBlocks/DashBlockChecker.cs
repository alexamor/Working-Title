using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashBlockChecker : MonoBehaviour {

    private int[] dir;
    public GameObject trigger;

    private void Awake()
    {
        dir = new int[2];

        //Get which side this collider is on
        if (this.GetComponent<Collider2D>().offset.x > 0)
            dir[0] = 1;
        else if (this.GetComponent<Collider2D>().offset.x == 0)
            dir[0] = 0;
        else
            dir[0] = -1;

        if (this.GetComponent<Collider2D>().offset.y > 0)
            dir[1] = 1;
        else if (this.GetComponent<Collider2D>().offset.y == 0)
            dir[1] = 0;
        else
            dir[1] = -1;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        trigger.GetComponent<DashBlock>().playerCol[dir[0] + 1, dir[1] + 1] = 1;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        trigger.GetComponent<DashBlock>().playerCol[dir[0] + 1, dir[1] + 1] = 0;
    }
}
