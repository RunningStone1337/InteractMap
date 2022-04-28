using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LoadImageAnimator : MonoBehaviour
{
    public bool IsActive { get=> isActive; set=> isActive = value; }
    #region Public Methods

    public IEnumerator Spin()
    {
        loadImg.rectTransform.rotation = Quaternion.Euler(new Vector3(0, 0, loadImg.rectTransform.rotation.eulerAngles.z - rotatingSpeed));
        yield return null;
    }

    public IEnumerator StartLoading()
    {
        loadImg.enabled = true;
        isActive = true;
        while (isActive)
            yield return Spin();
        loadImg.enabled = false;
    }

    #endregion Public Methods

    #region Private Fields

    [SerializeField] private bool isActive = false;
    [SerializeField] private Image loadImg;
    [SerializeField] [Range(0, 10f)] private float rotatingSpeed;

    #endregion Private Fields

    #region Private Methods

    private void Awake()
    {
        if (loadImg == null)
            loadImg = GetComponent<Image>();
    }

    #endregion Private Methods
}