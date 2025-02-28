using System;
using System.Collections.Generic;
using System.Linq;
using AddEvent;
using Bitsplash.DatePicker;
using DG.Tweening;
using EventData;
using MainScreen;
using UnityEngine;
using UnityEngine.UI;

namespace ScheduleScreen
{
    [RequireComponent(typeof(ScreenVisabilityHandler))]
    public class ScheduleScreen : MonoBehaviour
    {
        [SerializeField] private DatePickerSettings _datePicker;
        [SerializeField] private List<EventPlane> _planes;
        [SerializeField] private GameObject _emptyPlane;
        [SerializeField] private AddEventScreen _addEventScreen;
        [SerializeField] private Button _addEventButton;
        [SerializeField] private EditEvent _editEvent;
        [SerializeField] private MainScreenEventHolder _eventHolder;
        [SerializeField] private Menu _menu;
        [SerializeField] private BudgetScreen _budgetScreen;
        [SerializeField] private Exams.ExamsScreen _examsScreen;

        [Header("Animation Settings")]
        [SerializeField] private float _fadeInDuration = 0.5f;
        [SerializeField] private float _fadeOutDuration = 0.3f;
        [SerializeField] private float _scaleInDuration = 0.4f;
        [SerializeField] private float _moveInDuration = 0.6f;
        [SerializeField] private float _staggerDelay = 0.1f;
        [SerializeField] private Ease _fadeEase = Ease.OutQuad;
        [SerializeField] private Ease _scaleEase = Ease.OutBack;
        [SerializeField] private Ease _moveEase = Ease.OutCubic;

        private ScreenVisabilityHandler _screenVisabilityHandler;
        private CanvasGroup _canvasGroup;
        private RectTransform _rectTransform;
        
        private List<EventData.EventData> _datas = new();

        private EventData.EventData _previousData;
        private EventData.EventData _newData;

        private Sequence _currentSequence;
        private Vector3 _initialScale;
        private Vector2 _initialPosition;

        public event Action<EventPlane> Deleted;
        public event Action MainScreenClicked;
        public event Action<EventData.EventData> PreviousEditedData;
        public event Action<EventData.EventData> NewSavedData;
        
        private DateTime _selectedDate;

        private void Awake()
        {
            _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
            _canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
            _rectTransform = GetComponent<RectTransform>();
            _initialScale = _rectTransform.localScale;
            _initialPosition = _rectTransform.anchoredPosition;
            
            _canvasGroup.alpha = 0f;
        }

        private void OnEnable()
        {
            foreach (var plane in _planes)
            {
                plane.PlaneOpened += EditEvent;
            }
            
            _datePicker.Content.OnSelectionChanged.AddListener(SelectDate);
            _editEvent.Deleted += DeletePlane;
            _addEventButton.onClick.AddListener(AddEvent);
            _menu.MainScreenClicked += OpenMainScreen;
            _editEvent.Saved += SavedNewData;
            _editEvent.BackClicked += _screenVisabilityHandler.EnableScreen;
            _addEventScreen.BackClicked += Enable;
            _menu.ExamsClicked += OpenExams;
            _menu.BudgetClicked += OpenBudget;
        }

        private void OnDisable()
        {
            foreach (var plane in _planes)
            {
                plane.PlaneOpened -= EditEvent;
            }
            
            _datePicker.Content.OnSelectionChanged.RemoveListener(SelectDate);
            _editEvent.Deleted -= DeletePlane;
            _addEventButton.onClick.RemoveListener(AddEvent);
            _editEvent.Saved -= SavedNewData;
            _addEventScreen.BackClicked -= Enable;
            _menu.ExamsClicked -= OpenExams;
            _menu.MainScreenClicked -= OpenMainScreen;
            _editEvent.BackClicked -= _screenVisabilityHandler.EnableScreen;
            _menu.BudgetClicked -= OpenBudget;

            KillAllAnimations();
        }

        private void Start()
        {
            _screenVisabilityHandler.DisableScreen();
            DisableAllEvents();
        }

        public void Enable()
        {
            KillAllAnimations();
            
            _screenVisabilityHandler.EnableScreen();
            
            _canvasGroup.alpha = 0f;
            _rectTransform.localScale = _initialScale * 0.9f;
            
            _currentSequence = DOTween.Sequence();
            
            _currentSequence.Append(_canvasGroup.DOFade(1f, _fadeInDuration).SetEase(_fadeEase));
            _currentSequence.Join(_rectTransform.DOScale(_initialScale, _scaleInDuration).SetEase(_scaleEase));
            
            _currentSequence.OnComplete(() => {
                var datas = _eventHolder.Datas;
                
                if (datas.Count <= 0)
                {
                    AnimateEmptyPlane(true);
                    return;
                }

                SetCurrentDate();

                _datas = datas;

                foreach (var data in datas)
                {
                    if (DateTime.TryParseExact(data.Date, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None,
                            out DateTime eventDate))
                    {
                        if (eventDate.Date != DateTime.Today)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Invalid date format in data.Data");
                        continue;
                    }

                    var availablePlane = _planes.FirstOrDefault(plane => !plane.IsActive);

                    if (availablePlane != null)
                    {
                        availablePlane.Enable();
                        availablePlane.SetData(data);
                        availablePlane.SetPlannedSprite();
                        AnimatePlaneIn(availablePlane);
                    }

                    AnimateEmptyPlane(false);
                }
            });
            
            _currentSequence.Play();
        }

