using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/*
 * Author: [Dorey, Dylan]
 * Last Updated: [2/28/2024]
 * [Allows Qbert to move from block to block]
 */

public enum PlayerEvent
{
    lostLife,
    gainedLife,
}

public class PlayerController : MonoBehaviour
{
    //singelton for PlayerController
    private static PlayerController _instance;
    public static PlayerController Instance { get { return _instance; } }

    //if the player has hopped on a spinner or not
    public bool onSpinner = false;

    //reference to scriptable object PlayerInput
    public PlayerInput playerActions;

    //reference to the qbert player model
    private GameObject qbertPrefab;

    //bool to stop repetitive movements
    private bool hasMoved = false;

    //all 4 different move directions
    private Vector3 moveUp = new Vector3(0.7f, 1.1f, 0.7f);
    private Vector3 moveDown = new Vector3(-0.7f, -0.9f, -0.7f);
    private Vector3 moveLeft = new Vector3(-0.7f, 1.1f, 0.7f);
    private Vector3 moveRight = new Vector3(0.7f, -0.9f, -0.7f);

    //default starting position and the position Qbert died in
    public Vector3 startPos = new Vector3(0f, 5f, 0f);
    public Vector3 deathPos;

    //position of the spinner the player hops on, so coily can set this as its target
    public Vector3 spinnerPos;

    private void Awake()
    {
        //if _instance contains something and it isn't this
        if (_instance != null && _instance != this)
        {
            //Destroy it
            Destroy(this.gameObject);
        }
        else
        {
            //otherwise set this to _instance
            _instance = this;
        }
    }

    private void OnEnable()
    {
        //Subscribe to this function on the player event lost life: removes a life from the player's lives
        PlayerEventBus.Subscribe(PlayerEvent.lostLife, OnDeath);

        //Subscribe to this function on the game event start game: resets the player controller attributes to default
        GameEventBus.Subscribe(GameState.startGame, ResetPlayerController);

        //Subscribe to this function on the game event level over: resets the player controller attributes to default
        GameEventBus.Subscribe(GameState.levelOver, ResetPlayerController);

        //Subscribe to this function on the game event game over: disables the player controller
        GameEventBus.Subscribe(GameState.gameOver, DisablePlayerController);
    }

    private void OnDisable()
    {
        //Unsubscribe from this function on the player event lost life: removes a life from the player's lives
        PlayerEventBus.Unsubscribe(PlayerEvent.lostLife, OnDeath);

        //Unsubscribe from this function on the game event start game: resets the player controller attributes to default
        GameEventBus.Unsubscribe(GameState.startGame, ResetPlayerController);

        //Unsubscribe from this function on the game event level over: resets the player controller attributes to default
        GameEventBus.Unsubscribe(GameState.levelOver, ResetPlayerController);

        //Unsubscribe from this function on the game event game over: disables the player controller
        GameEventBus.Unsubscribe(GameState.gameOver, DisablePlayerController);
    }

    private void OnTriggerEnter(Collider other)
    {
        //if the other game object is tagged deathBarrier
        if (other.gameObject.CompareTag("deathBarrier"))
        {
            //publish the lost life event
            PlayerEventBus.Publish(PlayerEvent.lostLife);
        }
    }

    /// <summary>
    /// Moves Qbert up
    /// </summary>
    /// <param name="context"> the state of the input recieved </param>
    public void OnMoveUp(InputAction.CallbackContext context)
    {
        //if the input was performed
        if (context.performed)
        {
            //and if qbert hasn't moved already
            if (!hasMoved)
            {
                //rotate qbert and move up
                qbertPrefab.transform.eulerAngles = new Vector3(0f, -90f, 0f);
                StartCoroutine(Move(moveUp));
            }
        }
    }

    /// <summary>
    /// Moves Qbert down
    /// </summary>
    /// <param name="context"> the state of the input recieved </param>
    public void OnMoveDown(InputAction.CallbackContext context)
    {
        //if the input was performed
        if (context.performed)
        {
            //and if qbert hasn't moved already
            if (!hasMoved)
            {
                //rotate qbert and move down
                qbertPrefab.transform.eulerAngles = new Vector3(0f, 90f, 0f);
                StartCoroutine(Move(moveDown));
            }
        }
    }

