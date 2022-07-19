using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]

public class InputManagerScript : MonoBehaviour
{
    public static Player_input inputActions;
    public static event Action<InputActionMap> actionMapChange;

    Vector2 touchPos = Vector2.zero;
    Vector2 touchDirection = Vector2.zero;
    private bool isTouchingScreen = false;

    private static InputManagerScript instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one Input Manager in the scene.");
        }
        instance = this;
        inputActions = new Player_input();
    }
    void Start()
    {
        ToggleActionMap(inputActions.Player);
        inputActions.Player.TouchPress.started += cxt => StartTouch();
        inputActions.Player.TouchPress.canceled += cxt => StopTouch();
        inputActions.Player.TouchDirection.performed += cxt => ReadTouch();
        //inputActions.Player.TouchDirection.canceled += cxt => StopTouch();
    }

    private void StopTouch()
    {
        isTouchingScreen = false;
        touchPos = Vector2.zero;
        touchDirection = Vector2.zero;
        //Debug.LogError("Stopped" + touchDirection);
    }

    private void StartTouch()
    {
        isTouchingScreen = true;
    }

    private void ReadTouch()
    {

        if (inputActions.Player.Touch.phase == InputActionPhase.Started)
        {
            if (touchPos != Vector2.zero)
            {
                touchPos = inputActions.Player.TouchPosition.ReadValue<Vector2>();
                touchDirection = inputActions.Player.TouchDirection.ReadValue<Vector2>();
                //Debug.Log("Began" + touchDirection);
            }
            else
            {
                touchDirection = inputActions.Player.TouchDirection.ReadValue<Vector2>();
                //Debug.LogWarning("Moved" + touchDirection);
            }
        }
    }

    public Vector2 GetTouchDirection()
    {
        return touchDirection;
    }

    public bool GetTouchState()
    {
        return isTouchingScreen;
    }

    public static InputManagerScript GetInstance()
    {
        return instance;
    }

    public static void ToggleActionMap(InputActionMap actionMap)
    {
        if (actionMap.enabled)
            return;

        inputActions.Disable();
        actionMapChange?.Invoke(actionMap);
        actionMap.Enable();
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }
    private void OnDisable()
    {
        inputActions.Disable();
    }
}
