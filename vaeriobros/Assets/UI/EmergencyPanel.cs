using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmergencyPanel : MonoBehaviour
{
    void Start()
    {
        ToggleVisible(false);
    }

    public void OnUnplayableClicked()
    {
        GameManager.singleton.AbortLevel(EndReason.Unplayable);
    }

    public void OnBoringClicked()
    {
        GameManager.singleton.AbortLevel(EndReason.Boring);
    }

    internal void ToggleVisible(bool v)
    {
        gameObject.SetActive(v);
    }
}
