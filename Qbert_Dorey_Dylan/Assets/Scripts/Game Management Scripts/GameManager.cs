using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Author: [Dorey, Dylan]
 * Last Updated: [2/29/2024]
 * [This will manage all game functions initializing, starting, ending, and navigating]
 */

public class GameManager : MonoBehaviour
{
    //singelton for GameManager
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    //value to determine what level the player is on
    public int currentLevel;

    //target color
    public int targetColor;

    //references to the spinner spawn points
    public GameObject spinnerSpawnPoints;

    //prefab for the spinner
    public GameObject spinnerPrefab;

    //the list of cubes that have reached the target color
    public List<bool> completedCubes = new List<bool>();

    //list of spinners in the current level
    public List<GameObject> spinners = new List<GameObject>();

    void Awake()
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
        //subscribe to this function for the start game event: intializes all game elements
        GameEventBus.Subscribe(GameState.startGame, InitializeGame);
    }

    private void OnDisable()
    {
        //Unsubscribe from this function for the start game event: intializes all game elements
        GameEventBus.Unsubscribe(GameState.startGame, InitializeGame);
    }

    private void Start()
    {
        //start the game in the menu by publishing the menu game event
        GameEventBus.Publish(GameState.menu);
    }

    /// <summary>
    /// This will intialize all game elements for Qbert
    /// </summary>
    private void InitializeGame()
    {
        //clear the list of enemies and enemies present on the level
        LevelManager.Instance.RemoveAllEnemies();

        //initialize the player controller and player data
        PlayerController.Instance.InitializePlayerController();

        //set reference to spinner spawn points
        spinnerSpawnPoints = transform.GetChild(1).gameObject;

        //set the current level to 1
        currentLevel = 1;

        //start a new level
        GameEventBus.Publish(GameState.levelOver);
    }

    /// <summary>
    /// This will start the game Qbert
    /// </summary>
    public void StartGame()
    {
        //publish the startGame game event
        GameEventBus.Publish(GameState.startGame);
    }

    /// <summary>
    /// This will send the user back to the main menu
    /// </summary>
    public void ReturnToMenu()
    {
        //publish the menu game event
        GameEventBus.Publish(GameState.menu);
    }

    /// <summary>
    /// This will allow the user to close/quit Qbert
    /// </summary>
    public void QuitGame()
    {
        //quit the application
        Application.Quit();
    }
}
