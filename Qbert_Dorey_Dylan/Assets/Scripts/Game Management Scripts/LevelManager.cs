using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Author: [Dorey, Dylan]
 * Last Updated: [3/07/2024]
 * [Checks and manages all elements related to level management]
 */

public class LevelManager : MonoBehaviour
{
    //singelton for LevelManager
    private static LevelManager _instance;
    public static LevelManager Instance { get { return _instance; } }

    //list containing all the cubes in the scene
    public List<GameObject> cubes;

    //references to cubes
    public GameObject cubesContainer;
    public GameObject changeToCube;

    //milestone point values that get aawarded to the player
    private readonly int[] levelMilestones = new int[] { 1000, 1250, 1500 };
    private int currentLevelMilestone = 0;

    //check if the enemies should be immobilized or not
    public bool immobilize = false;

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
        //initialize cubes list
        for (int index = 0; index < cubesContainer.transform.childCount; index++)
        {
            //add all cubes to the cubes list
            cubes.Add(cubesContainer.transform.GetChild(index).gameObject);
        }

        //Subscribe this function to the lost life event: remove all enemies when the player dies
        PlayerEventBus.Subscribe(PlayerEvent.lostLife, RemoveAllEnemies);

        //Subscribe these functions to the level over event: remove all the enemies, start the next level, set the target color, initialize all the cubes to default state, and tell the game the player has beat the previous level
        GameEventBus.Subscribe(GameState.levelOver, RemoveAllEnemies);
        GameEventBus.Subscribe(GameState.levelOver, StartNextLevel);
        GameEventBus.Subscribe(GameState.levelOver, SetTargetColor);
        GameEventBus.Subscribe(GameState.levelOver, InitializeCubes);
        GameEventBus.Subscribe(GameState.levelOver, PlayerBeatLevel);

