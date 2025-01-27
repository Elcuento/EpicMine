using UnityEngine;
using System.Collections;
using System.Text;
using BlackTemple.Common;
using BlackTemple.EpicMine;
using UnityEngine.Profiling;
using TMPro;
using UnityEngine.Purchasing;
#if UNITY_EDITOR
using UnityEditor;
#endif
#if UNITY_5
using UnityEngine.Profiling;
#endif
//-----------------------------------------------------------------------------------------------------
public class DeveloperStats : MonoBehaviour
{

    public Color tx_Color = Color.white;
    private TextMeshProUGUI _attachedTo;
    private StringBuilder tx;

    float updateInterval = 1.0f;
    float lastInterval; // Last interval end time
    float frames = 0; // Frames over current interval

    float framesavtick = 0;
    float framesav = 0.0f;

    // Use this for initialization
    void Start()
    {
        lastInterval = Time.realtimeSinceStartup;
        frames = 0;
        framesav = 0;
        tx = new StringBuilder {Capacity = 200};
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    public string GetStats()
    {
        return tx.ToString();
    }

    public void AttackTo(TextMeshProUGUI to)
    {
        _attachedTo = to;
    }

    public void OnDisable()
    {
        DeAttach();
    }
    public void DeAttach()
    {
        _attachedTo = null;
    }
    // Update is called once per frame
    void Update()
    {
        ++frames;

        var timeNow = Time.realtimeSinceStartup;

        if (timeNow > lastInterval + updateInterval)
        {

            float fps = (int) (frames / (timeNow - lastInterval));
            float ms = 1000.0f / Mathf.Max(fps, 0.00001f);

            ++framesavtick;
            framesav += fps;
            float fpsav = (int)(framesav / framesavtick);

            tx.Length = 0;
            tx.Append("Using settings " +  (QualitySettings.names[QualitySettings.GetQualityLevel()]).ToString() + "\n");
            tx.AppendFormat("Time : {0} ms     Current FPS: {1}     AvgFPS: {2}\nGPU memory : {3}    Sys Memory : {4}\n Current Subscribers : {5}\n", ms, fps, fpsav, SystemInfo.graphicsMemorySize, SystemInfo.systemMemorySize, EventManager.Instance.EventsCount)

            .AppendFormat("TotalAllocatedMemory : {0}mb\nTotalReservedMemory : {1}mb\nTotalUnusedReservedMemory : {2}mb",
            Profiler.GetTotalAllocatedMemoryLong() / 1048576,
            Profiler.GetTotalReservedMemoryLong() / 1048576,
            Profiler.GetTotalUnusedReservedMemoryLong() / 1048576
            );

#if UNITY_EDITOR
            tx.AppendFormat("\nDrawCalls : {0}\nUsed Texture Memory : {1}\nrenderedTextureCount : {2}", UnityStats.drawCalls, UnityStats.usedTextureMemorySize / 1048576, UnityStats.usedTextureCount);
#endif

            if (_attachedTo != null)
                _attachedTo.text = tx.ToString();

            frames = 0;
            lastInterval = timeNow;
        }

    }
}