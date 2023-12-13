using CesiumForUnity;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FixSpawner : MonoBehaviour
{
    public GameObject fixObject;
    public GameObject trackDescriptor;
    public GameObject globalClock;
    public GameObject trackControllerObject;
    private LineRenderer lineRenderer;
    private int trackCount = 0;

    public void SpawnFixes(List<Fix> fixData)
    {
        List<GameObject> fixObjects = new List<GameObject>();
        ActiveTrackController activeTrackController = trackControllerObject.GetComponent<ActiveTrackController>();
        GameObject previousFix = null;


        if (fixData == null)
        {
            Debug.Log("No fix data to spawn...");
            return;
        }

        //Assigning to Tracks
        trackCount++;
        GameObject trackDescriptorInstance = Instantiate(trackDescriptor, trackControllerObject.transform);
        trackDescriptorInstance.name = "Track" + trackCount;
        trackDescriptorInstance.GetComponent<PilotMovementHandler>().fixes = fixObjects;
        trackDescriptorInstance.tag = "TrackInstance";
        activeTrackController.loadTrackUI(trackDescriptorInstance);

        CesiumGlobeAnchor anchorHandler = fixObject.gameObject.GetComponent<CesiumGlobeAnchor>();

        Debug.Log("Spawning fixes!");
        foreach (Fix fix in fixData)
        {
            anchorHandler.longitudeLatitudeHeight = fix.coordinates;
            GameObject instantiatedFix = Instantiate(fixObject, trackDescriptorInstance.transform);
            instantiatedFix.GetComponent<FixData>().timestamp = fix.timestamp.TimeOfDay;
            fixObjects.Add(instantiatedFix);

            previousFix = instantiatedFix;
        }

        //Resetting the TrackDescriptor position
        trackDescriptorInstance.transform.position = new Vector3(0, 0, 0);

        //Setting the timeline according to the track
        globalClock.GetComponent<GlobalClock>().setTimeframe(fixData.FirstOrDefault().timestamp, fixData.LastOrDefault().timestamp);

        //Code for trackDescriptor lineRenderer
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = fixObjects.Count;
            for (int i = 0; i < fixObjects.Count; i++)
            {
                lineRenderer.SetPosition(i, fixObjects[i].transform.position);
            }
        }
    }
}