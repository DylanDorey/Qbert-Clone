using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/*
 * Author: [Dorey, Dylan]
 * Last Updated: [3/05/2024]
 * [Subscribes, Unsubsribes, and Publishes Game events based upon the game's current state]
 */

public class GameEventBus
{
    //Initialize a dictionary of game events
    private static readonly IDictionary<GameState, UnityEvent> Events = new Dictionary<GameState, UnityEvent>();

    /// <summary>
    /// Adds a listener to a specific game event
    /// </summary>
    /// <param name="eventType"> the specific game event </param>
    /// <param name="listener"> the function/method getting added to the game event</param>
    public static void Subscribe(GameState eventType, UnityAction listener)
    {
        //the event
        UnityEvent thisEvent;

        //if the function is assigned to the specific unity event
        if (Events.TryGetValue(eventType, out thisEvent))
        {
            //add it as a listener
            thisEvent.AddListener(listener);
        }
        else
        {
            //otherwise it is a new listener
            thisEvent = new UnityEvent();
            thisEvent.AddListener(listener);
            Events.Add(eventType, thisEvent);
        }
    }

    /// <summary>
    /// Removes a listener from a specific game event
    /// </summary>
    /// <param name="type"> the specific game event </param>
    /// <param name="listener"> the function/method getting removed from the game event </param>
    public static void Unsubscribe(GameState type, UnityAction listener)
    {
        //the event
        UnityEvent thisEvent;

        //if the function is equal to the event being removed
        if (Events.TryGetValue(type, out thisEvent))
        {
            //remove the function from the list of listeners
            thisEvent.RemoveListener(listener);
        }
    }

    /// <summary>
    /// Establishes the current event happening
    /// </summary>
    /// <param name="type"> the specific event </param>
    public static void Publish(GameState type)
    {
        //the event
        UnityEvent thisEvent;

        //Invoke the various functions for the event
        if (Events.TryGetValue(type, out thisEvent))
        {
            thisEvent.Invoke();
        }
    }
}
