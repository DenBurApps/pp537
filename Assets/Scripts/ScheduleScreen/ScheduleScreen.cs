using System;
using System.Collections.Generic;
using System.Linq;
using AddEvent;
using Bitsplash.DatePicker;
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

        private ScreenVisabilityHandler _screenVisabilityHandler;

        private List<EventData.EventData> _datas = new();

        private EventData.EventData _previousData;
        private EventData.EventData _newData;

        public event Action<EventPlane> Deleted;
        public event Action MainScreenClicked;
        public event Action<EventData.EventData> PreviousEditedData;
        public event Action<EventData.EventData> NewSavedData;
        
        
        private DateTime _selectedDate;

        private void Awake()
        {
            _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
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
            _menu.MainScreenClicked += (() => MainScreenClicked?.Invoke());
            _editEvent.Saved += SavedNewData;
            _addEventScreen.BackClicked += Enable;
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
        }

        private void Start()
        {
            _screenVisabilityHandler.DisableScreen();
            DisableAllEvents();
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
                }

                _emptyPlane.gameObject.SetActive(false);
            }
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
            _editEvent.EnableScreen(plane);
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
            EnablePlanesForSelectedDate();
        }
        
        private void EnablePlanesForSelectedDate()
        {
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
                _emptyPlane.SetActive(true);
                return;
            }
            
            _emptyPlane.SetActive(false);
            
            foreach (var data in eventsForSelectedDate)
            {
                var availablePlane = _planes.FirstOrDefault(plane => !plane.IsActive);

                if (availablePlane != null)
                {
                    availablePlane.Enable();
                    availablePlane.SetData(data);
                    availablePlane.SetPlannedSprite();
                }
                else
                {
                    Debug.LogWarning("No available planes to display the event.");
                }
            }
        }

        private void DeletePlane(EventPlane plane)
        {
            if (_planes.Contains(plane))
            {
                plane.Reset();
                plane.Disable();
            }
            
            Deleted?.Invoke(plane);
        }

        private void AddEvent()
        {
            _screenVisabilityHandler.DisableScreen();
            _addEventScreen.EnableScreen();
        }
    }
}