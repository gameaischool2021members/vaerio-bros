using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestResetButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ToggleVisibility(false);
    }

    
    public void ToggleVisibility(bool visible)
    {
        gameObject.SetActive(visible);
    }
}
