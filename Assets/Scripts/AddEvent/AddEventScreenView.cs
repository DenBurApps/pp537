using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Bitsplash.DatePicker;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace AddEvent
{
    [RequireComponent(typeof(ScreenVisabilityHandler))]
    public class AddEventScreenView : MonoBehaviour
    {
        [SerializeField] private Sprite _notSelectedSprite;
        [SerializeField] private Sprite _selectedSprite;
        [SerializeField] private Button _backButton;
        [SerializeField] private TMP_InputField _nameInput;
        [SerializeField] private TMP_InputField _noteInput;
        [SerializeField] private Button _dateButton;
        [SerializeField] private DatePickerSettings _datePicker;
        [SerializeField] private TMP_Text _dateText;
        [SerializeField] private Button _selectDurationButton;
        [SerializeField] private TMP_Text _durationText;
        [SerializeField] private Button _selectTimeButton;
        [SerializeField] private TMP_Text _timeText;
        [SerializeField] private Button _examButton;
        [SerializeField] private Button _saveButton;

        private ScreenVisabilityHandler _screenVisabilityHandler;

        public event Action<string> NameInputed;
        public event Action<string> NoteInputed;
        public event Action<string> DateInputed;
        public event Action TimeClicked;
        public event Action ExamClicked;
        public event Action SaveClicked;
        public event Action BackClicked;
        public event Action DurationClicked;

        private void Awake()
        {
            _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
        }

        private void OnEnable()
        {
            _dateButton.onClick.AddListener(OnDateButtonClicked);
            _nameInput.onValueChanged.AddListener(OnNameInputed);
            _noteInput.onValueChanged.AddListener(OnNoteInputed);
            _saveButton.onClick.AddListener(OnSaveClicked);
            _examButton.onClick.AddListener(OnExamClicked);
            _datePicker.Content.OnSelectionChanged.AddListener(SetDate);
            _selectDurationButton.onClick.AddListener(OnDurationClicked);
            _selectTimeButton.onClick.AddListener(OnTimeClicked);
            _backButton.onClick.AddListener(OnBackClicked);

            AddButtonPunchAnimation(_dateButton);
            AddButtonPunchAnimation(_selectDurationButton);
            AddButtonPunchAnimation(_selectTimeButton);
            AddButtonPunchAnimation(_examButton);
            AddButtonPunchAnimation(_saveButton);
            AddButtonPunchAnimation(_backButton);
        }

        private void OnDisable()
        {
            _dateButton.onClick.RemoveListener(OnDateButtonClicked);
            _nameInput.onValueChanged.RemoveListener(OnNameInputed);
            _noteInput.onValueChanged.RemoveListener(OnNoteInputed);
            _saveButton.onClick.RemoveListener(OnSaveClicked);
            _examButton.onClick.RemoveListener(OnExamClicked);
            _datePicker.Content.OnSelectionChanged.RemoveListener(SetDate);
            _selectDurationButton.onClick.RemoveListener(OnDurationClicked);
            _selectTimeButton.onClick.RemoveListener(OnTimeClicked);
            _backButton.onClick.RemoveListener(OnBackClicked);
        }

        private void AddButtonPunchAnimation(Button button)
        {
            button.onClick.AddListener(() => { button.transform.DOPunchScale(Vector3.one * 0.2f, 0.3f, 2, 0.5f); });
        }

        public void Enable()
        {
            _screenVisabilityHandler.EnableScreen();
            AnimateInputFields();
        }

        public void Disable()
        {
            _screenVisabilityHandler.DisableScreen();
        }

        private void AnimateInputFields()
        {
            float delay = 0.1f;

            _nameInput.transform.localScale = Vector3.zero;
            _nameInput.transform.DOScale(1f, 0.3f)
                .SetEase(Ease.OutBack)
                .SetDelay(delay);

            _noteInput.transform.localScale = Vector3.zero;
            _noteInput.transform.DOScale(1f, 0.3f)
                .SetEase(Ease.OutBack)
                .SetDelay(delay * 2);

            _dateButton.transform.localScale = Vector3.zero;
            _dateButton.transform.DOScale(1f, 0.3f)
                .SetEase(Ease.OutBack)
                .SetDelay(delay * 3);

            _selectTimeButton.transform.localScale = Vector3.zero;
            _selectTimeButton.transform.DOScale(1f, 0.3f)
                .SetEase(Ease.OutBack)
                .SetDelay(delay * 4);

            _selectDurationButton.transform.localScale = Vector3.zero;
            _selectDurationButton.transform.DOScale(1f, 0.3f)
                .SetEase(Ease.OutBack)
                .SetDelay(delay * 5);

            _examButton.transform.localScale = Vector3.zero;
            _examButton.transform.DOScale(1f, 0.3f)
                .SetEase(Ease.OutBack)
                .SetDelay(delay * 6);

            _saveButton.transform.localScale = Vector3.zero;
            _saveButton.transform.DOScale(1f, 0.3f)
                .SetEase(Ease.OutBack)
                .SetDelay(delay * 7);
        }

        public void SetName(string name)
        {
            _nameInput.text = name;
        }

        public void SetNote(string note)
        {
            _noteInput.text = note;
        }

        public void SetDate(string date)
        {
            _dateText.text = date;
            _dateText.transform.DOPunchScale(Vector3.one * 0.2f, 0.3f, 2, 0.5f);
        }

        public void SetTime(string time)
        {
            _timeText.text = time;
            _timeText.transform.DOPunchScale(Vector3.one * 0.2f, 0.3f, 2, 0.5f);
        }

        public void SetDuaration(string duration)
        {
            _durationText.text = duration;
            _durationText.transform.DOPunchScale(Vector3.one * 0.2f, 0.3f, 2, 0.5f);
        }

        public void ToggleSaveButton(bool state)
        {
            _saveButton.interactable = state;
        }

        private void SetDate()
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
            _datePicker.transform.DOScale(0f, 0.3f)
                .SetEase(Ease.InBack)
                .OnComplete(() => _datePicker.gameObject.SetActive(false));
        }

        public void ToggleExamSprite(bool status)
        {
            _examButton.image.sprite = status ? _selectedSprite : _notSelectedSprite;
            _examButton.transform.DOPunchScale(Vector3.one * 0.2f, 0.3f, 2, 0.5f);
        }

        private void OnNameInputed(string input) => NameInputed?.Invoke(input);
        private void OnNoteInputed(string input) => NoteInputed?.Invoke(input);

        private void OnDateButtonClicked()
        {
            _datePicker.gameObject.SetActive(true);
            _datePicker.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
        }

        private void OnDurationClicked() => DurationClicked?.Invoke();
        private void OnTimeClicked() => TimeClicked?.Invoke();
        private void OnExamClicked() => ExamClicked?.Invoke();
        private void OnSaveClicked() => SaveClicked?.Invoke();
        private void OnBackClicked() => BackClicked?.Invoke();
    }
}