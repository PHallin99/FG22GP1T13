using System;
using GameInput;
using Managers;
using UnityEngine;

namespace Player
{
    internal enum MovementType
    {
        MoveTowards,
        SmoothDamp
    }

    internal enum RotationType
    {
        SlowEnd,
        LinearTimeBased
    }

    public class CharacterController : MonoBehaviour
    {
        public bool movementEnabled;

        [Header("Falling")] [SerializeField] private float gravity;

        [SerializeField] [Range(0, 5f)] [Tooltip("Gravity multiplication when falling")]
        private float fallingGravityMultiplier;

        [SerializeField] [Tooltip("At what downwards velocity the players counts as falling")] [Range(-2f, 10f)]
        private float startFallingVelocity;


        [Header("Movement")] [SerializeField] [Range(4f, 16f)]
        private float moveSpeedMax;

        [SerializeField]
        [Tooltip("How the player accelerates/deaccelerates. " +
                 "Using either Speed(MoveTowards) or Time(SmoothDamp)")]
        private MovementType movementType;

        [Space]
        [Header("MoveTowards")]
        [SerializeField]
        [Tooltip("How much speed is added per step untill the player reaches MaxSpeed")]
        [Range(0.01f, 1f)]
        private float accelSpeedMT;

        [SerializeField]
        [Tooltip("How much speed is removed per step untill the player stands still")]
        [Range(0.01f, 1f)]
        private float deaccelSpeedMT;

        [Space]
        [Header("SmoothDamp")]
        [SerializeField]
        [Tooltip("Time it takes for the player to speed up")]
        [Range(0f, 1f)]
        private float accelTimeSD;

        [SerializeField] [Tooltip("Time it takes for the player to slow down")] [Range(0f, 1f)]
        private float deaccelTimeSD;

        [Space]
        [Header("Rotation")]
        [SerializeField]
        [Tooltip("How the player accelerates/deaccelerates. " +
                 "Using either Speed(MoveTowards) or Time(SmoothDamp)")]
        private RotationType rotationType;

        [Header("SlowEnd")] [SerializeField] [Range(0.01f, 0.5f)]
        private float rotationSpeed;

        [Header("LinearTimeBased")] [SerializeField] [Range(0.0f, 1f)]
        private float rotationTime;

        [Header("X Rotation")] [SerializeField]
        private float minXRotation;

        [SerializeField] private float maxXRotation;

        [Header("Debug")] [SerializeField] private float targetSpeed;

        [Space] [SerializeField] private float speed;

        [Space] [SerializeField] private Vector3 currentGravity;

        [Space] [SerializeField] private float currentSpeed;

        [Space] [SerializeField] private Quaternion targetRotation;

        [Space] [SerializeField] private Quaternion currentRotation;

        [Space] [SerializeField] private Vector3 moveTargetDir;

        [Space] [SerializeField] private Quaternion originalRotation;

        [Space] [SerializeField] private Quaternion lastFrameRotationTarget;


        [SerializeField] private PlayerInputHandler inputHandler;

        private Camera cam;
        private Rigidbody rb;
        private PlayableRobot robot;
        private float timeElapsed;


        private void Awake()
        {
            robot = GetComponent<PlayableRobot>();
            rb = GetComponent<Rigidbody>();
            cam = Camera.main;
            rb.useGravity = false;
            movementEnabled = true;
            PlayerManager.GetInstance().OnInputChange += InputChange;
        }

        private void FixedUpdate()
        {
            HandleGravity();
            HandleAnimations();

            if (inputHandler == null)
                return;
            if (!movementEnabled)
                return;

            HandleXRotation();
            HandleRotation();
            HandleMovement();
        }

        private void InputChange()
        {
            inputHandler = null;
            inputHandler = GetComponentInChildren<PlayerInputHandler>();

            if (inputHandler == null)
                ResetMovement();
        }

        public void ResetMovement()
        {
            targetSpeed = 0;
            speed = 0;
        }

