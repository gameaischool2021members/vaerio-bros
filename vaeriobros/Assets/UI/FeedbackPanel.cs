using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FeedbackPanel : MonoBehaviour
{
    [SerializeField] Slider enjoymentSlider;
    [SerializeField] Slider noveltySlider;

    void Start()
    {
        ToggleVisible(false);
    }

    public void ToggleVisible(bool visible)
    {
        gameObject.SetActive(visible);
    }

    public void OnSubmitClicked()
    {
        Debug.Log(GameManager.singleton);
        GameManager.singleton.OnRestartButtonClicked();
    }

    internal void ResetFields()
    {
        enjoymentSlider.value = Random.Range((int)enjoymentSlider.minValue, (int)(enjoymentSlider.maxValue + 1));
        noveltySlider.value = Random.Range((int)noveltySlider.minValue, (int)(noveltySlider.maxValue + 1));
    }
}
