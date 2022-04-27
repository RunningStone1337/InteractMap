using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneObjectsStorage : MonoBehaviour
{
    [SerializeField] GameObject dialectPairPrefab;
    public GameObject DialectPairPrefab { get => dialectPairPrefab; }
    public static SceneObjectsStorage SceneStorage { get => sceneObjectsStorage; }
    static SceneObjectsStorage sceneObjectsStorage;
    private void Awake()
    {
        if (sceneObjectsStorage == null)
        {
            sceneObjectsStorage = this;
            return;
        }
        DestroyImmediate(this.gameObject);
    }
}
