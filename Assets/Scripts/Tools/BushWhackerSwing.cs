using System;
using System.Collections;
using GameInput;
using Managers;
using Player;
using UnityEngine;
using CharacterController = Player.CharacterController;

namespace Tools
{
    internal enum WhackingState
    {
        Inactive,
        Whacking
    }

    public class BushWhackerSwing : MonoBehaviour
    {
        [SerializeField] private float radius;
        [SerializeField] private float WhackCooldown;
        [SerializeField] private float startWhackMoveMultiplier;
        [SerializeField] private GameObject whackingParticle;

        [SerializeField] [Tooltip("Audio clip for whacking sound feedback. Can be left empty for no audio")]
        private AudioClip whackingSound;

        [SerializeField] [Range(0.1f, 2f)]private float audioPreDelaySeconds;
        [SerializeField] private Transform bush;

        private AudioSource audioSource;
        private GameObject bushGO;

        private float endWhackStateTime;

        private Collider[] hitColliders;
        private PlayerInputHandler inputHandler;
        private CharacterController movement;
        private PlayableRobot robot;
        private float startWhackStateTime;
        private WhackingState state;

        private void Awake()
        {
            robot = GetComponentInParent<PlayableRobot>();
            movement = GetComponentInParent<CharacterController>();
            PlayerManager.GetInstance().OnInputChange += InputChange;
            audioSource = GetComponent<AudioSource>();
        }

        private void Update()
        {
            if (inputHandler == null)
                return;
            HandleWhacking();
        }


        private void HandleWhacking()
        {
            switch (state)
            {
                case WhackingState.Inactive:
                    InactiveState();
                    break;
                case WhackingState.Whacking:
                    WhackState();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void InactiveState()
        {
            if (!inputHandler.actionInput || !(Time.time > endWhackStateTime + WhackCooldown)) return;
            Debug.Log("Looking for bush");
            DetectingBushes(transform.position, radius);
        }


        private void DetectingBushes(Vector3 centre, float sphereRadius)
        {
            hitColliders = Physics.OverlapSphere(centre, sphereRadius);
            foreach (var hitCollider in hitColliders)
                if (hitCollider.gameObject.CompareTag("Destroyable"))
                {
                    bush = hitCollider.transform;
                    bushGO = bush.gameObject;
                    ChangeState(WhackingState.Whacking);
                }
        }

        private void WhackState()
        {
            if (inputHandler.actionInput && Time.time > endWhackStateTime + WhackCooldown)
            {
                AlternateHands();
                StartCoroutine(FeedbackPreDelay(audioPreDelaySeconds));
                bushGO.GetComponent<DestructructibleObject>().currentHealth--;
                endWhackStateTime = Time.time;
                inputHandler.UseActionInput();
            }

            if (bushGO == null) ChangeState(WhackingState.Inactive);
        }


        private void EndState(WhackingState state)
        {
            switch (state)
            {
                case WhackingState.Inactive:
                    break;
                case WhackingState.Whacking:
                    DisableHands();
                    robot.isBushWhacking = false;
                    movement.movementEnabled = true;
                    endWhackStateTime = Time.time;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        private void StartUpState(WhackingState state)
        {
            switch (state)
            {
                case WhackingState.Inactive:
                    movement.movementEnabled = true;
                    break;
                case WhackingState.Whacking:
                    robot.isLeftSwinging = true;
                    robot.isBushWhacking = true;
                    movement.movementEnabled = false;
                    startWhackStateTime = Time.time;
                    movement.ResetMovement();
                    movement.SetMovement(startWhackMoveMultiplier);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }


        private void ChangeState(WhackingState newState)
        {
            Debug.Log("Changed state to: " + newState);
            EndState(state);
            state = newState;
            StartUpState(newState);
        }

        private void InputChange()
        {
            inputHandler = null;
            inputHandler = transform.parent.GetComponentInChildren<PlayerInputHandler>();
        }

        private void LookAtBush()
        {
            // after animation is implemented,  add a way for the player to rotate towards the bush 
            //gameObject.parent.transform.rotate ...
        }

        private void AlternateHands()
        {
            if (robot.isLeftSwinging)
                (robot.isRightSwinging, robot.isLeftSwinging) = (robot.isLeftSwinging, robot.isRightSwinging);
            else if (robot.isRightSwinging)
                (robot.isRightSwinging, robot.isLeftSwinging) = (robot.isLeftSwinging, robot.isRightSwinging);
        }

        private void DisableHands()
        {
            robot.isLeftSwinging = false;
            robot.isRightSwinging = false;
        }

        private void PlayWhackingAudio()
        {
            Debug.Log("Audio for Whack");
            if (audioSource.clip != whackingSound)
                audioSource.clip = whackingSound;
            audioSource.Play();
        }

        private IEnumerator FeedbackPreDelay(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            Instantiate(whackingParticle, bush);
            PlayWhackingAudio();
        }
    }
}