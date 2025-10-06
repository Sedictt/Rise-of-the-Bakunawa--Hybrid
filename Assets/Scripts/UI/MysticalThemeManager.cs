using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class MysticalThemeManager : MonoBehaviour
{
    [Header("Background Elements")]
    public Image backgroundImage;
    public ParticleSystem starField;
    public ParticleSystem celestialMist;
    
    [Header("Moon Display")]
    public Image moonImage;
    public Transform moonTransform;
    public Sprite[] moonPhaseSprites;
    
    [Header("Color Scheme")]
    public Gradient nightSkyGradient;
    public Color glowColor = Color.cyan;
    public Color mysticalPurple = new Color(0.4f, 0.2f, 0.8f);
    
    [Header("Animation Settings")]
    public float moonRotationSpeed = 10f;
    public float glowPulseSpeed = 2f;
    public float starTwinkleInterval = 3f;
    
    [Header("UI Elements to Theme")]
    public Button[] buttonsToTheme;
    public Image[] panelsToTheme;
    public TextMeshProUGUI[] textsToTheme;
    
    private void Start()
    {
        ApplyMysticalTheme();
        StartBackgroundAnimations();
    }
    
    private void ApplyMysticalTheme()
    {
        // Apply theme to background
        if (backgroundImage != null)
        {
            backgroundImage.color = nightSkyGradient.Evaluate(0.5f);
        }
        
        // Theme buttons with mystical glow
        foreach (Button button in buttonsToTheme)
        {
            if (button != null)
            {
                ApplyButtonTheme(button);
            }
        }
        
        // Theme panels with translucent mystical colors
        foreach (Image panel in panelsToTheme)
        {
            if (panel != null)
            {
                ApplyPanelTheme(panel);
            }
        }
        
        // Theme text with glowing effects
        foreach (TextMeshProUGUI text in textsToTheme)
        {
            if (text != null)
            {
                ApplyTextTheme(text);
            }
        }
    }
    
    private void ApplyButtonTheme(Button button)
    {
        Image buttonImage = button.GetComponent<Image>();
        if (buttonImage != null)
        {
            buttonImage.color = new Color(mysticalPurple.r, mysticalPurple.g, mysticalPurple.b, 0.7f);
        }
        
        // Add glow effect
        Outline outline = button.GetComponent<Outline>();
        if (outline == null)
        {
            outline = button.gameObject.AddComponent<Outline>();
        }
        outline.effectColor = glowColor;
        outline.effectDistance = new Vector2(2, 2);
        
        // Add hover effects
        StartCoroutine(AnimateButtonGlow(button));
    }
    
    private void ApplyPanelTheme(Image panel)
    {
        panel.color = new Color(mysticalPurple.r, mysticalPurple.g, mysticalPurple.b, 0.4f);
        
        // Add subtle glow
        Shadow shadow = panel.GetComponent<Shadow>();
        if (shadow == null)
        {
            shadow = panel.gameObject.AddComponent<Shadow>();
        }
        shadow.effectColor = glowColor;
        shadow.effectDistance = new Vector2(1, 1);
    }
    
    private void ApplyTextTheme(TextMeshProUGUI text)
    {
        text.color = Color.white;
        
        // Add outline for better readability
        if (text.fontMaterial != null)
        {
            text.fontMaterial.SetFloat("_OutlineWidth", 0.1f);
            text.fontMaterial.SetColor("_OutlineColor", Color.black);
        }
    }
    
    private void StartBackgroundAnimations()
    {
        // Start moon rotation
        if (moonTransform != null)
        {
            StartCoroutine(RotateMoon());
        }
        
        // Start star twinkling
        if (starField != null)
        {
            StartCoroutine(TwinkleStars());
        }
        
        // Start mystical mist animation
        if (celestialMist != null)
        {
            celestialMist.Play();
        }
        
        // Start background color cycling
        StartCoroutine(CycleBackgroundColor());
    }
    
    private IEnumerator RotateMoon()
    {
        while (true)
        {
            if (moonTransform != null)
            {
                moonTransform.Rotate(0, 0, moonRotationSpeed * Time.deltaTime);
            }
            yield return null;
        }
    }
    
    private IEnumerator TwinkleStars()
    {
        while (true)
        {
            if (starField != null)
            {
                var emission = starField.emission;
                emission.rateOverTime = Random.Range(10f, 30f);
            }
            
            yield return new WaitForSeconds(starTwinkleInterval);
        }
    }
    
    private IEnumerator CycleBackgroundColor()
    {
        float time = 0f;
        
        while (true)
        {
            if (backgroundImage != null)
            {
                time += Time.deltaTime * 0.1f; // Slow color cycling
                Color newColor = nightSkyGradient.Evaluate(Mathf.PingPong(time, 1f));
                backgroundImage.color = newColor;
            }
            yield return null;
        }
    }
    
    private IEnumerator AnimateButtonGlow(Button button)
    {
        Outline outline = button.GetComponent<Outline>();
        if (outline == null) yield break;
        
        float time = 0f;
        Color originalColor = outline.effectColor;
        
        while (button != null)
        {
            time += Time.deltaTime * glowPulseSpeed;
            float alpha = Mathf.PingPong(time, 1f) * 0.5f + 0.5f;
            outline.effectColor = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }
    }
    
    public void UpdateMoonPhase(int round)
    {
        if (moonImage != null && moonPhaseSprites.Length > 0)
        {
            int phaseIndex = (round - 1) % moonPhaseSprites.Length;
            moonImage.sprite = moonPhaseSprites[phaseIndex];
        }
    }
    
    public void TriggerCelestialEvent()
    {
        StartCoroutine(CelestialEventAnimation());
    }
    
    private IEnumerator CelestialEventAnimation()
    {
        // Intensify particle effects
        if (celestialMist != null)
        {
            var emission = celestialMist.emission;
            float originalRate = emission.rateOverTime.constant;
            emission.rateOverTime = originalRate * 3f;
            
            yield return new WaitForSeconds(2f);
            
            emission.rateOverTime = originalRate;
        }
        
        // Flash background color
        if (backgroundImage != null)
        {
            Color originalColor = backgroundImage.color;
            backgroundImage.color = glowColor;
            
            yield return new WaitForSeconds(0.1f);
            
            backgroundImage.color = originalColor;
        }
    }
}