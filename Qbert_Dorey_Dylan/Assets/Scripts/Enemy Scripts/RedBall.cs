using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Author: [Dorey, Dylan]
 * Last Updated: [3/08/2024]
 * [A ball that moves down the pyramid and kills the enemy when colliding with them]
 */

public class RedBall : Enemy
{
    //if the enemy is moving right or left
    private bool movingRight;

    public override void Start()
    {
        //intialize enemy values
        InitializeEnemy(true, false, 1.25f, 0);

        base.Start();

        //if starting right set moving right to true
        if (startRight)
        {
            movingRight = true;
        }
        else
        {
            //otherwise set moving right to false
            movingRight = false;
        }

        //start switching move directions
        StartCoroutine(SwitchDirections());
    }

    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        //if the other game object is the player
        if (other.gameObject.CompareTag("Player"))
        {
            //publish the player event lost life and call enemy on death function
            PlayerEventBus.Publish(PlayerEvent.lostLife);
            OnDeath();
        }
    }

    /// <summary>
    /// Switches movement directions from right to left or vice versa based on the enemies speed value
    /// </summary>
    /// <returns> the time between movements </returns>
    private IEnumerator SwitchDirections()
    {
        //while the enemy is alive
        while (isAlive)
        {
            //if moving right
            if (movingRight)
            {
                //set move direction to left movement and set moving right to false
                moveDirection = new Vector3(-0.7f, -1f, -0.7f);
                movingRight = false;
            }
            else
            {
                //otherwise set move direction to right movement and set moving right to true
                moveDirection = new Vector3(0.7f, -1f, -0.7f);
                movingRight = true;
            }

            //wait the enemies speed value
            yield return new WaitForSeconds(enemySpeed);
        }
    }
}
