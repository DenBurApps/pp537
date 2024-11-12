using System.Collections.Generic;
using AssigmentData;
using UnityEngine;

namespace AssigmentDataInputScreen
{
    public class AddAssigmentScreen : MonoBehaviour
    {
        [SerializeField] private AddAssigmentScreenView _view;
        [SerializeField] private AssigmentSourceHolder _assigmentSourceHolder;
        [SerializeField] private AssigmentStepHolder _assigmentStepHolder;
        [SerializeField] private List<AssigmentIcon> _icons;
        [SerializeField] private List<AssigmentColor> _colors;
        
        
        private string _name;
        private string _subject;
        private int _timeHr;
        private int _timeMin;
        private IconType _selectedIconType;
        private ColorType _selectedColorType;

        private AssigmentIcon _currentIcon;
        private AssigmentColor _currentColor;
    
        private void Start()
        {
            _view.Disable();
            _assigmentSourceHolder.gameObject.SetActive(false);
            _assigmentStepHolder.gameObject.SetActive(false);
        }
    
    
    }
}
