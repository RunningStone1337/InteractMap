using Scene;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Region : MonoBehaviour, IPointerClickHandler
{
    public event Action<string> OnRegionClick;

    [SerializeField] private InfoWindow infoWindow;
    [SerializeField] private string regionNameKey;

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

    #endregion Public Methods

    private void Awake()
    {
        infoWindow = GameObject.FindGameObjectWithTag("InfoPanel").GetComponent<InfoWindow>();
        OnRegionClick += infoWindow.OnRegionClickCallback;
    }

    private void OnDestroy()
    {
        OnRegionClick -= infoWindow.OnRegionClickCallback;
    }
}