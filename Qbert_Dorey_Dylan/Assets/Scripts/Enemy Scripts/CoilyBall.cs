using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Author: [Dorey, Dylan]
 * Last Updated: [3/20/2024]
 * []
 */

public class CoilyBall : Enemy
{
    //if the enemy is moving right or left
    private bool movingRight;

    //Coily snake prefab
    public GameObject coilySnake;

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
            //set the players death position and publish the player event lost life
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

            //if coily ball reaches the bottom of the pyramid
            if (transform.position.y < -1f)
            {
                //switch states to the snake
                SwitchState();
            }

            //wait the enemies speed value
            yield return new WaitForSeconds(enemySpeed);
        }
    }

    /// <summary>
    /// switches the state of coily from ball form to snake form
    /// </summary>
    private void SwitchState()
    {
        StartCoroutine(SwitchStateDelay());
    }

    /// <summary>
    /// adds a delay when switching from ball to snake form
    /// </summary>
    /// <returns> time to wait before switching states </returns>
    private IEnumerator SwitchStateDelay()
    {
        for (int index = 0; index < 1; index++)
        {
            //stop moving and switching directions
            StopCoroutine(Move());
            StopCoroutine(SwitchDirections());

            //wait 1 second
            yield return new WaitForSeconds(1f);
        }

        //spawn a coily snake on the coilyBall transform position
        Instantiate(coilySnake, transform.position, Quaternion.identity);

        //call the on death function
        OnDeath();
    }
}