        private void SelectDate()
        {
            var selection = _datePicker.Content.Selection;
            for (int i = 0; i < selection.Count; i++)
            {
                _selectedDate = selection.GetItem(i);
            }
            
            EnablePlanesForSelectedDate();
        }

        private void DisableAllEvents()
        {
            foreach (var plane in _planes)
            {
                plane.Disable();
            }
        }

        public void SetCurrentDate()
        {
            var selection = _datePicker.Content.Selection;
            selection.SelectOne(DateTime.Today);
        }

        private void EditEvent(EventPlane plane)
        {
            _previousData = plane.EventData;
            
            plane.transform.DOScale(1.05f, 0.2f).SetEase(Ease.OutQuad)
                .OnComplete(() => {
                    plane.transform.DOScale(1f, 0.2f).SetEase(Ease.InOutQuad);
                    _editEvent.EnableScreen(plane);
                });
        }

        private void SavedNewData(EventData.EventData data)
        {
            PreviousEditedData?.Invoke(_previousData);
            
            if (_datas.Contains(_previousData))
            {
                _datas.Remove(_previousData);
            }
            
            _datas.Add(data);
            NewSavedData?.Invoke(data);
            
            DOVirtual.DelayedCall(0.2f, () => EnablePlanesForSelectedDate());
        }
        
        private void EnablePlanesForSelectedDate()
        {
            var currentPlanes = _planes.Where(plane => plane.IsActive).ToList();
            
            Sequence fadeOutSequence = DOTween.Sequence();
            
            foreach (var plane in currentPlanes)
            {
                fadeOutSequence.Join(plane.transform.DOScale(0.9f, _fadeOutDuration).SetEase(Ease.InQuad));
                fadeOutSequence.Join(plane.GetComponent<CanvasGroup>()?.DOFade(0f, _fadeOutDuration).SetEase(Ease.InQuad));
            }
            
            fadeOutSequence.OnComplete(() => {
                DisableAllEvents();
                
                var eventsForSelectedDate = _datas.Where(data =>
                {
                    if (DateTime.TryParseExact(data.Date, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime eventDate))
                    {
                        return eventDate.Date == _selectedDate.Date;
                    }
                    else
                    {
                        Debug.LogWarning("Invalid date format in data.Date");
                        return false;
                    }
                }).ToList();
                
                if (eventsForSelectedDate.Count == 0)
                {
                    AnimateEmptyPlane(true);
                    return;
                }
                
                AnimateEmptyPlane(false);
                
                Sequence fadeInSequence = DOTween.Sequence();
                
                for (int i = 0; i < eventsForSelectedDate.Count; i++)
                {
                    var data = eventsForSelectedDate[i];
                    var availablePlane = _planes.FirstOrDefault(plane => !plane.IsActive);

                    if (availablePlane != null)
                    {
                        availablePlane.Enable();
                        availablePlane.SetData(data);
                        availablePlane.SetPlannedSprite();
                        
                        CanvasGroup planeCanvasGroup = availablePlane.GetComponent<CanvasGroup>() ?? availablePlane.gameObject.AddComponent<CanvasGroup>();
                        planeCanvasGroup.alpha = 0f;
                        availablePlane.transform.localScale = Vector3.one * 0.8f;
                        
                        fadeInSequence.Insert(i * _staggerDelay, planeCanvasGroup.DOFade(1f, _fadeInDuration).SetEase(_fadeEase));
                        fadeInSequence.Insert(i * _staggerDelay, availablePlane.transform.DOScale(1f, _scaleInDuration).SetEase(_scaleEase));
                    }
                    else
                    {
                        Debug.LogWarning("No available planes to display the event.");
                    }
                }
                
                fadeInSequence.Play();
            });
            
            fadeOutSequence.Play();
        }

