using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleInput : IState
{
    private InputManager inputManager;
    private CameraManager cameraManager;
    private SelectionManager selectionManager;

    public IdleInput(InputManager inputManager, CameraManager cameraManager, SelectionManager selectionManager)
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
        cameraManager.MouseZoom();

        selectionManager.MouseSelect();
    }

    public void OnEnter() { }

    public void OnExit() { }
}
