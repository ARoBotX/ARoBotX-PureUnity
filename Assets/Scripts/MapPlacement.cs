using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
//using XR.Interaction.Toolkit;

public class MapPlacement : XRBaseInteractable
{
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        Debug.Log("Interactable Selected");
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        Debug.Log("Interactable Unselected");
    }

    protected override void OnActivated(ActivateEventArgs args)
    {
        base.OnActivated(args);
        Debug.Log("Interactable Activated");
    }

    protected override void OnDeactivated(DeactivateEventArgs args)
    {
        base.OnDeactivated(args);
        Debug.Log("Interactable Deactivated");
    }
}
