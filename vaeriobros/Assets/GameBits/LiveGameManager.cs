using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiveGameManager : GameManager
{
    [SerializeField] EmergencyPanel emergencyPanel;
    protected override void ProcessGameStart()
    {
        Time.timeScale = 1;
        state = GameState.Playing;
        emergencyPanel.ToggleVisible(true);
    }

    protected override void ProcessGameEnd(EndReason reason)
    {

        Time.timeScale = 0;
        state = GameState.Finished;
        // remember why we lost
        Debug.Log("Finished, reason=" + reason.ToString());
        // SHOW SURVEY HERE
        feedbackPanel.ResetFields();
        emergencyPanel.ToggleVisible(false);
        feedbackPanel.ToggleVisible(true,reason);

    }

    
}
