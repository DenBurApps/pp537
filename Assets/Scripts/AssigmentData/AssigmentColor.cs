using System;
using TheraBytes.BetterUi;
using UnityEngine;
using UnityEngine.UI;

namespace AssigmentData
{
    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(BetterImage))]
    public class AssigmentColor : MonoBehaviour
    {
        [SerializeField] private ColorType _colorType;
        [SerializeField] private AssigmentColorHolder _colorHolder;
        [SerializeField] private Image _checkImage;

        private Button _button;
        private BetterImage _image;

        public event Action<ColorType> Clicked;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _image = GetComponent<BetterImage>();
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(OnButtonClicked);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(OnButtonClicked);
        }
        
        private void Start()
        {
            _image.color = _colorHolder.GetColorByType(_colorType);
            SetDefault();
        }

        public void SetSelected()
        {
            _checkImage.enabled = true;
        }

        public void SetDefault()
        {
            _checkImage.enabled = false;
        }

        private void OnButtonClicked() => Clicked?.Invoke(_colorType);
    }
}