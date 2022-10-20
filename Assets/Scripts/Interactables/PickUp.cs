using System.Collections;
using Player;
using Unity.VisualScripting;
using UnityEngine;

namespace Interactables
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(AudioSource))]
    public class PickUp : MonoBehaviour
    {
        [SerializeField] private AudioClip pickUpAudioClip;
        [SerializeField] private float interactionCooldown;

        private AudioSource audioSource;
        private Collider collider;
        private PlayableRobot holdingPlayer;
        private bool onCooldown;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            collider = GetComponent<Collider>();
            collider.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (onCooldown || !other.CompareTag("PlayerCollider")) return;

            var player = other.GetComponentInParent<PlayableRobot>();

            if (player.Collector.PickUpCollection >= player.Collector.maxPickUpsHeld) return;
            // Updating holder of pickup
            PlayAudioPickUp();

            // Dropping pickup from holder
            if (holdingPlayer != null)
            {
                holdingPlayer.Collector.Reset();
                holdingPlayer.Drop();
                holdingPlayer = null;
            }

            // Assigning pickup to new player
            holdingPlayer = player;
            player.Collector.PickUpCollection++;
            player.Carry(gameObject);
            StartCoroutine(PickUpCoolDown(interactionCooldown));
            onCooldown = true;
        }

        private IEnumerator PickUpCoolDown(float coolDown)
        {
            yield return new WaitForSeconds(coolDown);
            onCooldown = false;
        }

        private void PlayAudioPickUp()
        {
            if (audioSource.clip != pickUpAudioClip) audioSource.clip = pickUpAudioClip;
            if (!audioSource.isPlaying) audioSource.Play();
        }
    }
}