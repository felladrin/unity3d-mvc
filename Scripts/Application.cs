using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Application : MonoBehaviour
{
    public ModelContainer Model;
    public ControllerContainer Controller;
    public ViewContainer View;

    private Dictionary<string, UnityEvent> eventDictionary = new Dictionary<string, UnityEvent>();

    /// <summary>
    /// Add listener to a given event.
    /// Usage: Call it inside OnEnable() method of MonoBehaviours.
    /// </summary>
    /// <param name="eventName">Name of the event.</param>
    /// <param name="listener">Callback function.</param>
    public void AddEventListener(string eventName, UnityAction listener)
    {
        UnityEvent e;
        if (eventDictionary.TryGetValue(eventName, out e))
        {
            e.AddListener(listener);
        }
        else
        {
            e = new UnityEvent();
            e.AddListener(listener);
            eventDictionary.Add(eventName, e);
        }
    }

    /// <summary>
    /// Remove listener from a given event.
    /// Usage: Call it inside OnDisable() method of MonoBehaviours.
    /// </summary>
    /// <param name="eventName">Name of the event.</param>
    /// <param name="listener">Callback function.</param>
    public void RemoveEventListener(string eventName, UnityAction listener)
    {
        UnityEvent e;
        if (eventDictionary.TryGetValue(eventName, out e))
        {
            e.RemoveListener(listener);
        }
    }

    /// <summary>
    /// Triggers all registered callbacks of a given event.
    /// </summary>
    public void TriggerEvent(string eventName)
    {
        UnityEvent e;
        if (eventDictionary.TryGetValue(eventName, out e))
        {
            e.Invoke();
        }
    }
}