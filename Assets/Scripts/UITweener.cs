using UnityEngine;
using UnityEngine.Events;

public enum UIAnimationTypes
{
    Move,
    Rotate,
    Scale,
    SizeDelta,
    Fade
}

public class UITweener : MonoBehaviour
{
    [Header("Target Object")]
    [SerializeField]
    private GameObject objectToAnimate;

    [Header("Config")]
    [SerializeField]
    private UIAnimationTypes uiAnimationTypes;

    [SerializeField]
    private LeanTweenType easeType;

    [SerializeField]
    private float duration;

    [SerializeField]
    private bool loop;

    [SerializeField]
    private bool pingPong;

    [SerializeField]
    private float delay;

    [Header("Values")]
    [SerializeField]
    private bool animateFromCurrentValue;

    [SerializeField]
    private Vector3 from;

    [SerializeField]
    private Vector3 to;

    [Header("Triggers")]
    [SerializeField]
    private bool animateOnEnable = true;

    [SerializeField]
    private UnityEvent onCompleteEvent;

    [SerializeField]
    private bool returnToInitialStateOnDisable = true;

    [SerializeField]
    private bool disableOnComplete;

    // Private

    private LTDescr _tweenObject;

    #region Unity Methods

    private void OnEnable()
    {
        if (animateOnEnable)
        {
            StartAnimation();
        }
    }

    #endregion

    #region Public Methods

    // ReSharper disable once MemberCanBePrivate.Global
    public void StartAnimation()
    {
        HandleTween();
    }

    public void StopAnimation()
    {
        if (returnToInitialStateOnDisable)
        {
            InvertDirection();

            HandleTween();

            _tweenObject.setOnComplete(obj =>
            {
                InvertDirection();

                if (disableOnComplete)
                {
                    gameObject.SetActive(false);
                }
            });
        }
        else if (disableOnComplete)
        {
            gameObject.SetActive(false);
        }
    }

    #endregion

    #region Tween Controller

    private void HandleTween()
    {
        if (!objectToAnimate)
        {
            objectToAnimate = gameObject;
        }

        switch (uiAnimationTypes)
        {
            case UIAnimationTypes.Move:
                Move();

                break;

            case UIAnimationTypes.Rotate:
                Rotate();

                break;

            case UIAnimationTypes.Scale:
                Scale();

                break;

            case UIAnimationTypes.SizeDelta:
                SizeDelta();

                break;

            case UIAnimationTypes.Fade:
                Fade();

                break;
        }

        _tweenObject.setDelay(delay);
        _tweenObject.setEase(easeType);

        if (loop)
        {
            _tweenObject.loopCount = int.MaxValue;
        }

        if (pingPong)
        {
            _tweenObject.setLoopPingPong();
        }

        _tweenObject.setOnComplete(OnComplete);
    }

    #endregion

    #region Tween Methods

    private void Move()
    {
        var rectTransform = objectToAnimate.GetComponent<RectTransform>();

        if (!animateFromCurrentValue)
        {
            rectTransform.anchoredPosition = from;
        }

        _tweenObject = LeanTween.move(rectTransform, to, duration);
    }

    private void Rotate()
    {
        var rectTransform = objectToAnimate.GetComponent<RectTransform>();

        if (!animateFromCurrentValue)
        {
            rectTransform.eulerAngles = from;
        }

        _tweenObject = LeanTween.rotate(rectTransform, to, duration);
    }

    private void Scale()
    {
        var rectTransform = objectToAnimate.GetComponent<RectTransform>();

        if (!animateFromCurrentValue)
        {
            rectTransform.localScale = from;
        }

        _tweenObject = LeanTween.scale(rectTransform, to, duration);
    }

    private void SizeDelta()
    {
        var rectTransform = objectToAnimate.GetComponent<RectTransform>();

        if (!animateFromCurrentValue)
        {
            rectTransform.sizeDelta = from;
        }

        _tweenObject = LeanTween.size(rectTransform, to, duration);
    }

    private void Fade()
    {
        var canvasGroup = objectToAnimate.GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = objectToAnimate.AddComponent<CanvasGroup>();
        }

        canvasGroup.alpha = from.x;

        _tweenObject = LeanTween.alphaCanvas(canvasGroup, to.x, duration);
    }

    #endregion

    #region Callback

    private void OnComplete(object obj)
    {
        StopAnimation();

        onCompleteEvent?.Invoke();
    }

    #endregion

    #region Utils

    private void InvertDirection()
    {
        var temp = from;
        from = to;
        to = temp;
    }

    #endregion
}
