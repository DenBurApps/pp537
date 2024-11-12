using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AssigmentDataInputScreen
{
    public class AssigmentStepHolder : MonoBehaviour
    {
        [SerializeField] private List<AssigmentStep> _steps;

        public event Action AllPlanesDisabled;
        
        private void OnEnable()
        {
            DisableAllSteps();
        }

        private void OnDisable()
        {
            foreach (var step in _steps)
            {
                step.Deleted -= DisableStep;
            }
        }

        public void EnableStep()
        {
            var step = _steps.FirstOrDefault(step => step.IsActive == false);

            if (step!)
            {
                step.Enable();
                step.Deleted += DisableStep;
            }
        }
        
        public List<AssigmentStepData> GetDatas()
        {
            return _steps
                .Where(source => source.Data != null)
                .Select(source => source.Data)
                .ToList();
        }

        private void DisableStep(AssigmentStep step)
        {
            step.Disable();
            
            if(AreAllStepsDisabled())
            {
                AllPlanesDisabled?.Invoke();
            }
        }

        private void DisableAllSteps()
        {
            foreach (var step in _steps)
            {
                step.Disable();
            }
        }
        
        private bool AreAllStepsDisabled()
        {
            return _steps.All(source => !source.IsActive);
        }
    }
}