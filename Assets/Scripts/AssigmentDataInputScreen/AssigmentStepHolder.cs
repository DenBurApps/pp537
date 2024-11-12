using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AssigmentDataInputScreen
{
    public class AssigmentStepHolder : MonoBehaviour
    {
        [SerializeField] private List<AssigmentStep> _steps;

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

        private void DisableStep(AssigmentStep step)
        {
            step.Disable();
        }

        private void DisableAllSteps()
        {
            foreach (var step in _steps)
            {
                step.Disable();
            }
        }
    }
}