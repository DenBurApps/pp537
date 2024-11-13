using System;
using System.Collections.Generic;
using System.Linq;
using AssigmentData;
using AssigmentDataInputScreen;
using MainScreen;
using UnityEngine;
using UnityEngine.UI;

namespace AllAssigments
{
    [RequireComponent(typeof(ScreenVisabilityHandler))]
    public class AllAssigmentsScreen : MonoBehaviour
    {
        [SerializeField] private AssigmentIconHolder _iconHolder;
        [SerializeField] private AssigmentColorHolder _colorHolder;
        [SerializeField] private GameObject _emptyPlane;
        [SerializeField] private AddAssigmentScreen _addAssigmentScreen;
        [SerializeField] private AddAssigmentScreen _editAssigmentScreen;
        [SerializeField] private MainScreenAssigmentHolder _mainScreenAssigmentHolder;
        [SerializeField] private List<AssigmentPlane> _currentAssigments;
        [SerializeField] private List<AssigmentPlane> _checkedAssigments;
        [SerializeField] private List<AssigmentPlane> _deletedAssigments;
        [SerializeField] private Button _addAssigmentButton;
        [SerializeField] private Button _backButtonClicked;

        private ScreenVisabilityHandler _screenVisabilityHandler;

        public event Action BackClicked;

        private void Awake()
        {
            _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
        }

        private void OnEnable()
        {
            foreach (var plane in _currentAssigments)
            {
                plane.SetHolders(_colorHolder, _iconHolder);
                plane.GetComponent<AssigmentEditPlane>().OpenClicked += EditAssigment;
            }

            foreach (var plane in _checkedAssigments)
            {
                plane.SetHolders(_colorHolder, _iconHolder);
            }

            foreach (var plane in _deletedAssigments)
            {
                plane.SetHolders(_colorHolder, _iconHolder);
            }
            
            _addAssigmentButton.onClick.AddListener(AddNewAssigment);
            _addAssigmentScreen.Edited += UpdateAllPlanes;
            _addAssigmentScreen.BackEdited += UpdateAllPlanes;
            _editAssigmentScreen.BackEdited += UpdateAllPlanes;
            _backButtonClicked.onClick.AddListener(OnBackClicked);
        }

        private void OnDisable()
        {
            _addAssigmentButton.onClick.RemoveListener(AddNewAssigment);
            _addAssigmentScreen.Edited -= UpdateAllPlanes;
            _addAssigmentScreen.BackEdited -= UpdateAllPlanes;
            _editAssigmentScreen.BackEdited -= UpdateAllPlanes;
            _backButtonClicked.onClick.RemoveListener(OnBackClicked);
            
            foreach (var plane in _currentAssigments)
            {
                plane.GetComponent<AssigmentEditPlane>().OpenClicked -= EditAssigment;
            }
        }

        private void Start()
        {
            _screenVisabilityHandler.DisableScreen();
            DisableAllAssigments();
        }

        public void Enable()
        {
            _screenVisabilityHandler.EnableScreen();
            EnableAssigments(_mainScreenAssigmentHolder.Datas);
        }

        public void EnableAssigments(List<AssigmentData.AssigmentData> datas)
        {
            if (datas.Count <= 0)
            {
                _emptyPlane.gameObject.SetActive(true);
                return;
            }

            foreach (var data in datas)
            {
                if (data.IsSelected)
                {
                    var plane = _checkedAssigments.FirstOrDefault(plane => !plane.IsActive);

                    if (plane != null)
                    {
                        plane.Enable();
                        plane.SetData(data);
                    }
                }
                else
                {
                    var plane = _currentAssigments.FirstOrDefault(plane => !plane.IsActive);

                    if (plane != null)
                    {
                        plane.Enable();
                        plane.SetData(data);
                    }
                }

                _emptyPlane.gameObject.SetActive(false);
            }
        }

        private void DisableAllAssigments()
        {
            foreach (var plane in _currentAssigments)
            {
                plane.Disable();
            }

            foreach (var plane in _checkedAssigments)
            {
                plane.Disable();
            }

            foreach (var plane in _deletedAssigments)
            {
                plane.Disable();
            }
        }

        private void AddNewAssigment()
        {
            _addAssigmentScreen.EnableScreen();
            _screenVisabilityHandler.DisableScreen();
        }

        private void EditAssigment(AssigmentPlane plane)
        {
            _editAssigmentScreen.EnableScreen(plane);
            _screenVisabilityHandler.DisableScreen();
        }

        private void UpdateAllPlanes()
        {
            _screenVisabilityHandler.EnableScreen();
            
            foreach (var plane in _currentAssigments)
            {
                if (plane.IsActive)
                {
                    plane.UpdateText();
                }
            }
        }

        private void OnBackClicked()
        {
            BackClicked?.Invoke();
            _screenVisabilityHandler.DisableScreen();
        }
    }
}