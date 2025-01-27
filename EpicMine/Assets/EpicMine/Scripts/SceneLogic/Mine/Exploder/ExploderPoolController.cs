using System.Collections.Generic;
using BlackTemple.EpicMine;
using Exploder;
using Exploder.Utils;
using UnityEngine;

public class ExploderPoolController : MonoBehaviour
{
    public static ExploderPoolController Instance
    {
        get
        {
            if (_instance == null)
            {
                var pool = new GameObject("ExploderPoolController");
                _instance = pool.AddComponent<ExploderPoolController>();
            }

            return _instance;
        }
    }

    private static ExploderPoolController _instance;

    public const string DefaultPoolWall = "rock_ore";
    public int DefaultPoolSize = 2;

    private List<ExploderPool> _poolList;

    public void Awake()
    {
        _poolList = new List<ExploderPool>();
    }

    public void Start()
    {
        FragmentPool.Instance.transform.SetParent(transform);
    }
    public void OnDestroy()
    {
        Clear();
    }

    private void ResetFragmentsOriginal(ExploderPool pool)
    {
        foreach (var fragment in pool.Fragments)
        {
            fragment.ResetOriginal();
        }
    }

    private void Clear()
    {
        foreach (var exploderPool in _poolList)
        {
            FragmentPool.Instance.DestroyFragments(exploderPool.Fragments);
        }
        _poolList.Clear();
    }

    public void SetFragmentPoolTexture(ExploderPool pool, Material material)
    {
        var poolsFragment = pool.Fragments;

        foreach (var fragment in poolsFragment)
        {
            fragment.GetComponentInChildren<MeshRenderer>().material = material;
        }
    }
    /// <summary>
    /// Explode saved pool(GetRandomPool), don't forget to set position first
    /// </summary>
    /// <param name="pool"></param>
    public void ExplodeCracked(ExploderPool pool)
    {
        ResetFragmentsOriginal(pool);
        var crackedObject = Core.Instance.GetCrackedObject(pool.ExpObject);
        crackedObject.Explode();
    }

    public ExploderPool GetRandomPool()
    {
        if (_poolList.Count == 0)
           GenerateDefaultPool(DefaultPoolSize);
        
        return _poolList[Random.Range(0, _poolList.Count)];
    }

    public ExploderPool GetRandomSpecificPool(string wallName)
    {
        if (_poolList.Count == 0)
            GenerateSpecificPool(wallName, 1);

        return _poolList[Random.Range(0, _poolList.Count)];
    }

    public void SetPosition(ExploderPool pool, Vector3 position)
    {
        FragmentPool.Instance.transform.position = position;
        ResetFragmentsOriginal(pool);
    }

    public void ClearPool()
    {
        Clear();
    }

    public void GenerateSpecificPool(string wall, int count)
    {
        Clear();

        ExploderSingleton.Instance.FragmentPoolSize = 2 * ExploderSingleton.Instance.TargetFragments;
        Core.Instance.parameters.FragmentPoolSize = ExploderSingleton.Instance.FragmentPoolSize;
        FragmentPool.Instance.Reset(Core.Instance.parameters);

        _poolList = new List<ExploderPool>();

        var prefab = Resources.Load<GameObject>($"{Paths.ResourcesPrefabsBossesPath}{wall}");
        for (var i = 0; i < 1; i++)
        {
            var obj = Instantiate(prefab);
            obj.transform.localScale = new Vector3(0.85f, 0.85f, 0.85f);
            obj.SetActive(false);

            ExploderSingleton.Instance.CrackObject(obj);
            var crackedObject = Core.Instance.GetCrackedObject(obj);
            var exploderPool = new ExploderPool
            {
                ExpObject = obj,
                Fragments = FragmentPool.Instance.GetAllCrackedFragmentsByObject(crackedObject)
            };
            _poolList.Add(exploderPool);
        }
    }

    public void GenerateDefaultPool(int count)
    {
        Clear();

        ExploderSingleton.Instance.FragmentPoolSize = count * ExploderSingleton.Instance.TargetFragments;
        Core.Instance.parameters.FragmentPoolSize = ExploderSingleton.Instance.FragmentPoolSize;
        FragmentPool.Instance.Reset(Core.Instance.parameters);

        _poolList = new List<ExploderPool>();

        var prefab = Resources.Load<GameObject>($"{Paths.ResourcesPrefabsWallsPath}{DefaultPoolWall}");

        for (var i = 0; i < count; i++)
        {
            var obj = Instantiate(prefab);
            obj.transform.localScale = new Vector3(0.85f, 0.85f, 0.85f);
            obj.SetActive(false);
            ExploderSingleton.Instance.CrackObject(obj);
            var crackedObject = Core.Instance.GetCrackedObject(obj);
            var exploderPool = new ExploderPool
            {
                ExpObject = obj,
                Fragments = FragmentPool.Instance.GetAllCrackedFragmentsByObject(crackedObject)
            };
            _poolList.Add(exploderPool);
        }
    }
}
