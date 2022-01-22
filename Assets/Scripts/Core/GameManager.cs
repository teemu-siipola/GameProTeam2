using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* - - - - - - - - - - - - - - - - - - - - - - *
*
*           ~ Game State singleton ~            
*
*   current debug functionality:
*   - add 5 pigs to score with "Up Arrow"
*   - remove 5 pigs from score with "Down Arrow"
*   - reset the Game State with "R"
*
* - - - - - - - - - - - - - - - - - - - - - - */

public class GameManager : ManagerInterface<GameManager>
{
    public float TimeToPlay = 30f;
    public uint PigsRequired = 20;

    public float TimeSincePlayerStarted;
    public float TimeRemaining;
    public uint PigsFenced;

    private string _debugName = "[GameManager] ";
    [SerializeField] private bool _isTimerRunning;

    void Start()
    {
        ResetGame(); // debug, temporarily start the game right away
    }

    void Update()
    {
        UpdateTimer();
        CheckEndCondition();

        ChangeStateOnKeyPress(); // debug, for testing the gamestate
    }

    public void ResetGame()
    {
        Debug.Log(_debugName + "Starting the Game.");
        PigsFenced = 0;
        ToggleTimer(true);
        ResetTimer();
    }

    public void ValidateAddPigs(uint amount) // call this when adding pigs to the fence
    {
        uint pigs = PigsFenced + amount;
        PigsFenced = pigs >= PigsRequired ? PigsRequired : pigs;
    }

    public void ValidateRemovePigs(uint amount) // call this when removing pigs from the fence
    {
        uint pigs = PigsFenced - amount;
        PigsFenced = pigs >= PigsRequired ? 0 : pigs;
    }

    private void UpdateTimer()
    {
        if (_isTimerRunning)
        {
            TimeSincePlayerStarted += Time.deltaTime;
            TimeRemaining = Mathf.Clamp(TimeToPlay - TimeSincePlayerStarted, 0, TimeToPlay);
        }
    }

    private void ToggleTimer(bool isEnabled)
    {
        _isTimerRunning = isEnabled;
    }

    private void ResetTimer()
    {
        TimeSincePlayerStarted = 0f;
    }

    private void CheckEndCondition()
    {
        if(!_isTimerRunning)
            return;

        if(PigsFenced >= PigsRequired)
            EndGame(true);
        if(TimeSincePlayerStarted >= TimeToPlay)
            EndGame(false);
    }

    private void EndGame(bool playerWon)
    {
        ToggleTimer(false);
        
        if(playerWon)
            WinGame();
        else
            FailGame();
    }

    private void WinGame()
    {
        // player was able to capture all pigs before time ran out
        Debug.LogFormat(_debugName
                        + "Player has won the game in {0} second(s)."
                        , Mathf.Round(TimeSincePlayerStarted));
    }

    private void FailGame()
    {
        // player ran out of time
        Debug.Log(_debugName + "Player has failed the game.");
    }

    /* - - - - - - - - - *
    *   DEBUG FUNCTIONS
    *   to be deleted
    *   when no longer
    *   needed
    * - - - - - - - - - */

    private void ChangeStateOnKeyPress()
    {
        if(Input.GetKeyDown(KeyCode.UpArrow))
            ValidateAddPigs(5);
        if(Input.GetKeyDown(KeyCode.DownArrow))
            ValidateRemovePigs(5);
        if(Input.GetKeyDown(KeyCode.R))
            ResetGame();
    }
}

// water is good. drink it.