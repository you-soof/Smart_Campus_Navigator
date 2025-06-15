using System.Collections.Generic;
using UnityEngine;

public class ArrowPointer : MonoBehaviour
{
    [Header("Arrow Path Settings")]
    public GameObject arrowPrefab; 
    public float arrowSpacing = 3f; 
    public float arrowHeightOffset = 0.8f;
    public float pathUpdateInterval = 0.5f; 
    
    [Header("Ground Detection")]
    public LayerMask groundLayerMask = 1;
    public float groundCheckDistance = 10f;
    public float groundCheckRadius = 0.1f;
    
    [Header("Player Reference")]
    public Transform player; // to be set by the LocationManager
    
    private Transform _target;
    private List<GameObject> _arrowInstances = new List<GameObject>();
    private float _lastPathUpdateTime;
    private Vector3 _lastPlayerPosition;
    private Vector3 _lastTargetPosition;

    void Start()
    {
        if (arrowPrefab == null)
        {
            LocationManager locationManager = FindObjectOfType<LocationManager>();
            if (locationManager != null && locationManager.arrowPrefab != null)
            {
                arrowPrefab = locationManager.arrowPrefab;
            }
        }
        
        if (player == null)
        {
            player = FindObjectOfType<Camera>()?.transform; 
        }
    }

    public void SetTarget(Transform newTarget)
    {
        _target = newTarget;
        
        LocationManager locationManager = FindObjectOfType<LocationManager>();
        if (locationManager != null && locationManager.player != null)
        {
            player = locationManager.player;
        }
        
        if (_target != null && player != null)
        {
            UpdateArrowPath();
        }
    }

    void Update()
    {
        if (_target == null || player == null) return;

        bool shouldUpdate = Time.time - _lastPathUpdateTime > pathUpdateInterval;

        if (Vector3.Distance(player.position, _lastPlayerPosition) > 1f ||
            Vector3.Distance(_target.position, _lastTargetPosition) > 1f)
        {
            shouldUpdate = true;
        }
        
        if (shouldUpdate)
        {
            UpdateArrowPath();
        }
    }

    void UpdateArrowPath()
    {
        ClearArrows();
        
        if (_target == null || player == null) return;
        
        Vector3 playerForward = player.forward;
        Vector3 startPosition = player.position + playerForward * 1.5f; // starting at 1.5 meters in front of player
        
        Vector3 targetPosition = _target.position;
        
        float totalDistance = Vector3.Distance(startPosition, targetPosition);
        int arrowCount = Mathf.Max(1, Mathf.FloorToInt(totalDistance / arrowSpacing));
        
        for (int i = 0; i < arrowCount; i++)
        {
            float t = (float)i / (arrowCount - 1);
            if (arrowCount == 1) t = 0f;
            
            // linear interpolation between start and target
            Vector3 pathPosition = Vector3.Lerp(startPosition, targetPosition, t);
            
            Vector3 groundAdjustedPosition = GetGroundAdjustedPosition(pathPosition);
            
            CreateArrowAtPosition(groundAdjustedPosition, GetDirectionToTarget(groundAdjustedPosition, targetPosition));
        }
        
        _lastPathUpdateTime = Time.time;
        _lastPlayerPosition = player.position;
        _lastTargetPosition = targetPosition;
    }

    Vector3 GetGroundAdjustedPosition(Vector3 position)
    {
        // casting ray down to find the ground terrain
        RaycastHit hit;
        Vector3 rayStart = position + Vector3.up * 5f; 
        
        if (Physics.SphereCast(rayStart, groundCheckRadius, Vector3.down, out hit, groundCheckDistance, groundLayerMask))
        {
            return hit.point + Vector3.up * arrowHeightOffset;
        }

        return new Vector3(position.x, position.y + arrowHeightOffset, position.z);
    }

    Vector3 GetDirectionToTarget(Vector3 fromPosition, Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - fromPosition).normalized;
        direction.y = 0;
        return direction;
    }

    void CreateArrowAtPosition(Vector3 position, Vector3 direction)
    {
        if (arrowPrefab == null) return;

        Quaternion rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 90, 0);
        GameObject arrow = Instantiate(arrowPrefab, position, rotation);
        arrow.transform.SetParent(transform);
        
        ArrowInstance arrowInstance = arrow.GetComponent<ArrowInstance>();
        if (arrowInstance == null)
        {
            arrowInstance = arrow.AddComponent<ArrowInstance>();
        }
        arrowInstance.originalPosition = position;
        
        _arrowInstances.Add(arrow);
    }

    void ClearArrows()
    {
        foreach (GameObject arrow in _arrowInstances)
        {
            if (arrow != null)
            {
                DestroyImmediate(arrow);
            }
        }
        _arrowInstances.Clear();
    }
    
    // public methods for external control
    public void SetPlayer(Transform playerTransform)
    {
        player = playerTransform;
    }

    public void SetArrowSpacing(float spacing)
    {
        arrowSpacing = spacing;
        if (_target != null) UpdateArrowPath();
    }

    public void SetArrowHeight(float height)
    {
        arrowHeightOffset = height;
        if (_target != null) UpdateArrowPath();
    }

    public int GetArrowCount()
    {
        return _arrowInstances.Count;
    }

    public bool HasTarget()
    {
        return _target != null;
    }

    public Transform GetTarget()
    {
        return _target;
    }

    void OnDestroy()
    {
        ClearArrows();
    }
}

// Helper component for individual arrow instances
public class ArrowInstance : MonoBehaviour
{
    public Vector3 originalPosition;
}

