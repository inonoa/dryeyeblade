using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using DG.Tweening;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyGroupsSpawner : MonoBehaviour, IDoOnTimeStopped
{
    [SerializeField] EnemyGroup[] groupSources;

    [SerializeField] MinMaxFloat intervalAt0s;
    [SerializeField] MinMaxFloat intervalAt100s;

    [SerializeField] int spawnSourcesBandWidth;
    [SerializeField] int WhenSpawnSourcesBandStartToSlide;
    [SerializeField] int WhenSpawnSourcesBandEndSliding;
    
#if UNITY_EDITOR
    [SerializeField] bool debugSpawns = true;
#endif

    [SerializeField, ReadOnly] int indexMin;
    [SerializeField, ReadOnly] int indexMax;

    [SerializeField] int numInitialSpawns;
    [SerializeField] int initialSpawnsIndexMin;
    [SerializeField] int initialSpawnsIndexMax;

    IEnumerator currentSpawns;
    
    Subject<EnemyGroup> _Spawned = new Subject<EnemyGroup>();
    public IObservable<EnemyGroup> Spawned => _Spawned;

    [Serializable]
    struct MinMaxFloat
    {
        [HorizontalGroup] public float min;
        [HorizontalGroup] public float max;
    }

    public void StartSpawn()
    {
        foreach (Transform childGroup in transform)
        {
            Destroy(childGroup.gameObject);
        }
#if UNITY_EDITOR
        if (debugSpawns)
#endif
        {
            InitialSpawns();
        }
        
        currentSpawns = Spawn(groupSources);
        StartCoroutine(currentSpawns);
    }

    void InitialSpawns()
    {
        foreach (var _ in Enumerable.Range(0, numInitialSpawns))
        {
            int index = Random.Range(initialSpawnsIndexMin, initialSpawnsIndexMax + 1);
            _Spawned.OnNext(Instantiate(groupSources[index], transform));
        }
    }

    IEnumerator Spawn(EnemyGroup[] spawns)
    {
        indexMin = 0;
        indexMax = 0;
        
        float time = 0;

        while ((time += Time.deltaTime) < 5) yield return null;
        
        while(true)
        {
            
#if UNITY_EDITOR
            if (debugSpawns)
#endif
            {
                float timeNormalized = Mathf.InverseLerp
                (
                    WhenSpawnSourcesBandStartToSlide,
                    WhenSpawnSourcesBandEndSliding,
                    time
                );
                indexMin = (int) Mathf.Lerp(0, spawns.Length - spawnSourcesBandWidth, timeNormalized);
                indexMax = indexMin + spawnSourcesBandWidth - 1;
                int nextIndex = Random.Range(indexMin, indexMax + 1);
                _Spawned.OnNext(Instantiate(spawns[nextIndex], transform));
            }

            float intervalMin = Mathf.Clamp(Mathf.LerpUnclamped(intervalAt0s.min, intervalAt100s.min, time / 100), 3, 100);
            float intervalMax = Mathf.Clamp(Mathf.LerpUnclamped(intervalAt0s.max, intervalAt100s.max, time / 100), intervalMin + 0.5f, 100);
            float nextInterval = Random.Range(intervalMin, intervalMax);
            
            // 場に敵が全くいなくなったら時間を待たずに次を出したい気持ちがある
            float t = 0;
            while ((t += Time.deltaTime) < nextInterval) yield return null;
            time += nextInterval;
        }
    }

    public void StopSpawn()
    {
        StopCoroutine(currentSpawns);
        currentSpawns = null;
    }

    public void OnTimeStopped()
    {
        if(currentSpawns == null) return;
        StopCoroutine(currentSpawns);
    }

    public void OnTimeRestarted()
    {
        if(currentSpawns == null) return;
        StartCoroutine(currentSpawns);
    }
}