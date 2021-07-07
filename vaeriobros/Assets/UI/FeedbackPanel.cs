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
    [SerializeField] Text headerText;

    void Start()
    {
        ToggleVisible(false,"");
    }

    public void ToggleVisible(bool visible, string header)
    {
        gameObject.SetActive(visible);
        headerText.text = "RESULT: " + header;
    }

    public void OnSubmitClicked()
    {
        SurveyResults results = new SurveyResults();
        results.enjoyment = enjoymentSlider.value;
        results.ratedNovelty = noveltySlider.value;
        results.desiredNovelty = desiredSlider.value;

        GameManager.singleton.SendFinishedSurvey(results);
    }

    internal void ResetFields()
    {
        enjoymentSlider.value = 3;
        noveltySlider.value = 3;
        desiredSlider.value = 3;
    }
}
