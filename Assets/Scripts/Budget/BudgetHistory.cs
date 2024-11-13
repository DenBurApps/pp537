using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BudgetHistory : MonoBehaviour
{
    [SerializeField] private Button _backButton;
    [SerializeField] private GameObject _emptyPlane;
    [SerializeField] private List<BudgetHistoryPlane> _planes;
    [SerializeField] private Menu _menu;

    private void OnEnable()
    {
        _backButton.onClick.AddListener(Cancel);
    }

    private void OnDisable()
    {
        _backButton.onClick.RemoveListener(Cancel);
    }

    public void Enable(BudgetData data)
    {
        gameObject.SetActive(true);
        DisableAllWindows();
        
        foreach (var spendingData in data.SpendingDatas)
        {
            var availablePlane = _planes.FirstOrDefault(plane => !plane.IsActive);
            if (availablePlane != null)
            {
                availablePlane.Enable(spendingData);
            }
        }

        _emptyPlane.gameObject.SetActive(ArePlanesEnabled());
    }

    private bool ArePlanesEnabled()
    {
        return _planes.All(plane => !plane.IsActive);
    }

    private void DisableAllWindows()
    {
        foreach (var plane in _planes)
        {
            plane.Disable();
        }
    }

    private void Cancel()
    {
        gameObject.SetActive(false);
    }
}