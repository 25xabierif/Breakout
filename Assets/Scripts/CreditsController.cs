using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsController : MonoBehaviour
{
    [Header("References")]
    public RectTransform scrollingPanel;  // Arrastra aquí tu ScrollingPanel
    public GameObject endPanel;           // Arrastra aquí tu EndPanel (desactivado al inicio)

    [Tooltip("CanvasGroup del texto 'THANK YOU FOR PLAYING' (dentro de EndPanel).")]
    public CanvasGroup thankYouGroup;

    [Tooltip("CanvasGroup del texto inferior izquierda (Space).")]
    public CanvasGroup pressSpaceGroup;

    [Tooltip("CanvasGroup del texto inferior derecha (Esc).")]
    public CanvasGroup pressEscGroup;

    [Header("Scroll Settings")]
    [Tooltip("Velocidad de scroll en unidades UI (pixeles) por segundo.")]
    public float scrollSpeed = 60f;

    [Tooltip("Cuando scrollingPanel.anchoredPosition.y sea mayor o igual a este valor, termina el scroll.")]
    public float endScrollY = 800f;

    [Header("End Screen Timings")]
    public float thankYouFadeTime = 0.6f;
    public float cornerHintsDelay = 1.0f;
    public float cornerHintsFadeTime = 0.35f;

    [Header("Scene Names")]
    [Tooltip("Nombre exacto de tu escena de juego (Build Settings).")]
    public string gameSceneName = "GameScene";

    private bool finished;

    AudioSource sfx;

    void Start()
    {
        // Estado inicial
        finished = false;

        if (endPanel != null) endPanel.SetActive(false);

        SetGroupAlpha(thankYouGroup, 0f);
        SetGroupAlpha(pressSpaceGroup, 0f);
        SetGroupAlpha(pressEscGroup, 0f);
    }

    void Update()
    {
        if (!finished)
        {
            ScrollCredits();
        }
        else
        {
            HandleEndInputs();
        }
    }

    private void ScrollCredits()
    {
        if (scrollingPanel == null) return;

        // Subimos el panel usando anchoredPosition (UI)
        Vector2 pos = scrollingPanel.anchoredPosition;
        pos.y += scrollSpeed * Time.deltaTime;
        scrollingPanel.anchoredPosition = pos;

        // ¿Hemos llegado al final?
        if (pos.y >= endScrollY)
        {
            FinishCredits();
        }
    }

    private void FinishCredits()
    {
        finished = true;

        if (scrollingPanel != null) scrollingPanel.gameObject.SetActive(false);
        if (endPanel != null) endPanel.SetActive(true);

        // Arranca la secuencia final
        StartCoroutine(EndSequence());
    }

    private IEnumerator EndSequence()
    {
        // Aparece el "Thank you..."
        if (thankYouGroup != null)
            yield return FadeCanvasGroup(thankYouGroup, 0f, 1f, thankYouFadeTime);

        // Pausa y luego aparecen indicaciones de esquinas
        yield return new WaitForSeconds(cornerHintsDelay);

        // Fade in simultáneo de las dos esquinas
        Coroutine a = null, b = null;

        if (pressSpaceGroup != null)
            a = StartCoroutine(FadeCanvasGroup(pressSpaceGroup, 0f, 1f, cornerHintsFadeTime));

        if (pressEscGroup != null)
            b = StartCoroutine(FadeCanvasGroup(pressEscGroup, 0f, 1f, cornerHintsFadeTime));

        if (a != null) yield return a;
        if (b != null) yield return b;
    }

    private void HandleEndInputs()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!string.IsNullOrWhiteSpace(gameSceneName))
                SceneManager.LoadScene(0);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // En editor no cierra la Play Mode, pero en build sí
            Application.Quit();
        }
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup group, float from, float to, float duration)
    {
        if (group == null) yield break;

        group.alpha = from;

        if (duration <= 0f)
        {
            group.alpha = to;
            yield break;
        }

        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / duration);
            group.alpha = Mathf.Lerp(from, to, t);
            yield return null;
        }

        group.alpha = to;
    }

    private void SetGroupAlpha(CanvasGroup group, float alpha)
    {
        if (group == null) return;
        group.alpha = alpha;
    }
}
