using System.Collections;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using Sequence = PrimeTween.Sequence;

public class CardVisual : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader inputReader;
    private CardBody parentCardBody;
    private Image _image;
    private Tween _punchTween;
    private RectTransform _rectTransform;
    private Transform _deckPosition;
    private Vector2 _mouseScreenPosition;
    private Canvas _canvas;

    private Sequence _playedSequence;
    private Vector2 LocalMousePosition => UIHelpers.GetLocalCoordsFromMouseScreenPosition(_rectTransform, _mouseScreenPosition, _canvas);

    [Header("Follow Parameters")]
    [SerializeField] private float maxSpeedInPixel = 500f;
    [SerializeField] private float maxRotationAmountInDegree = 80f;

    [Header("Rotation Parameters")]
    [SerializeField] private float autoTiltAmount = 30;
    [SerializeField] private float manualTiltAmount = 20;
    [SerializeField] private float tiltSpeed = 20;

    [Header("Scale Parameters")]
    [SerializeField] private float scaleOnHover = 1.1f;
    [SerializeField] private float scaleTransitionInSeconds = .1f;
    [SerializeField] private Ease scaleEase = Ease.OutBack;
    private float _scale = 1f;

    [Header("Hover Parameters")]
    [SerializeField] private float hoverPunchAngle = 5;
    [SerializeField] private float hoverTransition = .15f;
    [SerializeField] private Ease hoverEase = Ease.OutBack;
    
    [Header("Play Card Parrameters")]
    [SerializeField] private TweenSettings onCardPlayedScaleTweenSettings;
    [SerializeField] private TweenSettings onCardPlayedPositionTweenSettings;

    private int _savedIndex;
    
    private bool _isHovering = false;
    private bool _isDragging = false;
    
    private void Start()
    {
        _canvas = Registry<MainUICanvas>.GetFirst().GetComponent<Canvas>();
        _rectTransform = GetComponent<RectTransform>();
        
        inputReader.Point += (p, isMouse) => _mouseScreenPosition = isMouse ? p : Vector2.zero;
    }
    
    private void OnDisable()
    {
        _playedSequence.Stop();
        Tween.StopAll(transform);
    }

    private void Update()
    {
        CardTilt();
    }

    public void Initialize(CardBody cardBody, CardData cardData)
    {
        if (_image == null)
            _image = this.GetOrAddComponent<Image>();
        
        if (parentCardBody == null)
        {
            parentCardBody = cardBody;
            parentCardBody.PointerEnterEvent.AddListener(OnPointerEnter);
            parentCardBody.PointerExitEvent.AddListener(OnPointerExit);
            parentCardBody.BeginDragEvent.AddListener(OnDragEnter);
            parentCardBody.EndDragEvent.AddListener(OnDragExit);
            parentCardBody.SelectEvent.AddListener(OnSelect);
            parentCardBody.OnMoveEvent.AddListener((body, movement) => AnimateVelocity(movement));
        }
        _image.sprite = cardData.CardImage;
        transform.localScale = Vector3.one;
    }

    private void ResetRotation()
    {
        transform.localRotation = Quaternion.identity;
    }

    private void OnPointerEnter(CardBody cardBody)
    {
        _isHovering = true;
        OnPointerEnterAnimation();
    }

    private void OnPointerExit(CardBody cardBody)
    {
        _isHovering = false;
        OnPointerExitAnimation();
    }

    private void OnPointerEnterAnimation()
    {
        Tween.Scale(transform, 1f, scaleOnHover, scaleTransitionInSeconds, scaleEase);
        _scale = scaleOnHover;
        
        _punchTween.Stop();
        _punchTween = Tween.PunchLocalRotation(transform, Vector3.forward * hoverPunchAngle, hoverTransition, easeBetweenShakes: hoverEase).OnComplete(ResetRotation);
    }

    private void OnPointerExitAnimation()
    {
        Tween.Scale(transform, _scale, 1f, scaleTransitionInSeconds, scaleEase);
        _scale = 1f;
        
        _punchTween.Stop();
        _punchTween = Tween.PunchLocalRotation(transform, Vector3.forward * hoverPunchAngle, hoverTransition, easeBetweenShakes: hoverEase).OnComplete(ResetRotation);
    }

    private void OnDragEnter(CardBody cardBody)
    {
        _isDragging = true;
        _savedIndex = cardBody.ParentIndex;
    }

    private void OnDragExit(CardBody cardBody, bool isPlaying)
    {
        _isDragging = false;
        if (!isPlaying) return;

        StartCoroutine(OnCardPlayed());
    }

    private IEnumerator OnCardPlayed()
    {
        parentCardBody.GoToTrash();
        _deckPosition ??= Registry<DeckHandler>.GetFirst().ReturnPosition;
        

        _playedSequence = Sequence
            .Create(Tween.Scale(transform, new TweenSettings<float>(0f, onCardPlayedScaleTweenSettings)))
            .Group(Tween.Position(transform, new(_deckPosition.position, onCardPlayedPositionTweenSettings)));
        
        yield return _playedSequence.ToYieldInstruction();
        parentCardBody.ReturnToDeck();
    }

    private void OnSelect(CardBody cardBody, bool isSelected)
    {
        if (isSelected)
        {
            Tween.UIAnchoredPositionY(_rectTransform, 50f, .1f);
            OnPointerEnterAnimation();
        }
        else
        {
            Tween.UIAnchoredPositionY(_rectTransform, 0f, .1f);
            OnPointerExitAnimation();
        }
        
    }
    
    private void CardTilt()
    {
        // TODO : Fix the card "center"
        _savedIndex = _isDragging ? _savedIndex : parentCardBody.ParentIndex;
        float sine = Mathf.Sin(Time.time + _savedIndex) * (_isHovering ? .2f : 1);
        float cosine = Mathf.Cos(Time.time + _savedIndex) * (_isHovering ? .2f : 1);

        Vector3 offset = (_rectTransform.anchoredPosition - LocalMousePosition).normalized;
        float tiltX = _isHovering ? ((offset.y * -1) * manualTiltAmount) : 0;
        float tiltY = _isHovering ? ((offset.x) * manualTiltAmount) : 0;
        float tiltZ = _isHovering ? transform.eulerAngles.z : 0;

        float lerpX = Mathf.LerpAngle(transform.eulerAngles.x, tiltX + (sine * autoTiltAmount), tiltSpeed * Time.deltaTime);
        float lerpY = Mathf.LerpAngle(transform.eulerAngles.y, tiltY + (cosine * autoTiltAmount), tiltSpeed * Time.deltaTime);
        float lerpZ = Mathf.LerpAngle(transform.eulerAngles.z, tiltZ, tiltSpeed / 2 * Time.deltaTime);

        transform.eulerAngles = new Vector3(lerpX, lerpY, lerpZ);
    }


    private void AnimateVelocity(Vector2 velocity)
    {
        var horizontalVelocity = velocity.x;
        var isRight = horizontalVelocity > 0;
        
        var horizontalPercent = Mathf.Abs(horizontalVelocity) / maxSpeedInPixel;
        
        transform.localRotation = Quaternion.Euler(0, 0, (isRight ? 1 : -1) * maxRotationAmountInDegree * horizontalPercent);
    }
}