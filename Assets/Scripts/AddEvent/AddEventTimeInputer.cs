using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AddEvent
{
    public class AddEventTimeInputer : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _hoursText;
        [SerializeField] private TMP_InputField _minText;
        [SerializeField] private Button _confirmButton;
        [SerializeField] private Button _cancelButton;

        private string _hr;
        private string _min;

        public event Action<string, string> ConfirmClicked;

        private void Start()
        {
            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            _hoursText.text = string.Empty;
            _minText.text = string.Empty;

            _hoursText.onValueChanged.AddListener(OnHrInputed);
            _minText.onValueChanged.AddListener(OnMinInputed);

            _confirmButton.onClick.AddListener(OnConfirmClicked);
            _cancelButton.onClick.AddListener(OnCancelClicked);
            ValidateInput();
        }

        private void OnDisable()
        {
            _hoursText.onValueChanged.RemoveListener(OnHrInputed);
            _minText.onValueChanged.RemoveListener(OnMinInputed);

            _confirmButton.onClick.RemoveListener(OnConfirmClicked);
            _cancelButton.onClick.RemoveListener(OnCancelClicked);
        }

        private void OnHrInputed(string text)
        {
            _hr = text;
            ValidateInput();
        }

        private void OnMinInputed(string text)
        {
            _min = text;
            ValidateInput();
        }

        private void OnCancelClicked()
        {
            gameObject.SetActive(false);
        }

        private void OnConfirmClicked()
        {
            ConfirmClicked?.Invoke(_hr, _min);
            gameObject.SetActive(false);
        }

        private void ValidateInput()
        {
            _confirmButton.interactable = !string.IsNullOrEmpty(_hr) && !string.IsNullOrEmpty(_min);
        }
    }
}