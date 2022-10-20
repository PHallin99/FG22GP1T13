using System;
using System.Collections.Generic;
using GameInput;
using Player;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace Managers
{
    public class PlayerManager : MonoBehaviour
    {
        public static PlayerManager Instance;

        [SerializeField] private GameObject p1InputPrefab;
        [SerializeField] private GameObject p2InputPrefab;

        private GameObject p1Input;
        private GameObject p2Input;
        [Space]
        public GameObject p1RobotGO;
        public GameObject p2RobotGO;
        [SerializeField] private GameObject inactiveRobot;

        public bool switchingDisabled;
        public bool useParticles;

        [Header("Particles")]
        [SerializeField] private GameObject activeRobotParticles;
        [SerializeField][Range(0.2f, 10f)] private float particleFollowSpeed;

        private GameObject activeRobot1Particles;
        private GameObject activeRobot2Particles;
        [Header("Debug")]
        [SerializeField] private PlayerInputHandler p1Inputhandler;
        [SerializeField] private PlayerInputHandler p2Inputhandler;

        [SerializeField] private PlayableRobot p1Robot;
        [SerializeField] private PlayableRobot p2Robot;

        private Vector3 currentParticle1Vel;
        private Vector3 currentParticle2Vel;


        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(this);
            DontDestroyOnLoad(transform.root);
        }
        private void Start()
        {
            GameManager.Instance.OnStartGame += StartGame;
        }

        public event Action OnInputChange;

        public static PlayerManager GetInstance()
        {
            return Instance;
        }

        private void FixedUpdate()
        {
            if (!useParticles)
                return;
            HandleActiveParticles();
        }

        private void ChangeRobotP1()
        {
            if (switchingDisabled)
                return;
            if (!p1Robot.canSwitch)
                return;

            (p1RobotGO, inactiveRobot) = (inactiveRobot, p1RobotGO);
            p1Input.transform.SetParent(p1RobotGO.transform);

            Debug.Log("Tried Changing RobotP1");
            p1Robot = p1Input.GetComponentInParent<PlayableRobot>();
            OnInputChange?.Invoke();
        }

        private void ChangeRobotP2()
        {
            if (switchingDisabled)
                return;
            if (!p2Robot.canSwitch)
                return;

            (p2RobotGO, inactiveRobot) = (inactiveRobot, p2RobotGO);
            p2Input.transform.SetParent(p2RobotGO.transform);

            Debug.Log("Tried Changing RobotP2");
            p2Robot = p2Input.GetComponentInParent<PlayableRobot>();
            OnInputChange?.Invoke();
        }

        private void StartGame()
        {
            GetNewRobots();
            SetInput();
            SpawnParticles();
            OnInputChange?.Invoke();
        }

        private void GetNewRobots()
        {
            var robotsInScene = GameObject.FindGameObjectsWithTag("Player");

            p1RobotGO = robotsInScene[0];
            p2RobotGO = robotsInScene[1];
            inactiveRobot = robotsInScene[2];

            if (robotsInScene.Length < 3)
                switchingDisabled = true;
            else switchingDisabled = false;

            Debug.Log(robotsInScene.Length);
        }

        private void SetInput()
        {
            p1Input = Instantiate(p1InputPrefab);
            p2Input = Instantiate(p2InputPrefab);

            p1Input.transform.SetParent(p1RobotGO.transform);
            p2Input.transform.SetParent(p2RobotGO.transform);

            p1Robot = p1Input.GetComponentInParent<PlayableRobot>();
            p2Robot = p2Input.GetComponentInParent<PlayableRobot>();

            p1Inputhandler = p1Input.GetComponent<PlayerInputHandler>();
            p2Inputhandler = p2Input.GetComponent<PlayerInputHandler>();

            p1Inputhandler.OnChangeRobot += ChangeRobotP1;
            p2Inputhandler.OnChangeRobot += ChangeRobotP2;
        }

        public void UpdateInput()
        {
            OnInputChange?.Invoke();
        }

        private void SpawnParticles()
        {
            activeRobot2Particles = Instantiate(activeRobotParticles, p1RobotGO.transform.position, Quaternion.identity);
            activeRobot1Particles = Instantiate(activeRobotParticles, p1RobotGO.transform.position, Quaternion.identity);
        }
        private void HandleActiveParticles()
        {
            var target1 = p1RobotGO.transform.position;
            activeRobot1Particles.transform.position =
            Vector3.SmoothDamp(activeRobot1Particles.transform.position, target1, ref currentParticle1Vel, particleFollowSpeed * Time.deltaTime);

            var target2 = p2RobotGO.transform.position;
            activeRobot2Particles.transform.position =
            Vector3.SmoothDamp(activeRobot2Particles.transform.position, target2, ref currentParticle2Vel, particleFollowSpeed * Time.deltaTime);
        }
    }
}