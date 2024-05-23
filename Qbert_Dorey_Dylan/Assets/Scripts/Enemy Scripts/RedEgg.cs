using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Author: [Dorey, Dylan]
 * Last Updated: [3/13/2024]
 * [A red egg enemy that will bounce down the pyramid and fall off the bottom]
 */

public class RedEgg : Enemy
{
    //if the enemy is moving right or left
    private bool movingRight;

    public override void Start()
    {
        //intialize enemy values
        InitializeEnemy(true, false, 1.5f, 0);

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
            //set the players death position to their current position and remove a life from the player
            PlayerController.Instance.deathPos = transform.position;
            PlayerEventBus.Publish(PlayerEvent.lostLife);

            //call the on death function
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
                moveDirection = MoveDown;
                movingRight = false;
            }
            else
            {
                //otherwise set move direction to right movement and set moving right to true
                moveDirection = MoveRight;
                movingRight = true;
            }

            //wait the enemies speed value
            yield return new WaitForSeconds(enemySpeed);
        }
    }
}
