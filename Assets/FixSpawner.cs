using CesiumForUnity;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEditor.UIElements;
using UnityEngine;

public class FixSpawner : MonoBehaviour
{
    public GameObject fixObject;
    public GameObject trackDescriptor;
    public GameObject globalClock;
    private LineRenderer lineRenderer;
    private int trackCount = 0;

    //Color list for tracks
    private Color[] colorPool = { Color.red, Color.green, Color.blue, Color.yellow, Color.cyan, Color.magenta };

    public void SpawnFixes(List<Fix> fixData)
    {
        List<GameObject> fixObjects = new List<GameObject>();
        GameObject previousFix = null;
        System.Random rnd = new System.Random();
        Color randomColor = colorPool[rnd.Next() % colorPool.Length];

        if (fixData == null)
        {
            Debug.Log("No fix data to spawn...");
            return;
        }

        //Assigning to Tracks
        trackCount++;
        GameObject trackDescriptorInstance = Instantiate(trackDescriptor, transform.parent);
        trackDescriptorInstance.name = "Track" + trackCount;
        trackDescriptorInstance.GetComponent<PilotMovementHandler>().fixes = fixObjects;

        //Setting up the Line Renderer
        lineRenderer = trackDescriptorInstance.GetComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
        lineRenderer.startWidth = 1f;
        lineRenderer.endWidth = 1f;
        lineRenderer.startColor = randomColor;
        lineRenderer.endColor = randomColor;

        CesiumGlobeAnchor anchorHandler = fixObject.gameObject.GetComponent<CesiumGlobeAnchor>();
        FixData fixDataHandler = fixObject.gameObject.GetComponent<FixData>();

        Debug.Log("Spawning fixes!");
        foreach (Fix fix in fixData)
        {
            anchorHandler.longitudeLatitudeHeight = fix.coordinates;
            fixDataHandler.timestamp = fix.timestamp.TimeOfDay;
            GameObject instantiatedFix = Instantiate(fixObject, trackDescriptorInstance.transform);
            fixObjects.Add(instantiatedFix);

            previousFix = instantiatedFix;
        }

        //Resetting the TrackDescriptor position
        trackDescriptorInstance.transform.position = new Vector3(0, 0, 0);

        //Setting the timeline according to the track
        globalClock.GetComponent<GlobalClock>().setTimeframe(fixData.FirstOrDefault().timestamp, fixData.LastOrDefault().timestamp);

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