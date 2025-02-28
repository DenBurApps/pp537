using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Bitsplash.DatePicker;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AssigmentDataInputScreen
{
    [RequireComponent(typeof(ScreenVisabilityHandler))]
    public class AddAssigmentScreenView : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _nameInput;
        [SerializeField] private TMP_InputField _subjectInput;
        [SerializeField] private TMP_InputField _noteInput;
        [SerializeField] private Button _dateButton;
        [SerializeField] private TMP_Text _dateText;
        [SerializeField] private DatePickerSettings _datePicker;
        [SerializeField] private Button _addStepButton;
        [SerializeField] private Button _addSourceButton;
        [SerializeField] private Button _saveButton;
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _selectTimeButton;
        [SerializeField] private TMP_Text _timeText;

        [Header("Animation Settings")]
        [SerializeField] private float _animationDuration = 0.3f;
        [SerializeField] private Ease _animationEase = Ease.OutQuad;
        [SerializeField] private float _shakeIntensity = 10f;

        private ScreenVisabilityHandler _screenVisabilityHandler;
        private CanvasGroup _canvasGroup;

        public event Action<string> NameInputed;
        public event Action<string> SubjectInputed;
        public event Action<string> NoteInputed;
        public event Action<string> DateInputed;
        public event Action<string> TimeInputed;
        public event Action AddStepClicked;
        public event Action AddSourceClicked;
        public event Action SaveClicked;
        public event Action BackClicked;
        public event Action TimeClicked;

        private void Awake()
        {
            _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
            _canvasGroup = GetComponent<CanvasGroup>();

            if (_canvasGroup == null)
            {
                _canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }

        private void OnEnable()
        {
            AddButtonAnimations();
            AddAdvancedInputFieldAnimations();

            _dateButton.onClick.AddListener(OnDateButtonClicked);
            _addStepButton.onClick.AddListener(OnAddStepClicked);
            _addSourceButton.onClick.AddListener(OnAddSourceClicked);
            _saveButton.onClick.AddListener(OnSaveClicked);
            _backButton.onClick.AddListener(OnBackClicked);

            _nameInput.onValueChanged.AddListener(OnNameInputed);
            _subjectInput.onValueChanged.AddListener(OnSubjectInputed);
            _noteInput.onValueChanged.AddListener(OnNoteInputed);

            _selectTimeButton.onClick.AddListener(OnTimeClicked);
            
            _datePicker.Content.OnSelectionChanged.AddListener(SetDate);
        }

        private void OnDisable()
        {
            _dateButton.onClick.RemoveListener(OnDateButtonClicked);
            _addStepButton.onClick.RemoveListener(OnAddStepClicked);
            _addSourceButton.onClick.RemoveListener(OnAddSourceClicked);
            _saveButton.onClick.RemoveListener(OnSaveClicked);
            _backButton.onClick.RemoveListener(OnBackClicked);

            _nameInput.onValueChanged.RemoveListener(OnNameInputed);
            _subjectInput.onValueChanged.RemoveListener(OnSubjectInputed);
            _noteInput.onValueChanged.RemoveListener(OnNoteInputed);
            
            _selectTimeButton.onClick.RemoveListener(OnTimeClicked);

            _datePicker.Content.OnSelectionChanged.RemoveListener(SetDate);
        }

        private void AddButtonAnimations()
        {
            AddButtonPunchScale(_dateButton);
            AddButtonPunchScale(_addStepButton);
            AddButtonPunchScale(_addSourceButton);
            AddButtonPunchScale(_saveButton);
            AddButtonPunchScale(_backButton);
            AddButtonPunchScale(_selectTimeButton);
        }

        private void AddButtonPunchScale(Button button)
        {
            button.onClick.AddListener(() => 
            {
                button.transform.DOPunchScale(Vector3.one * 0.2f, _animationDuration, 2, 1f)
                    .SetEase(_animationEase);
            });
        }

        private void AddAdvancedInputFieldAnimations()
        {
            AddInputFieldFocusAnimation(_nameInput);
            AddInputFieldFocusAnimation(_subjectInput);
            AddInputFieldFocusAnimation(_noteInput);
        }

        private void AddInputFieldFocusAnimation(TMP_InputField inputField)
        {
            var rectTransform = inputField.GetComponent<RectTransform>();
            var originalScale = rectTransform.localScale;
            
            inputField.onSelect.AddListener(_ => 
            {
                rectTransform.DOScale(originalScale * 1.05f, _animationDuration)
                    .SetEase(_animationEase);
                
                rectTransform.DOShakePosition(_animationDuration, _shakeIntensity / 10f)
                    .SetEase(_animationEase);
            });

            inputField.onDeselect.AddListener(_ => 
            {
                rectTransform.DOScale(originalScale, _animationDuration)
                    .SetEase(Ease.OutBounce);
            });
        }

        private void OnDateButtonClicked()
        {
            var datePicker = _datePicker.gameObject;
            
            if (!datePicker.activeSelf)
            {
                datePicker.SetActive(true);
                datePicker.transform.localScale = Vector3.zero;
                datePicker.transform.DOScale(1f, _animationDuration)
                    .SetEase(Ease.OutBack);
            }
            else
            {
                datePicker.transform.DOScale(0f, _animationDuration)
                    .SetEase(Ease.InBack)
                    .OnComplete(() => datePicker.SetActive(false));
            }
        }

        public void SetDate(string date)
        {
            _dateText.transform.DOScale(0.8f, _animationDuration / 2)
                .OnComplete(() => 
                {
                    _dateText.text = date;
                    _dateText.transform.DOScale(1f, _animationDuration / 2)
                        .SetEase(_animationEase);
                });
        }

        public void SetDate()
        {
            string text = "";
            var selection = _datePicker.Content.Selection;
            for (int i = 0; i < selection.Count; i++)
            {
                var date = selection.GetItem(i);
                text += date.ToString(format: "dd.MM.yyyy");
            }

            _dateText.text = text;
            DateInputed?.Invoke(text);
            CloseCalendar();
        }

        public void CloseCalendar()
        {
            var datePicker = _datePicker.gameObject;
            datePicker.transform.DOScale(0f, _animationDuration)
                .SetEase(Ease.InBack)
                .OnComplete(() => datePicker.SetActive(false));
        }

        public void Enable()
        {
            _screenVisabilityHandler.EnableScreen();
            
            _canvasGroup.alpha = 0;
            _canvasGroup.DOFade(1f, _animationDuration)
                .SetEase(_animationEase);
            
            transform.localScale = Vector3.zero;
            transform.DOScale(1f, _animationDuration)
                .SetEase(Ease.OutBack);
            
            transform.DOShakePosition(_animationDuration, _shakeIntensity)
                .SetEase(_animationEase);
        }

        public void Disable()
        {
            _canvasGroup.DOFade(0f, _animationDuration)
                .SetEase(_animationEase);
            
            transform.DOScale(0f, _animationDuration)
                .SetEase(Ease.InBack)
                .OnComplete(() => _screenVisabilityHandler.DisableScreen());
        }

        public void ToggleSaveButton(bool isValid)
        {
            _saveButton.interactable = isValid;
            
            if (isValid)
            {
                _saveButton.transform.DOShakeScale(_animationDuration, 0.5f, 2)
                    .SetEase(_animationEase);
                
                _saveButton.transform.DOPunchRotation(Vector3.forward * 10f, _animationDuration)
                    .SetEase(_animationEase);
            }
        }

        public void SetName(string name)
        {
            _nameInput.text = name;
        }

        public void SetSubject(string subject)
        {
            _subjectInput.text = subject;
        }

        public void SetTime(string time)
        {
            _timeText.text = time;
        }

        public void SetNote(string text)
        {
            _noteInput.text = text;
        }

        private void OnNameInputed(string input) => NameInputed?.Invoke(input);
        private void OnSubjectInputed(string input) => SubjectInputed?.Invoke(input);
        private void OnNoteInputed(string input) => NoteInputed?.Invoke(input);
        private void OnAddStepClicked() => AddStepClicked?.Invoke();
        private void OnAddSourceClicked() => AddSourceClicked?.Invoke();
        private void OnSaveClicked() => SaveClicked?.Invoke();
        private void OnBackClicked() => BackClicked?.Invoke();
        private void OnTimeClicked() => TimeClicked?.Invoke();
    }
}