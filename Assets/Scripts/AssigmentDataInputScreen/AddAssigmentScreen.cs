using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using AddEvent;
using AssigmentData;
using MainScreen;
using UnityEngine;

namespace AssigmentDataInputScreen
{
    public class AddAssigmentScreen : MonoBehaviour
    {
        [SerializeField] private Color _selectedIconColor;
        [SerializeField] private MainScreenAssigmentHolder _mainScreenAssigmentHolder;
        [SerializeField] private AddAssigmentScreenView _view;
        [SerializeField] private AssigmentSourceHolder _assigmentSourceHolder;
        [SerializeField] private AssigmentStepHolder _assigmentStepHolder;
        [SerializeField] private List<AssigmentIcon> _icons;
        [SerializeField] private List<AssigmentColor> _colors;
        [SerializeField] private AddEventTimeInputer _timeInputer;

        private string _name;
        private string _subject;
        private string _note;
        private int _timeHr;
        private int _timeMin;
        private string _date;
        private IconType _selectedIconType;
        private ColorType _selectedColorType;

        private AssigmentIcon _currentIcon;
        private AssigmentColor _currentColor;
        private AssigmentPlane _currentPlane;

        public event Action<AssigmentData.AssigmentData> Saved;
        public event Action Edited;
        public event Action BackEdited;

        private void OnEnable()
        {
            foreach (var icon in _icons)
            {
                icon.Clicked += SelectCurrentIcon;
            }

            foreach (var color in _colors)
            {
                color.Clicked += SelectCurrentColor;
            }

            _assigmentSourceHolder.AllPlanesDisabled += () => _assigmentSourceHolder.gameObject.SetActive(false);
            _assigmentStepHolder.AllPlanesDisabled += () => _assigmentStepHolder.gameObject.SetActive(false);

            _view.NameInputed += SetName;
            _view.DateInputed += SetDate;
            _view.NoteInputed += SetNote;
            //_view.TimeInputed += SetTime;
            _view.TimeClicked += OpenTimeInputer;
            _view.SubjectInputed += SetSubject;

            _view.AddStepClicked += AddStep;
            _view.AddSourceClicked += AddSource;
            _view.SaveClicked += SaveData;
            _timeInputer.ConfirmClicked += SetTime;
            _view.BackClicked += OnBackClicked;
        }

        private void OnDisable()
        {
            foreach (var icon in _icons)
            {
                icon.Clicked -= SelectCurrentIcon;
            }

            foreach (var color in _colors)
            {
                color.Clicked -= SelectCurrentColor;
            }

            _view.NameInputed -= SetName;
            _view.DateInputed -= SetDate;
            _view.NoteInputed -= SetNote;
            _view.TimeClicked -= OpenTimeInputer;
            //  _view.TimeInputed -= SetTime;
            _view.SubjectInputed -= SetSubject;

            _view.AddStepClicked -= AddStep;
            _view.AddSourceClicked -= AddSource;
            _view.SaveClicked -= SaveData;
            _view.BackClicked -= OnBackClicked;
            _timeInputer.ConfirmClicked -= SetTime;
        }

        private void Start()
        {
            _view.Disable();
            _assigmentSourceHolder.gameObject.SetActive(false);
            _assigmentStepHolder.gameObject.SetActive(false);
            ResetValues();
            ValidateInput();
        }

        public void EnableScreen()
        {
            ResetValues();
            _view.Enable();
        }

