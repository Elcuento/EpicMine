using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class ThreadWorker : MonoBehaviour
{
    private Action<bool> _onCompleted;

    public bool IsComplete { get; private set; }

    public bool IsError { get; private set; }

    public bool IsCancelled { get; private set; }

    private Task _thread;

    private CancellationTokenSource _cancelToken;

    public void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public ThreadWorker Initialize(Action action, Action<bool> callBack)
    {
        _cancelToken = new CancellationTokenSource();

        _onCompleted = callBack;

        _thread = new Task(() =>
        {
            try
            {
                action.Invoke();
                OnCompleted(true, false);
            }
            catch (Exception)
            {
                OnCompleted(true, true);
            }
        }, _cancelToken.Token);

        StartCoroutine(Work());

        return this;
    }

    public void Cancel()
    {
        IsCancelled = true;

        Destroy();
    }

    public IEnumerator Work()
    {
        _thread.Start();

        yield return new WaitUntil(()=> IsComplete || IsCancelled);

        if (IsCancelled)
        {
            Destroy();
            yield break;
        }

        _onCompleted?.Invoke(IsError);

        Destroy();
    }

    private void OnCompleted(bool isCompleted, bool isError)
    {
        IsComplete = isCompleted;
        IsError = isError;
    }

    private void Destroy()
    {
        Destroy(gameObject);
    }

    public void OnDestroy()
    {
        _cancelToken?.Cancel();
        _cancelToken?.Dispose();
    }
}
