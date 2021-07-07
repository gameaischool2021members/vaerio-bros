using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmergencyPanel : MonoBehaviour
{

    public void OnUnplayableClicked()
    {
        GameManager.singleton.AbortLevel(EndReason.Unplayable);
    }

    public void OnBoringClicked()
    {
        GameManager.singleton.AbortLevel(EndReason.Boring);
    }
}
