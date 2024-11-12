using System;
using System.Collections.Generic;
using System.Linq;
using AllAssigments;
using AssigmentData;
using AssigmentDataInputScreen;
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

        public List<AssigmentData.AssigmentData> Datas { get; private set; } = new (); 
        
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
        }

        private void Start()
        {
            DisableAllPlanes();
            _emptyPlane.gameObject.SetActive(ArePlanesActive());
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
                Datas.Add(data);
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
        }
    }
}