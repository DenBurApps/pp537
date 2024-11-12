using System;
using UnityEngine;
using UnityEngine.UI;

namespace AssigmentData
{
    [RequireComponent(typeof(Image))]
    [RequireComponent(typeof(Button))]
    public class AssigmentIcon : MonoBehaviour
    {
        [SerializeField] private IconType _type;
        [SerializeField] private AssigmentIconHolder _iconHolder;
        
        private Image _image;
        private Button _button;

        public event Action<IconType> Clicked; 
        
        private void Awake()
        {
            _image = GetComponent<Image>();
            _button = GetComponent<Button>();
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
            _image.sprite = _iconHolder.GetSpriteByType(_type);
        }

        private void OnButtonClicked()
        {
            Clicked?.Invoke(_type);
        }
    }
}
