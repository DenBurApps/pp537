using System;
using System.Text.RegularExpressions;
using EventData;
using UnityEngine;

namespace AddEvent
{
    public class AddEventScreen : MonoBehaviour
    {
        [SerializeField] private AddEventScreenView _view;
        [SerializeField] private AddEventTimeInputer _timeInputer;
        
        private string _name;
        private string _date;
        private string _note;
        private int _timeHr;
        private int _timeMin;
        private int _durationHr;
        private int _durationMin;
        private bool _isExam;
        private ExamData _examData;

        public event Action<EventData.EventData> Saved;

        private void OnEnable()
        {
            _view.NameInputed += OnNameInputed;
            _view.NoteInputed += OnNoteInputed;
            _view.DateInputed += OnDateInputed;
            _view.TimeInputed += SetTime;
            _view.DurationClicked += OpenTimeInputer;
            _view.SaveClicked += SaveData;
            _view.ExamClicked += OnExamClicked;

            _timeInputer.ConfirmClicked += OnDurationInputed;
        }

        private void OnDisable()
        {
            _view.NameInputed -= OnNameInputed;
            _view.NoteInputed -= OnNoteInputed;
            _view.DateInputed -= OnDateInputed;
            _view.TimeInputed -= SetTime;
            _view.DurationClicked -= OpenTimeInputer;
            _view.SaveClicked -= SaveData;
            _view.ExamClicked -= OnExamClicked;

            _timeInputer.ConfirmClicked -= OnDurationInputed;
        }

        private void Start()
        {
           _view.Disable();
            _isExam = false;
        }

        public void EnableScreen()
        {
            ResetValues();
            _view.Enable();
            ValidateInput();
        }

        private void OpenTimeInputer()
        {
            _timeInputer.gameObject.SetActive(true);
        }

        private void OnNameInputed(string name)
        {
            _name = name;
            ValidateInput();
        }

        private void OnNoteInputed(string note)
        {
            _note = note;
            ValidateInput();
        }

        private void OnDateInputed(string date)
        {
            _date = date;
            ValidateInput();
        }

        private void OnDurationInputed(string hr, string min)
        {
            int hrDuration;
            int minDuration;

            if (int.TryParse(hr, out hrDuration))
            {
                _durationHr = hrDuration;
            }
            
            if (int.TryParse(min, out minDuration))
            {
                _durationMin = minDuration;
            }

            _view.SetDuaration($"{hr}:{min}");
            ValidateInput();
        }
        
        private void SetTime(string input)
        {
            TryParseTime(input, out _timeHr, out _timeMin);
            ValidateInput();
        }
        
        private void TryParseTime(string input, out int hr, out int min)
        {
            hr = 0;
            min = 0;
            
            string timePattern = @"^([01]?[0-9]|2[0-3]):[0-5][0-9]$";
            if (Regex.IsMatch(input, timePattern))
            {
                string[] timeParts = input.Split(':');
                hr = int.Parse(timeParts[0]);
                min = int.Parse(timeParts[1]);
            }
        }

        private void OnExamClicked()
        {
            if (!_isExam)
            {
                _isExam = true;
            }
            else
            {
                _isExam = false;
            }
            
            _view.ToggleExamSprite(_isExam);
        }

        private void SaveData()
        {
            var eventData = new EventData.EventData(_timeHr, _timeMin, _date, _name, _note, _durationHr, _durationMin,
                _isExam);
            
            Saved?.Invoke(eventData);
            _view.Disable();
        }
        
        private void ResetValues()
        {
            _name = string.Empty;
            _note = string.Empty;
            _date = string.Empty;
            _timeHr = 0;
            _timeMin = 0;
            _durationHr = 0;
            _durationMin = 0;
            _view.SetName(_name);
            _view.SetDate("Select date");
            _view.SetNote(_note);
            _view.SetDuaration("SelectDuration");
            _view.SetTime(string.Empty);
            _view.CloseCalendar();
        }

        private void ValidateInput()
        {
            bool isValid = !string.IsNullOrEmpty(_name) && !string.IsNullOrEmpty(_date) && !string.IsNullOrEmpty(_note);
            
            _view.ToggleSaveButton(isValid);
        }
    }
}
