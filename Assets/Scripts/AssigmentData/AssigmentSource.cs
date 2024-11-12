using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AssigmentData
{
    public class AssigmentSource : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _name;
        [SerializeField] private Button _deleteButton;

        public event Action<AssigmentSource> Deleted; 
        
        public AssigmentSourceData Data { get; private set; }
        public bool IsActive { get; private set; }

        public void OnEnable()
        {
            _deleteButton.onClick.AddListener(OnDeleteButtonClicked);
            _name.onValueChanged.AddListener(OnNameInputed);
        }

        private void OnDisable()
        {
            _deleteButton.onClick.RemoveListener(OnDeleteButtonClicked);
            _name.onValueChanged.RemoveListener(OnNameInputed);
        }

        public void Enable()
        {
            gameObject.SetActive(true);
            IsActive = true;
        }

        public void Disable()
        {
            gameObject.SetActive(false);
            IsActive = false;
        }

        private void OnNameInputed(string text)
        {
            Data.Name = text;
        }

        private void OnDeleteButtonClicked()
        {
            Deleted?.Invoke(this);
            _name.text = "Enter name...";
        }
    }

    [Serializable]
    public class AssigmentSourceData
    {
        public string Name;
    }
}