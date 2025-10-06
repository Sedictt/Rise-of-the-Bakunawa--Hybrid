using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class PhaseManager : MonoBehaviour
{
    [Header("Phase Settings")]
    public float strategyPhaseDuration = 90f; // 1.5 minutes
    public float cardClashingDuration = 5f;
    public float phaseTransitionDelay = 2f;
    
    [Header("Current State")]
    public GamePhase currentPhase = GamePhase.RoundStart;
    public float currentPhaseTimer;
    public bool isPhaseActive = true;
    
    [Header("Events")]
    public UnityEvent<GamePhase> OnPhaseChanged;
    public UnityEvent<float> OnTimerUpdate;
    public UnityEvent OnStrategyPhaseStart;
    public UnityEvent OnStrategyPhaseEnd;
    public UnityEvent OnCardClashingStart;
    public UnityEvent OnRoundComplete;
    
    private GameManager gameManager;
    private Coroutine phaseCoroutine;
    
    public void Initialize(GameManager manager)
    {
        gameManager = manager;
        currentPhase = GamePhase.RoundStart;
        isPhaseActive = false;
    }
    
    public void StartNewRound()
    {
        if (phaseCoroutine != null)
        {
            StopCoroutine(phaseCoroutine);
        }
        
        phaseCoroutine = StartCoroutine(RunRoundPhases());
    }
    
    private IEnumerator RunRoundPhases()
    {
        // Phase 1: Round Start
        yield return StartCoroutine(ExecutePhase(GamePhase.RoundStart, 0f));
        
        // Phase 2: Dice Roll
        yield return StartCoroutine(ExecutePhase(GamePhase.DiceRoll, 0f));
        
        // Phase 3: Event Check
        yield return StartCoroutine(ExecutePhase(GamePhase.EventCheck, 0f));
        
        // Phase 4: Strategy Phase (1.5 minutes)
        yield return StartCoroutine(ExecutePhase(GamePhase.StrategyPhase, strategyPhaseDuration));
        
        // Phase 5: Card Clashing
        yield return StartCoroutine(ExecutePhase(GamePhase.CardClashing, cardClashingDuration));
        
        // Phase 6: Round Resolution
        yield return StartCoroutine(ExecutePhase(GamePhase.RoundResolution, 0f));
        
        // Check if game should end
        if (ShouldGameEnd())
        {
            SetPhase(GamePhase.GameEnd);
        }
        else
        {
            // Prepare for next round
            yield return new WaitForSeconds(phaseTransitionDelay);
            gameManager.currentRound++;
            StartNewRound();
        }
    }
    
    private IEnumerator ExecutePhase(GamePhase phase, float duration)
    {
        SetPhase(phase);
        
        // Execute phase-specific logic
        switch (phase)
        {
            case GamePhase.RoundStart:
                yield return ExecuteRoundStart();
                break;
                
            case GamePhase.DiceRoll:
                yield return ExecuteDiceRoll();
                break;
                
            case GamePhase.EventCheck:
                yield return ExecuteEventCheck();
                break;
                
            case GamePhase.StrategyPhase:
                yield return ExecuteStrategyPhase(duration);
                break;
                
            case GamePhase.CardClashing:
                yield return ExecuteCardClashing(duration);
                break;
                
            case GamePhase.RoundResolution:
                yield return ExecuteRoundResolution();
                break;
        }
        
        yield return new WaitForSeconds(phaseTransitionDelay);
    }
    
    private void SetPhase(GamePhase newPhase)
    {
        currentPhase = newPhase;
        currentPhaseTimer = 0f;
        isPhaseActive = true;
        
        OnPhaseChanged.Invoke(currentPhase);
        Debug.Log($"Phase changed to: {currentPhase}");
    }
    
    private IEnumerator ExecuteRoundStart()
    {
        Debug.Log($"=== ROUND {gameManager.currentRound} START ===");
        yield return new WaitForSeconds(1f);
    }
    
    private IEnumerator ExecuteDiceRoll()
    {
        Debug.Log("Rolling dice for initiative...");
        
        if (gameManager.diceSystem != null)
        {
            DiceRollResult result = gameManager.diceSystem.RollForInitiative();
            gameManager.SetDiceResult(result);
            
            string message = result.isTie ? 
                $"Tie! Both rolled {result.bakunawaRoll}" :
                $"{result.winner} wins initiative! ({result.bakunawaRoll} vs {result.tribesmenRoll})";
            
            Debug.Log(message);
        }
        
        yield return new WaitForSeconds(2f);
    }
    
    private IEnumerator ExecuteEventCheck()
    {
        Debug.Log("Checking for events...");
        
        bool eventTriggered = false;
        
        // Check Moon's Judgment
        if (gameManager.ShouldTriggerMoonJudgment())
        {
            gameManager.moonsJudgment.TriggerMoonsJudgment();
            eventTriggered = true;
            yield return new WaitForSeconds(3f);
        }
        
        // Check Celestial Phenomena
        if (gameManager.ShouldTriggerCelestialPhenomena())
        {
            gameManager.celestialPhenomena.TriggerPhenomenon(gameManager.favorTracker);
            eventTriggered = true;
            yield return new WaitForSeconds(3f);
        }
        
        if (!eventTriggered)
        {
            Debug.Log("No events triggered this round.");
            yield return new WaitForSeconds(1f);
        }
    }
    
    private IEnumerator ExecuteStrategyPhase(float duration)
    {
        Debug.Log($"Strategy Phase: {duration} seconds to plan your moves!");
        OnStrategyPhaseStart.Invoke();
        
        currentPhaseTimer = duration;
        
        while (currentPhaseTimer > 0f)
        {
            currentPhaseTimer -= Time.deltaTime;
            OnTimerUpdate.Invoke(currentPhaseTimer);
            yield return null;
        }
        
        OnStrategyPhaseEnd.Invoke();
        Debug.Log("Strategy Phase ended! Lock in your cards!");
    }
    
    private IEnumerator ExecuteCardClashing(float duration)
    {
        Debug.Log("CARDS ARE CLASHING!");
        OnCardClashingStart.Invoke();
        
        currentPhaseTimer = duration;
        
        while (currentPhaseTimer > 0f)
        {
            currentPhaseTimer -= Time.deltaTime;
            yield return null;
        }
        
        Debug.Log("Card clash resolved!");
    }
    
    private IEnumerator ExecuteRoundResolution()
    {
        Debug.Log("Resolving round...");
        
        // This would typically be set by player input or game logic
        // For now, we'll simulate it
        Team roundWinner = SimulateRoundWinner();
        
        gameManager.SetRoundWinner(roundWinner);
        
        string winnerText = roundWinner == Team.Bakunawa ? "Bakunawa" : "Tribesmen";
        Debug.Log($"Round {gameManager.currentRound} winner: {winnerText}");
        
        OnRoundComplete.Invoke();
        
        yield return new WaitForSeconds(2f);
    }
    
    private Team SimulateRoundWinner()
    {
        // This is just for testing - in real game, this would be determined by actual card play
        return Random.Range(0, 2) == 0 ? Team.Bakunawa : Team.Tribesmen;
    }
    
    private bool ShouldGameEnd()
    {
        if (gameManager.currentRound >= 10)
        {
            if (gameManager.favorTracker == 0)
            {
                // Tie breaker round
                return gameManager.currentRound >= 11;
            }
            return true;
        }
        return false;
    }
    
    public void ForceNextPhase()
    {
        if (phaseCoroutine != null)
        {
            StopCoroutine(phaseCoroutine);
        }
        
        // Skip current phase timer
        currentPhaseTimer = 0f;
        isPhaseActive = false;
    }
    
    public void PausePhase()
    {
        isPhaseActive = false;
        if (phaseCoroutine != null)
        {
            StopCoroutine(phaseCoroutine);
        }
    }
    
    public void ResumePhase()
    {
        isPhaseActive = true;
        // Resume logic would go here
    }
}