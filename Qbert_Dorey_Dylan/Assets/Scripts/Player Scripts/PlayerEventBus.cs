using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/*
 * Author: [Dorey, Dylan]
 * Last Updated: [2/29/2024]
 * [Subscribes, Unsubsribes, and Publishes Player events based upon the players current state]
 */

public class PlayerEventBus
{
    //Initialize a dictionary of player events
    private static readonly IDictionary<PlayerEvent, UnityEvent> Events = new Dictionary<PlayerEvent, UnityEvent>();

    /// <summary>
    /// Adds a listener to a specific player event
    /// </summary>
    /// <param name="eventType"> the specific player event </param>
    /// <param name="listener"> the function/method getting added to player event</param>
    public static void Subscribe(PlayerEvent eventType, UnityAction listener)
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
            //otherwise add it as a new listener
            thisEvent = new UnityEvent();
            thisEvent.AddListener(listener);
            Events.Add(eventType, thisEvent);
        }
    }

    /// <summary>
    /// Removes a listener from a specific player event
    /// </summary>
    /// <param name="type"> the specific player event </param>
    /// <param name="listener"> the function/method getting removed from the player event </param>
    public static void Unsubscribe(PlayerEvent type, UnityAction listener)
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
    public static void Publish(PlayerEvent type)
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
