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
        [SerializeField] private Button _backButton;
        [SerializeField] private GameObject _emptyPlane;
        [SerializeField] private MainScreenEventHolder _eventHolder;

        private ScreenVisabilityHandler _screenVisabilityHandler;

        private void Awake()
        {
            _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
        }

        public void Enable()
        {
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
        }

        private bool ArePlanesActive()
        {
            return _planes.All(plane => !plane.IsActive);
        }
    }
}
