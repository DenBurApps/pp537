using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AssigmentStep : MonoBehaviour
{
    [SerializeField] private Color _selectedColor;
    [SerializeField] private Color _defaultColor;

    [SerializeField] private Image _toggleImage;
    [SerializeField] private Sprite _selectedToggleSprite;
    [SerializeField] private Sprite _unselectedToggleSprite;

    [SerializeField] private TMP_InputField _name;
    [SerializeField] private Button _deleteButton;
    [SerializeField] private Button _selectButton;

    public event Action<AssigmentStep> Deleted;

    public bool IsActive { get; private set; }
    public AssigmentStepData Data { get; private set; }

    private void OnEnable()
    {
        _deleteButton.onClick.AddListener(OnDeleteButtonClicked);
        _selectButton.onClick.AddListener(OnToggleButtonClicked);
        _name.onValueChanged.AddListener(OnNameInputed);
    }

    private void OnDisable()
    {
        _deleteButton.onClick.RemoveListener(OnDeleteButtonClicked);
        _selectButton.onClick.RemoveListener(OnToggleButtonClicked);
        _name.onValueChanged.RemoveListener(OnNameInputed);
    }

    public void Enable()
    {
        gameObject.SetActive(true);
        IsActive = true;

        ToggleSelection();
    }

    public void Disable()
    {
        gameObject.SetActive(false);
        _name.text = "Enter name...";
        IsActive = false;
    }

    private void ToggleSelection()
    {
        if (Data.IsSelected)
        {
            _name.textComponent.color = _selectedColor;
            _toggleImage.sprite = _selectedToggleSprite;
        }
        else
        {
            _name.textComponent.color = _defaultColor;
            _toggleImage.sprite = _unselectedToggleSprite;
        }
    }

    private void OnToggleButtonClicked()
    {
        if (Data != null)
        {
            if (Data.IsSelected)
            {
                Data.IsSelected = true;
            }
            else
            {
                Data.IsSelected = false;
            }
            
            ToggleSelection();
        }
    }

    private void OnNameInputed(string name)
    {
        Data.Name = name;
    }

    private void OnDeleteButtonClicked()
    {
        Deleted?.Invoke(this);
        _name.text = "Enter name...";
    }
}

[Serializable]
public class AssigmentStepData
{
    public string Name;
    public bool IsSelected;

    public AssigmentStepData(string name, bool isSelected)
    {
        Name = name;
        IsSelected = isSelected;
    }
}