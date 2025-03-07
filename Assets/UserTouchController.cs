using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class UserTouchControllerNew : MonoBehaviour
{
    private ARRaycastManager rayCastManager;
    private GameObject spawnedRoom;
    [SerializeField] private GameObject _room;

    static List<ARRaycastHit> aRRaycastHits = new List<ARRaycastHit>();


    private void Awake()
    {
        rayCastManager = GetComponent<ARRaycastManager>();
    }

    bool TryGetTouchPosition(out Vector2 touchPosition)
    {

        Debug.Log(Input.touchCount);
        if (Input.touchCount >0)
        {
            touchPosition = Input.GetTouch(0).position;
            return true;
        }

        touchPosition = default;
        return false;
    }

    private void Update()
    {
        if(!TryGetTouchPosition(out Vector2 touchPosition))
        {
            return;
        }
        if(rayCastManager.Raycast(touchPosition, aRRaycastHits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
        {
            var hitPosition = aRRaycastHits[0].pose;
            Debug.Log(spawnedRoom + "sp");
            if(spawnedRoom == null)
            {
                spawnedRoom = Instantiate(_room, hitPosition.position, hitPosition.rotation);
                spawnedRoom.tag = "Room";
            }
        }
    }
}
