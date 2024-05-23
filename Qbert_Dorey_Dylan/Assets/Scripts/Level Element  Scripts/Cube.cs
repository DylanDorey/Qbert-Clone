using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Author: [Dorey, Dylan]
 * Last Updated: [2/29/2024]
 * [Manages all functionality of the cubes on the level]
 */

public class Cube : MonoBehaviour
{
    //colors for the top of the cube and sides of the cube
    public Color[] cubeTopColor = new Color[] { Color.magenta, Color.blue, Color.yellow, Color.red, Color.cyan, Color.green };
    public Color[] cubeSideColor = new Color[] { Color.yellow, Color.blue, Color.grey };

    //the indexes of the top and side colors of the cube
    public int topColorIndex;
    public int sideColorIndex;

    //amount of points changing the color of the cube is worth
    private int cubePoints = 25;

    //bool to determine if the top color has reached the target color
    public bool reachedTargetColor = false;

    private void OnCollisionEnter(Collision collision)
    {
        //if the other game object is tagged player
        if (collision.gameObject.CompareTag("Player"))
        {
            //change the top color and check the status of the level
            ChangeTopColor();
            LevelManager.Instance.CheckLevelStatus();
        }
    }

    /// <summary>
    /// changes the color of the top of the cube to the next color in the array based on the current level
    /// </summary>
    private void ChangeTopColor()
    {
        //if it is currently level 1 or level 2
        if (GameManager.Instance.currentLevel == 1 || GameManager.Instance.currentLevel == 2)
        {
            //and if the cube has not reached the target color
            if (!reachedTargetColor)
            {
                //loop the top color back to index 0 if it is at the max index color
                LoopTopColorArray();

                //check if the top color is at the target color
                CheckTopColorIndex();
            }
        }
        else //otherwise the player is on a higher level
        {
            //loop the top color back to index 0 if it is at the max index color
            LoopTopColorArray();

            //check if the top color is at the target color
            CheckTopColorIndex();
        }

        //apply color change
        transform.GetChild(0).gameObject.GetComponent<Renderer>().material.color = cubeTopColor[topColorIndex];
    }

    /// <summary>
    /// Adds 25 points to the players score everytime they change the color of a cube
    /// </summary>
    private void PassPoints()
    {
        //add the points to the players score
        PlayerData.Instance.AddScore(cubePoints);
    }

    /// <summary>
    /// Loops the color back to the starting color if it has reached the final color
    /// </summary>
    private void LoopTopColorArray()
    {
        //check if the top color is at the last array index in the color array
        if (topColorIndex >= cubeTopColor.Length)
        {
            //if so, set the color index back to 0 and pass points to the player for changing colors
            topColorIndex = 0;
            PassPoints();
        }
        else
        {
            //otherwise increase the top color by 1 and pass points to the player for changing colors
            topColorIndex++;
            PassPoints();
        }
    }

    /// <summary>
    /// Checks if the top color is equal to the target color for the round
    /// </summary>
    private void CheckTopColorIndex()
    {
        //check to see if the top color is equal to target color
        if (topColorIndex == GameManager.Instance.targetColor)
        {
            //if so, set reached target color to true and add a true to the completed cubes list
            reachedTargetColor = true;
            GameManager.Instance.completedCubes.Add(this.reachedTargetColor);
        }
        else
        {
            //otherwise set reached target color to false and remove a true to the completed cubes list
            reachedTargetColor = false;
            GameManager.Instance.completedCubes.Remove(this.reachedTargetColor);
        }
    }
}
