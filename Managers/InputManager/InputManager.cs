using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InputManager : MonoBehaviour
{
    public CameraManager cameraManager;
    public SelectionManager selectionManager;

    private StateMachine stateMachine;

    private void Awake()
    {
        stateMachine = new StateMachine();

        IdleInput idleInput = new IdleInput(this, cameraManager, selectionManager);
        ShipSelected shipSelected = new ShipSelected(this, cameraManager, selectionManager);

        AddTransition(idleInput, shipSelected, IsShipSelected());
        AddTransition(shipSelected, idleInput, IsNoShipSelected());

        stateMachine.SetState(idleInput);

        void AddTransition(IState to, IState from, Func<bool> condition) => stateMachine.AddTransition(to, from, condition);
        Func<bool> IsShipSelected() => () => selectionManager.hasSelected;
        Func<bool> IsNoShipSelected() => () => !selectionManager.hasSelected;
    }

    private void Update()
    {
        stateMachine.Tick();
    }
}
