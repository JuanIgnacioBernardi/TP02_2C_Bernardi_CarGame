using UnityEngine;
using System.Collections.Generic;
public class Pool<T> where T : MonoBehaviour
{
    private readonly T prefab;
    private readonly Transform container;
    private readonly int maxCapacity;
    private readonly Queue<T> available = new Queue<T>();
    private int totalCreated;

    public Pool(T prefab, Transform container, int initialSize, int maxCapacity)
    {
        this.prefab = prefab;
        this.container = container;
        this.maxCapacity = maxCapacity;

        for (int i = 0; i < initialSize; i++)
            CreateNew();
    }
    private T CreateNew()
    {
        if (totalCreated >= maxCapacity) return null;

        T obj = Object.Instantiate(prefab, container);
        obj.gameObject.SetActive(false);
        available.Enqueue(obj);
        totalCreated++;
        return obj;
    }
    public T Get()
    {
        if (available.Count == 0)
        {
            T newObj = CreateNew();
            if (newObj == null) return null;
        }

        T item = available.Dequeue();
        item.gameObject.SetActive(true);
        return item;
    }
    public void Return(T item)
    {
        item.gameObject.SetActive(false);
        item.transform.SetParent(container);
        available.Enqueue(item);
    }
}
