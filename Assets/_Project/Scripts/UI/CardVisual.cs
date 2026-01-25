using PrimeTween;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CardVisual : MonoBehaviour
{
    [Header("References")]
    private CardBody parentCardBody;
    private Image _image;
    private Tween _punchTween;

    [Header("Follow Parameters")]
    [SerializeField] private float followSpeed = 30;
    [SerializeField] private float maxSpeedInPixel = 500f;
    [SerializeField] private float maxRotationAmountInDegree = 80f;

    [Header("Rotation Parameters")]
    [SerializeField] private float rotationAmount = 20;
    [SerializeField] private float rotationSpeed = 20;
    [SerializeField] private float autoTiltAmount = 30;
    [SerializeField] private float manualTiltAmount = 20;
    [SerializeField] private float tiltSpeed = 20;

    [Header("Scale Parameters")]
    [SerializeField] private float scaleOnHover = 1.1f;
    [SerializeField] private float scaleOnSelected = 1.1f;
    [SerializeField] private float scaleTransitionInSeconds = .1f;
    [SerializeField] private Ease scaleEase = Ease.OutBack;
    private float _scale = 1f;

    [Header("Hover Parameters")]
    [SerializeField] private float hoverPunchAngle = 5;
    [SerializeField] private float hoverTransition = .15f;
    [SerializeField] private Ease hoverEase = Ease.OutBack;
    
    [Header("Swap Parameters")]
    [SerializeField] private bool swapAnimations = true;
    [SerializeField] private float swapRotationAngle = 30;
    [SerializeField] private float swapTransition = .15f;
    [SerializeField] private int swapVibrato = 5;

    private void OnPointerEnterAnimation(CardBody cardBody)
    {
        Tween.Scale(transform, 1f, scaleOnHover, scaleTransitionInSeconds, scaleEase);
        _scale = scaleOnHover;
        
        _punchTween.Stop();
        _punchTween = Tween.PunchLocalRotation(transform, Vector3.forward * hoverPunchAngle, hoverTransition, easeBetweenShakes: hoverEase).OnComplete(ResetRotation);
    }

    private void OnPointerExitAnimation(CardBody cardBody)
    {
        Tween.Scale(transform, _scale, 1f, scaleTransitionInSeconds, scaleEase);
        _scale = 1f;
        
        _punchTween.Stop();
        _punchTween = Tween.PunchLocalRotation(transform, Vector3.forward * hoverPunchAngle, hoverTransition, easeBetweenShakes: hoverEase).OnComplete(ResetRotation);
    }

    private void ResetRotation()
    {
        transform.localRotation = Quaternion.identity;
    }

    public void Initialize(CardBody cardBody, CardData cardData)
    {
        if (_image == null)
        {
            _image = this.GetOrAddComponent<Image>();
        }
        
        if (parentCardBody == null)
        {
            parentCardBody = cardBody;
            parentCardBody.PointerEnterEvent.AddListener(OnPointerEnterAnimation);
            parentCardBody.PointerExitEvent.AddListener(OnPointerExitAnimation);
        }
        _image.sprite = cardData.CardImage;
    }

    public void SetVelocity(Vector2 velocity)
    {
        var horizontalVelocity = velocity.x;
        var isRight = horizontalVelocity > 0;
        
        var horizontalPercent = Mathf.Abs(horizontalVelocity) / maxSpeedInPixel;
        
        transform.localRotation = Quaternion.Euler(0, 0, (isRight ? 1 : -1) * maxRotationAmountInDegree * horizontalPercent);
    }
}