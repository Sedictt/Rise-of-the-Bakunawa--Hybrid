using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [Header("Game State")]
    public int currentRound = 1;
    public int favorTracker = 0;
    public bool isGameActive = false;
    
    [Header("Round Data")]
    public DiceRollResult currentDiceResult;
    public Team lastRoundWinner;
    public List<RoundData> roundHistory;
    
    [Header("System References")]
    public PhaseManager phaseManager;
    public DiceSystem diceSystem;
    public MoonsJudgmentSystem moonsJudgment;
    public CelestialPhenomenaSystem celestialPhenomena;
    public UIManager uiManager;
    public AudioManager audioManager;
    
    [Header("Events")]
    public UnityEvent<int> OnRoundChanged;
    public UnityEvent<int> OnFavorChanged;
    public UnityEvent<Team> OnRoundWinner;
    public UnityEvent<Team> OnGameWinner;
    public UnityEvent<DiceRollResult> OnDiceRolled;
    
    public static GameManager Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        InitializeSystems();
    }
    
    private void InitializeSystems()
    {
        roundHistory = new List<RoundData>();
        
        if (phaseManager != null)
            phaseManager.Initialize(this);
        
        if (diceSystem != null)
            diceSystem.Initialize(audioManager);
        
        if (moonsJudgment != null)
            moonsJudgment.Initialize(this);
        
        if (celestialPhenomena != null)
            celestialPhenomena.Initialize(this);
        
        if (uiManager != null)
            uiManager.Initialize(this);
        
        OnRoundChanged.Invoke(currentRound);
        OnFavorChanged.Invoke(favorTracker);
    }
    
    public void StartGame()
    {
        isGameActive = true;
        currentRound = 1;
        favorTracker = 0;
        roundHistory.Clear();
        
        OnRoundChanged.Invoke(currentRound);
        OnFavorChanged.Invoke(favorTracker);
        
        if (phaseManager != null)
            phaseManager.StartNewRound();
        
        Debug.Log("=== GAME STARTED ===");
    }
    
    public void SetDiceResult(DiceRollResult result)
    {
        currentDiceResult = result;
        OnDiceRolled.Invoke(result);
        
        if (uiManager != null)
            uiManager.UpdateDiceDisplay(result);
    }
    
    public void SetRoundWinner(Team winner)
    {
        lastRoundWinner = winner;
        OnRoundWinner.Invoke(winner);
        
        // Move favor marker
        int favorChange = winner == Team.Bakunawa ? 1 : -1;
        AdjustFavor(favorChange);
        
        // Record round data
        RoundData roundData = new RoundData(currentRound);
        roundData.diceResult = currentDiceResult;
        roundData.roundWinner = winner;
        roundHistory.Add(roundData);
        
        // Update moon's judgment system with winner info
        if (moonsJudgment != null)
        {
            int winnerValue = winner == Team.Bakunawa ? 1 : -1;
            moonsJudgment.SetLastRoundWinner(winnerValue);
        }
        
        Debug.Log($"Round {currentRound} won by {winner}. Favor is now {favorTracker}");
        
        // Check for game end
        CheckGameEnd();
    }
    
    private void AdjustFavor(int amount)
    {
        int previousFavor = favorTracker;
        favorTracker = Mathf.Clamp(favorTracker + amount, -5, 5);
        
        if (favorTracker != previousFavor)
        {
            OnFavorChanged.Invoke(favorTracker);
            
            if (uiManager != null)
                uiManager.UpdateFavorDisplay(favorTracker);
            
            if (audioManager != null)
                audioManager.PlayFavorChangeSFX();
        }
    }
    
    public void ManualAdjustFavor(int amount)
    {
        AdjustFavor(amount);
    }
    
    public bool ShouldTriggerMoonJudgment()
    {
        return currentRound == 3 || currentRound == 5 || currentRound == 7 || currentRound == 9;
    }
    
    public bool ShouldTriggerCelestialPhenomena()
    {
        return Mathf.Abs(favorTracker) == 2 || Mathf.Abs(favorTracker) == 4;
    }
    
    private void CheckGameEnd()
    {
        if (currentRound >= 10)
        {
            if (favorTracker == 0)
            {
                if (currentRound == 10)
                {
                    Debug.Log("TIE! Proceeding to Round 11 for tiebreaker!");
                    return; // Continue to round 11
                }
                else
                {
                    // Round 11 ended in tie - could implement sudden death
                    Debug.Log("Game ended in tie after round 11!");
                    EndGame(Team.Bakunawa); // Default or implement tie logic
                }
            }
            else
            {
                // Determine winner based on favor position
                Team winner = DetermineGameWinner();
                EndGame(winner);
            }
        }
    }
    
    private Team DetermineGameWinner()
    {
        if (favorTracker >= 1)
        {
            return Team.Bakunawa; // Favor 1-5 = Bakunawa wins
        }
        else if (favorTracker <= -1)
        {
            return Team.Tribesmen; // Favor -1 to -5 = Tribesmen wins
        }
        else
        {
            return Team.Bakunawa; // Default (shouldn't happen based on logic)
        }
    }
    
    private void EndGame(Team winner)
    {
        isGameActive = false;
        OnGameWinner.Invoke(winner);
        
        string winnerText = winner == Team.Bakunawa ? "Bakunawa" : "Tribesmen";
        Debug.Log($"=== GAME OVER ===\n{winnerText} WINS!\nFinal Favor: {favorTracker}");
        
        if (uiManager != null)
            uiManager.ShowGameEndScreen(winner, favorTracker);
    }
    
    public void ResetGame()
    {
        isGameActive = false;
        currentRound = 1;
        favorTracker = 0;
        lastRoundWinner = Team.Bakunawa;
        currentDiceResult = null;
        roundHistory.Clear();
        
        if (celestialPhenomena != null)
            celestialPhenomena.ClearActivePhenomenon();
        
        if (phaseManager != null)
            phaseManager.currentPhase = GamePhase.RoundStart;
        
        OnRoundChanged.Invoke(currentRound);
        OnFavorChanged.Invoke(favorTracker);
        
        if (uiManager != null)
        {
            uiManager.UpdateRoundDisplay(currentRound);
            uiManager.UpdateFavorDisplay(favorTracker);
        }
        
        Debug.Log("Game Reset");
    }
    
    public void ManualTriggerMoonsJudgment()
    {
        if (moonsJudgment != null)
            moonsJudgment.TriggerMoonsJudgment();
    }
    
    public void ManualTriggerPhenomenon()
    {
        if (celestialPhenomena != null)
            celestialPhenomena.TriggerPhenomenon(favorTracker);
    }
    
    public void ForceNextPhase()
    {
        if (phaseManager != null)
            phaseManager.ForceNextPhase();
    }
    
    public GamePhase GetCurrentPhase()
    {
        return phaseManager != null ? phaseManager.currentPhase : GamePhase.RoundStart;
    }
    
    public RoundData GetCurrentRoundData()
    {
        if (roundHistory.Count > 0)
        {
            return roundHistory[roundHistory.Count - 1];
        }
        return new RoundData(currentRound);
    }
    
    public bool IsGameActive()
    {
        return isGameActive;
    }
}