        public void EnableScreen(AssigmentPlane plane)
        {
            _view.Enable();
            ResetValues();

            _currentPlane = plane;

            _name = plane.Data.Name;
            _date = plane.Data.Date;
            _timeHr = plane.Data.TimeHr;
            _timeMin = plane.Data.TimeMin;
            _subject = plane.Data.Subject;
            _note = plane.Data.Note;
            _selectedColorType = plane.Data.ColorType;
            _selectedIconType = plane.Data.IconType;

            foreach (var icon in _icons)
            {
                if (icon.Type == _selectedIconType)
                {
                    SelectCurrentIcon(icon);
                }
            }

            foreach (var color in _colors)
            {
                if (color.Type == _selectedColorType)
                {
                    SelectCurrentColor(color);
                }
            }

            _view.SetTime($"{_timeHr}:{_timeMin}");
            _view.SetDate(_date);
            _view.SetName(_name);
            _view.SetNote(_note);
            _view.SetSubject(_subject);
        }

        private void OpenTimeInputer()
        {
            _timeInputer.gameObject.SetActive(true);
        }

        private void SelectCurrentIcon(AssigmentIcon icon)
        {
            if (_currentIcon != null)
            {
                _currentIcon.SetDefault();
            }

            _currentIcon = icon;
            _selectedIconType = _currentIcon.Type;
            _currentIcon.SetSelected(_selectedIconColor);
            ValidateInput();
        }

        private void SelectCurrentColor(AssigmentColor color)
        {
            if (_currentColor != null)
            {
                _currentColor.SetDefault();
            }

            _currentColor = color;
            _selectedColorType = _currentColor.Type;
            _currentColor.SetSelected();
            ValidateInput();
        }

        private void AddStep()
        {
            if (!_assigmentStepHolder.isActiveAndEnabled)
            {
                _assigmentStepHolder.gameObject.SetActive(true);
            }

            _assigmentStepHolder.EnableStep();
        }

        private void AddSource()
        {
            if (!_assigmentSourceHolder.isActiveAndEnabled)
            {
                _assigmentSourceHolder.gameObject.SetActive(true);
            }

            _assigmentSourceHolder.EnableSource();
        }

        private void SetName(string name)
        {
            _name = name;
            ValidateInput();
        }

        private void SetSubject(string subject)
        {
            _subject = subject;
            ValidateInput();
        }

        private void SetNote(string note)
        {
            _note = note;
            ValidateInput();
        }

        private void SetDate(string date)
        {
            _date = date;
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

        private void SaveData()
        {
            var assigmentData = new AssigmentData.AssigmentData(_name, _subject, _note, _date, _timeHr, _timeMin,
                _selectedIconType, _selectedColorType);

            var sources = _assigmentSourceHolder.GetDatas();
            var steps = _assigmentStepHolder.GetDatas();

            if (sources.Count > 0)
            {
                assigmentData.SourceDatas = sources;
            }

            if (steps.Count > 0)
            {
                assigmentData.StepDatas = steps;
            }

            if (_currentPlane != null)
            {
                _currentPlane.SetData(assigmentData);
                Edited?.Invoke();
                _view.Disable();
                return;
            }

            Saved?.Invoke(assigmentData);
            OnBackClicked();
        }

        private void ValidateInput()
        {
            bool isValid = !string.IsNullOrEmpty(_name) && !string.IsNullOrEmpty(_subject) &&
                           _selectedColorType != ColorType.None
                           && _selectedIconType != IconType.None && !string.IsNullOrEmpty(_date);

            _view.ToggleSaveButton(isValid);
        }

        private void ResetValues()
        {
            _name = string.Empty;
            _subject = string.Empty;
            _note = string.Empty;
            _date = string.Empty;
            _timeHr = 0;
            _timeMin = 0;
            _selectedColorType = ColorType.None;
            _selectedIconType = IconType.None;
            _view.CloseCalendar();
            _view.SetDate(_date);
            _view.SetName(_name);
            _view.SetNote(_note);
            _view.SetSubject(_subject);
            _view.SetTime("Select time");

            foreach (var icon in _icons)
            {
                icon.SetDefault();
            }

            foreach (var color in _colors)
            {
                color.SetDefault();
            }
        }

        private void OnBackClicked()
        {
            BackEdited?.Invoke();
            _view.Disable();
        }
    }
}