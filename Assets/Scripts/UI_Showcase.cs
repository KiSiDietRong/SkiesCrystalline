using DG.Tweening;
using UnityEngine;

public class UI_Showcase : MonoBehaviour
{
    [Header("Logo (UI Image - RectTransform)")]
    public RectTransform logoRect;
    public Vector2 logoDropOffset = new Vector2(0, 800f);
    public float dropDuration = 1.2f;

    [Header("UI Buttons")]
    public RectTransform newGameButton;
    public RectTransform guideButton;

    [Header("Help Popup")]
    public Canvas canvasHelp;
    public RectTransform helpPanel;

    public float delayBetweenButtons = 0.3f;

    void Start()
    {
        AnimateLogo();
    }

    void AnimateLogo()
    {
        Vector2 targetPos = logoRect.anchoredPosition;
        logoRect.anchoredPosition = targetPos + logoDropOffset;

        logoRect.DOAnchorPos(targetPos, dropDuration)
            .SetEase(Ease.OutBounce)
            .OnComplete(() => ShowUIButtons());
    }

    void ShowUIButtons()
    {
        newGameButton.localScale = Vector3.zero;
        guideButton.localScale = Vector3.zero;

        newGameButton.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
        guideButton.DOScale(Vector3.one, 0.5f)
            .SetDelay(delayBetweenButtons).SetEase(Ease.OutBack);
    }

    public void ShowHelpPopup()
    {
        canvasHelp.gameObject.SetActive(true);
        helpPanel.localScale = Vector3.zero;

        helpPanel.DOScale(Vector3.one, 0.6f)
            .SetEase(Ease.OutBack)
            .SetDelay(0.05f);
    }
}
