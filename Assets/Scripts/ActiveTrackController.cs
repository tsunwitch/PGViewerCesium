using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActiveTrackController : MonoBehaviour
{
    public List<GameObject> tracks = new List<GameObject>();
    public GameObject trackContainer;
    public GameObject trackUIPrefab;

    public void loadTrackUI(GameObject trackDescriptorInstance)
    {
        GameObject instantiatedTrackUIElement = Instantiate(trackUIPrefab);
        instantiatedTrackUIElement.transform.parent = trackContainer.transform;
        instantiatedTrackUIElement.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = trackDescriptorInstance.name;


        //Tnitialize the remove button with destroyTrack() function
        Button removeButton = instantiatedTrackUIElement.transform.GetChild(2).GetComponent<Button>();
        removeButton.onClick.AddListener(() => 
        { 
            trackDescriptorInstance.GetComponent<TrackActionHandler>().destroyTrack();
            Destroy(instantiatedTrackUIElement);
        });

        //Initialize the focus button with focusTrack() function
        Button focusButton = instantiatedTrackUIElement.GetComponent<Button>();
        focusButton.onClick.AddListener(() => trackDescriptorInstance.GetComponent<TrackActionHandler>().focusTrack());
    }
}
