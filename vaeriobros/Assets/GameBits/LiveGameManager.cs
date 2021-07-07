using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiveGameManager : GameManager
{

    protected override void ProcessGameStart()
    {
        Time.timeScale = 1;
        state = GameState.Playing;
    }

    protected override void ProcessGameEnd(EndReason reason)
    {

        Time.timeScale = 0;
        state = GameState.Finished;
        // remember why we lost
        endReason = reason;
        Debug.Log("Finished, reason=" + endReason.ToString());
        // SHOW SURVEY HERE
        feedbackPanel.ResetFields();
        var desc = ReasonToDescription(reason);
        feedbackPanel.ToggleVisible(true,desc);
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
