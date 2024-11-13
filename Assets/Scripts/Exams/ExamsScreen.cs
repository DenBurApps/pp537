using System;
using System.Collections.Generic;
using System.Linq;
using AddEvent;
using MainScreen;
using UnityEngine;
using UnityEngine.UI;

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

        private ScreenVisabilityHandler _screenVisabilityHandler;

        private void Awake()
        {
            _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
        }

        private void OnEnable()
        {
            _addEventButton.onClick.AddListener(AddEvent);

            _menu.ScheduleClicked += ScheduleOpen;
            _menu.BudgetClicked += BudgetOpen;
            _menu.MainScreenClicked += MainScreenOpen;
        }

        private void OnDisable()
        {
            _addEventButton.onClick.RemoveListener(AddEvent);
            
            _menu.ScheduleClicked -= ScheduleOpen;
            _menu.BudgetClicked -= BudgetOpen;
            _menu.MainScreenClicked -= MainScreenOpen;
        }

        private void Start()
        {
            DisableAllPlanes();
        }

        public void Enable()
        {
            _screenVisabilityHandler.EnableScreen();
            var datas = _eventHolder.Datas;

            if (datas.Count <= 0)
            {
                _emptyPlane.gameObject.SetActive(true);
                return;
            }

            foreach (var data in datas)
            {
                var plane = _planes.FirstOrDefault(plane => !plane.IsActive);
                
                if(plane != null)
                {
                    plane.Enable(data);
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
            _addEventScreen.EnableScreen();
            _screenVisabilityHandler.DisableScreen();
        }
        
        private void MainScreenOpen()
        {
            _screenVisabilityHandler.DisableScreen();
            _mainScreenView.Enable();
        }

        private void ScheduleOpen()
        {
            _screenVisabilityHandler.DisableScreen();
            _scheduleScreen.Enable();
        }

        private void BudgetOpen()
        {
            _budgetScreen.Enable();
            _screenVisabilityHandler.DisableScreen();
        }
    }
}
