using System.Collections.Generic;
using UnityEngine;

public class LocationManager : MonoBehaviour
{
    [System.Serializable]
    public class LocationData
    {
        public string locationId;
        public string locationName; 
        public Transform locationTransform;
        public Vector3 offset = Vector3.zero;
        public float arrivalDistance = 5f;
    }

    [Header("All Campus Destinations")]
    [SerializeField] public List<LocationData> campusLocations = new List<LocationData>();

    [Header("References")]
    public Transform player;
    public GameObject arrowPrefab;
    public GameObject destinationMarkerPrefab; 
    
    [Header("Navigation Settings")]
    public float arrowHeight = 0.8f;
    public bool showDestinationMarker = true;
    public LayerMask groundLayerMask = 1; // for ground detection

    private GameObject _currentArrow;
    private GameObject _currentDestinationMarker;
    private Transform _targetLocation;
    private LocationData _currentLocationData;
    private bool _hasArrivedAtDestination = false;

    // Events for UI feedback
    public System.Action<string> OnDestinationReached;
    public System.Action<float> OnDistanceUpdate; 

    void Start()
    {
        if (player == null)
        {
            if (Camera.main != null)
            {
                GameObject playerObj = Camera.main.gameObject;
            }
        }
        else
        {
            Debug.LogError("Player not found! Please assign player transform in LocationManager.");
        }
        
        if (campusLocations.Count == 0)
        {
            InitializeDefaultLocations();
        }
    }

    void InitializeDefaultLocations()
    {
        campusLocations.Clear();
        string[] buildingCodes = {"Building A", "Building B", "Building C", "Building D", 
                                 "Building E", "Building F", "Building K", "Building L", 
                                 "Building R", "Building S"};
        
        foreach (string code in buildingCodes)
        {
            GameObject buildingObj = GameObject.Find(code);
            if (buildingObj != null)
            {
                LocationData newLocation = new LocationData
                {
                    locationId = code,
                    locationName = GetBuildingDisplayName(code),
                    locationTransform = buildingObj.transform,
                    arrivalDistance = 5f
                };
                campusLocations.Add(newLocation);
                Debug.Log($"Added location: {code} at {buildingObj.transform.position}");
            }
            else
            {
                Debug.LogWarning($"Building GameObject not found: {code}. Make sure building GameObjects are named correctly.");
            }
        }
        
        Debug.Log($"Initialized {campusLocations.Count} campus locations");
    }

    string GetBuildingDisplayName(string code)
    {
        switch (code)
        {
            case "Building A": return "East Building A";
            case "Building B": return "East Building B";
            case "Building C": return "East Building C";
            case "Building D": return "West Building D";
            case "Building E": return "West Building E";
            case "Building F": return "West Building F";
            case "Building K": return "North Building K";
            case "Building L": return "Student Services Building";
            case "Building R": return "Research Building";
            case "Building S": return "Student House";
            default: return code;
        }
    }

    public void SetDestination(string locationId)
    {
        var targetData = campusLocations.Find(loc => loc.locationId == locationId);
        if (targetData != null)
        {
            _currentLocationData = targetData;
            _targetLocation = targetData.locationTransform;
            _hasArrivedAtDestination = false;
            
            Debug.Log($"Target set: {targetData.locationName} ({locationId}) at position {_targetLocation.position}");
            
            CreateNavigationArrow();
            
            if (showDestinationMarker)
            {
                CreateDestinationMarker();
            }
        }
        else
        {
            Debug.LogWarning($"Location not found: {locationId}. Available locations:");
            foreach (var loc in campusLocations)
            {
                Debug.Log($"  - {loc.locationId}: {loc.locationName}");
            }
        }
    }

    void CreateNavigationArrow()
    {
        if (_currentArrow != null)
        {
            DestroyImmediate(_currentArrow);
        }

        if (arrowPrefab != null)
        {
            _currentArrow = Instantiate(arrowPrefab);
            
            ArrowPointer arrowPointer = _currentArrow.GetComponent<ArrowPointer>();
            if (arrowPointer == null)
            {
                arrowPointer = _currentArrow.AddComponent<ArrowPointer>();
            }
            
            arrowPointer.SetPlayer(player);
            arrowPointer.SetArrowHeight(arrowHeight);
            arrowPointer.SetTarget(_targetLocation);
            
            Debug.Log($"Created navigation arrow for {_currentLocationData.locationName}");
        }
        else
        {
            Debug.LogWarning("Arrow prefab not assigned! Arrow navigation will not work.");
        }
    }

    void CreateDestinationMarker()
    {
        if (_currentDestinationMarker != null)
        {
            DestroyImmediate(_currentDestinationMarker);
        }

        if (destinationMarkerPrefab != null && _targetLocation != null)
        {
            Vector3 markerPosition = _targetLocation.position + _currentLocationData.offset;
            
            // to adjust the marker height to be above ground since the ground is a terrain and not plane
            RaycastHit hit;
            if (Physics.Raycast(markerPosition + Vector3.up * 10f, Vector3.down, out hit, 20f, groundLayerMask))
            {
                markerPosition = hit.point + Vector3.up * 3f; // position set at 3 meters above ground
            }
            
            _currentDestinationMarker = Instantiate(destinationMarkerPrefab, markerPosition, Quaternion.identity);
        }
    }

    public void ClearDestination()
    {
        _targetLocation = null;
        _currentLocationData = null;
        _hasArrivedAtDestination = false;
        
        if (_currentArrow != null)
        {
            DestroyImmediate(_currentArrow);
            _currentArrow = null;
        }
        
        if (_currentDestinationMarker != null)
        {
            DestroyImmediate(_currentDestinationMarker);
            _currentDestinationMarker = null;
        }
    }

    void Update()
    {
        if (_targetLocation != null && player != null)
        {
            CheckArrivalDistance();
        }
    }

    void CheckArrivalDistance()
    {
        if (_currentLocationData == null || _hasArrivedAtDestination) return;

        float distanceToTarget = Vector3.Distance(player.position, _targetLocation.position);
        
        OnDistanceUpdate?.Invoke(distanceToTarget);
        
        if (distanceToTarget <= _currentLocationData.arrivalDistance)
        {
            _hasArrivedAtDestination = true;
            OnDestinationReached?.Invoke(_currentLocationData.locationName);
        }
    }

    // some utility methods for UI integration
    public bool IsNavigating()
    {
        return _targetLocation != null && !_hasArrivedAtDestination;
    }

    public string GetCurrentDestinationName()
    {
        return _currentLocationData?.locationName ?? "None";
    }

    public float GetDistanceToDestination()
    {
        if (_targetLocation == null || player == null) return 0f;
        return Vector3.Distance(player.position, _targetLocation.position);
    }

    public LocationData GetCurrentLocationData()
    {
        return _currentLocationData;
    }
    
    public List<LocationData> GetAllLocations()
    {
        return campusLocations;
    }
    
    public LocationData FindLocationByPartialName(string partialName)
    {
        return campusLocations.Find(loc => 
            loc.locationName.ToLower().Contains(partialName.ToLower()) ||
            loc.locationId.ToLower().Contains(partialName.ToLower()));
    }
}