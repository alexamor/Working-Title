using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DashBlock : MonoBehaviour {

    public float velocity = 5;
    private bool onTheMove = false;
    private GameObject player = null;
    [HideInInspector]
    public int[,] playerCol;
    private int[] dir;

    private float[] triggerDim;

    private void Start()
    {
        playerCol = new int[3,3];

        triggerDim = new float[2];
        triggerDim[0] = transform.GetComponent<Collider2D>().bounds.extents.x;
        triggerDim[1] = transform.GetComponent<Collider2D>().bounds.extents.y;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {

        //if player dashes into object
        if (collision.gameObject.tag == "Player")
        {
            //Debug.Log("Player colided");
            player = collision.gameObject;

            if (player.GetComponent<PlayerMovement>().dashTime > 0)
            {
                dir = player.GetComponent<PlayerMovement>().dashDir;

                RaycastHit2D hit;
                hit = Physics2D.Raycast(new Vector2(triggerDim[0] * dir[0] + this.transform.position.x, triggerDim[1] * dir[1] + this.transform.position.y), (Vector2.right * dir[0] + Vector2.up * dir[1]), 0.1f);

                //Debug.Log(hit.collider.name);

                //Only Shoot Block if block has no obstructions
                if(hit == false || hit.collider.tag != "Obs")
                {
                    ShootBlock();
                }
                         
            }

        }
    }

    private void FixedUpdate()
    {
        if (onTheMove == true)
        {
            //Debug.Log("dir " + dir[0] + " " + dir[1]);

            player.transform.position = this.transform.position;

            //If it has stopped
            if ((Mathf.Abs(transform.parent.GetComponent<Rigidbody2D>().velocity.x) < 0.5 && dir[0] != 0) || (Mathf.Abs(transform.parent.GetComponent<Rigidbody2D>().velocity.y) < 0.5 && dir[1] != 0))
            {
                onTheMove = false;

                //Deactivate Trigger collider to avoid bugs
                this.GetComponent<Collider2D>().enabled = false;

                //Raycast for collider in dash direction
                RaycastHit2D hit;
                hit = Physics2D.Raycast(new Vector2((triggerDim[0] + 0.01f) * dir[0] + this.transform.position.x, (triggerDim[1] + 0.01f) * dir[1] + this.transform.position.y), (Vector2.right * dir[0] + Vector2.up * dir[1]), 0.1f);

                Debug.Log("land " + hit.collider.name);
                
                //If it can project player in correct direction
                if (hit.collider.tag != "Obs")
                {
                    player.transform.position = new Vector3(this.transform.position.x + dir[0] * (triggerDim[0] + 0.17f + 0.11f), this.transform.position.y + dir[1] * (triggerDim[1] + 0.51f + 0.11f), 0);
                    player.SetActive(true);

                    //Add momentum
                    player.GetComponent<PlayerMovement>().lauched = true;
                    player.GetComponent<PlayerMovement>().dashDir = dir;
                }
                //If not simply spawn player on opposite site
                else
                {
                    player.transform.position = new Vector3(this.transform.position.x - dir[0] * (triggerDim[0] + 0.17f + 0.11f), this.transform.position.y - dir[1] * (triggerDim[1] + 0.51f + 0.11f), 0);
                    player.SetActive(true);
                }

                //Turn collider on again
                this.GetComponent<Collider2D>().enabled = true;
            }
        }
        
    }

    void ShootBlock()
    {
        //diagonal movement
        if (Mathf.Abs(dir[0]) == 1 && Mathf.Abs(dir[1]) == 1)
            transform.parent.GetComponent<Rigidbody2D>().velocity = new Vector3(velocity * 0.71f * dir[0], velocity * 0.71f * dir[1], 0);
        else
            transform.parent.GetComponent<Rigidbody2D>().velocity = new Vector3(velocity * dir[0], velocity * dir[1], 0);

        player.SetActive(false);

        onTheMove = true;
    }
}
