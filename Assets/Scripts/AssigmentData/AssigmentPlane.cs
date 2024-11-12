using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace AssigmentData
{
    public class AssigmentPlane : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private Color _lowTimeColor;
        [SerializeField] private Color _defaultColor;
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _dateText;
        [SerializeField] private TMP_Text _timeText;
        [SerializeField] private Button _checkButton;
        [SerializeField] private Image _logo;
        [SerializeField] private AssigmentPlaneDayCounter _dayCounter;

        private AssigmentIconHolder _iconHolder;
        private AssigmentColorHolder _colorHolder;

        public event Action<AssigmentPlane> Checked;

        public AssigmentData Data { get; private set; }
        public bool IsActive { get; private set; }
        public bool IsChecked { get; private set; }

        private void OnEnable()
        {
            _dayCounter.LowTimeLeft += () => _image.color = _lowTimeColor;

            _checkButton.onClick.AddListener(SetChecked);
        }

        public void Enable()
        {
            gameObject.SetActive(true);
            IsActive = true;
        }

        public void Disable()
        {
            gameObject.SetActive(false);
            IsActive = false;
        }

        public void SetHolders(AssigmentColorHolder colorHolder, AssigmentIconHolder iconHolder)
        {
            _colorHolder = colorHolder;
            _iconHolder = iconHolder;
        }

        public void SetData(AssigmentData data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            Data = data;

            _nameText.text = Data.Name;
            _dateText.text = Data.Date;
            _timeText.text = $"{Data.TimeHr}:{Data.TimeMin}";

            if (DateTime.TryParse($"{Data.Date} {Data.TimeHr}:{Data.TimeMin}", out DateTime dueDate))
            {
                if (_dayCounter != null && _dayCounter.isActiveAndEnabled)
                    _dayCounter.CalculateTime(dueDate);
            }
            else
            {
                Debug.LogError("Failed to parse date and time for assignment.");
            }

            _logo.sprite = _iconHolder.GetSpriteByType(Data.IconType);
            _logo.color = _colorHolder.GetColorByType(Data.ColorType);
        }

        private void SetChecked()
        {
            IsChecked = true;
            Data.IsSelected = true;
            Checked?.Invoke(this);
        }
    }
}