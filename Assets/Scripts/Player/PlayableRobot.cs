using Interactables;
using System.Security.Cryptography;
using UnityEngine;

namespace Player
{
    public class PlayableRobot : MonoBehaviour
    {
        [SerializeField] private int maxHeldPickUps;
        [SerializeField] private Vector3 carryOffset;

        [Header("Animation bools")]
        public bool isIdle;
        public bool isMoving;
        public bool isCarrying;
        [Space]
        public bool isDigging;
        [Space]
        public bool isScanning;
        [Space]
        public bool isBushWhacking;
        public bool isRightSwinging;
        public bool isLeftSwinging;
        [Space]
        public bool isHappy;

        private Animator anim;
        public PickUpCollector Collector;
        public bool canSwitch;
        private GameObject pickUpSlot;

        private void Start()
        {
            isHappy = true; // :D
            anim = GetComponentInChildren<Animator>();
            Collector = new PickUpCollector(maxHeldPickUps);
        }

        private void Update()
        {
            HandleAnimations();
        }
        public void Carry(GameObject pickUp)
        {
            pickUpSlot = pickUp;
            var pickupTransform = pickUp.transform;
            pickupTransform.SetParent(transform);
            pickupTransform.localPosition = carryOffset;
            isCarrying = true;
        }

        public void Drop()
        {
            pickUpSlot.transform.SetParent(null);
            pickUpSlot = null;
            isCarrying = false;
        }

        public void CollectPickUp()
        {
            pickUpSlot.transform.SetParent(null);
            pickUpSlot.SetActive(false);
            pickUpSlot = null;
            isCarrying = false;
        }

        public void HandleAnimations()
        {
            anim.SetBool("isWalking", isMoving);
            anim.SetBool("isIdle", isIdle);
            anim.SetBool("isCarrying", isCarrying);

            anim.SetBool("isDigging", isDigging);
            
            anim.SetBool("isScanning", isScanning);

            anim.SetBool("isBushWhacking", isBushWhacking);
            anim.SetBool("isRightSwing", isRightSwinging);
            anim.SetBool("isLeftSwing", isLeftSwinging);
        }

        private void OnDrawGizmos()
        {
            var offset = transform.position + carryOffset;
            Gizmos.DrawSphere(offset, 0.2f);
        }
    }
}