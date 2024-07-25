using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.AR;

public class ObjectPlacing : MonoBehaviour
{
    private ARPlacementInteractable placementInteractable;
    [SerializeField] private GameObject objectA;
    [SerializeField] private GameObject objectB;
    private TextMeshProUGUI uiGuide;
    // This function will be called when an object is placed
    public void PlaceObjectA(ARObjectPlacementEventArgs args)
    {
        uiGuide = GameObject.FindWithTag("UserGuide").GetComponent<TextMeshProUGUI>();
        // Retrieve the ARPlacementInteractable component
        placementInteractable = args.placementInteractable;
        uiGuide.text = "";

        // Check if the placementPrefab is objectA
        if (placementInteractable.placementPrefab == objectA)
        {
            // If objectA is placed, change the placementPrefab to objectB
            placementInteractable.placementPrefab = objectB;
        }
    }
}