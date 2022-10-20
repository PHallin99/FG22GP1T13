using System;
using System.Collections.Generic;
using System.Linq;
using Player;
using UnityEngine;

namespace Interactables
{
    [RequireComponent(typeof(Rigidbody))]
    public class Absorbing : MonoBehaviour
    {
        [SerializeField] private int minDistance;
        [SerializeField] private float force;
        private Rigidbody rigidbody;
        private List<PlayableRobot> robots;
        private GameObject absorbingRobot;
        private bool shouldMove;

        private void Start()
        {
            robots.AddRange(FindObjectsOfType<PlayableRobot>());
            rigidbody = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            foreach (var robot in robots.Where(robot =>
                         Vector3.Distance(robot.transform.position, transform.position) < minDistance))
            {
                absorbingRobot = robot.gameObject;
            }

            if (absorbingRobot != null)
            {
                shouldMove = true;
            }
        }

        private void FixedUpdate()
        {
            rigidbody.AddForce(transform.position - absorbingRobot.transform.position * force, ForceMode.Acceleration);
        }

        private void LateUpdate()
        {
            absorbingRobot = null;
        }
    }
}