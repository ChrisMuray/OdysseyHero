using UnityEngine;
using UnityEngine.UI;

public class BarUI : MonoBehaviour
{
    [SerializeField] private Image m_fillImage;

    public void SetFillAmount(float proportion)
    {
        m_fillImage.fillAmount = proportion;
    }

    public void SetFillColor(Color color)
    {
        m_fillImage.color = color;
    }
}