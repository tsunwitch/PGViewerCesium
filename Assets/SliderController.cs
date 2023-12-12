using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SliderController : MonoBehaviour
{
    public GameObject clock;

    public void OnPointerClick(PointerEventData eventData)
    {
        //JumpToTimestamp();
    }

    public void JumpToTimestamp(TimeSpan targetValue)
    {
        GameObject[] movementHandlers = GameObject.FindGameObjectsWithTag("PilotInstance");

        foreach (GameObject movementHandler in movementHandlers)
        {
            movementHandler.GetComponent<PilotMovementHandler>().SetCurrentWaypoint(targetValue);
        }
    }
}
