using UnityEngine;
using TMPro;

public class DialoguePanelResizer : MonoBehaviour
{
    [Header("Riferimenti")]
    public RectTransform testoRect;
    public TextMeshProUGUI testoTMP;

    [Header("Impostazioni Layout")]
    public float larghezzaMaxTesto = 300f;
    public float paddingOrizzontale = 40f;
    public float paddingVerticale = 30f;

    private RectTransform pannelloRect;

    void Awake()
    {
        pannelloRect = GetComponent<RectTransform>();
    }

    public void AggiornaDimensioni()
    {
        float larghezzaTesto = larghezzaMaxTesto - paddingOrizzontale * 2f;

        // Forza il testo a quella larghezza fissa
        testoRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, larghezzaTesto);
        testoTMP.ForceMeshUpdate();

        // Legge altezza reale del testo con quella larghezza
        Vector2 dimTesto = testoTMP.GetRenderedValues(false);

        // Ridimensiona testo
        testoRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, larghezzaTesto);
        testoRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, dimTesto.y);

        // Box: larghezza SEMPRE fissa, altezza si adatta
        pannelloRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, larghezzaMaxTesto);
        pannelloRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, dimTesto.y + paddingVerticale * 2f);

        testoRect.anchoredPosition = Vector2.zero;
    }
}