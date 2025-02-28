using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using MainScreen;

public class Onboarding : MonoBehaviour
{
    [SerializeField] private List<GameObject> _steps;
    [SerializeField] private float _fadeInDuration = 0.5f;
    [SerializeField] private float _fadeOutDuration = 0.3f;
    [SerializeField] private float _scaleInDuration = 0.5f;
    [SerializeField] private Ease _fadeInEase = Ease.OutQuad;
    [SerializeField] private Ease _fadeOutEase = Ease.InQuad;
    [SerializeField] private Ease _scaleEase = Ease.OutBack;
    [SerializeField] private MainScreenView _mainScreen;

    private int _currentIndex = 0;

    private void Start()
    {
        Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

        foreach (var step in _steps)
        {
            if (step.TryGetComponent<CanvasGroup>(out var canvasGroup))
            {
                canvasGroup.alpha = 0f;
            }
            else
            {
                canvasGroup = step.AddComponent<CanvasGroup>();
                canvasGroup.alpha = 0f;
            }

            step.transform.localScale = Vector3.zero;
            step.SetActive(false);
        }

        if (PlayerPrefs.HasKey("Onboarding"))
        {
            gameObject.SetActive(false);
            _mainScreen.Enable();
        }
        else
        {
            gameObject.SetActive(true);
            ShowOnboarding();
            _mainScreen.Disable();
        }
    }

    private void ShowOnboarding()
    {
        _currentIndex = 0;
        AnimateStepIn(_steps[_currentIndex]);
    }

    public void ShowNextStep()
    {
        GameObject currentStep = _steps[_currentIndex];

        AnimateStepOut(currentStep, () =>
        {
            _currentIndex++;
            if (_currentIndex < _steps.Count)
            {
                AnimateStepIn(_steps[_currentIndex]);
            }
            else
            {
                PlayerPrefs.SetInt("Onboarding", 1);
                gameObject.SetActive(false);
                _mainScreen.Enable();
            }
        });
    }

    private void AnimateStepIn(GameObject step)
    {
        step.SetActive(true);

        CanvasGroup canvasGroup = step.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = step.AddComponent<CanvasGroup>();
        }

        canvasGroup.alpha = 0f;
        step.transform.localScale = Vector3.zero;

        Sequence sequence = DOTween.Sequence();

        sequence.Append(canvasGroup.DOFade(1f, _fadeInDuration).SetEase(_fadeInEase));

        sequence.Join(step.transform.DOScale(Vector3.one, _scaleInDuration).SetEase(_scaleEase));

        sequence.Play();
    }

    private void AnimateStepOut(GameObject step, TweenCallback onComplete)
    {
        CanvasGroup canvasGroup = step.GetComponent<CanvasGroup>();

        Sequence sequence = DOTween.Sequence();

        sequence.Append(canvasGroup.DOFade(0f, _fadeOutDuration).SetEase(_fadeOutEase));

        sequence.Join(step.transform.DOScale(0.8f, _fadeOutDuration).SetEase(_fadeOutEase));

        sequence.OnComplete(() =>
        {
            step.SetActive(false);
            onComplete?.Invoke();
        });

        sequence.Play();
    }
}