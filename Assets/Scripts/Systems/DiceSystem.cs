using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class DiceSystem : MonoBehaviour
{
    [Header("Dice Settings")]
    public int diceMin = 1;
    public int diceMax = 6;
    public float rollAnimationDuration = 2f;
    
    [Header("Events")]
    public UnityEvent<DiceRollResult> OnDiceRolled;
    public UnityEvent<int, int> OnDiceRolling; // For animation updates
    
    [Header("Audio")]
    public AudioClip diceRollSFX;
    
    private AudioManager audioManager;
    private bool isRolling = false;
    
    public void Initialize(AudioManager audio)
    {
        audioManager = audio;
    }
    
    public DiceRollResult RollForInitiative()
    {
        if (isRolling) return null;
        
        StartCoroutine(AnimatedDiceRoll());
        
        int bakunawaRoll = Random.Range(diceMin, diceMax + 1);
        int tribesmenRoll = Random.Range(diceMin, diceMax + 1);
        
        DiceRollResult result = new DiceRollResult(bakunawaRoll, tribesmenRoll);
        
        return result;
    }
    
    private IEnumerator AnimatedDiceRoll()
    {
        isRolling = true;
        
        if (audioManager != null && diceRollSFX != null)
        {
            audioManager.PlayCustomSFX(diceRollSFX);
        }
        
        float timer = 0f;
        
        // Animate dice rolling
        while (timer < rollAnimationDuration)
        {
            timer += Time.deltaTime;
            
            // Generate random numbers for animation
            int animBakunawa = Random.Range(diceMin, diceMax + 1);
            int animTribesmen = Random.Range(diceMin, diceMax + 1);
            
            OnDiceRolling.Invoke(animBakunawa, animTribesmen);
            
            yield return new WaitForSeconds(0.1f);
        }
        
        isRolling = false;
    }
    
    public DiceRollResult RollSingleDie()
    {
        int roll = Random.Range(diceMin, diceMax + 1);
        return new DiceRollResult(roll, 0);
    }
    
    public bool IsRolling()
    {
        return isRolling;
    }
}