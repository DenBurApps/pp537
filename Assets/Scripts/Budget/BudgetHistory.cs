using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BudgetHistory : MonoBehaviour
{
    [SerializeField] private Button _backButton;
    [SerializeField] private GameObject _emptyPlane;
    [SerializeField] private List<BudgetHistoryPlane> _planes;
    [SerializeField] private Menu _menu;

    [Header("Animation Settings")]
    [SerializeField] private float _planeEntryDuration = 0.5f;
    [SerializeField] private Ease _planeEntryEase = Ease.OutBack;
    [SerializeField] private float _fadeOutDuration = 0.3f;

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
        transform.localScale = Vector3.zero;
        gameObject.SetActive(true);
        transform.DOScale(1f, _planeEntryDuration).SetEase(_planeEntryEase);

        DisableAllWindows();
        
        for (int i = 0; i < data.SpendingDatas.Count; i++)
        {
            var availablePlane = _planes.FirstOrDefault(plane => !plane.IsActive);
            if (availablePlane != null)
            {
                availablePlane.Enable(data.SpendingDatas[i]);
                AnimatePlaneEntrance(availablePlane, i);
            }
        }

        _emptyPlane.gameObject.SetActive(ArePlanesEnabled());
    }

    private void AnimatePlaneEntrance(BudgetHistoryPlane plane, int index)
    {
        RectTransform rectTransform = plane.GetComponent<RectTransform>();
        Vector2 originalPosition = rectTransform.anchoredPosition;
        rectTransform.anchoredPosition = originalPosition + new Vector2(Screen.width, 0);

        rectTransform.DOAnchorPos(originalPosition, _planeEntryDuration)
            .SetEase(_planeEntryEase)
            .SetDelay(index * 0.1f);
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
        transform.DOScale(0f, _fadeOutDuration)
            .SetEase(Ease.InBack)
            .OnComplete(() => 
            {
                gameObject.SetActive(false);
                transform.localScale = Vector3.one;
            });
    }

    private void AnimatePlanesExit()
    {
        for (int i = 0; i < _planes.Count; i++)
        {
            if (_planes[i].IsActive)
            {
                RectTransform rectTransform = _planes[i].GetComponent<RectTransform>();
                rectTransform.DOAnchorPos(
                    rectTransform.anchoredPosition + new Vector2(-Screen.width, 0), 
                    _planeEntryDuration)
                    .SetEase(Ease.InBack)
                    .SetDelay(i * 0.1f)
                    .OnComplete(() => _planes[i].Disable());
            }
        }
    }
}