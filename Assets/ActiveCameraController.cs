using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveCameraController : MonoBehaviour
{
    public GameObject TPPCamera, FPPCamera, VRPCamera;

    // Start is called before the first frame update
    void Start()
    {
        TPPCamera.SetActive(true);
        FPPCamera.SetActive(false);
    }

    public void ChangeCameraToTPP()
    {
        FPPCamera.SetActive(false);
        TPPCamera.SetActive(true);
    }

    public void ChangeCameraToFPP()
    {
        TPPCamera.SetActive(false);
        FPPCamera.SetActive(true);
    }
}
