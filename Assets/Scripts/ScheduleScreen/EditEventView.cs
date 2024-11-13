using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Bitsplash.DatePicker;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ScheduleScreen
{
    public class EditEventView : MonoBehaviour
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
        [SerializeField] private TMP_InputField _timeInput;
        [SerializeField] private Button _examButton;
        [SerializeField] private Button _saveButton;
        [SerializeField] private Button _deleteButton;

        private ScreenVisabilityHandler _screenVisabilityHandler;
        
        public event Action<string> NameInputed;
        public event Action<string> NoteInputed;
        public event Action<string> DateInputed;
        public event Action<string> TimeInputed;
        public event Action ExamClicked;
        public event Action SaveClicked;
        public event Action BackClicked;
        public event Action DurationClicked;
        public event Action DeleteClicked;

        private void Awake()
        {
            _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
        }

        private void OnEnable()
        {
            _dateButton.onClick.AddListener(OnDateButtonClicked);
            _nameInput.onValueChanged.AddListener(OnNameInputed);
            _noteInput.onValueChanged.AddListener(OnNoteInputed);
            _timeInput.onValueChanged.AddListener(ValidateTimeInput);
            _timeInput.onEndEdit.AddListener(FormatTimeInput);
            _saveButton.onClick.AddListener(OnSaveClicked);
            _examButton.onClick.AddListener(OnExamClicked);
            _datePicker.Content.OnSelectionChanged.AddListener(SetDate);
            _selectDurationButton.onClick.AddListener(OnDurationClicked);
            _deleteButton.onClick.AddListener(OnDeleteClicked);
        }

        private void OnDisable()
        {
            _dateButton.onClick.RemoveListener(OnDateButtonClicked);
            _nameInput.onValueChanged.RemoveListener(OnNameInputed);
            _noteInput.onValueChanged.RemoveListener(OnNoteInputed);
            _timeInput.onValueChanged.RemoveListener(ValidateTimeInput);
            _timeInput.onEndEdit.RemoveListener(FormatTimeInput);
            _saveButton.onClick.RemoveListener(OnSaveClicked);
            _examButton.onClick.RemoveListener(OnExamClicked);
            _datePicker.Content.OnSelectionChanged.RemoveListener(SetDate);
            _selectDurationButton.onClick.RemoveListener(OnDurationClicked);
            _deleteButton.onClick.RemoveListener(OnDeleteClicked);
        }

        public void Enable()
        {
            _screenVisabilityHandler.EnableScreen();
        }

        public void Disable()
        {
            _screenVisabilityHandler.DisableScreen();
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
        }

        public void SetTime(string time)
        {
            _timeInput.text = time;
        }

        public void SetDuaration(string duration)
        {
            _durationText.text = duration;
        }

        public void ToggleSaveButton(bool state)
        {
            _saveButton.interactable = state;
        }


        private void ValidateTimeInput(string input)
        {
            string timePattern = @"^([01]?[0-9]|2[0-3]):[0-5][0-9]$";
            bool isValid = Regex.IsMatch(input, timePattern);
        }

        private void FormatTimeInput(string input)
        {
            if (DateTime.TryParseExact(input, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None,
                    out DateTime parsedTime))
            {
                var time = parsedTime.ToString("HH:mm");
                _timeInput.text = time;
                TimeInputed?.Invoke(time);
            }

            if (input.Length == 4 && int.TryParse(input, out int numericInput))
            {
                int hours = numericInput / 100;
                int minutes = numericInput % 100;

                if (hours >= 0 && hours <= 23 && minutes >= 0 && minutes <= 59)
                {
                    var time = $"{hours:D2}:{minutes:D2}";
                    _timeInput.text = time;
                    TimeInputed?.Invoke(time);
                }
            }
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
            _datePicker.gameObject.SetActive(false);
        }

        public void ToggleExamSprite(bool status)
        {
            _examButton.image.sprite = status ? _selectedSprite : _notSelectedSprite;
        }

        private void OnNameInputed(string input) => NameInputed?.Invoke(input);
        private void OnNoteInputed(string input) => NoteInputed?.Invoke(input);
        private void OnDateButtonClicked() => _datePicker.gameObject.SetActive(true);
        private void OnDurationClicked() => DurationClicked?.Invoke();
        private void OnExamClicked() => ExamClicked?.Invoke();
        private void OnSaveClicked() => SaveClicked?.Invoke();
        private void OnDeleteClicked() => DeleteClicked?.Invoke();
    }
}