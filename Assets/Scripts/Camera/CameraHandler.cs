using System.Collections.Generic;
using Managers;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    [SerializeField] private Transform targetTransform;

    [Space]
    [Header("Follow")]
    [Tooltip("The speed the camera uses to move to targetPosition. A lower value means sharper camera movement")]
    [Range(10f, 300f)]
    [SerializeField]
    private float cameraFollowSpeed;

    [Space]
    [Header("Zoom")]
    [Tooltip("The distance needed between the player for the camera to start zooming out")]
    [SerializeField]
    private float zoomMinPlayerDistance;

    [Space]
    [Tooltip("The zoom speed of the camera. Higher value makes the camera reach maxZoom faster")]
    [Range(0.001f, 0.1f)]
    [SerializeField]
    private float zoomSpeed;

    [Tooltip("The zoom smoothing of the camera.  A higher value means sharper camera zooming")]
    [Range(0.05f, 0.5f)]
    [SerializeField]
    private float zoomSmoothing;

    [Space] [Tooltip("Minimum zoom distance")] [Range(10f, 30f)] [SerializeField]
    private float cameraZoomMin;

    [Tooltip("Maximum zoom distance")] [Range(10f, 40f)] [SerializeField]
    private float cameraZoomMax;


    public Transform target1;
    public Transform target2;

    private Vector3 cameraPosition;
    private Transform cameraTransform;

    private Vector3 moveVelocity;
    private Vector3 target;

    private float zoom;
    private float zoomVelocity;


    private void Awake()
    {
        cameraTransform = GetComponentInChildren<Camera>().transform;

        if (targetTransform == null)
            Debug.LogError("Missing a CameraTarget. Assign in Inpector");
        PlayerManager.GetInstance().OnInputChange += SetPlayers;
    }

    private void LateUpdate()
    {
        CameraFollow();
        CameraZoom();
    }

    public void SetPlayers()
    {
        target1 = PlayerManager.GetInstance().p1RobotGO.transform;
        target2 = PlayerManager.GetInstance().p2RobotGO.transform;
    }

    private void CameraFollow()
    {
        target = target1.position + (target2.position - target1.position) / 2;
        targetTransform.position =
            Vector3.SmoothDamp(targetTransform.position, target, ref moveVelocity, cameraFollowSpeed * Time.deltaTime);
    }

    private void CameraZoom() //Could be done better but works
    {
        cameraPosition = cameraTransform.position;
        var zoomTarget = 0f;

        if (DistanceBetweenPlayers() - zoomMinPlayerDistance > 0)
            zoomTarget = (DistanceBetweenPlayers() - zoomMinPlayerDistance) * zoomSpeed;

        zoom = Mathf.SmoothDamp(zoom, zoomTarget, ref zoomVelocity, zoomSmoothing);
        cameraPosition.y = Mathf.Lerp(cameraZoomMin, cameraZoomMax, zoom);

        cameraTransform.position = cameraPosition;
    }

    private float DistanceBetweenPlayers()
    {
        return Vector3.Distance(target1.position, target2.position);
    }
}