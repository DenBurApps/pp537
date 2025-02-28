using System;
using System.Text.RegularExpressions;
using EventData;
using UnityEngine;
using DG.Tweening;

namespace AddEvent
{
    public class AddEventScreen : MonoBehaviour
    {
        [SerializeField] private AddEventScreenView _view;
        [SerializeField] private AddEventTimeInputer _timeInputer;
        [SerializeField] private AddEventTimeInputer _timeInputer2;

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
        public event Action BackClicked;

        private void OnEnable()
        {
            _view.NameInputed += OnNameInputed;
            _view.NoteInputed += OnNoteInputed;
            _view.DateInputed += OnDateInputed;
            _view.DurationClicked += OpenTimeInputer;
            _view.TimeClicked += OpenTimeInputer2;
            _view.SaveClicked += SaveData;
            _view.ExamClicked += OnExamClicked;
            _view.BackClicked += OnBackClicked;

            _timeInputer.ConfirmClicked += OnDurationInputed;
            _timeInputer2.ConfirmClicked += SetTime;
        }

        private void OnDisable()
        {
            _view.NameInputed -= OnNameInputed;
            _view.NoteInputed -= OnNoteInputed;
            _view.DateInputed -= OnDateInputed;
            _view.TimeClicked -= OpenTimeInputer2;
            _view.DurationClicked -= OpenTimeInputer;
            _view.SaveClicked -= SaveData;
            _view.ExamClicked -= OnExamClicked;

            _timeInputer.ConfirmClicked -= OnDurationInputed;
            _timeInputer2.ConfirmClicked -= SetTime;
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
            
            transform.localScale = Vector3.zero;
            transform.DOScale(1f, 0.3f)
                .SetEase(Ease.OutBack);
            
            ValidateInput();
        }

        private void OpenTimeInputer()
        {
            _timeInputer.gameObject.SetActive(true);

            _timeInputer.transform.localScale = Vector3.zero;
            _timeInputer.transform.DOScale(1f, 0.3f)
                .SetEase(Ease.OutBack);
        }

        private void OpenTimeInputer2()
        {
            _timeInputer2.gameObject.SetActive(true);
            
            _timeInputer2.transform.localScale = Vector3.zero;
            _timeInputer2.transform.DOScale(1f, 0.3f)
                .SetEase(Ease.OutBack);
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

        private void SetTime(string hr, string min)
        {
            int hrDuration;
            int minDuration;

            if (int.TryParse(hr, out hrDuration))
            {
                _timeHr = hrDuration;
            }

            if (int.TryParse(min, out minDuration))
            {
                _timeMin = minDuration;
            }

            _view.SetTime($"{hr}:{min}");
            ValidateInput();
        }

        private void OnExamClicked()
        {
            _isExam = !_isExam;
            _view.ToggleExamSprite(_isExam);
        }

        private void SaveData()
        {
            var eventData = new EventData.EventData(_timeHr, _timeMin, _date, _name, _note, _durationHr, _durationMin, _isExam);

            transform.DOScale(0.8f, 0.2f)
                .SetEase(Ease.InBack)
                .OnComplete(() => {
                    Saved?.Invoke(eventData);
                    _view.Disable();
                    transform.localScale = Vector3.one;
                });
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
            _view.SetDate("Choose date");
            _view.SetNote(_note);
            _view.SetDuaration("Choose duration");
            _view.SetTime("Choose time");
            _view.CloseCalendar();
        }

        private void ValidateInput()
        {
            bool isValid = !string.IsNullOrEmpty(_name) && 
                           !string.IsNullOrEmpty(_date) && 
                           !string.IsNullOrEmpty(_note);

            _view.ToggleSaveButton(isValid);
        }

        private void OnBackClicked()
        {
            transform.DOScale(0f, 0.3f)
                .SetEase(Ease.InBack)
                .OnComplete(() => {
                    BackClicked?.Invoke();
                    _view.Disable();
                });
        }
    }
}