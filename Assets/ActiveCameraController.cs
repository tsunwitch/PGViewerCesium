using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveCameraController : MonoBehaviour
{
    public GameObject TPPCamera, FPPCamera, VRPCamera;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ChangeCameraToTPP()
    {
        TPPCamera.SetActive(false);
        TPPCamera.SetActive(true);
    }

    public void ChangeCameraToFPP()
    {
        FPPCamera.SetActive(false);
        FPPCamera.SetActive(true);
    }
}
