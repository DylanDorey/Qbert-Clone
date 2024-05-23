using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Author: [Dorey, Dylan]
 * Last Updated: [3/21/2024]
 * []
 */

public class UggandWrongway : Enemy
{
    private bool movingRight;
    private bool movingLeft;

    public Vector3 startRightRotation = new Vector3(0f, 45f, -90f);
    public Vector3 startLeftRotation = new Vector3(0f, -45f, 90f);

    public override void Start()
    {
        InitializeEnemy(true, true, 1.25f, 0);

        base.Start();

        if (startRight)
        {
            movingRight = true;
        }
        else
        {
            movingLeft = true;
        }

        StartCoroutine(SwitchDirections());
    }

    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if (other.gameObject.CompareTag("Player"))
        {
            PlayerController.Instance.deathPos = other.gameObject.transform.position;
            PlayerEventBus.Publish(PlayerEvent.lostLife);
        }
    }

    private IEnumerator SwitchDirections()
    {
        while (isAlive)
        {
            if (startRight)
            {
                if (movingRight)
                {
                    moveDirection = MoveDown;
                    movingRight = false;
                }
                else
                {
                    moveDirection = MoveRight;
                    movingRight = true;
                }
            }
            else
            {
                if (movingLeft)
                {
                    moveDirection = MoveUp;
                    movingLeft = false;
                }
                else
                {
                    moveDirection = MoveLeft;
                    movingLeft = true;
                }
            }

            yield return new WaitForSeconds(enemySpeed);
        }
    }
}
