using System;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSound : MonoBehaviour
{
    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            AudioSource.PlayClipAtPoint(SoundDatabase.Instance.buttonPushed, Camera.main.transform.position);
        });
    }
}