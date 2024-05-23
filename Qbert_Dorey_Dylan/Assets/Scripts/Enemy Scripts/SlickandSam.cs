using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Author: [Dorey, Dylan]
 * Last Updated: [3/19/2024]
 * [Slick and Sam will descend down the pyramid and revert any cube back to the previous color if they land on it]
 */
public class SlickandSam : Enemy
{
    //if the enemy is moving right or left
    private bool movingRight;

    public override void Start()
    {
        //intialize enemy values
        InitializeEnemy(true, false, 1.25f, 300);

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

        //if the other game object is a cube
        if (other.gameObject.GetComponent<Cube>())
        {
            //intialize cube script reference
            Cube cube = other.gameObject.GetComponent<Cube>();

            //if the cubes top color index is not equal to the first color
            if (cube.topColorIndex != 0)
            {
                //set the color to the previous color in the cubes top color array and decrement the top color index
                cube.transform.GetChild(0).gameObject.GetComponent<Renderer>().material.color = cube.cubeTopColor[cube.topColorIndex - 1];
                cube.topColorIndex--;

                //if the cube had reached the target color
                if (cube.reachedTargetColor == true)
                {
                    //set reached target color to false and remove true from the completed cubes
                    cube.reachedTargetColor = false;
                    GameManager.Instance.completedCubes.Remove(true);
                }
            }
        }

        //if the other game object is the player
        if (other.gameObject.CompareTag("Player"))
        {
            //pass points to the player
            PlayerData.Instance.AddScore(enemyPoints);

            //call on death function
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
