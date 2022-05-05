using Scene;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Region : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public event Action<string> OnRegionClick;

    [SerializeField] private Animator regionAnimator;
    [SerializeField] private InfoWindow infoWindow;
    [SerializeField] private MainCamera camera;
    [SerializeField] private string regionNameKey;
    [SerializeField] private MeshCollider meshCollider;
    [SerializeField] private MeshFilter meshFilter;
    public Animator RegionAnimator
    {
        get
        {
            if (regionAnimator == null)
                regionAnimator = GetComponent<Animator>();
            return regionAnimator;
        }
        private set { regionAnimator = value; }
    }

    public bool RegionClickPermissed { get; private set; } = true;
    #region Public Methods

    public void OnPointerClick(PointerEventData eventData)
    {
#if UNITY_EDITOR
        Debug.Log("Region click");
#endif
        if (!string.IsNullOrEmpty(regionNameKey))
        {
            if (RegionClickPermissed)
                OnRegionClick?.Invoke(regionNameKey);
        }
        else
        {
#if UNITY_EDITOR
            Debug.Log("Region name missing!");
#endif
            throw new Exception("Region name missing!");
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        RegionAnimator.SetBool("RegionShow", true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        RegionAnimator.SetBool("RegionShow", false);
    }

    #endregion Public Methods

    private void Awake()
    {
        infoWindow = GameObject.FindGameObjectWithTag("InfoPanel").GetComponent<InfoWindow>();
        camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<MainCamera>();
        camera.OnCameraSwipeStartEvent += OnCameraSwipeStartCallback;
        camera.OnCameraSwipeEndEvent += OnCameraSwipeEndCallback;
        OnRegionClick += infoWindow.OnRegionClickCallback;
    }

    private void OnCameraSwipeEndCallback()
    {
        RegionClickPermissed = true;
    }

    private void OnCameraSwipeStartCallback()
    {
        RegionClickPermissed = false;
    }

    private void OnDestroy()
    {
        OnRegionClick -= infoWindow.OnRegionClickCallback;
    }
}