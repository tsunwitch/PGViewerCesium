
using Cinemachine;
using System.Collections;
using UnityEngine;
using UnityEngine.SpatialTracking;
using UnityEngine.XR.Management;

public class ActiveCameraController : MonoBehaviour
{
    public GameObject TPPCamera, FPPCamera, MainCamera;
    private TrackedPoseDriver trackedPoseDriver;
    private XRLoader loader;

    // Start is called before the first frame update
    void Start()
    {
        TPPCamera.SetActive(true);
        FPPCamera.SetActive(false);
        trackedPoseDriver = MainCamera.GetComponent<TrackedPoseDriver>();
        trackedPoseDriver.enabled = false;
    }

    public void ChangeCameraToTPP()
    {
        StopXR();
        FPPCamera.SetActive(false);
        trackedPoseDriver.enabled = false;
        TPPCamera.SetActive(true);
    }

    public void ChangeCameraToFPP()
    {
        StopXR();
        TPPCamera.SetActive(false);
        trackedPoseDriver.enabled = false;
        FPPCamera.SetActive(true);
    }

    public void ChangeCameraToVRP()
    {
        StartXR();
        TPPCamera.SetActive(false);
        FPPCamera.SetActive(true);
        trackedPoseDriver.enabled = true;
    }
    public void UpdateCameraTarget(GameObject target)
    {
        TPPCamera.GetComponent<CinemachineFreeLook>().Follow = target.transform;
        TPPCamera.GetComponent<CinemachineFreeLook>().LookAt = target.transform;

        FPPCamera.transform.SetParent(target.transform, false);
        FPPCamera.GetComponent<CinemachineVirtualCamera>().Follow = target.transform;

        //Move MainCamera to target, so VR orientation stays correct
        MainCamera.transform.SetParent(target.transform, false);
    }

    public void RemoveCameraTarget()
    {
        FPPCamera.transform.parent = null;
        MainCamera.transform.parent = null;
    }


    //XR loader functions
    public IEnumerator StartXRCoroutine()
    {
        Debug.Log("Initializing XR...");
        yield return XRGeneralSettings.Instance.Manager.InitializeLoader();

        if (XRGeneralSettings.Instance.Manager.activeLoader == null)
        {
            Debug.LogError("Initializing XR Failed. Check Editor or Player log for details.");
        }
        else
        {
            Debug.Log("Starting XR...");
            XRGeneralSettings.Instance.Manager.StartSubsystems();
        }
    }

    void StopXR()
    {
        Debug.Log("Stopping XR...");

        XRGeneralSettings.Instance.Manager.StopSubsystems();
        XRGeneralSettings.Instance.Manager.DeinitializeLoader();
        Debug.Log("XR stopped completely.");
    }

    void StartXR()
    {
        StartCoroutine(StartXRCoroutine());
    }
}
