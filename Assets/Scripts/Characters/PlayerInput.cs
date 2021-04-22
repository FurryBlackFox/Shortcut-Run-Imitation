using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TouchPhase = UnityEngine.TouchPhase;

public enum InputMode
{
    Debug,
    InputSystem
}

public class PlayerInput : MonoBehaviour
{
    public static event Action<Vector2> OnMovementInput;
    public static event Action OnJump;

    [SerializeField] private InputMode inputMode;

    private PlayerInputController playerInputController;
    private Vector2 input;

    private int screenWidth;
    
    private void Awake()
    {
        screenWidth = Screen.width;
        playerInputController = new PlayerInputController();
        if (inputMode == InputMode.InputSystem)
        {
            playerInputController.Player.Rotate.performed += cxt =>
            {
                var value = cxt.ReadValue<float>();
                OnMovementInput?.Invoke(new Vector2(value, 0f));
            };
        }

    }

    private void OnEnable()
    {
        playerInputController.Enable();
    }

    private void OnDisable()
    {
        playerInputController.Disable();
    }

    private void Update()
    {
        if(inputMode == InputMode.Debug)
            GetDebugInput();
    }
    
    private void GetDebugInput()
    {
        input.x = Input.GetAxis("Horizontal");
        input.y = Input.GetAxis("Vertical");
            
        OnMovementInput?.Invoke(input);

        if(Input.GetKeyDown(KeyCode.Space))
            OnJump?.Invoke();
    }

}
