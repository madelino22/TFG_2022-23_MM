using UnityEngine;
using UnityEngine.UI;

public class UI_HealthBar : MonoBehaviour
{
    [SerializeField]
    private Gradient _gradient = null;

    [SerializeField]
    private Image _bar = null;
    [SerializeField]
    private Text _text = null;

    public void UpdateLife(int hp, int totalHp)
    {
        float f = (float)hp / (float)totalHp * 0.35f;
        _bar.fillAmount = f;
        Color c = _gradient.Evaluate(f/0.35f);
        _bar.color = c;
        _text.text = hp.ToString();
    }
}