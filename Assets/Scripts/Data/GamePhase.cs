using UnityEngine;

public enum GamePhase
{
    RoundStart,
    DiceRoll,
    EventCheck,
    StrategyPhase,
    CardClashing,
    RoundResolution,
    GameEnd
}

public enum Team
{
    Bakunawa,
    Tribesmen
}

[System.Serializable]
public class DiceRollResult
{
    public int bakunawaRoll;
    public int tribesmenRoll;
    public Team winner;
    public bool isTie;
    
    public DiceRollResult(int bakunawa, int tribesmen)
    {
        bakunawaRoll = bakunawa;
        tribesmenRoll = tribesmen;
        
        if (bakunawa > tribesmen)
        {
            winner = Team.Bakunawa;
            isTie = false;
        }
        else if (tribesmen > bakunawa)
        {
            winner = Team.Tribesmen;
            isTie = false;
        }
        else
        {
            winner = Team.Bakunawa; // Default for tie
            isTie = true;
        }
    }
}

[System.Serializable]
public class RoundData
{
    public int roundNumber;
    public DiceRollResult diceResult;
    public Team roundWinner;
    public bool moonJudgmentTriggered;
    public bool phenomenonTriggered;
    public string activeMoonEffect;
    public string activePhenomenon;
    
    public RoundData(int round)
    {
        roundNumber = round;
        roundWinner = Team.Bakunawa;
        moonJudgmentTriggered = false;
        phenomenonTriggered = false;
        activeMoonEffect = "";
        activePhenomenon = "";
    }
}