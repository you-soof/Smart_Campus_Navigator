using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UINavigationMenu : MonoBehaviour
{
    [Header("UI References")]
    public LocationManager locationManager;
    public TMP_Dropdown locationDropdown;
    public GameObject instructionPanel;
    public TextMeshProUGUI buildingNameText;
    public TextMeshProUGUI buildingPurposeText;
    public TextMeshProUGUI facilitiesText;
    public TextMeshProUGUI navigationInstructionText;
    public TextMeshProUGUI distanceText;
    public Button startNavigationButton;
    public Button cancelNavigationButton;
    public NavigationCanvasToggleController navigationCanvasActive;

    [Header("Building Information")]
    public List<BuildingInfo> buildingData = new List<BuildingInfo>();
    
    [Header("Audio Feedbacks")]
    public AudioSource audioSource;
    public AudioClip welcomeClip;
    public AudioClip instructionClip;
    public AudioClip navigationStartedClip;
    public AudioClip navigationCanceledClip;
    public AudioClip lessThanTenClip;
    public AudioClip arrivedClip;

    private BuildingInfo _currentSelectedBuilding;
    private bool _isNavigating = false;
    private bool _hasPlayed = false;

    [System.Serializable]
    public class BuildingInfo
    {
        public string buildingCode;
        public string buildingName;
        public string buildingType; 
        public string purpose;
        [TextArea(3, 5)]
        public string facilities;
        [TextArea(2, 3)]
        public string navigationHint; 
    }

    void Start()
    {
        InitializeBuildingData();
        PopulateDropdown();
        SetupEventListeners();
        
        if (instructionPanel != null)
            instructionPanel.SetActive(false);

        locationManager.OnDestinationReached += OnArrivalNotification;
    }

    void OnLastTenMeters()
    {
        audioSource.PlayOneShot(lessThanTenClip);
    }
    
    void OnArrivalNotification(string buildingName)
    {
        audioSource.PlayOneShot(arrivedClip);
        navigationInstructionText.text = $"Arrived at {buildingName}!";
        navigationInstructionText.color = Color.green;
        
        Invoke("ResetNavigationText", 3f);
    }
    
    void Update()
    {
        if (!_hasPlayed && navigationCanvasActive.isCanvasVisible == true)
        {
            audioSource.PlayOneShot(welcomeClip);
            _hasPlayed = true;
        }
        
        if (locationManager.IsNavigating() && distanceText != null)
        {
            float distance = locationManager.GetDistanceToDestination();
            distanceText.text = $"Distance: {distance:F1} meters";
            
            if (distance < 10f)
            {
                distanceText.color = Color.green;
            }
            else if (distance < 30f)
                distanceText.color = Color.yellow;
            else
                distanceText.color = Color.white;
        }
    }
    

    void InitializeBuildingData()
    {
        buildingData = new List<BuildingInfo>
        {
            new BuildingInfo
            {
                buildingCode = "Building A",
                buildingName = "East Building A",
                buildingType = "East Building",
                purpose = "Houses Kansai University Elementary School",
                facilities = "Elementary School Homeroom Classrooms, Elementary School Library, English Language Workshop, Music Room, Practice Booths, Broadcasting Room, Principal's Office, Staff Room, Administration Office, Multipurpose Room, Nurse's Office, Counseling Room, Guest Reception Room, Kitchen",
                navigationHint = "Located in the eastern section of campus. Look for the elementary school entrance."
            },
            new BuildingInfo
            {
                buildingCode = "Building B",
                buildingName = "East Building B",
                buildingType = "East Building",
                purpose = "Accommodates Kansai University Junior High School",
                facilities = "Junior High School Homeroom Classrooms, English Language Workshop 1 & 2, Multipurpose Room, Large Meeting Room, Technical Arts Workshop, Elective Subject Room, School Student Council Room, Small Meeting Room, Music Room 1 & 2, Practice Booths 1-4, Principal's Office, Staff Room, Administration Office, Academic & Career Counseling Room, Nurse's Office, Counseling Room, Guidance Room, Broadcasting Room",
                navigationHint = "Adjacent to Building A in the eastern campus area. Follow signs for Junior High School."
            },
            new BuildingInfo
            {
                buildingCode = "Building C",
                buildingName = "East Building C",
                buildingType = "East Building",
                purpose = "Dedicated to Kansai University Senior High School",
                facilities = "Senior High School Homeroom Classrooms, Physics Room, Computer Room, Medium Lecture Room, Chemistry Laboratory, Multimedia Room, Geology & Safety Science Room, Biology Room, English Language Workshop 1 & 2, Clothing Room, Cooking Room, Junior/Senior High School Library, Arts & Crafts Room 1 & 2, Kiln, Teaching Materials Room, Science Laboratory, Tea Ceremony Room, Homemaking Room, Student Council Room, Teaching Material Preparation Room",
                navigationHint = "Part of the eastern building complex. Look for the Senior High School signage and science laboratories."
            },
            new BuildingInfo
            {
                buildingCode = "Building D",
                buildingName = "West Building D",
                buildingType = "West Building",
                purpose = "Hosts the Faculty and Graduate School of Societal Safety Sciences",
                facilities = "Lecture Halls, Seminar Rooms, Research Laboratories, Faculty Offices, Graduate Student Rooms, Administrative Offices",
                navigationHint = "Located in the western part of campus. Main building for Societal Safety Sciences faculty."
            },
            new BuildingInfo
            {
                buildingCode = "Building E",
                buildingName = "West Building E",
                buildingType = "West Building",
                purpose = "Additional space for the Faculty and Graduate School of Societal Safety Sciences",
                facilities = "Advanced Research Laboratories, Specialized Seminar Rooms, Collaborative Workspaces, Faculty and Graduate Student Offices",
                navigationHint = "Adjacent to Building D in the western campus area. Advanced research facilities."
            },
            new BuildingInfo
            {
                buildingCode = "Building F",
                buildingName = "West Building F",
                buildingType = "West Building",
                purpose = "Supports research and education in societal safety sciences",
                facilities = "Simulation Rooms, Data Analysis Centers, Project-Based Learning Spaces, Meeting and Conference Rooms",
                navigationHint = "Part of the western building cluster. Features specialized simulation and analysis facilities."
            },
            new BuildingInfo
            {
                buildingCode = "Building K",
                buildingName = "North Building K",
                buildingType = "North Building",
                purpose = "Physical education and welfare building",
                facilities = "Gymnasium, Indoor Heated Swimming Pool, Fitness Rooms, Locker Rooms, Health and Counseling Centers",
                navigationHint = "Located in the northern section of campus. Look for the large gymnasium structure."
            },
            new BuildingInfo
            {
                buildingCode = "Building L",
                buildingName = "Student Services Building",
                buildingType = "Student Services Building",
                purpose = "Provides various services to students",
                facilities = "Cafeteria, Student Lounge, Bookstore, Convenience Store, ATM Services, Travel Desk",
                navigationHint = "Central location for student services. Look for dining and shopping areas."
            },
            new BuildingInfo
            {
                buildingCode = "Building R",
                buildingName = "Research Building",
                buildingType = "Research Building",
                purpose = "Dedicated to research activities",
                facilities = "Research Laboratories, Faculty Offices, Graduate Student Workspaces, Meeting Rooms",
                navigationHint = "Specialized research facility. Look for laboratory signage and research equipment."
            },
            new BuildingInfo
            {
                buildingCode = "Building S",
                buildingName = "Student House",
                buildingType = "Student House",
                purpose = "Residential facility for students",
                facilities = "Dormitory Rooms, Common Lounge Areas, Study Rooms, Laundry Facilities, Kitchenettes",
                navigationHint = "Student residential area. Look for dormitory entrance and common areas."
            }
        };
    }

    void PopulateDropdown()
    {
        if (locationDropdown == null) return;

        locationDropdown.ClearOptions();
        List<string> options = new List<string> { "Select a Building..." };
        
        foreach (var building in buildingData)
        {
            options.Add($"{building.buildingCode}");
            //options.Add($"{building.buildingCode} - {building.buildingName}");
        }
        
        locationDropdown.AddOptions(options);
    }

    void SetupEventListeners()
    {
        if (locationDropdown != null)
            locationDropdown.onValueChanged.AddListener(OnBuildingSelected);
        
        if (startNavigationButton != null)
            startNavigationButton.onClick.AddListener(StartNavigation);
        
        if (cancelNavigationButton != null)
            cancelNavigationButton.onClick.AddListener(CancelNavigation);
    }

    void OnBuildingSelected(int index)
    {
        if (index == 0) // setting action for "Select a Building..." option
        {
            if (instructionPanel != null)
                instructionPanel.SetActive(false);
            return;
        }
        
        if (index - 1 < buildingData.Count)
        {
            _currentSelectedBuilding = buildingData[index - 1];
            DisplayBuildingInformation();
            audioSource.PlayOneShot(instructionClip);
        }
    }

    void DisplayBuildingInformation()
    {
        if (_currentSelectedBuilding == null || instructionPanel == null) return;
        
        instructionPanel.SetActive(true);
        
        if (buildingNameText != null)
            buildingNameText.text = $"{_currentSelectedBuilding.buildingCode}\n{_currentSelectedBuilding.buildingName}";

        if (buildingPurposeText != null)
            buildingPurposeText.text = $"Purpose: {_currentSelectedBuilding.purpose}";

        if (facilitiesText != null)
        {
            string formattedFacilities = FormatFacilitiesList(_currentSelectedBuilding.facilities);
            facilitiesText.text = $"Key Facilities:\n{formattedFacilities}";
        }

        if (navigationInstructionText != null)
            navigationInstructionText.text = $"Navigation Tip: {_currentSelectedBuilding.navigationHint}";


        if (startNavigationButton != null)
        {
            startNavigationButton.interactable = true;
            startNavigationButton.GetComponentInChildren<TextMeshProUGUI>().text = 
                _isNavigating ? "Navigation Active" : "Start Navigation";
        }
    }

    string FormatFacilitiesList(string facilities)
    {
        string[] facilityArray = facilities.Split(',');
        string formatted = "";
        
        int count = 0;
        foreach (string facility in facilityArray)
        {
            if (count >= 8) 
            {
                formatted += "• ... and more";
                break;
            }
            formatted += $"• {facility.Trim()}\n";
            count++;
        }
        
        return formatted.TrimEnd('\n');
    }

    void StartNavigation()
    {
        if (_currentSelectedBuilding == null || locationManager == null) return;
        
        locationManager.SetDestination(_currentSelectedBuilding.buildingCode);
        _isNavigating = true;
        audioSource.PlayOneShot(navigationStartedClip);
        
        if (startNavigationButton != null)
        {
            startNavigationButton.GetComponentInChildren<TextMeshProUGUI>().text = "Navigation Active";
            startNavigationButton.interactable = false;
        }

        if (cancelNavigationButton != null)
            cancelNavigationButton.interactable = true;
        
        if (navigationInstructionText != null)
            navigationInstructionText.text = $"Following arrow to {_currentSelectedBuilding.buildingName}. {_currentSelectedBuilding.navigationHint}";
    }

    void CancelNavigation()
    {
        if (locationManager == null) return;
        
        locationManager.ClearDestination();
        _isNavigating = false;
        audioSource.PlayOneShot(navigationCanceledClip);
        
        if (startNavigationButton != null)
        {
            startNavigationButton.GetComponentInChildren<TextMeshProUGUI>().text = "Start Navigation";
            startNavigationButton.interactable = true;
        }

        if (cancelNavigationButton != null)
            cancelNavigationButton.interactable = false;
        
        if (navigationInstructionText != null && _currentSelectedBuilding != null)
            navigationInstructionText.text = $"Navigation Tip: {_currentSelectedBuilding.navigationHint}";
    }
    
    public bool IsNavigating()
    {
        return _isNavigating;
    }
    
    public BuildingInfo GetCurrentSelectedBuilding()
    {
        return _currentSelectedBuilding;
    }
}