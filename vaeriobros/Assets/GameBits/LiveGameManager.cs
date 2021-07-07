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

    protected override void ProcessGameEnd()
    {

        Time.timeScale = 0;
        state = GameState.Finished;

        // SHOW SURVEY HERE
        feedbackPanel.ResetFields();
        feedbackPanel.ToggleVisible(true);
    }
}
