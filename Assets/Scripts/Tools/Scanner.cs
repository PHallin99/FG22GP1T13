using System.Collections;
using GameInput;
using Managers;
using Player;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using CharacterController = Player.CharacterController;

namespace Tools
{
    enum ScanningState
    {
        Inactive,
        Scanning,
    }

    [RequireComponent(typeof(AudioSource))]
    public class Scanner : MonoBehaviour
    {
        [SerializeField] private float radius;
        [SerializeField] private float rotationSpeed;
        [SerializeField] private float scanCooldown;
        [SerializeField] private float leaveScanTimer;
        [SerializeField] private float startScanMoveMultiplier;
        [Space] [SerializeField] private CharacterController movement;
        [SerializeField] private Transform ShootPoint;
        [SerializeField] private Transform FinalRotation;
        [Space] [SerializeField] private GameObject Zone;
        [SerializeField] private GameObject Mark;
        [Space] [SerializeField] private AudioClip scanDetectionBeep;
        [SerializeField] private AudioClip scanPing;
        [SerializeField] private float pingAudioDelay;
        private bool onPingCooldown;
        private AudioSource audioSource;

        [SerializeField] private PlayerInputHandler inputHandler;
        [SerializeField] private PlayableRobot robot;


        private ScanningState state;
        private Ray ray;
        private Vector3 rayDirection;
        private float lastScanTime;
        private float startScanTime;

        //Visuals
        private LineRenderer lr;
        private GameObject tempMark;
        private GameObject tempZone;
        private Ray theRay;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            lr = GetComponent<LineRenderer>();
            robot = GetComponentInParent<PlayableRobot>();
            movement = GetComponentInParent<CharacterController>();
            PlayerManager.GetInstance().OnInputChange += InputChange;

            state = ScanningState.Inactive;
        }


        private void Update()
        {
            if (inputHandler == null)
                return;
            HandleScanner();
        }

        private void HandleScanner()
        {
            switch (state)
            {
                case ScanningState.Inactive:
                    InactiveState();
                    break;
                case ScanningState.Scanning:
                    ScannerState();
                    if (!onPingCooldown) AudioStartPing();
                    break;
            }
        }

        private void InactiveState()
        {
            if (inputHandler.actionInput && Time.time > scanCooldown + lastScanTime)
                ChangeState(ScanningState.Scanning);
        }

        private void ScannerState()
        {
            while (inputHandler.actionInput)
            {
                transform.rotation =
                    Quaternion.LerpUnclamped(ShootPoint.rotation, FinalRotation.rotation,
                        rotationSpeed * Time.deltaTime);
                Debug.Log("Rotating Scanner");
                break;
            }

            rayDirection = Vector3.forward;
            ray = new Ray(ShootPoint.position, transform.TransformDirection(rayDirection * radius));

            if (Physics.Raycast(ray, out var hit, radius))
            {
                if (hit.collider.gameObject.transform.Find("Mark(Clone)") == null &&
                    (hit.collider.gameObject.CompareTag("Trash") ||
                     hit.collider.gameObject.CompareTag("Invisible Trash")))
                {
                    tempMark = Instantiate(Mark, hit.collider.transform);
                    AudioStartDetectionBeep();
                }
            }

            lr.SetPosition(0, new Vector3(transform.position.x, transform.position.y, transform.position.z));
            lr.SetPosition(1, ray.GetPoint(radius));

            if (ShootPoint.rotation.y > 0 && ShootPoint.rotation.y < 0.1)
            {
                ChangeState(ScanningState.Inactive);
            }

            if (inputHandler.moveInput == 1 && Time.time > startScanTime + leaveScanTimer)
                ChangeState(ScanningState.Inactive);
        }

        private void StartUpState(ScanningState state)
        {
            if (state == ScanningState.Scanning)
            {
                startScanTime = Time.time;

                robot.isScanning = true;
                robot.canSwitch = false;

                movement.SetMovement(startScanMoveMultiplier);
                movement.movementEnabled = false;
                movement.ResetMovement();

                tempZone = Instantiate(Zone, transform.parent.transform);
                tempZone.transform.localScale = new Vector3(radius * 2, 0.5f, radius * 2);

                transform.rotation = Quaternion.identity;
            }
            else if (state == ScanningState.Inactive)
            {
                lastScanTime = Time.time;

                robot.canSwitch = true;
                robot.isScanning = false;

                movement.movementEnabled = true;

                Destroy(tempZone);

                if (audioSource.isPlaying) audioSource.Stop();

                lr.SetPosition(0, new Vector3(transform.position.x, transform.position.y, transform.position.z));
                lr.SetPosition(1, new Vector3(transform.position.x, transform.position.y, transform.position.z));

                ShootPoint.rotation = new Quaternion(0f, 0f, 0f, 0f);
            }
        }

        private void ChangeState(ScanningState state)
        {
            Debug.Log("Change State to: " + state);
            this.state = state;
            StartUpState(state);
        }

        private void InputChange()
        {
            inputHandler = null;
            inputHandler = transform.parent.GetComponentInChildren<PlayerInputHandler>();
        }

        private void AudioStartPing()
        {
            Debug.Log("Audio for scan pinging.");
            audioSource.clip = scanPing;
            if (!audioSource.isPlaying) audioSource.Play();
            StartCoroutine(PingCooldown(pingAudioDelay));
            onPingCooldown = true;
        }

        private void AudioStartDetectionBeep()
        {
            Debug.Log("Audio for scan detection beep.");
            if (audioSource.clip != scanDetectionBeep)
                audioSource.clip = scanDetectionBeep;
            if (!audioSource.isPlaying) audioSource.Play();
        }

        private IEnumerator PingCooldown(float secondsDelay)
        {
            yield return new WaitForSeconds(secondsDelay);
            onPingCooldown = !onPingCooldown;
        }
    }
}