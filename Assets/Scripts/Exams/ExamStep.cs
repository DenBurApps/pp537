using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Exams
{
    public class ExamStep : MonoBehaviour
    {
        [SerializeField] private Sprite _selectedSprite;
        [SerializeField] private Sprite _unselectedSprite;
        [SerializeField] private TMP_InputField _name;
        [SerializeField] private Button _completeButton;

        public EventData.ExamStep StepData { get; private set; } = new();
        public bool IsActive { get; private set; }
        public event Action<ExamStep> ExamDataChanged; 

        private void OnEnable()
        {
            _name.onValueChanged.AddListener(OnNameInputed);
            _completeButton.onClick.AddListener(Select);
        }

        private void OnDisable()
        {
            _name.onValueChanged.RemoveListener(OnNameInputed);
            _completeButton.onClick.RemoveListener(Select);
        }

        public void Enable()
        {
            gameObject.SetActive(true);
            IsActive = true;
        }
        
        public void Enable(EventData.ExamStep data)
        {
            StepData = data;

            if (StepData.IsCompleted)
            {
                _completeButton.image.sprite = _selectedSprite;
                StepData.IsCompleted = true;
            }
            else
            {
                _completeButton.image.sprite = _unselectedSprite;
                StepData.IsCompleted = false;
            }
            
            gameObject.SetActive(true);
            IsActive = true;
        }

        public void Disable()
        {
            gameObject.SetActive(false);
            _name.text = "Enter name...";
            IsActive = false;
        }

        private void OnNameInputed(string name)
        {
            StepData.Name = name;
            ExamDataChanged?.Invoke(this);
        }

        private void Select()
        {
            if (StepData.IsCompleted)
            {
                _completeButton.image.sprite = _unselectedSprite;
                StepData.IsCompleted = false;
            }
            else
            {
                _completeButton.image.sprite = _selectedSprite;
                StepData.IsCompleted = true;
            }
            
            ExamDataChanged?.Invoke(this);
        }
    }
}