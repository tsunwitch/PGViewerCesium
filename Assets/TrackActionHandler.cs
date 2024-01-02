using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class TrackActionHandler : MonoBehaviour
{
    public GameObject originShifter;
    public GlobalClock clock;

    private void Awake()
    {
        originShifter = GameObject.Find("OriginShifter");
        clock = GameObject.FindGameObjectWithTag("GlobalClock").GetComponent<GlobalClock>();
    }

    [ContextMenu("Focus Track")]
    public void focusTrack()
    {
        originShifter.transform.parent = transform.GetChild(transform.childCount - 1).transform;
    }

    [ContextMenu("Destroy Track")]
    public void destroyTrack()
    {
        //Move originShifter out of track
        originShifter.transform.parent = transform.parent;

        //Destroy track container with its contents
        DestroyImmediate(gameObject);

        //Set new timeframe with globalClock
        clock.resetTimeFrame();
    }
}
