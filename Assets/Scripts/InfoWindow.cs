using Newtonsoft.Json.Linq;
using Scene;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
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
        //открытие окошка с инфой если скрыто
        ShowIfHidedInfoPanel();
        //выключить список диалектизмов
        foreach (var item in dialectPairs)
            item.gameObject.SetActive(false);
        //заменить на время загрузки ответа от сервера текст и изображение
        //анимациями загрузки
        StartCoroutine(regionNameLoader.StartLoading());
        StartCoroutine(regionImageLoader.StartLoading());
        //Текст названия региона
        StartCoroutine(GetRegionNameText(regionName));
        //изображение региона
        StartCoroutine(GetRegionImage(regionName));
        //диалектизмы региона
        StartCoroutine(GetRegionDialectisms(regionName));
    }

    public void ShowIfHidedInfoPanel()
    {
        if (!animator.GetBool("IsShowing"))
            animator.SetTrigger("Show");
    }

    public void SetWindowShowing(string val)
    {
        animator.SetBool("Show", bool.Parse(val));
    }

    private void Awake()
    {
        camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<MainCamera>();
    }

    private IEnumerator GetRegionDialectisms(string regionName)
    {
        //для каждого ответа активируем объект DialectPair из существуюшего пула,
        //если нет доступных - создаём новые объекты
        using UnityWebRequest req = UnityWebRequest.Get($"http://localhost:8002/dialects/d/all/region-name/{regionName}/");
        yield return req.SendWebRequest();
        if (req.result == UnityWebRequest.Result.ProtocolError || req.result == UnityWebRequest.Result.ConnectionError)
        {
            regionNameText.text = $"Ошибка сети {req.error}";
#if DEBUG
                Debug.Log($"{req.error}");
#endif
        }
        else
        {
            var arr = JArray.Parse(req.downloadHandler.text);
            var dialects = arr.Children<JObject>().ToList();
            for (int i = 0; i < dialects.Count; i++)
            {
                HandleDialect(dialects, i);
                yield return null;
            }
        }
    }

    private void HandleDialect(List<JObject> dialects, int i)
    {
        if (dialectPairs.Count <= i)
            InsertNewPair();
        var props = dialects[i].Properties();
        foreach (var prop in props)
            SetDialectPairProps(i, prop);
    }

    private void SetDialectPairProps(int i, JProperty prop)
    {
        if (prop.Name.Equals("title"))
            dialectPairs[i].DialectismName.text = prop.Value.ToString();
        else if (prop.Name.Equals("description"))
            dialectPairs[i].DialectismMeaning.text = prop.Value.ToString();
        dialectPairs[i].gameObject.SetActive(true);
    }

    private IEnumerator GetRegionImage(string regionName)
    {
        regionImage.sprite = null;
        regionImage.color = new Color(regionImage.color.r, regionImage.color.g, regionImage.color.b, 1f/255f);
        using UnityWebRequest req = UnityWebRequest.Get($"http://localhost:8002/files/download/named/{regionName}/");
        yield return req.SendWebRequest();
        if (req.result == UnityWebRequest.Result.ProtocolError || req.result == UnityWebRequest.Result.ConnectionError)
        {
            regionImage.sprite = null;
#if DEBUG
                Debug.Log($"{req.error}");
#endif
        }
        else
        {
            var bytes = req.downloadHandler.data;
            var tex = new Texture2D(10, 10);
            yield return null;
            yield return tex.LoadImage(bytes);
            yield return null;
            var spr = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100f);
            regionImage.sprite = spr;
            regionImage.color = new Color(regionImage.color.r, regionImage.color.g, regionImage.color.b, 255);
            regionImage.preserveAspect = true;
            regionImageLoader.IsActive = false;
        }

        //var bytes = Convert.FromBase64String(@"");
        //var tex = new Texture2D(10, 10);
        //tex.LoadImage(bytes);
        //var spr = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100f);
        //regionImage.sprite = spr;
        //regionImage.preserveAspect = true;
        //regionImageLoader.IsActive = false;
    }

    private IEnumerator GetRegionNameText(string regionName)
    {
        regionNameText.text = string.Empty;
        using (UnityWebRequest req = UnityWebRequest.Get($"http://localhost:8002/regions/named/{regionName}/")) 
        {
            yield return req.SendWebRequest();
            if (req.result == UnityWebRequest.Result.ProtocolError || req.result == UnityWebRequest.Result.ConnectionError)
            {
                regionNameText.text = $"Ошибка сети {req.error}";
#if DEBUG
                Debug.Log($"{req.error}");
#endif
            }
            else
            {
                JObject js = JObject.Parse(req.downloadHandler.text);
                foreach (var prop in js.Properties())
                {
                    if (prop.Name.Equals("title"))
                        regionNameText.text = prop.Value.ToString();
                }
            }
        } 
        regionNameLoader.IsActive = false;
    }

    private void InsertNewPair()
    {
        var newPair = Instantiate(SceneObjectsStorage.SceneStorage.DialectPairPrefab, dialectismsScrollRectTransform).GetComponent<DialectPair>();
        var rectTransform = newPair.gameObject.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, 170f);
        dialectPairs.Add(newPair);
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