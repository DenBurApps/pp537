using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using AssigmentData;
using UnityEngine;

namespace AssigmentDataInputScreen
{
    public class AddAssigmentScreen : MonoBehaviour
    {
        [SerializeField] private Color _selectedIconColor;
        
        [SerializeField] private AddAssigmentScreenView _view;
        [SerializeField] private AssigmentSourceHolder _assigmentSourceHolder;
        [SerializeField] private AssigmentStepHolder _assigmentStepHolder;
        [SerializeField] private List<AssigmentIcon> _icons;
        [SerializeField] private List<AssigmentColor> _colors;
        
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

        public event Action<AssigmentData.AssigmentData> Saved;
        
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
            _view.TimeInputed += SetTime;
            _view.SubjectInputed += SetSubject;

            _view.AddStepClicked += AddStep;
            _view.AddSourceClicked += AddSource;
            _view.SaveClicked += SaveData;
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
            _view.TimeInputed -= SetTime;
            _view.SubjectInputed -= SetSubject;

            _view.AddStepClicked -= AddStep;
            _view.AddSourceClicked -= AddSource;
            _view.SaveClicked -= SaveData;
        }

        private void Start()
        {
            //_view.Disable();
            _assigmentSourceHolder.gameObject.SetActive(false);
            _assigmentStepHolder.gameObject.SetActive(false);
            ResetValues();
            ValidateInput();
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

        private void SetTime(string input)
        {
            TryParseTime(input, out _timeHr, out _timeMin);
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
            
            Saved?.Invoke(assigmentData);
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
    }
}