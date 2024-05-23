using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Author: [Dorey, Dylan]
 * Last Updated: [3/20/2024]
 * [A ball that moves down the pyramid and immobilzes the enemies when colliding with the player]
 */

public class GreenBall : Enemy
{
    //if the enemy is moving right or left
    private bool movingRight;

    //the time to immobilize all enemeies for
    private readonly float immobilizeDuration = 5f;

    public override void Start()
    {
        //intialize enemy values
        InitializeEnemy(true, false, 1.25f, 100);

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
            //start the immobilize coroutine in level manager
            LevelManager.Instance.StartCoroutine(LevelManager.Instance.Immobilize(immobilizeDuration));

            //add the green ball score to the player's points
            PlayerData.Instance.AddScore(enemyPoints);

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
