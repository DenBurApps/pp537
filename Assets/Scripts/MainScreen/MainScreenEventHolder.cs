using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using AddEvent;
using EventData;
using Exams;
using TMPro;
using Unity.VisualScripting;
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
        [SerializeField] private ScheduleScreen.ScheduleScreen _scheduleScreen;
        [SerializeField] private Menu _menu;
        [SerializeField] private ExamsScreen _examsScreen;

        private string _savePath;

        public List<EventData.EventData> Datas { get; private set; } = new();

        private void Awake()
        {
            _savePath = Path.Combine(Application.persistentDataPath, "EventData.json");
        }

        private void OnEnable()
        {
            _addEventScreen.Saved += EnableEvent;
            _addEventButton.onClick.AddListener(OnAddNewEvent);
            _scheduleScreen.Deleted += DeletePlane;
            _menu.ScheduleClicked += OpenSchedule;
            _scheduleScreen.MainScreenClicked += UpdatePlanes;
            _scheduleScreen.PreviousEditedData += RemoveOldData;
            _scheduleScreen.NewSavedData += SaveNewData;
            _examsScreen.NewDataSaved += SaveNewData;
        }

        private void OnDisable()
        {
            _addEventScreen.Saved -= EnableEvent;
            _addEventButton.onClick.RemoveListener(OnAddNewEvent);
            _scheduleScreen.Deleted -= DeletePlane;
            _menu.ScheduleClicked -= OpenSchedule;
            _scheduleScreen.MainScreenClicked -= UpdatePlanes;
            _scheduleScreen.PreviousEditedData -= RemoveOldData;
            _scheduleScreen.NewSavedData -= SaveNewData;
            _examsScreen.NewDataSaved -= SaveNewData;
        }

        private void Start()
        {
            DisableAllPlanes();
            _emptyPlane.gameObject.SetActive(ArePlanesActive());
            _dateText.text = DateTime.Now.ToString("ddd, dd.MM");
            Load();
        }

        private void EnableEvent(EventData.EventData data)
        {
            _view.Enable();

            if (data == null)
                throw new ArgumentNullException(nameof(data));


            if (DateTime.TryParseExact(data.Date, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None,
                    out DateTime eventDate))
            {
                if (eventDate.Date != DateTime.Today)
                {
                    Datas.Add(data);
                    Save();
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
                Datas.Add(data);

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

            Save();
            _emptyPlane.gameObject.SetActive(ArePlanesActive());
        }

        private void UpdatePlanes()
        {
            _view.Enable();
            UpdatePlanesDatas();
            EnableTodaysPlanes();
        }

        private void UpdatePlanesDatas()
        {
            Datas.Clear();

            foreach (var plane in _planes)
            {
                if (plane.EventData != null)
                {
                    Datas.Add(plane.EventData);
                }
            }

            Save();
        }

        private void RemoveOldData(EventData.EventData data)
        {
            if (Datas.Contains(data))
            {
                Datas.Remove(data);
            }

            Save();
        }

        private void SaveNewData(EventData.EventData data)
        {
            if (!Datas.Contains(data))
                Datas.Add(data);
            
            Save();
        }

        private void EnableTodaysPlanes()
        {
            DisableAllPlanes();

            var todaysEvents = Datas.Where(data =>
            {
                if (DateTime.TryParseExact(data.Date, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None,
                        out DateTime eventDate))
                {
                    return eventDate.Date == DateTime.Today;
                }
                else
                {
                    Debug.LogWarning("Invalid date format in data.Date");
                    return false;
                }
            }).ToList();

            if (todaysEvents.Count == 0)
            {
                _emptyPlane.SetActive(true);
                return;
            }

            _emptyPlane.SetActive(false);

            foreach (var data in todaysEvents)
            {
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
            }
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

        private void DeletePlane(EventPlane plane)
        {
            if (Datas.Contains(plane.EventData))
            {
                Datas.Remove(plane.EventData);
            }

            if (_planes.Contains(plane))
            {
                plane.Reset();
                plane.Disable();
            }

            Save();
        }

        private void OpenSchedule()
        {
            _scheduleScreen.Enable();
            _view.Disable();
        }

        private void Save()
        {
            try
            {
                string jsonData = JsonUtility.ToJson(new ActiveEventDataList(Datas), true);

                File.WriteAllText(_savePath, jsonData);

                Debug.Log($"Data saved successfully to {_savePath}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to save data to {_savePath}: {e.Message}");
            }
        }

        private void Load()
        {
            try
            {
                if (File.Exists(_savePath))
                {
                    string jsonData = File.ReadAllText(_savePath);


                    ActiveEventDataList loadedData = JsonUtility.FromJson<ActiveEventDataList>(jsonData);
                    Datas = loadedData?.Data ?? new List<EventData.EventData>();

                    Debug.Log("Data loaded successfully.");
                    EnableTodaysPlanes();
                }
                else
                {
                    Debug.Log("No saved data found.");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load data from {_savePath}: {e.Message}");
            }
        }
    }
}

[Serializable]
public class ActiveEventDataList
{
    public List<EventData.EventData> Data;

    public ActiveEventDataList(List<EventData.EventData> data)
    {
        Data = data;
    }
}