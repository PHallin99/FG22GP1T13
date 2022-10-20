using Managers;
using Player;
using UnityEngine;

namespace Interactables
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(AudioSource))]
    public class GarbageDropoff : MonoBehaviour
    {
        [SerializeField] private GameObject particleSystem;
        [SerializeField] private AudioClip audioClip;

        private AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("PlayerCollider"))
                return;

            var player = other.GetComponentInParent<PlayableRobot>();
            Debug.Log(player.Collector.PickUpCollection);
            if (player.Collector.PickUpCollection <= 0) return;
            GameManager.Instance.CollectableCollect(player.Collector.PickUpCollection);
            Instantiate(particleSystem, transform.position, Quaternion.identity);
            PlayDropOffAudio();
            player.Collector.Reset();
            player.CollectPickUp();
        }

        private void PlayDropOffAudio()
        {
            if (audioSource.clip != audioClip) audioSource.clip = audioClip;
            if (!audioSource.isPlaying) audioSource.Play();
        }
    }
}