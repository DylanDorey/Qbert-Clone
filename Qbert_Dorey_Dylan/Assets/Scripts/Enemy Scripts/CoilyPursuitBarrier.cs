using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Author: [Dorey, Dylan]
 * Last Updated: [3/28/2024]
 * [Sets Coily's closePursuit to true when the player is within pursuit distance]
 */

public class CoilyPursuitBarrier : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        //if the other game object is the player
        if (other.gameObject.CompareTag("Player"))
        {
            //set clost pursuit to true
            GetComponentInParent<CoilySnake>().closePursuit = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //if the other game object exiting is the player
        if (other.gameObject.CompareTag("Player"))
        {
            //delay setting close pursuit to false
            StartCoroutine(SetFalse());
        }
    }

    /// <summary>
    /// Waits 1 second after Qbert exits pursuit distance to set close pursuit to false
    /// </summary>
    /// <returns> time to wait before setting to false </returns>
    private IEnumerator SetFalse()
    {
        for (int index = 0; index < 1; index++)
        {
            //gets the close pursuit boolean and sets it to tru
            GetComponentInParent<CoilySnake>().closePursuit = true;

            //wait 1,5 seconds
            yield return new WaitForSeconds(1.5f);
        }

        //set close pursuit to false
        GetComponentInParent<CoilySnake>().closePursuit = false;
    }
}
