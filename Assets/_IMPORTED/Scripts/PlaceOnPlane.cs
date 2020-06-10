using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

/// <summary>
/// Listens for touch events and performs an AR raycast from the screen touch point.
/// AR raycasts will only hit detected trackables like feature points and planes.
///
/// If a raycast hits a trackable, the <see cref="placedPrefab"/> is instantiated
/// and moved to the hit position.
/// </summary>
[RequireComponent(typeof(ARRaycastManager))]
public class PlaceOnPlane : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Instantiates this prefab on a plane at the touch location.")]
    GameObject m_PlacedPrefab;

    /// <summary>
    /// The prefab to instantiate on touch.
    /// </summary>
    public GameObject placedPrefab
    {
        get { return m_PlacedPrefab; }
        set { m_PlacedPrefab = value; }
    }

    /// <summary>
    /// The object instantiated as a result of a successful raycast intersection with a plane.
    /// </summary>
    public GameObject spawnedObject1 { get; private set; }
    public GameObject spawnedObject2 { get; private set; }

    private bool toggle = false;

    private Transform firstSphere;
    private Transform secondSphere;

    public GameObject spherePrefab;
    public LineRenderer lineRenderer;
    public Text distanceText;
    public Vector3 offset;
    
    void Awake()
    {
        m_RaycastManager = GetComponent<ARRaycastManager>();
    }

    bool TryGetTouchPosition(out Vector2 touchPosition)
    {
#if UNITY_EDITOR
        if (Input.GetMouseButton(0))
        {
            var mousePosition = Input.mousePosition;
            touchPosition = new Vector2(mousePosition.x, mousePosition.y);
            return true;
        }
#else
        if (Input.touchCount > 0)
        {
            touchPosition = Input.GetTouch(0).position;
            return true;
        }
#endif

        touchPosition = default;
        return false;
    }

    void Update()
    {
        if (toggle && secondSphere != null)
        {
            distanceText.text = System.Math.Round((Vector3.Distance(firstSphere.position, secondSphere.position) * 100f), 2) + " cm";
            lineRenderer.SetPosition(0, firstSphere.position);
            lineRenderer.SetPosition(1, secondSphere.position);
        }

        if (!TryGetTouchPosition(out Vector2 touchPosition))
            return;

        if (m_RaycastManager.Raycast(touchPosition, s_Hits, TrackableType.PlaneWithinPolygon))
        {
            // Raycast hits are sorted by distance, so the first one
            // will be the closest hit.
            var hitPose = s_Hits[0].pose;

            if(!toggle)
            {
                if (spawnedObject1 == null)
                {
                    spawnedObject1 = Instantiate(spherePrefab, hitPose.position, hitPose.rotation);
                    firstSphere = spawnedObject1.transform;
                }
                //else
                //{
                //    spawnedObject1.transform.position = hitPose.position;
                //}
            }
            else if (toggle)
            {
                if (spawnedObject2 == null)
                {
                    spawnedObject2 = Instantiate(spherePrefab, hitPose.position, hitPose.rotation);
                    secondSphere = spawnedObject2.transform;
                }
                else
                {
                    spawnedObject2.transform.position = hitPose.position;
                }
            }
        }

        
    }

    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    ARRaycastManager m_RaycastManager;

    public void OnToggleButtonClicked()
    {
        toggle = !toggle;
    }
}
