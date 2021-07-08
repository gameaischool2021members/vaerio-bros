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
    [SerializeField] Toggle impossibleToggle;

    private EndReason endReason = EndReason.Death;

    public void ToggleVisible(bool visible, EndReason reason)
    {
        gameObject.SetActive(visible);
        impossibleToggle.isOn = (reason == EndReason.Unplayable);
        impossibleToggle.interactable = (reason == EndReason.Death);
        headerText.text = "RESULT: " + ReasonToDescription(reason);
        endReason = reason;
    }

    public void OnSubmitClicked()
    {
        SurveyResults results = new SurveyResults();
        results.enjoyment = enjoymentSlider.value;
        results.ratedNovelty = noveltySlider.value;
        results.desiredNovelty = desiredSlider.value;

        if (impossibleToggle.isOn)
        {
            endReason = EndReason.Unplayable;
        }

        GameManager.singleton.SendFinishedSurvey(results,endReason);
    }

    internal void ResetFields()
    {
        enjoymentSlider.value = 3;
        noveltySlider.value = 3;
        desiredSlider.value = 3;
        
    }

    private string ReasonToDescription(EndReason reason)
    {
        switch (reason)
        {
            case EndReason.Win:
                return "You Win!";
            case EndReason.Death:
                return "You Died!";
            case EndReason.Unplayable:
                return "Level is Unplayable.";
            case EndReason.Boring:
                return "Level is Boring.";
        }
        return "Uh... Oops?";
    }
}