        private void DeletePlane(EventPlane plane)
        {
            if (_planes.Contains(plane))
            {
                CanvasGroup planeCanvasGroup = plane.GetComponent<CanvasGroup>() ?? plane.gameObject.AddComponent<CanvasGroup>();
                
                Sequence deleteSequence = DOTween.Sequence();
                deleteSequence.Append(planeCanvasGroup.DOFade(0f, _fadeOutDuration).SetEase(Ease.InQuad));
                deleteSequence.Join(plane.transform.DOScale(0.8f, _fadeOutDuration).SetEase(Ease.InBack));
                deleteSequence.OnComplete(() => {
                    plane.Reset();
                    plane.Disable();
                    Deleted?.Invoke(plane);
                    
                    bool hasActiveEvents = _planes.Any(p => p.IsActive);
                    AnimateEmptyPlane(!hasActiveEvents);
                });
                
                deleteSequence.Play();
            }
            else
            {
                Deleted?.Invoke(plane);
            }
        }

        private void AddEvent()
        {
            _canvasGroup.DOFade(0f, _fadeOutDuration).SetEase(_fadeEase)
                .OnComplete(() => {
                    _screenVisabilityHandler.DisableScreen();
                    _addEventScreen.EnableScreen();
                });
        }

        private void OpenMainScreen()
        {
            Sequence exitSequence = DOTween.Sequence();
            exitSequence.Append(_canvasGroup.DOFade(0f, _fadeOutDuration).SetEase(_fadeEase));
            exitSequence.Join(_rectTransform.DOScale(_initialScale * 0.9f, _fadeOutDuration).SetEase(Ease.InBack));
            
            exitSequence.OnComplete(() => {
                MainScreenClicked?.Invoke();
                _screenVisabilityHandler.DisableScreen();
            });
            
            exitSequence.Play();
        }

        private void OpenExams()
        {
            _rectTransform.DOAnchorPos(_initialPosition + new Vector2(Screen.width, 0), _fadeOutDuration).SetEase(_moveEase)
                .OnComplete(() => {
                    _examsScreen.Enable();
                    _screenVisabilityHandler.DisableScreen();
                    _rectTransform.anchoredPosition = _initialPosition;
                });
        }

        private void OpenBudget()
        {
            _rectTransform.DOAnchorPos(_initialPosition + new Vector2(-Screen.width, 0), _fadeOutDuration).SetEase(_moveEase)
                .OnComplete(() => {
                    _budgetScreen.Enable();
                    _screenVisabilityHandler.DisableScreen();
                    _rectTransform.anchoredPosition = _initialPosition;
                });
        }
        
        private void AnimatePlaneIn(EventPlane plane)
        {
            CanvasGroup planeCanvasGroup = plane.GetComponent<CanvasGroup>() ?? plane.gameObject.AddComponent<CanvasGroup>();
            
            planeCanvasGroup.alpha = 0f;
            plane.transform.localScale = Vector3.one * 0.8f;
            
            Sequence planeSequence = DOTween.Sequence();
            planeSequence.Append(planeCanvasGroup.DOFade(1f, _fadeInDuration).SetEase(_fadeEase));
            planeSequence.Join(plane.transform.DOScale(1f, _scaleInDuration).SetEase(_scaleEase));
            
            planeSequence.Play();
        }
        
        private void AnimateEmptyPlane(bool show)
        {
            if (_emptyPlane == null) return;
            
            CanvasGroup emptyPlaneCanvasGroup = _emptyPlane.GetComponent<CanvasGroup>() ?? _emptyPlane.AddComponent<CanvasGroup>();
            
            if (show)
            {
                _emptyPlane.SetActive(true);
                emptyPlaneCanvasGroup.alpha = 0f;
                
                Sequence emptySequence = DOTween.Sequence();
                emptySequence.Append(emptyPlaneCanvasGroup.DOFade(1f, _fadeInDuration).SetEase(_fadeEase));
                emptySequence.Play();
            }
            else
            {
                Sequence emptySequence = DOTween.Sequence();
                emptySequence.Append(emptyPlaneCanvasGroup.DOFade(0f, _fadeOutDuration).SetEase(_fadeEase));
                emptySequence.OnComplete(() => _emptyPlane.SetActive(false));
                emptySequence.Play();
            }
        }
        
        private void KillAllAnimations()
        {
            _currentSequence?.Kill();
            DOTween.Kill(_canvasGroup);
            DOTween.Kill(_rectTransform);
            
            foreach (var plane in _planes)
            {
                DOTween.Kill(plane.transform);
                if (plane.GetComponent<CanvasGroup>() != null)
                {
                    DOTween.Kill(plane.GetComponent<CanvasGroup>());
                }
            }
            
            if (_emptyPlane != null && _emptyPlane.GetComponent<CanvasGroup>() != null)
            {
                DOTween.Kill(_emptyPlane.GetComponent<CanvasGroup>());
            }
        }

        private void OnDestroy()
        {
            KillAllAnimations();
        }
    }
}