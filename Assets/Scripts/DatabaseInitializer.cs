using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class DatabaseInitializer : MonoBehaviour
{
    [SerializeField, ListDrawerSettings(Expanded = true)] ScriptableObject[] databases;

    // void Awake()
    // {
    //     foreach (var database in databases)
    //     {
    //         print($"loaded: {database.name}");
    //     }
    // }
}