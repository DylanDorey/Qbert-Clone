using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Author: [Dorey, Dylan]
 * Last Updated: [2/28/2024]
 * [Stores all player data such as player lives, score, and point milestones]
 */

public class PlayerData : MonoBehaviour
{
    //singelton for PlayerData
    private static PlayerData _instance;
    public static PlayerData Instance { get { return _instance; } }

    //player lives and score
    public int playerLives = 3;
    public int playerScore = 0;

    //point milestones
    private readonly int[] pointMilestones = new int[] { 1000, 2500, 4250, 6500, 9000 };
    private int currentPointMilestone = 0;

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
        //Subscribe to this function on the game event start: resets all player data back to default
        GameEventBus.Subscribe(GameState.startGame, ResetPlayerData);

        //Subscribe to these functions on player events gained life, and lost life: adds a life to the player, removes a life on player death, and checks the game is over
        PlayerEventBus.Subscribe(PlayerEvent.gainedLife, AddLife);
        PlayerEventBus.Subscribe(PlayerEvent.lostLife, LoseLife);
        PlayerEventBus.Subscribe(PlayerEvent.lostLife, CheckLives);
    }

    private void OnDisable()
    {
        //Unsubscribe from this function on the game event start: resets all player data back to default
        GameEventBus.Unsubscribe(GameState.startGame, ResetPlayerData);

        //Unsubscribe from these functions on player events gained life, and lost life: adds a life to the player, removes a life on player death, and checks the game is over
        PlayerEventBus.Unsubscribe(PlayerEvent.gainedLife, AddLife);
        PlayerEventBus.Unsubscribe(PlayerEvent.lostLife, LoseLife);
        PlayerEventBus.Unsubscribe(PlayerEvent.lostLife, CheckLives);
    }

    /// <summary>
    /// checks when the player runs out of lives and ends the game
    /// </summary>
    private void CheckLives()
    {
        //if the player has 0 lives
        if (playerLives <= 0)
        {
            //publish the game over event
            GameEventBus.Publish(GameState.gameOver);
        }
    }

    /// <summary>
    /// adds points to the players score
    /// </summary>
    /// <param name="points"> the points that are being passed to the player </param>
    /// <returns> the players new score </returns>
    public int AddScore(int points)
    {
        //add points to the players score
        playerScore += points;

        //if the player has crossed or reached a point milestone
        if (playerScore >= pointMilestones[currentPointMilestone])
        {
            //award another life
            PlayerEventBus.Publish(PlayerEvent.gainedLife);

            //increase the current point milestone if the play has not already hit the highest milestone
            if (currentPointMilestone != pointMilestones.Length)
            {
                currentPointMilestone++;
            }
        }

        //return the players new score
        return playerScore;
    }

    /// <summary>
    /// takes a life away from the player
    /// </summary>
    public void LoseLife()
    {
        //decrement lives by 1
        playerLives--;
    }

    /// <summary>
    /// adds a life to the player
    /// </summary>
    public void AddLife()
    {
        //increment lives by 1
        playerLives++;
    }

    /// <summary>
    /// Resets all player data to default values
    /// </summary>
    private void ResetPlayerData()
    {
        //set player lives to 3, player score to 0, and point milestone to 0
        playerLives = 3;
        playerScore = 0;
        currentPointMilestone = 0;
    }
}
