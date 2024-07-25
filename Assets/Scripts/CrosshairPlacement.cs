using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class CrosshairPlacement : MonoBehaviour
{
    private bool hasInstantiatedCrosshair = false;
    [SerializeField]private GameObject crosshairPrefab;
    [SerializeField]private ARRaycastManager arRaycastManager;
    private GameObject instantiatedCrossHair;

    void Update()
    {
        try {
            Debug.Log("Started Crosshair");
            Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
            Debug.Log(screenCenter);
            List<ARRaycastHit> hits = new List<ARRaycastHit>();
            Debug.Log(hits);
            if (arRaycastManager.Raycast(screenCenter, hits, TrackableType.Planes))
            {
                ARRaycastHit hit = hits[0];
                Debug.Log("In if");
                if (!hasInstantiatedCrosshair)
                {
                    Debug.Log("Has no Crosshair");

                    Debug.Log("Hit");
                    Debug.Log(hit);
                    instantiatedCrossHair = Instantiate(crosshairPrefab, hit.pose.position, hit.pose.rotation);
                    hasInstantiatedCrosshair = true;
                }
                else
                {
                    instantiatedCrossHair.transform.SetPositionAndRotation(hit.pose.position, hit.pose.rotation);
                }
            }
            else
            {
                Debug.Log("In else");
                //if (hasInstantiatedCrosshair)
                //{
                //    DestroyCrosshair();
                //    hasInstantiatedCrosshair = false;
                //}
            }

        }catch(System.Exception e)
        {
            Debug.Log(e.Message);
        }
        
    }
    //void DestroyCrosshair()
    //{
    //    Destroy(GameObject.FindWithTag("Crosshair"));
    //}
}
