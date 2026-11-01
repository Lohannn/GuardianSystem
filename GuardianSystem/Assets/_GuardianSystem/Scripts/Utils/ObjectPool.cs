using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [Header("Pool Configuration")]
    [SerializeField] private GameObject objectPrefab;
    [SerializeField] private int initialPoolSize = 6;

    private List<GameObject> pool = new List<GameObject>();

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        InitializePool();
    }

    private void InitializePool()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject obj = Instantiate(objectPrefab, rectTransform, false);
            obj.SetActive(false);
            pool.Add(obj);
        }
    }

    public GameObject GetObjectFromPool()
    {
        foreach (GameObject obj in pool)
        {
            if (!obj.activeInHierarchy)
            {
                return obj;
            }
        }

        Debug.LogWarning($"Pool of {objectPrefab.name} exhausted. Instantiating new object.");
        GameObject newObj = Instantiate(objectPrefab, rectTransform, false);
        pool.Add(newObj);
        return newObj;
    }

    public void ReturnObjectToPool(GameObject obj)
    {
        obj.transform.SetParent(rectTransform, false);
        obj.SetActive(false);
    }
}
