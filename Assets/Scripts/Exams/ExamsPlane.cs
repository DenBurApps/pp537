using System;
using System.Collections.Generic;
using System.Linq;
using EventData;
using UnityEngine;
using UnityEngine.UI;

namespace Exams
{
    public class ExamsPlane : MonoBehaviour
    {
        [SerializeField] private EventPlane _plane;
        [SerializeField] private List<ExamStep> _steps;
        [SerializeField] private Button _addStepButton;

        public event Action<EventData.EventData> NewData;
        public bool IsActive { get; private set; }

        private void OnEnable()
        {
            _addStepButton.onClick.AddListener(EnableStep);

            foreach (var step in _steps)
            {
                step.ExamDataChanged += UpdateExamData;
            }
        }

        private void OnDisable()
        {
            _addStepButton.onClick.RemoveListener(EnableStep);

            foreach (var step in _steps)
            {
                step.ExamDataChanged -= UpdateExamData;
            }
        }

        public void Enable(EventData.EventData data)
        {
            if(!data.IsExam)
                return;
            
            gameObject.SetActive(true);
            IsActive = true;
            _plane.SetData(data);
            DisableAllSteps();

            if (_plane.EventData.ExamData.Steps.Count > 0)
            {
                foreach (var examStepData in _plane.EventData.ExamData.Steps)
                {
                    var step = _steps.FirstOrDefault(step => !step.IsActive);

                    if (step != null)
                    {
                        step.Enable(examStepData);
                    }
                }
            }
        }

        public void Disable()
        {
            gameObject.SetActive(false);
            IsActive = false;
        }
        
        private void EnableStep()
        {
            var step = _steps.FirstOrDefault(step => !step.IsActive);

            if (step != null)
            {
                step.Enable();
                _plane.EventData.ExamData.Steps.Add(step.StepData);
            }
            
            NewData?.Invoke(_plane.EventData);
        }
        
        private void DisableAllSteps()
        {
            foreach (var step in _steps)
            {
                step.Disable();
            }
        }

        private void UpdateExamData(ExamStep step)
        {
            if (_plane.EventData.ExamData.Steps.Contains(step.StepData))
            {
                _plane.EventData.ExamData.GetCompletionPercentage();
            }
            
            NewData?.Invoke(_plane.EventData);
        }
    }
}
