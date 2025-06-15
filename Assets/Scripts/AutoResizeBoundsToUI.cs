using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class AutoResizeBoundsToUI : MonoBehaviour
{
    [SerializeField] private RectTransform targetUI; 
    [SerializeField] private float depth = 1f;    

    private BoxCollider _boxCollider;

    void Awake()
    {
        _boxCollider = GetComponent<BoxCollider>();
        UpdateBounds();
    }

    void Update()
    {
        UpdateBounds(); 
    }

    void UpdateBounds()
    {
        if (targetUI == null) return;

        Vector2 size = targetUI.sizeDelta;

        _boxCollider.size = new Vector3(size.x, size.y, depth);
        _boxCollider.center = Vector3.zero;  
    }
}