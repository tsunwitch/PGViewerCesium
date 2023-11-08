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
        }

        //Assigning to Tracks
        trackCount++;
        GameObject trackDescriptorInstance = Instantiate(trackDescriptor, transform);
        trackDescriptorInstance.name = "Track" + trackCount;
        trackDescriptorInstance.GetComponent<CesiumGlobeAnchor>().longitudeLatitudeHeight = fixData.FirstOrDefault().coordinates;

        //dupa
        //Setting up the Line Renderer
        lineRenderer = trackDescriptorInstance.GetComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
        lineRenderer.startWidth = 0.4f;
        lineRenderer.endWidth = 0.4f;
        lineRenderer.startColor = randomColor;
        lineRenderer.endColor = randomColor;

        CesiumGlobeAnchor anchorHandler = fixObject.gameObject.GetComponent<CesiumGlobeAnchor>();

        Debug.Log("Spawning fixes!");
        foreach (Fix fix in fixData)
        {
            anchorHandler.longitudeLatitudeHeight = fix.coordinates;
            GameObject instantiatedFix = Instantiate(fixObject, trackDescriptorInstance.transform);
            fixObjects.Add(instantiatedFix);

            previousFix = instantiatedFix;
        }

        if (lineRenderer != null)
        {
            // Assuming the LineRenderer is set up to draw lines between child objects
            lineRenderer.positionCount = fixObjects.Count;
            for (int i = 0; i < fixObjects.Count; i++)
            {
                lineRenderer.SetPosition(i, fixObjects[i].transform.position);
            }
        }
    }
}