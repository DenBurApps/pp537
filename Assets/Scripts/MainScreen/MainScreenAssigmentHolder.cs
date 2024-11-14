using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AllAssigments;
using AssigmentData;
using AssigmentDataInputScreen;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace MainScreen
{
    public class MainScreenAssigmentHolder : MonoBehaviour
    {
        [SerializeField] private MainScreenView _view;
        [SerializeField] private AssigmentIconHolder _iconHolder;
        [SerializeField] private AssigmentColorHolder _colorHolder;
        [SerializeField] private GameObject _emptyPlane;
        [SerializeField] private List<AssigmentPlane> _planes;
        [SerializeField] private Button _addAssigmentButton;
        [SerializeField] private Button _allAssigmentsButton;
        [SerializeField] private AddAssigmentScreen _addAssigmentScreen;
        [SerializeField] private AllAssigmentsScreen _allAssigmentsScreen;

        private string _savePath;

        public List<AssigmentData.AssigmentData> Datas { get; private set; } = new();

        private void Awake()
        {
            _savePath = Path.Combine(Application.persistentDataPath, "AssigmentsData.json");
        }

        private void OnEnable()
        {
            foreach (var plane in _planes)
            {
                plane.SetHolders(_colorHolder, _iconHolder);
                plane.Checked += PlaneChecked;
            }

            _addAssigmentScreen.Saved += EnableAssigment;
            _addAssigmentButton.onClick.AddListener(OpenAddAssigment);
            _allAssigmentsButton.onClick.AddListener(OpenAllAssigments);
            _allAssigmentsScreen.UpdatedDatas += UpdateAllDatas;
        }

        private void OnDisable()
        {
            foreach (var plane in _planes)
            {
                plane.Checked -= PlaneChecked;
            }

            _addAssigmentButton.onClick.RemoveListener(OpenAddAssigment);
            _addAssigmentScreen.Saved -= EnableAssigment;
            _allAssigmentsButton.onClick.RemoveListener(OpenAllAssigments);
            _allAssigmentsScreen.UpdatedDatas -= UpdateAllDatas;
        }

        private void Start()
        {
            DisableAllPlanes();
            _emptyPlane.gameObject.SetActive(ArePlanesActive());
            Load();
        }

        private void EnableAssigment(AssigmentData.AssigmentData data)
        {
            _view.Enable();

            if (data == null)
                throw new ArgumentNullException(nameof(data));

            if (data.IsSelected)
                return;

            var availablePlane = _planes.FirstOrDefault(plane => !plane.IsActive);

            if (availablePlane != null)
            {
                availablePlane.Enable();
                availablePlane.SetData(data);

                if (!Datas.Contains(data))
                    Datas.Add(data);
            }

            Save();

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

        private void OpenAddAssigment()
        {
            _addAssigmentScreen.EnableScreen();
            _view.Disable();
        }

        private void OpenAllAssigments()
        {
            _allAssigmentsScreen.Enable();
            _view.Disable();
        }

        private void PlaneChecked(AssigmentPlane plane)
        {
            if (plane.IsActive)
            {
                plane.Disable();
            }

            _emptyPlane.gameObject.SetActive(ArePlanesActive());
            Save();
        }

        private void UpdateAllDatas(List<AssigmentData.AssigmentData> datas)
        {
            for (int i = 0; i < datas.Count; i++)
            {
                if (_planes[i].IsActive)
                {
                    _planes[i].SetData(datas[i]);
                }
            }

            Save();
        }

        private void Save()
        {
            try
            {
                string jsonData = JsonUtility.ToJson(new AssignmentDataListWrapper(Datas), true);
                File.WriteAllText(_savePath, jsonData);
                Debug.Log($"Data saved successfully to {_savePath}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to save data to {_savePath}: {e.Message}");
            }
        }

        public void Load()
        {
            try
            {
                if (File.Exists(_savePath))
                {
                    string json = File.ReadAllText(_savePath);

                    AssignmentDataListWrapper dataWrapper = JsonUtility.FromJson<AssignmentDataListWrapper>(json);
                    List<AssigmentData.AssigmentData> loadedData =
                        dataWrapper?.Data ?? new List<AssigmentData.AssigmentData>();

                    Datas.Clear();
                    DisableAllPlanes();


                    foreach (var data in loadedData)
                    {
                        EnableAssigment(data);
                    }

                    Debug.Log("Assignment data loaded successfully.");
                }
                else
                {
                    Debug.Log("No saved assignment data found.");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load assignment data: {e.Message}");
            }
        }
    }

    [Serializable]
    public class AssignmentDataListWrapper
    {
        public List<AssigmentData.AssigmentData> Data;

        public AssignmentDataListWrapper(List<AssigmentData.AssigmentData> data)
        {
            Data = data;
        }
    }
}