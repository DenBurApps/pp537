using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LogSpending : MonoBehaviour
{
    [SerializeField] private TMP_InputField _nameInput;
    [SerializeField] private TMP_InputField _amountInput;
    [SerializeField] private Button _saveButton;
    [SerializeField] private Button _cancelButton;
    [SerializeField] private Button _backButton;

    [Header("Animation Settings")]
    [SerializeField] private float _animationDuration = 0.3f;
    [SerializeField] private Ease _animationEase = Ease.OutBack;

    private BudgetData _currentData;
    private string _name;
    private int _amount;

    public event Action<BudgetData> Saved;

    private void Start()
    {
        // Initially hide the panel with a scale animation
        transform.localScale = Vector3.zero;
        gameObject.SetActive(false);
    }

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

        // Animate panel entrance
        transform.DOScale(1f, _animationDuration)
            .SetEase(_animationEase);

        // Reset input fields with animation
        _nameInput.text = string.Empty;
        _amountInput.text = string.Empty;
        AnimateInputFields();
    }

    private void AnimateInputFields()
    {
        // Animate input fields with a slight bounce
        _nameInput.transform.localScale = Vector3.zero;
        _amountInput.transform.localScale = Vector3.zero;

        _nameInput.transform.DOScale(1f, _animationDuration)
            .SetEase(Ease.OutBack)
            .SetDelay(0.1f);

        _amountInput.transform.DOScale(1f, _animationDuration)
            .SetEase(Ease.OutBack)
            .SetDelay(0.2f);
    }

    private void OnNameInputed(string name)
    {
        _name = name;
        ValidateInput();
        AnimateSaveButton();
    }

    private void OnAmountInputed(string amount)
    {
        if (int.TryParse(amount, out int parsedAmount))
        {
            _amount = parsedAmount;
            ValidateInput();
            AnimateSaveButton();
        }
    }

    private void AnimateSaveButton()
    {
        // Subtle pulse animation when input is valid
        if (_saveButton.interactable)
        {
            _saveButton.transform
                .DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), 0.3f, 1, 1f);
        }
    }

    private void ValidateInput()
    {
        bool isValid = !string.IsNullOrEmpty(_name) && _amount > 0;
        _saveButton.interactable = isValid;
    }

    private void Save()
    {
        // Save animation
        transform.DOShakePosition(0.3f, 10f, 10, 90f, false, true)
            .OnComplete(() => {
                var data = new SpendingData(_amount, _name);
                _currentData.AddSpendAmount(data);
                Saved?.Invoke(_currentData);
                ClosePanel();
            });
    }

    private void CancelClicked()
    {
        ClosePanel();
    }

    private void ClosePanel()
    {
        // Animate panel exit
        transform.DOScale(0f, _animationDuration)
            .SetEase(Ease.InBack)
            .OnComplete(() => {
                _name = string.Empty;
                _amount = 0;
                _currentData = null;
                gameObject.SetActive(false);
            });
    }
}