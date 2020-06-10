using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARRaycastManager))]
public class FocusSqure : MonoBehaviour
{
    private ARRaycastManager arRaycastManager;
    private Transform cameraTransform;
    private Vector2 centerScreen;
    private Vector3 initialScale;
    
    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    [SerializeField] private ARCameraManager arCameraManager;
    [Header("Focus Square")]
    [SerializeField] private GameObject focusSquare;
    [Header("Model To Augment")]
    [SerializeField] private GameObject modal;

    private void Awake()
    {
        modal.SetActive(false);
        arRaycastManager = GetComponent<ARRaycastManager>();
        cameraTransform = Camera.main.transform;
        centerScreen = new Vector2(Screen.width / 2f, Screen.height / 2f);
        initialScale = focusSquare.transform.localScale;
    }

    private void Update()
    {
        HandleFocusSquare();
    }

    private void HandleFocusSquare()
    {
        if (arRaycastManager.Raycast(centerScreen, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
        {
            float distance = Vector3.Distance(cameraTransform.position, focusSquare.transform.position);
 
            Pose hitPose = hits[0].pose;
            focusSquare.transform.position = hitPose.position;
            focusSquare.transform.rotation = hitPose.rotation;
            focusSquare.transform.localScale = initialScale * distance;

            // Handle taps here
            if (Input.GetMouseButtonDown(0))
            {
                modal.SetActive(true);
                modal.transform.position = hitPose.position;
                modal.transform.rotation = hitPose.rotation;
                // Compansate Modal Rotation to face the Camera
                modal.transform.Rotate(modal.transform.rotation.x, modal.transform.rotation.y + 180f, modal.transform.rotation.z);
                focusSquare.SetActive(false);
            }
        }
    }    
}