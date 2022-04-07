using Scene;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InfoWindow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    #region Public Methods

    public void HideInfoPanel()
    {
        animator.SetTrigger("Hide");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        camera.FreezeSwipes = true;
        Debug.Log("OnPointerEnter");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!Input.GetMouseButton(0))
        {
            camera.FreezeSwipes = false;
        }
        Debug.Log("OnPointerExit");
    }

    public void OnRegionClickCallback(string regionName)
    {
        //открытие окошка с инфой
        ShowInfoPanel();
        //выключить список диалектизмов
        foreach (var item in dialectPairs)
            item.gameObject.SetActive(false);
        //очистить текст названия региона
        regionNameText.text = string.Empty;
        //очистить изображение региона
        regionImage.sprite = null;
        //заменить на время загрузки ответа от сервера текст и изображение
        //анимациями загрузки
        StartCoroutine(regionNameLoader.StartLoading());
        StartCoroutine(regionImageLoader.StartLoading());

        //заглушки до реализации клиент-серверного взаимодействия

        //Текст названия региона
        StartCoroutine(GetRegionNameText(regionName));
        //изображение региона
        StartCoroutine(GetRegionImage(regionName));
        //диалектизмы региона
        StartCoroutine(GetRegionDialectisms(regionName));
    }

    public void ShowInfoPanel()
    {
        animator.SetTrigger("Show");
    }

    private void Awake()
    {
        camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<MainCamera>();
    }

    private IEnumerator GetRegionDialectisms(string regionName)
    {
        //слать запросы диалектизмов до тех пор, пока сервер не ответит, что переданы все
        //или сервер не перестанет отвечать
        //для каждого ответа активируем объект DialectPair из существуюшего пула,
        //если нет доступных - создаём новые объекты
        int pairs = 10;
        string dialectName, dialectMeaning;
        for (int pair = 0; pair < pairs; pair++)
        {
            dialectName = $"dialectName #{pair}";
            dialectMeaning = $"here is description #{pair}";
            if (dialectPairs.Count <= pair)
            {
                var newPair = Instantiate(SceneObjectsStorage.SceneStorage.DialectPairPrefab, dialectismsScrollRectTransform).GetComponent<DialectPair>();
                var rectTransform = newPair.gameObject.GetComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, 170f);
                dialectPairs.Add(newPair);
            }
            dialectPairs[pair].DialectismName.text = dialectName;
            dialectPairs[pair].DialectismMeaning.text = dialectMeaning;
            dialectPairs[pair].gameObject.SetActive(true);
            yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));
        }
    }

    private IEnumerator GetRegionImage(string regionName)
    {
        var col = regionImage.color;
        yield return new WaitForSeconds(1.5f);
        regionImage.color = Color.white;
        regionImageLoader.IsActive = false;
        yield return new WaitForSeconds(2.5f);
        regionImage.color = col;
    }

    private IEnumerator GetRegionNameText(string regionName)
    {
        yield return new WaitForSeconds(0.7f);
        regionNameText.text = "Московская область";
        regionNameLoader.IsActive = false;
    }

    #endregion Public Methods

    #region Private Fields

    [SerializeField] private Animator animator;
    [SerializeField] private MainCamera camera;
    [SerializeField] private RectTransform dialectismsScrollRectTransform;
    [SerializeField] private List<DialectPair> dialectPairs;
    [SerializeField] private Image regionImage;
    [SerializeField] private LoadImageAnimator regionImageLoader;
    [SerializeField] private LoadImageAnimator regionNameLoader;
    [SerializeField] private Text regionNameText;

    #endregion Private Fields
}