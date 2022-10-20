using System;
using System.Collections;
using System.Collections.Generic;
using GameInput;
using Managers;
using Player;
using UnityEngine;
using CharacterController = Player.CharacterController;

namespace Tools
{
    enum DiggerState
    {
        Inactive,
        Digging
    }

    [RequireComponent(typeof(AudioSource))]
    public class Digger : MonoBehaviour
    {
        [SerializeField] private int digsNeeded;
        [SerializeField] private float digCooldown;
        [SerializeField] private float betweenDigsCoolown;
        [SerializeField] private float leaveDigTimer;
        [SerializeField] private float startDigMoveMultiplier;

        // [SerializeField] private bool cooldown;
        // [SerializeField] private bool isDigging;
        [SerializeField] private float radius;

        [Space] [SerializeField] [Tooltip("Audio clip for digging sound feedback. Can be left empty for no audio")]
        private AudioClip digAudioClip;

        [SerializeField][Range(0.1f, 2f)] private float diggingSoundPreDelay;

        [Space] [SerializeField] private GameObject digSmoke;
        [SerializeField] private GameObject digParticles;

        private PlayableRobot robot;
        private CharacterController movement;
        private AudioSource audioSource;
        private PlayerInputHandler inputHandler;
        private GameObject TheMark;
        private GameObject Trash;
        private Collider[] hitColliders;

        private DiggerState state;
        private float endDigStateTime;
        private float endDigTime;
        private float startDigStateTime;
        private int digsMade;

        private void Awake()
        {
            robot = GetComponentInParent<PlayableRobot>();
            audioSource = GetComponent<AudioSource>();
            movement = GetComponentInParent<CharacterController>();
            PlayerManager.GetInstance().OnInputChange += InputChange;
        }

        private void Update()
        {
            if (inputHandler == null)
                return;
            HandleDigging();
        }

        private void HandleDigging()
        {
            switch (state)
            {
                case DiggerState.Inactive:
                    InactiveState();
                    break;
                case DiggerState.Digging:
                    DiggingState();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void InactiveState()
        {
            if (inputHandler.actionInput && Time.time > endDigStateTime + digCooldown)
            {
                DetectingMarks(transform.position, radius);
            }
        }

        private void DiggingState()
        {
            if (inputHandler.actionInput && Time.time > endDigTime + digCooldown)
            {
                robot.isDigging = true;
                StartCoroutine(FeedbackPreDelay(diggingSoundPreDelay));
                digsMade++;
                endDigTime = Time.time;
                inputHandler.UseActionInput();
            }

            if (digsMade >= digsNeeded)
            {
                CompleteDigging();
                ChangeState(DiggerState.Inactive);
            }

            if (inputHandler.moveInput == 1 && Time.time > startDigStateTime + leaveDigTimer)
                ChangeState(DiggerState.Inactive);
        }

        private void ChangeState(DiggerState newState)
        {
            Debug.Log("Changed state to: " + newState);
            EndState(state);
            state = newState;
            StartUpState(newState);
        }

        private void EndState(DiggerState state)
        {
            switch (state)
            {
                case DiggerState.Inactive:
                    break;
                case DiggerState.Digging:
                    robot.isDigging = false;
                    movement.movementEnabled = true;
                    endDigStateTime = Time.time;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        private void StartUpState(DiggerState state)
        {
            switch (state)
            {
                case DiggerState.Inactive:
                    movement.movementEnabled = true;
                    break;
                case DiggerState.Digging:
                    movement.movementEnabled = false;
                    startDigStateTime = Time.time;
                    movement.ResetMovement();
                    movement.SetMovement(startDigMoveMultiplier);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        private void InputChange()
        {
            inputHandler = null;
            inputHandler = transform.parent.GetComponentInChildren<PlayerInputHandler>();
        }

        private void CompleteDigging()
        {
            endDigStateTime = Time.time;
            Trash = TheMark.transform.parent.gameObject.transform.Find("Trash").gameObject; //mega cursed
            Trash.transform.parent = null;
            Trash.transform.position = TheMark.transform.position;
            Destroy(TheMark.transform.parent.gameObject);
            Destroy(TheMark);
            digsMade = 0;
        }

        private void DetectingMarks(Vector3 centre, float sphereRadius)
        {
            hitColliders = Physics.OverlapSphere(centre, sphereRadius);
            foreach (var hitCollider in hitColliders)
                if (hitCollider.gameObject.CompareTag("Mark"))
                {
                    TheMark = hitCollider.gameObject;
                    ChangeState(DiggerState.Digging);
                }
        }

        private void PlayDiggingAudio()
        {
            Debug.Log("Audio for dig sound.");
            if (audioSource.clip != digAudioClip)
                audioSource.clip = digAudioClip;
            audioSource.Play();
        }

        private IEnumerator FeedbackPreDelay(float audioPreDelay)
        {
            yield return new WaitForSeconds(audioPreDelay);
            Instantiate(digSmoke, transform.position, Quaternion.identity);
            Instantiate(digParticles, transform.position, Quaternion.identity);
            PlayDiggingAudio();
        }
    }
}