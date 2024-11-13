using System;
using System.Text.RegularExpressions;
using AddEvent;
using EventData;
using UnityEngine;

namespace ScheduleScreen
{
    public class EditEvent : MonoBehaviour
    {
        [SerializeField] private EditEventView _view;
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
        private EventPlane _currentPlane;

        public event Action<EventData.EventData> Saved;
        public event Action<EventPlane> Deleted; 

        private void OnEnable()
        {
            _view.NameInputed += OnNameInputed;
            _view.NoteInputed += OnNoteInputed;
            _view.DateInputed += OnDateInputed;
            _view.TimeInputed += SetTime;
            _view.DurationClicked += OpenTimeInputer;
            _view.SaveClicked += SaveData;
            _view.ExamClicked += OnExamClicked;
            _view.DeleteClicked += DeleteEvent;

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
            _view.DeleteClicked -= DeleteEvent;

            _timeInputer.ConfirmClicked -= OnDurationInputed;
        }

        private void Start()
        {
            gameObject.SetActive(false);
        }

        public void EnableScreen(EventPlane plane)
        {
            if (plane == null)
                throw new ArgumentNullException(nameof(plane));

            gameObject.SetActive(true);
            _currentPlane = plane;

            ResetValues();

            _name = _currentPlane.EventData.Name;
            _note = _currentPlane.EventData.Note;
            _date = _currentPlane.EventData.Date;
            _durationHr = _currentPlane.EventData.DurationHr;
            _durationMin = _currentPlane.EventData.DurationMin;
            _timeHr = _currentPlane.EventData.TimeHr;
            _timeMin = _currentPlane.EventData.TimeMin;
            _isExam = _currentPlane.EventData.IsExam;

            _view.SetName(_name);
            _view.SetNote(_note);
            _view.SetDate(_date);
            _view.SetTime($"{_timeHr}:{_timeMin}");
            _view.SetDuaration($"{_durationHr} hour {_durationMin} min");
            _view.ToggleExamSprite(_isExam);

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

            _currentPlane.SetData(eventData);

            Saved?.Invoke(eventData);
            gameObject.SetActive(false);
        }

        private void ResetValues()
        {
            _isExam = false;
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

        private void DeleteEvent()
        {
            Deleted?.Invoke(_currentPlane);
            gameObject.SetActive(false);
        }
    }
}