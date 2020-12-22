using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyGroupsSpawner : MonoBehaviour
{
    [SerializeField] EnemyGroup[] groupSources;

    [SerializeField] MinMaxFloat intervalAt0s;
    [SerializeField] MinMaxFloat intervalAt100s;

    [SerializeField] int spawnSourcesBandWidth;
    [SerializeField] int WhenSpawnSourcesBandStartToSlide;
    [SerializeField] int WhenSpawnSourcesBandEndSliding;

    IEnumerator currentSpawns;

    [Serializable]
    struct MinMaxFloat
    {
        public float min;
        public float max;
    }

    public void StartSpawn()
    {
        foreach (Transform childGroup in transform)
        {
            Destroy(childGroup.gameObject);
        }
        
        currentSpawns = Spawn(groupSources);
        StartCoroutine(currentSpawns);
    }

    IEnumerator Spawn(EnemyGroup[] spawns)
    {
        float timeAtStart = Time.time;
        
        yield return new WaitForSeconds(5);
        
        while(true)
        {
            float time = Time.time - timeAtStart;
            
            float timeNormalized = Mathf.InverseLerp
            (
                WhenSpawnSourcesBandStartToSlide,
                WhenSpawnSourcesBandEndSliding,
                time
            );
            int indexMin = (int) Mathf.Lerp(0, spawns.Length - spawnSourcesBandWidth, timeNormalized);
            int indexMax = indexMin + spawnSourcesBandWidth - 1;
            int nextIndex = Random.Range(indexMin, indexMax + 1);
            Instantiate(spawns[nextIndex], transform);

            float intervalMin = Mathf.Clamp(Mathf.Lerp(intervalAt0s.min, intervalAt100s.min, time / 100), 3, 100);
            float intervalMax = Mathf.Clamp(Mathf.Lerp(intervalAt0s.max, intervalAt100s.max, time / 100), 3, 100);
            float nextInterval = Random.Range(intervalMin, intervalMax);
            // 場に敵が全くいなくなったら時間を待たずに次を出したい気持ちがある
            yield return new WaitForSeconds(nextInterval);
        }
    }

    public void StopSpawn()
    {
        StopCoroutine(currentSpawns);
    }
}