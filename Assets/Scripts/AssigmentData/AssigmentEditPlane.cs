using System;
using AssigmentData;
using UnityEngine;
using UnityEngine.UI;

public class AssigmentEditPlane : MonoBehaviour
{
    [SerializeField] private Button _openButton;
    [SerializeField] private AssigmentPlane _plane;

    public event Action<AssigmentPlane> OpenClicked;

    private void OnEnable()
    {
        _openButton.onClick.AddListener(OpenPlane);
    }

    private void OnDisable()
    {
        _openButton.onClick.RemoveListener(OpenPlane);
    }

    private void OpenPlane() => OpenClicked?.Invoke(_plane);
}