    /// <summary>
    /// Moves Qbert left
    /// </summary>
    /// <param name="context"> the state of the input recieved </param>
    public void OnMoveLeft(InputAction.CallbackContext context)
    {
        //if the input was performed
        if (context.performed)
        {
            //and if qbert hasn't moved already
            if (!hasMoved)
            {
                //rotate qbert and move left
                qbertPrefab.transform.eulerAngles = new Vector3(0f, 180f, 0f);
                StartCoroutine(Move(moveLeft));
            }
        }
    }

    /// <summary>
    /// Moves Qbert right
    /// </summary>
    /// <param name="context"> the state of the input recieved </param>
    public void OnMoveRight(InputAction.CallbackContext context)
    {
        //if the input was performed
        if (context.performed)
        {
            //and if qbert hasn't moved already
            if (!hasMoved)
            {
                //rotate qbert and move right
                qbertPrefab.transform.eulerAngles = new Vector3(0f, 0f, 0f);
                StartCoroutine(Move(moveRight));
            }
        }
    }

    /// <summary>
    /// Moves qbert the correct direction when an input is received
    /// </summary>
    /// <param name="moveDirection"> the direction to apply to the players movement </param>
    private IEnumerator Move(Vector3 moveDirection)
    {
        for (int index = 0; index < 1; index++)
        {
            //set has moved to true and apply the movement direction to the players position
            hasMoved = true;

            transform.position += moveDirection;

            //wait 0.3 seconds before receiving input again
            yield return new WaitForSeconds(0.3f);
        }
        
        //set has moved to false
        hasMoved = false;
    }

    /// <summary>
    /// resets the player controller to the start position
    /// </summary>
    public void ResetPlayerController()
    {
        //set position to top of pyramid
        transform.position = startPos;
    }

    /// <summary>
    /// initializes the player controller at the start of the game
    /// </summary>
    public void InitializePlayerController()
    {
        //reference for the PlayerInput scriptable object
        playerActions = new PlayerInput(); //constructor

        //turn playerActions on
        playerActions.Enable();

        //set the qbert prefab to the first child of the parent object
        qbertPrefab = transform.GetChild(0).gameObject;
    }

    /// <summary>
    /// disables player input
    /// </summary>
    public void DisablePlayerController()
    {
        //disable the player actions script
        playerActions.Disable();

        //set qbert prefab to null
        qbertPrefab = null;
    }

    /// <summary>
    /// Removes a life from the player, sets the death position, and removes all enemies from level when the player dies
    /// </summary>
    public void OnDeath()
    {
        //if the player fell off the pyramid
        if (transform.position.y < -6f)
        {
            //their death position is equal to the start position
            deathPos = startPos;
        }

        //set their current position to the death position
        transform.position = deathPos;

        //blink on death
        StartCoroutine(Blink());

        //remove all the enemies on the level
        LevelManager.Instance.RemoveAllEnemies();
    }

    /// <summary>
    /// When the player dies, make the character game object blink
    /// </summary>
    /// <returns> time between blinks </returns>
    private IEnumerator Blink()
    {
        //set the blink index to 0
        int blinkIndex = 0;

        //loop 10 times
        for (int index = 0; index < 10; index++)
        {
            //if the blink index is equal to 0
            if(blinkIndex == 0)
            {
                //disable the player object and set blink index to 1
                qbertPrefab.SetActive(false);
                blinkIndex = 1;
            }
            else //other wise it is 1
            {
                //enable the player object and set blink index to 0
                qbertPrefab.SetActive(true);
                blinkIndex = 0;
            }

            //wait 0.2 seconds between blinks
            yield return new WaitForSeconds(0.2f);
        }

        //enable the player object at the end
        qbertPrefab.SetActive(true);
    }
}
