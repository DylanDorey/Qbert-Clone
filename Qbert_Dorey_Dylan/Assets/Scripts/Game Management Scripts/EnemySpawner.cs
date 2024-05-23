using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Author: [Dorey, Dylan]
 * Last Updated: [3/21/2024]
 * [Spawns enemies every round]
 */

public class EnemySpawner : MonoBehaviour
{
    //singelton for EnemySpawner
    private static EnemySpawner _instance;
    public static EnemySpawner Instance { get { return _instance; } }

    private bool listHasElements = false;

    private float timeBetweenSpawns;

    //various spawn point references that the enemies can spawn at
    private GameObject spawnPoints;
    private Vector3 topRight;
    private Vector3 topLeft;
    private Vector3 bottomRight;
    private Vector3 bottomLeft;

    //list of enemies alive on the level
    public List<GameObject> enemiesPresent = new List<GameObject>();

    //array of enemies to spawn
    public GameObject[] enemiesToSpawn = new GameObject[6];

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
        //Subscribe to this function for the game event start game: initializes the spawn points for the enemies
        GameEventBus.Subscribe(GameState.startGame, InitializeSpawnpoints);

        //Subscribe to this function for the game event level over: starts spawning enemies at random
        GameEventBus.Subscribe(GameState.levelOver, StartSpawningEnemies);

        //Subscribe to this function for the game event game over: stops spawning enemies
        GameEventBus.Subscribe(GameState.gameOver, StopSpawningEnemies);
    }

    private void OnDisable()
    {
        //Unsubscribe from this function for the game event start game: initializes the spawn points for the enemies
        GameEventBus.Unsubscribe(GameState.startGame, InitializeSpawnpoints);

        //Unsubscribe from this function for the game event level over: starts spawning enemies at random
        GameEventBus.Unsubscribe(GameState.levelOver, StartSpawningEnemies);

        //Unsubscribe from this function for the game event game over: stops spawning enemies
        GameEventBus.Unsubscribe(GameState.gameOver, StopSpawningEnemies);
    }

    private void Update()
    {
        //if enemies present is greater than 0
        if (enemiesPresent.Count > 0)
        {
            //list has elements is true 
            listHasElements = true;
        }
        else
        {
            //otherwise list has elements is false
            listHasElements = false;
        }
    }

    /// <summary>
    /// starts spawning enemies after 3 seconds every 3 to 5 seconds
    /// </summary>
    public void StartSpawningEnemies()
    {
        timeBetweenSpawns = 4f;
        InvokeRepeating("SpawnRandomEnemy", 3f, timeBetweenSpawns);
    }

    /// <summary>
    /// stops spawning enemies
    /// </summary>
    public void StopSpawningEnemies()
    {
        CancelInvoke("SpawnRandomEnemy");
    }

    /// <summary>
    /// Spawns a random enemy in the level
    /// </summary>
    private void SpawnRandomEnemy()
    {
        //random indexes for the spawn side and enemy to spawn
        int randomSpawnSideIndex = Random.Range(0, 2);
        int randomSpawnEnemyIndex = Random.Range(0, 6);

        //set time between spawns
        timeBetweenSpawns = Random.Range(3f, 5f);

        //initialize a random enemy from the enemies to spawn array
        GameObject enemy = enemiesToSpawn[randomSpawnEnemyIndex];

        //spawn that enemy at 0,0,0
        enemy = Instantiate(enemy, new Vector3(0f, 0f, 0f), Quaternion.identity);

        //add the enemy that was just spawned into the enemies present list
        enemiesPresent.Add(enemy);

        //if the spawn side index is 0 spawn the enemy on the left side
        if (randomSpawnSideIndex == 0)
        {
            //if the enemy is Ugg and Wrongway
            if (enemy.GetComponent<UggandWrongway>())
            {
                //rotate Ugg the leftward facing direction, set the spawn point to bottom left, and set start right to false
                enemy.transform.Rotate(enemy.GetComponent<UggandWrongway>().startLeftRotation);
                enemy.transform.position = bottomLeft;
                enemy.GetComponent<Enemy>().startRight = false;
            }
            else
            {
                //otherwise the spawn position is top left and start right is false
                enemy.transform.position = topLeft;
                enemy.GetComponent<Enemy>().startRight = false;
            }
        }
        else //otherwise spawn the enemy on the right side
        {
            //if the enemy is Ugg and Wrongway
            if (enemy.GetComponent<UggandWrongway>())
            {
                //rotate Ugg the rightward facing direction, set the spawn point to bottom right, and set start right to true
                enemy.transform.Rotate(enemy.GetComponent<UggandWrongway>().startRightRotation);
                enemy.transform.position = bottomRight;
                enemy.GetComponent<Enemy>().startRight = true;
            }
            else
            {
                //otherwise the spawn position is top right and start right is true
                enemy.transform.position = topRight;
                enemy.GetComponent<Enemy>().startRight = true;
            }
        }
    }

    /// <summary>
    /// sets spawn point references for enemies
    /// </summary>
    private void InitializeSpawnpoints()
    {
        //intialize the game manager game object
        GameObject gameManagerGO = transform.parent.gameObject;

        //set spawnpoints, top right spawn point, top left spawn point, bottom right spawn point, and bottom left spawn point
        spawnPoints = gameManagerGO.transform.GetChild(3).gameObject;
        topRight = spawnPoints.transform.GetChild(0).transform.position;
        topLeft = spawnPoints.transform.GetChild(1).transform.position;
        bottomRight = spawnPoints.transform.GetChild(2).transform.position;
        bottomLeft = spawnPoints.transform.GetChild(3).transform.position;
    }
}
