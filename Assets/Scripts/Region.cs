using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Region : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public event Action<string> OnRegionClick;

    [SerializeField] private Animator regionAnimator;
    [SerializeField] private InfoWindow infoWindow;
    [SerializeField] private string regionNameKey;
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
    #region Public Methods

    public void OnPointerClick(PointerEventData eventData)
    {
#if UNITY_EDITOR
        Debug.Log("Region click");
#endif
        if (!string.IsNullOrEmpty(regionNameKey))
            OnRegionClick?.Invoke(regionNameKey);
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

    //public void SetRegionShowAnimator(string val)
    //{
    //    RegionAnimator.SetBool("RegionShow", bool.Parse(val));
    //}
    #endregion Public Methods

    private void Awake()
    {
        infoWindow = GameObject.FindGameObjectWithTag("InfoPanel").GetComponent<InfoWindow>();
        OnRegionClick += infoWindow.OnRegionClickCallback;
        var col = gameObject.AddComponent<MeshCollider>();
        col.sharedMesh = gameObject.GetComponent<MeshFilter>().mesh;
    }

    private void OnDestroy()
    {
        OnRegionClick -= infoWindow.OnRegionClickCallback;
    }

    
}