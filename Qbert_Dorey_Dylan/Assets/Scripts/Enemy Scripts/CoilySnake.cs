using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Author: [Dorey, Dylan]
 * Last Updated: [3/20/2024]
 * [Coily Snake variant that follows the player and chases them when on a spinner]
 */

public class CoilySnake : Enemy
{
    //the target position to follow
    private Vector3 targetPos;

    //checks if coily is in close pursuit of the player
    public bool closePursuit = false;

    // Start is called before the first frame update
    public override void Start()
    {
        //add coily to the enemies present list
        EnemySpawner.Instance.enemiesPresent.Add(gameObject);

        //initialize coily's elements
        InitializeEnemy(true, false, 1.5f, 500);

        base.Start();
    }

    // Update is called once per frame
    public override void Update()
    {
        //update the target position
        SwitchMoveDirectionToPlayerPos();

        base.Update();
    }

    public override void OnTriggerEnter(Collider other)
    {
        //if the other game object is tagged player
        if (other.gameObject.CompareTag("Player"))
        {
            //set the players death position to coily's position
            PlayerController.Instance.deathPos = transform.position;

            //remove a life from the player
            PlayerEventBus.Publish(PlayerEvent.lostLife);

            //trigger the on death function for Coily
            OnDeath();
        }

        //if the other game object is tagged fall barrier
        if (other.gameObject.CompareTag("fallBarrier"))
        {
            //add points to the player when triggering a fall barrier (closr pursuit)
            PlayerData.Instance.AddScore(enemyPoints);

            //remove Coily from the enemies present list and remove all the enemies from the scen
            EnemySpawner.Instance.enemiesPresent.Remove(gameObject);
            LevelManager.Instance.RemoveAllEnemies();
        }

        base.OnTriggerEnter(other);
    }

    /// <summary>
    /// Sets Coily's target position to follow 
    /// </summary>
    private void SetTargetPos()
    {
        //if Coily is in close pursuit and the player is on a spinner
        if (closePursuit && PlayerController.Instance.onSpinner)
        {
            //set Coily's target position to the spinners original position
            targetPos = PlayerController.Instance.spinnerPos;
        }
        else
        {
            //otherwise set Coily's target position to the players position
            targetPos = PlayerController.Instance.transform.position;
        }
    }

    /// <summary>
    /// switches Coily's movement direction based on the target position
    /// </summary>
    private void SwitchMoveDirectionToPlayerPos()
    {
        //if the target position is up and to the right of Coily
        if (targetPos.y > transform.position.y && targetPos.x > transform.position.x)
        {
            //set the target position and move up
            SetTargetPos();
            moveDirection = MoveUp;
        }
        //if the target position is bellow and to the right of Coily
        else if (targetPos.y < transform.position.y && targetPos.x < transform.position.x)
        {
            //set the target position
            SetTargetPos();

            //if on the last row of cubes
            if (transform.position.y < -1f)
            {
                //move left to avoid jumping off of the pyramid
                moveDirection = MoveLeft;
            }
            else
            {
                //otherwise move down towards the player
                moveDirection = MoveDown;
            }
        }
        //if the target position is up an to the left of Coily
        else if (targetPos.y > transform.position.y && targetPos.x < transform.position.x)
        {
            //set the target position and move left
            SetTargetPos();
            moveDirection = MoveLeft;
        }
        //if the target position is below and to the left of Coily
        else if (targetPos.y < transform.position.y && targetPos.x > transform.position.x)
        {
            //set the target position
            SetTargetPos();

            //if on the last row of cubes
            if (transform.position.y < -1f)
            {
                //move up to avoid jumping off of the pyramid
                moveDirection = MoveUp;
            }
            else
            {
                //otherwise move right towards the player
                moveDirection = MoveRight;
            }
        }
    }
}
