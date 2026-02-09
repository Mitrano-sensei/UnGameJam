using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "Inputs/Input Reader")]
public class InputReader : ScriptableObject, InputSystem_Actions.IUIActions, InputSystem_Actions.IPlayerActions
{
    public event UnityAction<Vector2, bool> Point = delegate { }; // bool is true if the user is using the mouse, false for controller
    public event UnityAction Interact = delegate { };
    public event UnityAction<Vector2> Move = delegate { };
    
    private InputSystem_Actions inputActions;

    void OnEnable()
    {
        if (inputActions == null)
        {
            inputActions = new InputSystem_Actions();
            inputActions.Player.SetCallbacks(this);
            inputActions.UI.SetCallbacks(this);
        }

        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    #region UI

    public void OnNavigate(InputAction.CallbackContext context)
    {
        // throw new System.NotImplementedException();
    }

    public void OnSubmit(InputAction.CallbackContext context)
    {
        // throw new System.NotImplementedException();
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
        // throw new System.NotImplementedException();
    }

    public void OnPoint(InputAction.CallbackContext context)
    {
        Point.Invoke(context.ReadValue<Vector2>(), IsDeviceMouse(context));
    }

    bool IsDeviceMouse(InputAction.CallbackContext context) => context.control.device.name == "Mouse";


    public void OnClick(InputAction.CallbackContext context)
    {
        // throw new System.NotImplementedException();
    }

    public void OnRightClick(InputAction.CallbackContext context)
    {
        // throw new System.NotImplementedException();
    }

    public void OnMiddleClick(InputAction.CallbackContext context)
    {
        // throw new System.NotImplementedException();
    }

    public void OnScrollWheel(InputAction.CallbackContext context)
    {
        // throw new System.NotImplementedException();
    }

    public void OnTrackedDevicePosition(InputAction.CallbackContext context)
    {
        // throw new System.NotImplementedException();
    }

    public void OnTrackedDeviceOrientation(InputAction.CallbackContext context)
    {
        // throw new System.NotImplementedException();
    }

    #endregion

    #region Player

    public void OnMove(InputAction.CallbackContext context)
    {
        Move.Invoke(context.ReadValue<Vector2>());
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        // throw new System.NotImplementedException();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        // throw new System.NotImplementedException();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        Interact.Invoke();
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        // throw new System.NotImplementedException();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        // throw new System.NotImplementedException();
    }

    public void OnPrevious(InputAction.CallbackContext context)
    {
        // throw new System.NotImplementedException();
    }

    public void OnNext(InputAction.CallbackContext context)
    {
        // throw new System.NotImplementedException();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        // throw new System.NotImplementedException();
    }

    #endregion
}