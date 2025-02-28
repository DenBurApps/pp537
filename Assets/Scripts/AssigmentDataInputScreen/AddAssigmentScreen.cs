using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using AddEvent;
using AssigmentData;
using MainScreen;
using UnityEngine;
using DG.Tweening;

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

        [Header("Animation Settings")]
        [SerializeField] private float _animationDuration = 0.3f;
        [SerializeField] private Ease _animationEase = Ease.OutQuad;

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

        private void AddAnimatedTransition(GameObject targetObject)
        {
            // Slide and scale in animation
            targetObject.transform.localPosition = new Vector3(0, -Screen.height, 0);
            targetObject.SetActive(true);
            targetObject.transform.DOLocalMoveY(0, _animationDuration)
                .SetEase(_animationEase);
            
            targetObject.transform.localScale = Vector3.zero;
            targetObject.transform.DOScale(1f, _animationDuration)
                .SetEase(Ease.OutBack);
        }

        private void AddAnimatedPunchScale(Transform targetTransform)
        {
            targetTransform.DOPunchScale(Vector3.one * 0.2f, _animationDuration, 2, 1f)
                .SetEase(_animationEase);
        }

        private void AddAnimatedSourceStep(GameObject targetObject)
        {
            // Simply activate the object without any animation
            if (!targetObject.activeSelf)
            {
                // Reset position to center of screen
                targetObject.transform.localPosition = Vector3.zero;
                targetObject.transform.localScale = Vector3.one;
                
                // Activate the object
                targetObject.SetActive(true);
            }
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

        private void AddStep()
        {
            if (!_assigmentStepHolder.isActiveAndEnabled)
            {
                AddAnimatedSourceStep(_assigmentStepHolder.gameObject);
            }

            _assigmentStepHolder.EnableStep();
        }

        private void AddSource()
        {
            if (!_assigmentSourceHolder.isActiveAndEnabled)
            {
                AddAnimatedSourceStep(_assigmentSourceHolder.gameObject);
            }

            _assigmentSourceHolder.EnableSource();
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
            
            // Add a punch scale animation to the selected icon
            AddAnimatedPunchScale(_currentIcon.transform);
            
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
            
            // Add a punch scale animation to the selected color
            AddAnimatedPunchScale(_currentColor.transform);
            
            ValidateInput();
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

            transform.DOShakePosition(_animationDuration, new Vector3(10f, 0, 0), 10, 90f)
                .SetEase(Ease.InOutQuad);

            if (_currentPlane != null)
            {
                _currentPlane.SetData(assigmentData);
                Edited?.Invoke();
                
                var canvasGroup = _view.GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                {
                    canvasGroup.DOFade(0, _animationDuration)
                        .OnComplete(() => _view.Disable());
                }
                else
                {
                    _view.transform.DOScale(0, _animationDuration)
                        .OnComplete(() => _view.Disable());
                }
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
            transform.DOShakeRotation(_animationDuration, new Vector3(0, 0, 10f), 10, 90f)
                .SetEase(Ease.InOutQuad);

            BackEdited?.Invoke();
            _view.Disable();
        }
    }
}