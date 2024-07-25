using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class UIGuide : MonoBehaviour
{
    private bool hasInstantiatedCrosshair = false;
    [SerializeField] private ARRaycastManager arRaycastManager;
    [SerializeField] private TextMeshProUGUI uiGuide;
    void Update()
    {
        try
        {
            Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
            List<ARRaycastHit> hits = new List<ARRaycastHit>();
            if (arRaycastManager.Raycast(screenCenter, hits, TrackableType.Planes))
            {
                ARRaycastHit hit = hits[0];
                if (!hasInstantiatedCrosshair)
                {
                    Debug.Log(hit);
                    uiGuide.text = "Place map by tapping on a surface";
                    hasInstantiatedCrosshair = true;
                }
            }
            else
            {
                //Debug.Log("In else");
            }

        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
        }

    }
}
