using System;
using UnityEngine;

public class AsyncHelper : MonoBehaviour
{
    public static ThreadWorker StartAsync(Action action, Action<bool> callBack)
    {       
        return new GameObject("ThreadWorker")
            .AddComponent<ThreadWorker>()
            .Initialize(action, callBack);
    }
}
