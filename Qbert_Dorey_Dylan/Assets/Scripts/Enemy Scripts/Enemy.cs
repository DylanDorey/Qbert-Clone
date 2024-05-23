using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Author: [Dorey, Dylan]
 * Last Updated: [3/13/2024]
 * [Base enemy class for all enemies to inherit from]
 */

public class Enemy : MonoBehaviour
{
    //bool for the movement type of the enemy
    public bool isEscheresque;

    //bool to determine if the enemy is alive or not
    public bool isAlive;

    //enemy values such as point values, enemy speed, and default speed
    public int enemyPoints;
    public float enemySpeed;
    public float defaultSpeed;

    //bool if the enemy is starting on the right or left
    public bool startRight;

    //the current movement direction
    public Vector3 moveDirection;

    //all 4 different move directions, determined by if the enemy moves in an Escheresque manner
    ////////////////////////////////////////////////////////////////////////////////////////////
    public Vector3 MoveUp
    {
        get
        {
            if (isEscheresque)
            {
                return new Vector3(1.4f, 0f, 0f);
            }
            else
            {
                return new Vector3(0.7f, 1f, 0.7f);
            }
        }
    }

    public Vector3 MoveDown
    {
        get
        {
            if (isEscheresque)
            {
                return new Vector3(-1.4f, 0f, 0f);
            }
            else
            {
                return new Vector3(-0.7f, -1f, -0.7f);
            }
        }
    }

    public Vector3 MoveLeft
    {
        get
        {
            if (isEscheresque)
            {
                return new Vector3(0.7f, 1f, 0.7f);
            }
            else
            {
                return new Vector3(-0.7f, 1f, 0.7f);
            }
        }
    }

    public Vector3 MoveRight
    {
        get
        {
            if (isEscheresque)
            {
                return new Vector3(-0.7f, 1f, 0.7f);
            }
            else
            {
                return new Vector3(0.7f, -1f, -0.7f);
            }
        }
    }
    ////////////////////////////////////////////////////////////////////////////////////////////

    public virtual void Start()
    {
        //start moving when spawned in
        StartCoroutine(Move());
    }

    public virtual void Update()
    {
        //if the enemy is dead
        if (!isAlive)
        {
            //fall
            Fall();
        }
        
        //if the enemy reaches passed map bounds
        if (transform.position.y < -10f || transform.position.x > 10f || transform.position.x < -10f)
        {
            //call on death function
            OnDeath();
        }
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        //if the other game object is the fall barrier
        if (other.gameObject.CompareTag("fallBarrier"))
        {
            //set is alive to false
            isAlive = false;
        }
    }

    /// <summary>
    /// Allows the enemy to move in a certain move direction based on the enemies movement speed
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerator Move()
    {
        //while the enemy is alive
        while (isAlive)
        {
            //add the move direction to the enemy position
            transform.position += moveDirection;

            //if the enemy moves in an escheresque manner
            if (isEscheresque)
            {
                //check of they have fallen
                CheckEscheresqueFall();
            }

            //wait enemy speed to move again
            yield return new WaitForSeconds(enemySpeed);
        }
    }

    /// <summary>
    /// Check if the escheresque movement enemy has fallen off the map
    /// </summary>
    /// <returns></returns>
    private bool CheckEscheresqueFall()
    {
        //if the enemy is escheredque and started on the right
        if (isEscheresque && startRight)
        {
            //if the the enemy reached the falling point
            if (transform.position.x < -2.6f && transform.position.z < 2.6f)
            {
                //set is alive to false
                isAlive = false;
            }
        }
        //otherwise if the enemy started on the left
        else if (isEscheresque && !startRight)
        {
            //if the enemy reached the fall point
            if (transform.position.x > 2.6f && transform.position.z > -2.6f)
            {
                //set is alive to false
                isAlive = false;
            }
        }

        //return is alive
        return isAlive;
    }

    /// <summary>
    /// Mkaes the enemy fall in the correct direction
    /// </summary>
    public void Fall()
    {
        //stop movinging
        StopCoroutine(Move());

        //if the enemy is an escheresque enemy
        if (isEscheresque)
        {
            //if they started on the right
            if(startRight)
            {
                //fall left
                transform.position += Vector3.left * 5f * Time.deltaTime;
            }
            else
            {
                //otherwise fall right
                transform.position += Vector3.right * 5f * Time.deltaTime;
            }
        }
        else
        {
            //otherwise if theyre a regular enemy, fall down
            transform.position += Vector3.down * 5f * Time.deltaTime;
        }
    }

    /// <summary>
    /// Gets called when the enemy is not alive anymore
    /// </summary>
    public virtual void OnDeath()
    {
        //remove the enemy from the enemies present list then destroy the game object
        EnemySpawner.Instance.enemiesPresent.Remove(gameObject);
        Destroy(gameObject);
    }

    /// <summary>
    /// initializes all of the enemies values when spawned in
    /// </summary>
    /// <param name="alive"> is the enemy alive or not </param>
    /// <param name="escheresque"> does the enemy move in an escheresque manner </param>
    /// <param name="speed"> enenmies speed </param>
    /// <param name="points"> the amount of points the enemy is worth </param>
    public void InitializeEnemy(bool alive, bool escheresque, float speed, int points)
    {
        //initialize is alive, is escheresque, enemy speed, enemy points, and the default speed
        isAlive = alive;
        isEscheresque = escheresque;
        enemySpeed = speed;
        enemyPoints = points;
        defaultSpeed = enemySpeed;
    }

    /// <summary>
    /// immobilizes the enemy by increasing theyre time between movements
    /// </summary>
    public void ImmobilizeEnemy()
    {
        //set enemy speed to 10f
        enemySpeed = 10f;
    }
}