        public void SetMovement(Vector3 velocity)
        {
            rb.velocity = velocity;
        }

        public void SetMovement(float multiplier)
        {
            rb.velocity *= multiplier;
        }

        private void HandleMovement()
        {
            targetSpeed = inputHandler.moveInput * moveSpeedMax;

            switch (movementType)
            {
                case MovementType.MoveTowards:
                    MoveTowardsMovement();
                    break;
                case MovementType.SmoothDamp:
                    SmoothDampMovement();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            rb.velocity = new Vector3(moveTargetDir.x * speed, rb.velocity.y, moveTargetDir.z * speed);
        }

        private void HandleRotation()
        {
            switch (rotationType)
            {
                case RotationType.SlowEnd:
                    SlowEndRotation();
                    break;
                case RotationType.LinearTimeBased:
                    SlerpRotation();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            moveTargetDir = transform.forward;
        }

        private void HandleXRotation()
        {
            var xRotation = 0f;
            switch (targetSpeed)
            {
                case > 0:
                {
                    var t = speed / moveSpeedMax;
                    xRotation = Mathf.Lerp(0, maxXRotation, t);
                    break;
                }
                case 0:
                {
                    var t = speed / moveSpeedMax;
                    xRotation = Mathf.Lerp(-maxXRotation, 0, t);
                    break;
                }
            }

            //    rb.transform.rotation = Quaternion.Euler(xRotation, rb.transform.rotation.y, rb.transform.rotation.z) ;
        }

        private void HandleGravity()
        {
            currentGravity = gravity * Vector3.up;
            if (rb.velocity.y < -startFallingVelocity)
                currentGravity *= fallingGravityMultiplier;
            rb.AddForce(-currentGravity, ForceMode.Acceleration);
        }

        private void HandleAnimations()
        {
            if (rb.velocity == Vector3.zero)
            {
                robot.isMoving = false;
                robot.isIdle = true;
            }
            else
            {
                robot.isIdle = false;
                robot.isMoving = true;
            }
        }

        #region Movement

        private void SmoothDampMovement()
        {
            speed = targetSpeed == 0
                ? Mathf.SmoothDamp(speed, targetSpeed, ref currentSpeed, deaccelTimeSD)
                : Mathf.SmoothDamp(speed, targetSpeed, ref currentSpeed, accelTimeSD);
        }

        private void MoveTowardsMovement()
        {
            speed = Mathf.MoveTowards(speed, targetSpeed, targetSpeed == 0 ? deaccelSpeedMT : accelSpeedMT);
        }

        #endregion

        #region Rotation

        private void SlowEndRotation()
        {
            var targetDir = cam.transform.forward * inputHandler.moveDirInput.z;
            targetDir += cam.transform.right * inputHandler.moveDirInput.x;
            targetDir.y = 0;


            if (targetDir == Vector3.zero) targetDir = rb.transform.forward;

            var tr = Quaternion.LookRotation(targetDir);
            rb.transform.rotation = Quaternion.Slerp(rb.transform.rotation, tr, rotationSpeed);

            targetRotation = tr;
            currentRotation = rb.transform.rotation;
        }

        private void SlerpRotation()
        {
            var targetDir = cam.transform.forward * inputHandler.moveDirInput.z;
            targetDir += cam.transform.right * inputHandler.moveDirInput.x;
            targetDir.y = 0;

            if (targetDir == Vector3.zero) targetDir = rb.transform.forward;

            var tr = Quaternion.LookRotation(targetDir);

            if (tr != lastFrameRotationTarget)
            {
                originalRotation = rb.transform.rotation;
                timeElapsed = 0;
            }

            lastFrameRotationTarget = tr;

            if (!(timeElapsed < rotationTime)) return;
            rb.transform.rotation = Quaternion.Lerp(originalRotation, tr, timeElapsed / rotationTime);
            timeElapsed += Time.deltaTime;
            Debug.Log("Rotating");
        }

        #endregion
    }
}