using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Author: [Dorey, Dylan]
 * Last Updated: [3/05/2024]
 * [Spins and sends the player back to the top of the pyramid when stepped on]
 */

public class Spinner : MonoBehaviour
{
    private float rotateSpeed = 7f;
    private Vector3 startPos = new Vector3(0f, 7f, 0f);

    //tells the spinner when to move
    private bool move = false;

    //the duration of the spinner interpolation
    private float timer;

    //player ref
    private GameObject player;

    //camera reference
    private Camera mainCamera;

    private void Start()
    {
        //add the spinner to the spinners list and initialize the main camera game object
        GameManager.Instance.spinners.Add(gameObject);
        mainCamera = FindObjectOfType<Camera>().gameObject.GetComponent<Camera>();
    }

    private void FixedUpdate()
    {
        //rotate indefinetely
        Rotate();

        //if move is set to true
        if (move)
        {
            //lerp to the starting position
            transform.position = Vector3.Lerp(transform.position, startPos, timer/50f);

            timer += Time.deltaTime;
        }

        //if the spinner's y pos reaches further than 4.998
        if (transform.position.y > 6.998f)
        {
            //remove the player game object as a child
            player.transform.parent = null;

            //set onSpinner to false in the PlayerController class
            PlayerController.Instance.onSpinner = false;

            //remove the spinner from the list of spinners on the map and destroy the spinner
            GameManager.Instance.spinners.Remove(gameObject);
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //if the gameobject colliding is tagged Player
        if (collision.gameObject.CompareTag("Player"))
        {
            //set onSpinner to true in the PlayerController class
            PlayerController.Instance.onSpinner = true;

            //set spinnerPos in the PlayerController class (coily pursuit element)
            PlayerController.Instance.spinnerPos = transform.position;

            //make the background flash green
            StartCoroutine(UIManager.Instance.FlashBackground(mainCamera));

            //set the player reference to the collision gameobject
            player = collision.gameObject;

            //tell the spinner to move and stop spinning
            move = true;
            rotateSpeed = 0f;

            //lock the player onto the spinner when moving
            player.transform.position = transform.position;
            player.transform.parent = transform;
        }
    }

    /// <summary>
    /// Rotates the spinner at a constant speed
    /// </summary>
    private void Rotate()
    {
        //spin on the y axis at rotateSpeed
        transform.Rotate(0f, rotateSpeed, 0f);
    }
}
