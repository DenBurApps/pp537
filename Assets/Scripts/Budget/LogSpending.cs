using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LogSpending : MonoBehaviour
{
    [SerializeField] private TMP_InputField _nameInput;
    [SerializeField] private TMP_InputField _amountInput;
    [SerializeField] private Button _saveButton;
    [SerializeField] private Button _cancelButton;
    [SerializeField] private Button _backButton;

    private BudgetData _currentData;
    private string _name;
    private int _amount;

    public event Action<BudgetData> Saved;

    private void OnEnable()
    {
        _saveButton.onClick.AddListener(Save);
        _cancelButton.onClick.AddListener(CancelClicked);
        _backButton.onClick.AddListener(CancelClicked);
        
        _nameInput.onValueChanged.AddListener(OnNameInputed);
        _amountInput.onValueChanged.AddListener(OnAmountInputed);
    }

    private void OnDisable()
    {
        _saveButton.onClick.RemoveListener(Save);
        _cancelButton.onClick.RemoveListener(CancelClicked);
        _backButton.onClick.RemoveListener(CancelClicked);
        
        _nameInput.onValueChanged.RemoveListener(OnNameInputed);
        _amountInput.onValueChanged.RemoveListener(OnAmountInputed);
    }

    public void Enable(BudgetData data)
    {
        gameObject.SetActive(true);
        _currentData = data;
    }

    private void OnNameInputed(string name)
    {
        _name = name;
        ValidateInput();
    }

    private void OnAmountInputed(string amount)
    {
        _amount = int.Parse(amount);
        ValidateInput();
    }

    private void ValidateInput()
    {
        bool isValid = !string.IsNullOrEmpty(_name) && _amount > 0;
        _saveButton.interactable = isValid;
    }

    private void Save()
    {
        var data = new SpendingData(_amount, _name);
        _currentData.AddSpendAmount(data);
        Saved?.Invoke(_currentData);
        gameObject.SetActive(false);
    }

    private void CancelClicked()
    {
        _name = string.Empty;
        _amount = 0;

        _currentData = null;
        _nameInput.text = _name;
        _amountInput.text = _amount.ToString();
        gameObject.SetActive(false);
    }
}
