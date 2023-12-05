using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackActionHandler : MonoBehaviour
{
    public GameObject originShifter;

    private void Awake()
    {
        originShifter = GameObject.Find("OriginShifter");
    }

    [ContextMenu("Focus Track")]
    public void focusTrack()
    {
        originShifter.transform.parent = transform.GetChild(transform.childCount - 1).transform;
        Debug.Log("Focused " + gameObject.name);
    }

    [ContextMenu("Destroy Track")]
    public void destroyTrack()
    {
        //Move originShifter out of track
        originShifter.transform.parent = transform.parent;

        Debug.Log("Destroyed " + gameObject.name);
        Destroy(gameObject);
    }
}
