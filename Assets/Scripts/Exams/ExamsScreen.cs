using System;
using System.Collections.Generic;
using System.Linq;
using AddEvent;
using MainScreen;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Exams
{
    [RequireComponent(typeof(ScreenVisabilityHandler))]
    public class ExamsScreen : MonoBehaviour
    {
        [SerializeField] private List<ExamsPlane> _planes;
        [SerializeField] private AddEventScreen _addEventScreen;
        [SerializeField] private Menu _menu;
        [SerializeField] private Button _addEventButton;
        [SerializeField] private GameObject _emptyPlane;
        [SerializeField] private MainScreenEventHolder _eventHolder;
        [SerializeField] private BudgetScreen _budgetScreen;
        [SerializeField] private MainScreenView _mainScreenView;
        [SerializeField] private ScheduleScreen.ScheduleScreen _scheduleScreen;

        [Header("Animation Settings")]
        [SerializeField] private float _planeEntryDuration = 0.5f;
        [SerializeField] private Ease _planeEntryEase = Ease.OutBack;
        [SerializeField] private float _screenTransitionDuration = 0.3f;
        [SerializeField] private Ease _screenTransitionEase = Ease.InOutQuad;

        private ScreenVisabilityHandler _screenVisabilityHandler;

        public event Action<EventData.EventData> NewDataSaved; 

        private void Awake()
        {
            _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
        }

        private void Start()
        {
            transform.localScale = Vector3.zero;
            _screenVisabilityHandler.DisableScreen();
            DisableAllPlanes();
        }

        private void OnEnable()
        {
            _addEventButton.onClick.AddListener(AddEvent);

            _menu.ScheduleClicked += ScheduleOpen;
            _menu.BudgetClicked += BudgetOpen;
            _menu.MainScreenClicked += MainScreenOpen;

            foreach (var plane in _planes)
            {
                plane.NewData += data => NewDataSaved?.Invoke(data);
            }
        }

        private void OnDisable()
        {
            _addEventButton.onClick.RemoveListener(AddEvent);
            
            _menu.ScheduleClicked -= ScheduleOpen;
            _menu.BudgetClicked -= BudgetOpen;
            _menu.MainScreenClicked -= MainScreenOpen;
        }

        public void Enable()
        {
            transform.DOScale(1f, _screenTransitionDuration)
                .SetEase(_screenTransitionEase);

            _screenVisabilityHandler.EnableScreen();
            DisableAllPlanes();
            var datas = _eventHolder.Datas;
            
            Debug.Log(_eventHolder.Datas.Count);

            if (datas.Count <= 0)
            {
                _emptyPlane.transform.localScale = Vector3.zero;
                _emptyPlane.gameObject.SetActive(true);
                _emptyPlane.transform.DOScale(1f, _planeEntryDuration)
                    .SetEase(_planeEntryEase);
                return;
            }

            var assignedData = _planes
                .Where(plane => plane.IsActive)
                .Select(plane => plane.Plane.EventData)
                .ToHashSet();
            
            for (int i = 0; i < datas.Count; i++)
            {
                var data = datas[i];
                if (assignedData.Contains(data))
                {
                    continue;
                }

                var plane = _planes.FirstOrDefault(p => !p.IsActive);

                if (plane != null)
                {
                    plane.Enable(data);
                    assignedData.Add(data);
                }
            }

            _emptyPlane.gameObject.SetActive(ArePlanesActive());
        }

        private void DisableAllPlanes()
        {
            foreach (var plane in _planes)
            {
                plane.Disable();
            }
            _emptyPlane.gameObject.SetActive(ArePlanesActive());
        }

        private bool ArePlanesActive()
        {
            return _planes.All(plane => !plane.IsActive);
        }

        private void AddEvent()
        {
            transform.DOScale(0f, _screenTransitionDuration)
                .SetEase(_screenTransitionEase)
                .OnComplete(() => 
                {
                    _addEventScreen.EnableScreen();
                    _screenVisabilityHandler.DisableScreen();
                });

            _addEventButton.transform.DOShakeScale(0.3f, 0.5f);
        }
        
        private void MainScreenOpen()
        {
            transform.DOScale(0f, _screenTransitionDuration)
                .SetEase(_screenTransitionEase)
                .OnComplete(() => 
                {
                    _screenVisabilityHandler.DisableScreen();
                    _mainScreenView.Enable();
                });
        }

        private void ScheduleOpen()
        {
            transform.DOScale(0f, _screenTransitionDuration)
                .SetEase(_screenTransitionEase)
                .OnComplete(() => 
                {
                    _screenVisabilityHandler.DisableScreen();
                    _scheduleScreen.Enable();
                });
        }

        private void BudgetOpen()
        {
            transform.DOScale(0f, _screenTransitionDuration)
                .SetEase(_screenTransitionEase)
                .OnComplete(() => 
                {
                    _budgetScreen.Enable();
                    _screenVisabilityHandler.DisableScreen();
                });
        }
    }
}