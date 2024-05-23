using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*
 * Author: [Dorey, Dylan]
 * Last Updated: [3/07/2024]
 * [Displays the correct UI for the user depending on what state the game is in]
 */

public class UIManager : MonoBehaviour
{
    //singelton for UIManager
    private static UIManager _instance;
    public static UIManager Instance { get { return _instance; } }

    //references to the different UI screens
    public GameObject menuScreen, gameScreen, gameOverScreen, livesPanel;

    //references to the different UI text
    public TextMeshProUGUI scoreText, levelText;

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
        //Subscribe to this function for the game event menu: enables the menu UI
        GameEventBus.Subscribe(GameState.menu, EnableMenuUI);

        //Subscribe to these functions for the game event start game: enables the game UI and updates the life images
        GameEventBus.Subscribe(GameState.startGame, EnableGameUI);
        GameEventBus.Subscribe(GameState.startGame, SetLifeImage);

        //Subscribe to these function for the game event game over: enables the game over UI
        GameEventBus.Subscribe(GameState.gameOver, EnableGameOverUI);

        //Subscribe to this function for the player event lost life: updates the life images
        PlayerEventBus.Subscribe(PlayerEvent.lostLife, SetLifeImage);

        //Subscribe to this function for the player event gained life: updates the life images
        PlayerEventBus.Subscribe(PlayerEvent.gainedLife, SetLifeImage);
    }

    private void OnDisable()
    {
        //Unsubscribe from this function for the game event menu: enables the menu UI
        GameEventBus.Unsubscribe(GameState.menu, EnableMenuUI);

        //unsubscribe from these functions for the game event start game: enables the game UI and updates the life images
        GameEventBus.Unsubscribe(GameState.startGame, EnableGameUI);
        GameEventBus.Unsubscribe(GameState.startGame, SetLifeImage);

        //Unsubscribe from these function for the game event game over: enables the game over UI
        GameEventBus.Unsubscribe(GameState.gameOver, EnableGameOverUI);

        //Unsubscribe from this function for the player event lost life: updates the life images
        PlayerEventBus.Unsubscribe(PlayerEvent.lostLife, SetLifeImage);

        //Unsubscribe from this function for the player event gained life: updates the life images
        PlayerEventBus.Unsubscribe(PlayerEvent.gainedLife, SetLifeImage);
    }

    private void Update()
    {
        levelText.text = GameManager.Instance.currentLevel.ToString();
        scoreText.text = PlayerData.Instance.playerScore.ToString();
    }

    /// <summary>
    /// enables the menu screen UI and disables the game and game over UI
    /// </summary>
    public void EnableMenuUI()
    {
        menuScreen.SetActive(true);
        gameScreen.SetActive(false);
        gameOverScreen.SetActive(false);
    }

    /// <summary>
    /// enables the game screen UI and disables the menu and game over UI
    /// </summary>
    public void EnableGameUI()
    {
        menuScreen.SetActive(false);
        gameScreen.SetActive(true);
        gameOverScreen.SetActive(false);
    }

    /// <summary>
    /// enables the game over screen UI and disables the menu and game UI
    /// </summary>
    public void EnableGameOverUI()
    {
        menuScreen.SetActive(false);
        gameScreen.SetActive(false);
        gameOverScreen.SetActive(true);
    }

    /// <summary>
    /// adds a life to the players UI
    /// </summary>
    public void SetLifeImage()
    {
        //loop through the livesPanel life images and disable all of them
        for (int index1 = 0; index1 < livesPanel.transform.childCount; index1++)
        {
            livesPanel.transform.GetChild(index1).GetComponent<Image>().enabled = false;
        }

        //loop throught the livesPanel life images as many times as player lives
        for (int index2 = 0; index2 < PlayerData.Instance.playerLives; index2++)
        {
            //if the image is turned off
            if (livesPanel.transform.GetChild(index2).GetComponent<Image>().enabled == false)
            {
                //turn it on
                livesPanel.transform.GetChild(index2).GetComponent<Image>().enabled = true;
            }
        }
    }

    /// <summary>
    /// makes the background flash green for a given duration when the player lands on a spinner
    /// </summary>
    /// <param name="mainCamera"> the main game camera </param>
    /// <returns> flash duration </returns>
    public IEnumerator FlashBackground(Camera mainCamera)
    {
        for (int index = 0; index < 1; index++)
        {
            //set the camera background color to green
            mainCamera.backgroundColor = Color.green;

            //wait 0.2 secconds
            yield return new WaitForSeconds(0.2f);
        }

        //set the camera background color to black
        mainCamera.backgroundColor = Color.black;
    }
}
