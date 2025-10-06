using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameDashboard : MonoBehaviour
{
    [Header("UI References")]
    public UIManager uiManager;
    public GameManager gameManager;
    
    [Header("Moon Judgment UI")]
    public GameObject moonJudgmentPanel;
    public Button triggerMoonButton;
    public TextMeshProUGUI moonRoundsText;
    
    [Header("Celestial Phenomena UI")]
    public GameObject celestialPanel;
    public Button triggerPhenomenonButton;
    public TextMeshProUGUI thresholdText;
    
    [Header("Round Winner Selection")]
    public GameObject roundWinnerPanel;
    public Button bakunawaWinButton;
    public Button tiesmenWinButton;
    public Button tieRoundButton;
    
    [Header("Visual Feedback")]
    public Image moonPhaseImage;
    public Sprite[] moonPhases;
    public ParticleSystem backgroundStars;
    
    private void Start()
    {
        SetupDashboard();
    }
    
    private void SetupDashboard()
    {
        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();
        
        if (uiManager == null)
            uiManager = FindObjectOfType<UIManager>();
        
        SetupButtonListeners();
        UpdateMoonPhaseDisplay();
        UpdateThresholdDisplay();
        
        if (backgroundStars != null)
            backgroundStars.Play();
    }
    
    private void SetupButtonListeners()
    {
        if (triggerMoonButton != null)
            triggerMoonButton.onClick.AddListener(TriggerMoonJudgment);
        
        if (triggerPhenomenonButton != null)
            triggerPhenomenonButton.onClick.AddListener(TriggerPhenomenon);
        
        if (bakunawaWinButton != null)
            bakunawaWinButton.onClick.AddListener(() => SetLastRoundWinner(-1));
        
        if (tiesmenWinButton != null)
            tiesmenWinButton.onClick.AddListener(() => SetLastRoundWinner(1));
        
        if (tieRoundButton != null)
            tieRoundButton.onClick.AddListener(() => SetLastRoundWinner(0));
    }
    
    public void TriggerMoonJudgment()
    {
        if (gameManager != null && gameManager.moonsJudgment != null)
        {
            gameManager.moonsJudgment.TriggerMoonsJudgment();
            UpdateMoonPhaseDisplay();
        }
    }
    
    public void TriggerPhenomenon()
    {
        if (gameManager != null && gameManager.celestialPhenomena != null)
        {
            gameManager.celestialPhenomena.TriggerPhenomenon(gameManager.favorTracker);
        }
    }
    
    private void SetLastRoundWinner(int winner)
    {
        if (gameManager != null && gameManager.moonsJudgment != null)
        {
            gameManager.moonsJudgment.SetLastRoundWinner(winner);
            
            string winnerText = winner == -1 ? "Bakunawa" : winner == 1 ? "Tribesmen" : "Tie";
            Debug.Log($"Last round winner set to: {winnerText}");
        }
    }
    
    private void UpdateMoonPhaseDisplay()
    {
        if (moonPhaseImage != null && moonPhases.Length > 0)
        {
            int currentRound = gameManager != null ? gameManager.currentRound : 1;
            int phaseIndex = (currentRound - 1) % moonPhases.Length;
            moonPhaseImage.sprite = moonPhases[phaseIndex];
        }
    }
    
    private void UpdateThresholdDisplay()
    {
        if (thresholdText != null && gameManager != null)
        {
            int favor = gameManager.favorTracker;
            string nextThreshold = "";
            
            if (Mathf.Abs(favor) < 2)
                nextThreshold = "Next: ±2";
            else if (Mathf.Abs(favor) < 4)
                nextThreshold = "Next: ±4";
            else
                nextThreshold = "Max threshold reached";
            
            thresholdText.text = nextThreshold;
        }
    }
    
    public void UpdateDisplay()
    {
        UpdateMoonPhaseDisplay();
        UpdateThresholdDisplay();
        UpdateMoonRoundsText();
    }
    
    private void UpdateMoonRoundsText()
    {
        if (moonRoundsText != null && gameManager != null)
        {
            int currentRound = gameManager.currentRound;
            bool isMoonRound = currentRound == 3 || currentRound == 5 || currentRound == 7 || currentRound == 9;
            
            if (isMoonRound)
            {
                moonRoundsText.text = "MOON ROUND!";
                moonRoundsText.color = Color.cyan;
            }
            else
            {
                int nextMoonRound = 0;
                if (currentRound < 3) nextMoonRound = 3;
                else if (currentRound < 5) nextMoonRound = 5;
                else if (currentRound < 7) nextMoonRound = 7;
                else if (currentRound < 9) nextMoonRound = 9;
                
                if (nextMoonRound > 0)
                {
                    moonRoundsText.text = $"Next Moon: Round {nextMoonRound}";
                    moonRoundsText.color = Color.white;
                }
                else
                {
                    moonRoundsText.text = "No more Moon rounds";
                    moonRoundsText.color = Color.gray;
                }
            }
        }
    }
}