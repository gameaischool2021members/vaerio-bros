using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Model;
public class FeedbackPanel : MonoBehaviour
{
    [SerializeField] Slider enjoymentSlider;
    [SerializeField] Slider noveltySlider;
    [SerializeField] Slider desiredSlider;

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
        SurveyResults results = new SurveyResults();
        results.enjoyment = enjoymentSlider.value;
        results.ratedNovelty = noveltySlider.value;
        results.desiredNovelty = desiredSlider.value;

        GameManager.singleton.SendFinishedSurvey(results, false, false);
    }

    internal void ResetFields()
    {
        enjoymentSlider.value = Random.Range((int)enjoymentSlider.minValue, (int)(enjoymentSlider.maxValue + 1));
        noveltySlider.value = Random.Range((int)noveltySlider.minValue, (int)(noveltySlider.maxValue + 1));
    }
}
