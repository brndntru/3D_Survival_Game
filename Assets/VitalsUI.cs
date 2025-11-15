using UnityEngine;
using UnityEngine.UI;

public class VitalsUI : MonoBehaviour
{
    public PlayerVitals vitals;
    public Slider healthSlider;
    public Slider freezeSlider;
    public Image healthFill;
    public Image freezeFill;
    public Text healthText;
    public Text freezeText;

    float lastH = -1f, lastF = -1f;

    void Start()
    {
        if (!vitals) vitals = FindObjectOfType<PlayerVitals>();
        if (healthSlider) { healthSlider.minValue = 0; healthSlider.maxValue = 1; }
        if (freezeSlider) { freezeSlider.minValue = 0; freezeSlider.maxValue = 1; }
        Refresh(force: true);
    }

    void Update() => Refresh();

    void Refresh(bool force = false)
    {
        if (!vitals) return;
        float h = vitals.Health01;
        float f = vitals.Freeze01;

        if (force || Mathf.Abs(h - lastH) > 0.0001f)
        {
            if (healthSlider) healthSlider.value = h;
            if (healthFill) healthFill.color = Color.Lerp(Color.red, Color.green, h);
            if (healthText) healthText.text = Mathf.RoundToInt(h * 100f).ToString();
            lastH = h;
        }
        if (force || Mathf.Abs(f - lastF) > 0.0001f)
        {
            if (freezeSlider) freezeSlider.value = f;
            if (freezeFill) freezeFill.color = Color.Lerp(new Color(0.85f, 0.92f, 1f), new Color(0.2f, 0.5f, 1f), f);
            if (freezeText) freezeText.text = Mathf.RoundToInt(f * 100f).ToString();
            lastF = f;
        }
    }
}
