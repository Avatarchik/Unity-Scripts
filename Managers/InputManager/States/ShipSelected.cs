using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipSelected : IState
{
    private InputManager inputManager;
    private CameraManager cameraManager;
    private SelectionManager selectionManager;

    public ShipSelected(InputManager inputManager, CameraManager cameraManager, SelectionManager selectionManager)
    {
        this.inputManager = inputManager;
        this.cameraManager = cameraManager;
        this.selectionManager = selectionManager;
    }

    public void Tick()
    {
        cameraManager.KeyboardPan();
        cameraManager.KeyboardZoom();
        cameraManager.KeyboardRotate();
        cameraManager.MousePan();
        // cameraManager.MouseZoom();

        selectionManager.MouseSelect();
        selectionManager.MouseVerticalOffset();
        selectionManager.MouseHorizontalPosition();
        selectionManager.MouseSetDestination();
    }

    public void OnEnter() { }

    public void OnExit() { }
}
