using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineManager : MonoBehaviour
{
    private static CoroutineManager instance;

    private List<Coroutine> activeCoroutines = new List<Coroutine>();

    public static CoroutineManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameObject("CoroutineManager").AddComponent<CoroutineManager>();
                DontDestroyOnLoad(instance.gameObject);
            }
            return instance;
        }
    }

    public new Coroutine StartCoroutine(IEnumerator routine)
    {
        Coroutine coroutine = StartCoroutineInternal(routine);
        activeCoroutines.Add(coroutine);
        return coroutine;
    }

    private Coroutine StartCoroutineInternal(IEnumerator routine)
    {
        return base.StartCoroutine(routine);
    }

    public new void StopCoroutine(Coroutine coroutine)
    {
        if (coroutine != null)
        {
            StopCoroutineInternal(coroutine);
            activeCoroutines.Remove(coroutine);
        }
    }

    private void StopCoroutineInternal(Coroutine coroutine)
    {
        base.StopCoroutine(coroutine);
    }

    public new void StopAllCoroutines()
    {
        foreach (Coroutine coroutine in activeCoroutines)
        {
            StopCoroutineInternal(coroutine);
        }
        activeCoroutines.Clear();
    }

    private void OnDestroy()
    {
        instance = null;
    }
}