        //subscribe this function to the game over event: remove all the enmies when the game is over
        GameEventBus.Subscribe(GameState.gameOver, RemoveAllEnemies);
    }

    private void OnDisable()
    {
        //Unsubscribe this function from the lost life event: remove all enemies when the player dies
        PlayerEventBus.Unsubscribe(PlayerEvent.lostLife, RemoveAllEnemies);

        //Unsubscribe these functions from the level over event: remove all the enemies, start the next level, set the target color, initialize all the cubes to default state, and tell the game the player has beat the previous level
        GameEventBus.Unsubscribe(GameState.levelOver, RemoveAllEnemies);
        GameEventBus.Unsubscribe(GameState.levelOver, StartNextLevel);
        GameEventBus.Unsubscribe(GameState.levelOver, SetTargetColor);
        GameEventBus.Unsubscribe(GameState.levelOver, InitializeCubes);
        GameEventBus.Unsubscribe(GameState.levelOver, PlayerBeatLevel);

        //Unsubscribe this function from the game over event: remove all the enmies when the game is over
        GameEventBus.Unsubscribe(GameState.gameOver, RemoveAllEnemies);
    }

    /// <summary>
    /// starts nthe next round of Qbert
    /// </summary>
    public void StartNextLevel()
    {
        //for all of the spinners on the level, add points for each remaining and remove them
        foreach (GameObject spinner in GameManager.Instance.spinners)
        {
            //if the level is 2
            if (GameManager.Instance.currentLevel == 2)
            {
                //add 50 points for every spinner left
                PlayerData.Instance.AddScore(50);
            }
            //otherwise if the level is 3
            else if (GameManager.Instance.currentLevel == 3)
            {
                //add 100 points for every spinner left
                PlayerData.Instance.AddScore(100);
            }

            //destroy the spinner
            Destroy(spinner);
        }

        //spawn new spinners at the spawn points
        for (int index = 0; index < Random.Range(GameManager.Instance.currentLevel, GameManager.Instance.currentLevel * 3); index++)
        {
            //spawn a spinner at one of the random spinner spawn points
            Instantiate(GameManager.Instance.spinnerPrefab, GameManager.Instance.spinnerSpawnPoints.transform.GetChild(Random.Range(0, GameManager.Instance.spinnerSpawnPoints.transform.childCount)).gameObject.transform.position, Quaternion.identity);
        }

        //increase the current round by 1 if the completed cubes amount is greater than 27
        if (GameManager.Instance.completedCubes.Count > 27)
        {
            GameManager.Instance.currentLevel++;
        }
    }

    /// <summary>
    /// Intializes all of the cubes at the beginning of each level
    /// </summary>
    public void InitializeCubes()
    {
        //for each of the cubes in the cubes list
        foreach (GameObject cube in cubes)
        {
            //set reached target color to false
            cube.GetComponent<Cube>().reachedTargetColor = false;

            //set the top color to the first color in the cubeTopColor array
            cube.GetComponent<Cube>().topColorIndex = 0;
            cube.transform.GetChild(0).gameObject.GetComponent<Renderer>().material.color = cube.GetComponent<Cube>().cubeTopColor[cube.GetComponent<Cube>().topColorIndex];

            //switch on the current level
            switch (GameManager.Instance.currentLevel)
            {
                //level 1 set the side color to the first element in cubeSideColor array
                case 0:
                    cube.GetComponent<Cube>().sideColorIndex = 0;
                    cube.transform.GetChild(1).gameObject.GetComponent<Renderer>().material.color = cube.GetComponent<Cube>().cubeSideColor[GameManager.Instance.currentLevel];
                    cube.transform.GetChild(2).gameObject.GetComponent<Renderer>().material.color = cube.GetComponent<Cube>().cubeSideColor[GameManager.Instance.currentLevel];
                    break;
                //level 2 set the side color to the second element in cubeSideColor array
                case 1:
                    cube.GetComponent<Cube>().sideColorIndex = 1;
                    cube.transform.GetChild(1).gameObject.GetComponent<Renderer>().material.color = cube.GetComponent<Cube>().cubeSideColor[GameManager.Instance.currentLevel];
                    cube.transform.GetChild(2).gameObject.GetComponent<Renderer>().material.color = cube.GetComponent<Cube>().cubeSideColor[GameManager.Instance.currentLevel];
                    break;
                //level 3 set the side color to the third element in cubeSideColor array
                case 2:
                    cube.GetComponent<Cube>().sideColorIndex = 2;
                    cube.transform.GetChild(1).gameObject.GetComponent<Renderer>().material.color = cube.GetComponent<Cube>().cubeSideColor[GameManager.Instance.currentLevel];
                    cube.transform.GetChild(2).gameObject.GetComponent<Renderer>().material.color = cube.GetComponent<Cube>().cubeSideColor[GameManager.Instance.currentLevel];
                    break;
            }
        }

        //get a reference to the cube script
        Cube cubeRef = cubesContainer.transform.GetChild(0).GetComponent<Cube>();

        //set the colors of the target color cube
        changeToCube.transform.GetChild(0).gameObject.GetComponent<Renderer>().material.color = cubeRef.cubeTopColor[GameManager.Instance.targetColor];
        changeToCube.transform.GetChild(1).gameObject.GetComponent<Renderer>().material.color = cubeRef.cubeSideColor[GameManager.Instance.currentLevel];
        changeToCube.transform.GetChild(2).gameObject.GetComponent<Renderer>().material.color = cubeRef.cubeSideColor[GameManager.Instance.currentLevel];

        //clear the list of completed cubes
        GameManager.Instance.completedCubes.Clear();
    }

    /// <summary>
    /// check the status of the level
    /// </summary>
    public void CheckLevelStatus()
    {
        //checks how many cubes have been completed
        int numCompletedCubes = 0;

        //get each completed cube bool in the completed cubes list
        foreach (bool completedCube in GameManager.Instance.completedCubes)
        {
            //if the cube is completed
            if (completedCube == true)
            {
                //increase the number of completed cubes by 1
                numCompletedCubes++;

                //if the number of completed cubes reaches 28
                if (numCompletedCubes == 28)
                {
                    //if the game is on level 3
                    if(GameManager.Instance.currentLevel == 3)
                    {
                        //end the game
                        GameEventBus.Publish(GameState.gameOver);
                    }
                    else
                    {
                        //otherwise start the next level
                        GameEventBus.Publish(GameState.levelOver);
                    }
                }
            }
            else
            {
                //break if a false is returned
                break;
            }
        }
    }

    /// <summary>
    /// Sets the target color for each level
    /// </summary>
    public void SetTargetColor()
    {
        //switch on the current level
        switch (GameManager.Instance.currentLevel)
        {
            //level 1 set the target color to the first target color
            case 1:
                GameManager.Instance.targetColor = 1;
                break;
                //level 2 set the target color to the second target color
            case 2:
                GameManager.Instance.targetColor = 2;
                break;
                //level 3 set the target color to any color 3-5
            case 3:
                GameManager.Instance.targetColor = Random.Range(3, 6);
                break;
        }
    }

    /// <summary>
    /// removes all enemies from the current level
    /// </summary>
    public void RemoveAllEnemies()
    {
        //for each enemy in the enemies present list
        foreach (GameObject enemy in EnemySpawner.Instance.enemiesPresent)
        {
            //destroy enemy
            Destroy(enemy);
        }

        //clear the enemies present list
        EnemySpawner.Instance.enemiesPresent.Clear();
    }

    /// <summary>
    /// Immobilizes all enemies in the scene for a specified duration
    /// </summary>
    /// <param name="immobilizeDuration"> the immobilization duration </param>
    /// <returns>time to wait </returns>
    public IEnumerator Immobilize(float immobilizeDuration)
    {
        //for each enemy in the enemies present list
        foreach (GameObject enemy in EnemySpawner.Instance.enemiesPresent)
        {
            //stop spawning enemies, stop the enemies movement coroutine, and immobilize the enemy
            EnemySpawner.Instance.StopSpawningEnemies();
            StopCoroutine(enemy.GetComponent<Enemy>().Move());
            enemy.GetComponent<Enemy>().ImmobilizeEnemy();
        }

        //wait the immobilization duration
        yield return new WaitForSeconds(immobilizeDuration);

        //set the enemies speed back to their default speed
        foreach (GameObject enemy in EnemySpawner.Instance.enemiesPresent)
        {
            enemy.GetComponent<Enemy>().enemySpeed = enemy.GetComponent<Enemy>().defaultSpeed;
        }

        //start spawning enemies again
        EnemySpawner.Instance.StartSpawningEnemies();
    }

    /// <summary>
    /// Checks when the player has beat the level to award milestone points
    /// </summary>
    public void PlayerBeatLevel()
    {
        //if the player is not on level 1
        if (GameManager.Instance.currentLevel != 1)
        {
            //add level milestone points to the players score
            PlayerData.Instance.playerScore += levelMilestones[currentLevelMilestone];

            //increase the current level milestone
            currentLevelMilestone++;
        }
    }
}
