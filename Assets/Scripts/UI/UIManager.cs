using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour
{
    [Header("Phase Display")]
    public TextMeshProUGUI phaseText;
    public TextMeshProUGUI timerText;
    public Image phaseProgressBar;
    
    [Header("Game State UI")]
    public TextMeshProUGUI roundText;
    public TextMeshProUGUI favorText;
    public Slider favorSlider;
    
    [Header("Dice UI")]
    public GameObject dicePanel;
    public TextMeshProUGUI bakunawaDiceText;
    public TextMeshProUGUI tribesmenDiceText;
    public TextMeshProUGUI initiativeWinnerText;
    
    [Header("Team Selection")]
    public GameObject roundWinnerPanel;
    public Button bakunawaWinButton;
    public Button tribesmenWinButton;
    
    [Header("Control Buttons")]
    public Button startGameButton;
    public Button nextPhaseButton;
    public Button resetGameButton;
    public Button favorUpButton;
    public Button favorDownButton;
    public Button triggerMoonButton;
    public Button triggerPhenomenonButton;
    
    [Header("Popup UI")]
    public GameObject effectPopup;
    public Image effectIcon;
    public TextMeshProUGUI effectTitle;
    public TextMeshProUGUI effectDescription;
    public Button closePopupButton;
    
    [Header("Game End UI")]
    public GameObject gameEndPanel;
    public TextMeshProUGUI gameEndTitle;
    public TextMeshProUGUI gameEndDetails;
    public Button playAgainButton;
    
    [Header("Card Clashing Effect")]
    public GameObject cardClashingEffect;
    public TextMeshProUGUI clashingText;
    
    [Header("Visual Effects")]
    public CanvasGroup popupCanvasGroup;
    public float fadeSpeed = 2f;
    
    private GameManager gameManager;
    private Coroutine popupCoroutine;
    private Coroutine timerCoroutine;
    
    public void Initialize(GameManager manager)
    {
        gameManager = manager;
        SetupButtonListeners();
        InitializeUIElements();
        
        // Subscribe to phase manager events
        if (gameManager.phaseManager != null)
        {
            gameManager.phaseManager.OnPhaseChanged.AddListener(UpdatePhaseDisplay);
            gameManager.phaseManager.OnTimerUpdate.AddListener(UpdateTimerDisplay);
            gameManager.phaseManager.OnCardClashingStart.AddListener(ShowCardClashingEffect);
        }
        
        // Subscribe to game events
        gameManager.OnDiceRolled.AddListener(UpdateDiceDisplay);
        gameManager.OnRoundWinner.AddListener(OnRoundWinner);
        gameManager.OnGameWinner.AddListener(ShowGameEndScreen);
    }
    
    private void SetupButtonListeners()
    {
        if (startGameButton != null)
            startGameButton.onClick.AddListener(() => gameManager.StartGame());
        
        if (nextPhaseButton != null)
            nextPhaseButton.onClick.AddListener(() => gameManager.ForceNextPhase());
        
        if (resetGameButton != null)
            resetGameButton.onClick.AddListener(() => gameManager.ResetGame());
        
        if (favorUpButton != null)
            favorUpButton.onClick.AddListener(() => gameManager.ManualAdjustFavor(1));
        
        if (favorDownButton != null)
            favorDownButton.onClick.AddListener(() => gameManager.ManualAdjustFavor(-1));
        
        if (triggerMoonButton != null)
            triggerMoonButton.onClick.AddListener(() => gameManager.ManualTriggerMoonsJudgment());
        
        if (triggerPhenomenonButton != null)
            triggerPhenomenonButton.onClick.AddListener(() => gameManager.ManualTriggerPhenomenon());
        
        if (bakunawaWinButton != null)
            bakunawaWinButton.onClick.AddListener(() => gameManager.SetRoundWinner(Team.Bakunawa));
        
        if (tribesmenWinButton != null)
            tribesmenWinButton.onClick.AddListener(() => gameManager.SetRoundWinner(Team.Tribesmen));
        
        if (closePopupButton != null)
            closePopupButton.onClick.AddListener(CloseEffectPopup);
        
        if (playAgainButton != null)
            playAgainButton.onClick.AddListener(() => {
                gameEndPanel.SetActive(false);
                gameManager.ResetGame();
            });
    }
    
    private void InitializeUIElements()
    {
        if (effectPopup != null)
            effectPopup.SetActive(false);
        
        if (gameEndPanel != null)
            gameEndPanel.SetActive(false);
        
        if (cardClashingEffect != null)
            cardClashingEffect.SetActive(false);
        
        if (roundWinnerPanel != null)
            roundWinnerPanel.SetActive(false);
        
        if (dicePanel != null)
            dicePanel.SetActive(false);
        
        UpdateUIElements();
    }
    
    public void UpdatePhaseDisplay(GamePhase phase)
    {
        if (phaseText != null)
        {
            string phaseString = GetPhaseDisplayText(phase);
            phaseText.text = phaseString;
            phaseText.color = GetPhaseColor(phase);
        }
        
        // Show/hide panels based on phase
        UpdatePanelVisibility(phase);
    }
    
    private string GetPhaseDisplayText(GamePhase phase)
    {
        switch (phase)
        {
            case GamePhase.RoundStart:
                return $"ROUND {gameManager.currentRound} START";
            case GamePhase.DiceRoll:
                return "ROLLING FOR INITIATIVE";
            case GamePhase.EventCheck:
                return "CHECKING EVENTS";
            case GamePhase.StrategyPhase:
                return "STRATEGY PHASE";
            case GamePhase.CardClashing:
                return "CARDS CLASHING!";
            case GamePhase.RoundResolution:
                return "ROUND RESOLUTION";
            case GamePhase.GameEnd:
                return "GAME OVER";
            default:
                return "UNKNOWN PHASE";
        }
    }
    
    private Color GetPhaseColor(GamePhase phase)
    {
        switch (phase)
        {
            case GamePhase.RoundStart:
                return Color.white;
            case GamePhase.DiceRoll:
                return Color.yellow;
            case GamePhase.EventCheck:
                return Color.cyan;
            case GamePhase.StrategyPhase:
                return Color.green;
            case GamePhase.CardClashing:
                return Color.red;
            case GamePhase.RoundResolution:
                return Color.blue;
            case GamePhase.GameEnd:
                return Color.magenta;
            default:
                return Color.white;
        }
    }
    
    private void UpdatePanelVisibility(GamePhase phase)
    {
        // Show dice panel during dice roll
        if (dicePanel != null)
            dicePanel.SetActive(phase == GamePhase.DiceRoll);
        
        // Show round winner panel during resolution
        if (roundWinnerPanel != null)
            roundWinnerPanel.SetActive(phase == GamePhase.RoundResolution);
        
        // Show card clashing effect
        if (cardClashingEffect != null)
            cardClashingEffect.SetActive(phase == GamePhase.CardClashing);
    }
    
    public void UpdateTimerDisplay(float remainingTime)
    {
        if (timerText != null)
        {
            if (remainingTime > 0)
            {
                int minutes = Mathf.FloorToInt(remainingTime / 60f);
                int seconds = Mathf.FloorToInt(remainingTime % 60f);
                timerText.text = $"{minutes:00}:{seconds:00}";
                
                // Change color as time runs out
                if (remainingTime <= 10f)
                    timerText.color = Color.red;
                else if (remainingTime <= 30f)
                    timerText.color = Color.yellow;
                else
                    timerText.color = Color.white;
            }
            else
            {
                timerText.text = "00:00";
            }
        }
    }
    
    public void UpdateDiceDisplay(DiceRollResult result)
    {
        if (bakunawaDiceText != null)
            bakunawaDiceText.text = result.bakunawaRoll.ToString();
        
        if (tribesmenDiceText != null)
            tribesmenDiceText.text = result.tribesmenRoll.ToString();
        
        if (initiativeWinnerText != null)
        {
            if (result.isTie)
            {
                initiativeWinnerText.text = "TIE!";
                initiativeWinnerText.color = Color.yellow;
            }
            else
            {
                string winner = result.winner == Team.Bakunawa ? "BAKUNAWA" : "TRIBESMEN";
                initiativeWinnerText.text = $"{winner} WINS INITIATIVE!";
                initiativeWinnerText.color = result.winner == Team.Bakunawa ? Color.blue : Color.red;
            }
        }
    }
    
    public void UpdateRoundDisplay(int round)
    {
        if (roundText != null)
            roundText.text = $"Round {round}";
    }
    
    public void UpdateFavorDisplay(int favor)
    {
        if (favorText != null)
            favorText.text = $"Favor: {favor}";
        
        if (favorSlider != null)
        {
            favorSlider.value = favor + 5; // Convert -5 to +5 range to 0 to 10
            
            // Update slider color based on favor
            Image fillImage = favorSlider.fillRect.GetComponent<Image>();
            if (fillImage != null)
            {
                if (favor > 0)
                    fillImage.color = Color.Lerp(Color.white, Color.blue, favor / 5f);
                else if (favor < 0)
                    fillImage.color = Color.Lerp(Color.white, Color.red, Mathf.Abs(favor) / 5f);
                else
                    fillImage.color = Color.white;
            }
        }
    }
    
    private void OnRoundWinner(Team winner)
    {
        string winnerText = winner == Team.Bakunawa ? "Bakunawa" : "Tribesmen";
        Debug.Log($"UI: Round won by {winnerText}");
        
        // Hide round winner panel after selection
        if (roundWinnerPanel != null)
            roundWinnerPanel.SetActive(false);
    }
    
    public void ShowGameEndScreen(Team winner, int finalFavor)
    {
        if (gameEndPanel != null)
        {
            gameEndPanel.SetActive(true);
            
            string winnerText = winner == Team.Bakunawa ? "BAKUNAWA" : "TRIBESMEN";
            
            if (gameEndTitle != null)
                gameEndTitle.text = $"{winnerText} WINS!";
            
            if (gameEndDetails != null)
            {
                string favorSide = finalFavor > 0 ? "Bakunawa" : "Tribesmen";
                gameEndDetails.text = $"Final Favor: {finalFavor}\nAfter Round {gameManager.currentRound}";
            }
        }
    }
    
    private void ShowCardClashingEffect()
    {
        if (cardClashingEffect != null)
        {
            cardClashingEffect.SetActive(true);
            
            if (clashingText != null)
            {
                StartCoroutine(AnimateClashingText());
            }
        }
    }
    
    private IEnumerator AnimateClashingText()
    {
        if (clashingText == null) yield break;
        
        string[] messages = { "CLASH!", "CARDS COLLIDE!", "BATTLE!", "CLASH!" };
        
        for (int i = 0; i < messages.Length; i++)
        {
            clashingText.text = messages[i];
            clashingText.transform.localScale = Vector3.one * 1.5f;
            
            // Scale down animation
            float timer = 0f;
            while (timer < 0.5f)
            {
                timer += Time.deltaTime;
                float scale = Mathf.Lerp(1.5f, 1f, timer / 0.5f);
                clashingText.transform.localScale = Vector3.one * scale;
                yield return null;
            }
            
            yield return new WaitForSeconds(0.5f);
        }
    }
    
    public void ShowMoonEffectPopup(MoonEffect effect)
    {
        if (effect != null)
        {
            ShowEffectPopup(effect.effectName, effect.description, effect.icon);
        }
    }
    
    public void ShowPhenomenonPopup(PhenomenonEffect phenomenon)
    {
        if (phenomenon != null)
        {
            ShowEffectPopup(phenomenon.effectName, phenomenon.description, phenomenon.icon);
        }
    }
    
    private void ShowEffectPopup(string title, string description, Sprite icon)
    {
        if (effectPopup != null)
        {
            effectPopup.SetActive(true);
            
            if (effectTitle != null)
                effectTitle.text = title;
            
            if (effectDescription != null)
                effectDescription.text = description;
            
            if (effectIcon != null && icon != null)
                effectIcon.sprite = icon;
            
            // Start fade in animation
            if (popupCanvasGroup != null)
            {
                if (popupCoroutine != null)
                    StopCoroutine(popupCoroutine);
                
                popupCoroutine = StartCoroutine(FadePopup(true));
            }
        }
    }
    
    public void CloseEffectPopup()
    {
        if (popupCanvasGroup != null)
        {
            if (popupCoroutine != null)
                StopCoroutine(popupCoroutine);
            
            popupCoroutine = StartCoroutine(FadePopup(false));
        }
        else if (effectPopup != null)
        {
            effectPopup.SetActive(false);
        }
    }
    
    private IEnumerator FadePopup(bool fadeIn)
    {
        float startAlpha = fadeIn ? 0f : 1f;
        float targetAlpha = fadeIn ? 1f : 0f;
        float timer = 0f;
        
        popupCanvasGroup.alpha = startAlpha;
        
        while (timer < 1f)
        {
            timer += Time.deltaTime * fadeSpeed;
            popupCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, timer);
            yield return null;
        }
        
        popupCanvasGroup.alpha = targetAlpha;
        
        if (!fadeIn && effectPopup != null)
        {
            effectPopup.SetActive(false);
        }
    }
    
    public void UpdatePhenomenonCountdown(int remaining)
    {
        // Implementation for phenomenon countdown display
    }
    
    public void ShowPhenomenonEndMessage()
    {
        StartCoroutine(ShowTemporaryMessage("The heavens calm..."));
    }
    
    private IEnumerator ShowTemporaryMessage(string message)
    {
        ShowEffectPopup("Celestial Calm", message, null);
        yield return new WaitForSeconds(2f);
        CloseEffectPopup();
    }
    
    private void UpdateUIElements()
    {
        if (gameManager != null)
        {
            UpdateRoundDisplay(gameManager.currentRound);
            UpdateFavorDisplay(gameManager.favorTracker);
        }
    }
}