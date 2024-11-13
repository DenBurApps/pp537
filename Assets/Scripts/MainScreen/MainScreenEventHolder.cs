using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AddEvent;
using EventData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MainScreen
{
    public class MainScreenEventHolder : MonoBehaviour
    {
        [SerializeField] private TMP_Text _dateText;
        [SerializeField] private List<EventPlane> _planes;
        [SerializeField] private MainScreenView _view;
        [SerializeField] private GameObject _emptyPlane;
        [SerializeField] private Button _addEventButton;
        [SerializeField] private AddEventScreen _addEventScreen;

        public List<EventData.EventData> Datas { get; private set; } = new();

        private void Awake()
        {
            Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
        }

        private void OnEnable()
        {
            _addEventScreen.Saved += EnableEvent;
            _addEventButton.onClick.AddListener(OnAddNewEvent);
        }

        private void OnDisable()
        {
            _addEventScreen.Saved -= EnableEvent;
            _addEventButton.onClick.RemoveListener(OnAddNewEvent);
        }

        private void Start()
        {
            DisableAllPlanes();
            _emptyPlane.gameObject.SetActive(ArePlanesActive());
            _dateText.text = DateTime.Now.ToString("ddd, dd.MM");
        }

        private void EnableEvent(EventData.EventData data)
        {
            _view.Enable();

            if (data == null)
                throw new ArgumentNullException(nameof(data));

            
            if (DateTime.TryParseExact(data.Date, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime eventDate))
            {
                if (eventDate.Date != DateTime.Today)
                {
                    Debug.Log(eventDate);
                    Datas.Add(data);
                    return;
                }
            }
            else
            {
                Debug.LogWarning("Invalid date format in data.Data");
                return;
            }
            
            var availablePlane = _planes.FirstOrDefault(plane => !plane.IsActive);

            if (availablePlane != null)
            {
                availablePlane.Enable();
                availablePlane.SetData(data);

                DateTime currentTime = DateTime.Now;
                DateTime eventTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, data.TimeHr,
                    data.TimeMin, 0);

                if (eventTime > currentTime)
                {
                    availablePlane.SetCompleted();

                    if ((eventTime - currentTime).TotalHours <= 1)
                    {
                        availablePlane.SetNextSprite();
                    }
                    else if ((eventTime - currentTime).TotalHours > 2)
                    {
                        availablePlane.SetPlannedSprite();
                    }
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
        }

        private bool ArePlanesActive()
        {
            return _planes.All(plane => !plane.IsActive);
        }

        private void OnAddNewEvent()
        {
            _addEventScreen.EnableScreen();
            _view.Disable();
        }
    }
}