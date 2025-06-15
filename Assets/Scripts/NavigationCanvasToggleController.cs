using System;
using UnityEngine;
using UnityEngine.Serialization;

public class NavigationCanvasToggleController : MonoBehaviour
{
    [SerializeField] private GameObject navigationCanvasToToggle;

    public bool isCanvasVisible = false;

    void Start()
    {
        navigationCanvasToToggle.SetActive(false);
    }

    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.Start))
        {
            ToggleCanvas();
        }
        
        bool isPinching = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) > 0.9f &&
                          OVRInput.GetConnectedControllers().HasFlag(OVRInput.Controller.Hands);

        if (isPinching)
        {
            ToggleCanvas();
        }
    }

    private void ToggleCanvas()
    {
        isCanvasVisible = !isCanvasVisible;
        if (navigationCanvasToToggle != null)
        {
            navigationCanvasToToggle.SetActive(isCanvasVisible);
        }
    }
}