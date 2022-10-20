using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameInput
{
    public class PlayerInputHandler : MonoBehaviour
    {
        public Vector3 moveDirInput;
        public float moveInput;
        public bool actionInput;
        public bool interactInput;

        private bool inputEnabled;

        public event Action OnChangeRobot;

        private void Start()
        {
            InputEnabled(true);
        }

        public void InputEnabled(bool state)
        {
            inputEnabled = state;
        }

        public void OnMoveDirInput(InputAction.CallbackContext context)
        {
            if (!inputEnabled)
                return;


            moveDirInput.x = context.ReadValue<Vector2>().x;
            moveDirInput.z = context.ReadValue<Vector2>().y;
            moveDirInput.Normalize();

            if (moveDirInput != Vector3.zero)
                moveInput = 1;
            else moveInput = 0;
        }

        public void OnPlayerChangeRobot(InputAction.CallbackContext context)
        {
            if (!inputEnabled)
                return;

            if (context.performed)
                OnChangeRobot?.Invoke();

        }

        public void OnPlayerAction(InputAction.CallbackContext context)
        {
            if (!inputEnabled)
                return;

            if (context.started)
                actionInput = true;

            if (context.canceled)
                actionInput = false;
        }

        public void OnPause(InputAction.CallbackContext context)
        {
            if (!inputEnabled)
                return;

            if (context.performed)
                PauseMenu.Instance.Pause();
        }

        public void UseActionInput()
        {
            actionInput = false;
        }

        public void UseInteractInput()
        {
            interactInput = false;
        }
    }
}