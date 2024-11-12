using System;
using System.Text.RegularExpressions;
using Bitsplash.DatePicker;
using TMPro;
using Unity.VisualScripting;
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
        [SerializeField] private TMP_InputField _timeInput;
        [SerializeField] private Button _addStepButton;
        [SerializeField] private Button _addSourceButton;
        [SerializeField] private Button _saveButton;
        [SerializeField] private Button _backButton;

        private ScreenVisabilityHandler _screenVisabilityHandler;

        public event Action<string> NameInputed;
        public event Action<string> SubjectInputed;
        public event Action<string> NoteInputed;
        public event Action<string> DateInputed;
        public event Action<string> TimeInputed;
        public event Action AddStepClicked;
        public event Action AddSourceClicked;
        public event Action AddAssigmentClicked;
        public event Action SaveClicked;

        private void Awake()
        {
            _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
        }

        private void OnEnable()
        {
            _dateButton.onClick.AddListener(OnDateButtonClicked);
            _addStepButton.onClick.AddListener(OnAddStepClicked);
            _addSourceButton.onClick.AddListener(OnAddSourceClicked);
            _saveButton.onClick.AddListener(OnSaveClicked);

            _datePicker.Content.OnSelectionChanged.AddListener(SetDate);
        }

        private void OnDisable()
        {
            
            _dateButton.onClick.RemoveListener(OnDateButtonClicked);
            _addStepButton.onClick.RemoveListener(OnAddStepClicked);
            _addSourceButton.onClick.RemoveListener(OnAddSourceClicked);
            _saveButton.onClick.RemoveListener(OnSaveClicked);
            
            _datePicker.Content.OnSelectionChanged.RemoveListener(SetDate);

        }

        public void SetDate(string date)
        {
            _dateText.text = date;
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
            _timeInput.text = time;
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

        public void Enable()
        {
            _screenVisabilityHandler.EnableScreen();
        }

        public void Disable()
        {
            _screenVisabilityHandler.DisableScreen();
        }

        public void ToggleSaveButton(bool isValid)
        {
            _saveButton.interactable = isValid;
        }

        private void ValidateTimeInput(string input)
        {
            string timePattern = @"^([01]?[0-9]|2[0-3]):[0-5][0-9]$";
            bool isValid = Regex.IsMatch(input, timePattern);
        }

        private void FormatTimeInput(string input)
        {
            if (!Regex.IsMatch(input, @"^([01]?[0-9]|2[0-3]):[0-5][0-9]$"))
            {
                Debug.Log("Invalid time format. Please enter time as HH:mm.");
                _timeInput.text = "";
            }

            TimeInputed?.Invoke(input);
        }

        private void OnNameInputed(string input) => NameInputed?.Invoke(input);
        private void OnSubjectInputed(string input) => SubjectInputed?.Invoke(input);
        private void OnNoteInputed(string input) => NoteInputed?.Invoke(input);
        private void OnDateButtonClicked() => _datePicker.gameObject.SetActive(true);
        private void OnAddStepClicked() => AddStepClicked?.Invoke();
        private void OnAddSourceClicked() => AddSourceClicked?.Invoke();
        private void OnSaveClicked() => SaveClicked?.Invoke();
    }
}