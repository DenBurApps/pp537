using System;
using System.Collections.Generic;
using System.Linq;
using AssigmentData;
using UnityEngine;
using UnityEngine.UI;

namespace MainScreen
{
    public class MainScreenAssigmentHolder : MonoBehaviour
    {
        [SerializeField] private GameObject _emptyPlane;
        [SerializeField] private List<AssigmentPlane> _planes;
        [SerializeField] private Button _addAssigmentButton;
        [SerializeField] private Button _allAssigmentsButton;

        public event Action AllAssigmentsClicked;
        public event Action AddAssigmentsClicked;
        
        private void Start()
        {
            DisableAllPlanes();
            _emptyPlane.gameObject.SetActive(ArePlanesActive());
        }

        public void EnableAssigment(AssigmentData.AssigmentData data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            var availablePlane = _planes.FirstOrDefault(plane => !plane.IsActive);

            if (availablePlane != null)
            {
                availablePlane.Enable();
                availablePlane.SetData(data);
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
            return _planes.Any(plane => !plane.IsActive);
        }
    }
}