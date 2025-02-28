using System;
using System.Collections.Generic;
using System.Linq;
using AssigmentData;
using AssigmentDataInputScreen;
using DG.Tweening;
using MainScreen;
using UnityEngine;
using UnityEngine.UI;

namespace AllAssigments
{
    [RequireComponent(typeof(ScreenVisabilityHandler))]
    public class AllAssigmentsScreen : MonoBehaviour
    {
        [SerializeField] private AssigmentIconHolder _iconHolder;
        [SerializeField] private AssigmentColorHolder _colorHolder;
        [SerializeField] private GameObject _emptyPlane;
        [SerializeField] private AddAssigmentScreen _addAssigmentScreen;
        [SerializeField] private AddAssigmentScreen _editAssigmentScreen;
        [SerializeField] private MainScreenAssigmentHolder _mainScreenAssigmentHolder;
        [SerializeField] private List<AssigmentPlane> _currentAssigments;
        [SerializeField] private List<AssigmentPlane> _checkedAssigments;
        [SerializeField] private List<AssigmentPlane> _deletedAssigments;
        [SerializeField] private Button _addAssigmentButton;
        [SerializeField] private Button _backButtonClicked;

        [Header("Animation Settings")] [SerializeField]
        private float _fadeInDuration = 0.3f;

        [SerializeField] private float _moveInDuration = 0.5f;
        [SerializeField] private float _scaleInDuration = 0.4f;
        [SerializeField] private float _staggerDelay = 0.1f;
        [SerializeField] private Ease _animationEaseType = Ease.OutBack;
        [SerializeField] private float _buttonAnimationDuration = 0.3f;
        [SerializeField] private float _buttonPunchScale = 0.2f;

        private ScreenVisabilityHandler _screenVisabilityHandler;
        private Sequence _showSequence;

        public event Action BackClicked;
        public event Action<List<AssigmentData.AssigmentData>> UpdatedDatas;

        private void Awake()
        {
            _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
            DOTween.SetTweensCapacity(500, 50);
        }

        private void OnEnable()
        {
            foreach (var plane in _currentAssigments)
            {
                plane.SetHolders(_colorHolder, _iconHolder);
                plane.GetComponent<AssigmentEditPlane>().OpenClicked += EditAssigment;
            }

            foreach (var plane in _checkedAssigments)
            {
                plane.SetHolders(_colorHolder, _iconHolder);
            }

            foreach (var plane in _deletedAssigments)
            {
                plane.SetHolders(_colorHolder, _iconHolder);
            }

            _addAssigmentButton.onClick.AddListener(AddNewAssigment);
            _editAssigmentScreen.Edited += UpdateAllPlanes;
            _addAssigmentScreen.BackEdited += UpdateAllPlanes;
            _editAssigmentScreen.BackEdited += UpdateAllPlanes;
            _backButtonClicked.onClick.AddListener(OnBackClicked);
        }

        private void OnDisable()
        {
            _addAssigmentButton.onClick.RemoveListener(AddNewAssigment);
            _editAssigmentScreen.Edited -= UpdateAllPlanes;
            _addAssigmentScreen.BackEdited -= UpdateAllPlanes;
            _editAssigmentScreen.BackEdited -= UpdateAllPlanes;
            _backButtonClicked.onClick.RemoveListener(OnBackClicked);

            foreach (var plane in _currentAssigments)
            {
                plane.GetComponent<AssigmentEditPlane>().OpenClicked -= EditAssigment;
            }

            KillAllAnimations();
        }

        private void Start()
        {
            _screenVisabilityHandler.DisableScreen();
            DisableAllAssigments();
        }

        public void Enable()
        {
            _screenVisabilityHandler.EnableScreen();
            EnableAssigments(_mainScreenAssigmentHolder.Datas);
        }

        public void EnableAssigments(List<AssigmentData.AssigmentData> datas)
        {
            DisableAllAssigments();

            if (datas.Count <= 0)
            {
                _emptyPlane.gameObject.SetActive(true);
                AnimateEmptyPlane();
                return;
            }

            Debug.Log(datas.Count);

            _showSequence = DOTween.Sequence();
            int animationIndex = 0;

            foreach (var data in datas)
            {
                if (data.IsSelected)
                {
                    var plane = _checkedAssigments.FirstOrDefault(plane => !plane.IsActive);

                    if (plane != null)
                    {
                        plane.gameObject.SetActive(true);
                        plane.SetData(data);

                        PrepareAssignmentForAnimation(plane.transform);

                        _showSequence.Insert(animationIndex * _staggerDelay,
                            AnimateAssignmentIn(plane.transform));

                        animationIndex++;
                    }
                }
                else
                {
                    var plane = _currentAssigments.FirstOrDefault(plane => !plane.IsActive);

                    if (plane != null)
                    {
                        plane.gameObject.SetActive(true);
                        plane.SetData(data);

                        PrepareAssignmentForAnimation(plane.transform);

                        _showSequence.Insert(animationIndex * _staggerDelay,
                            AnimateAssignmentIn(plane.transform));

                        animationIndex++;
                    }
                }

                _emptyPlane.gameObject.SetActive(false);
            }

            _showSequence.Play();
        }

        private void DisableAllAssigments()
        {
            KillAllAnimations();

            foreach (var plane in _currentAssigments)
            {
                plane.Disable();
            }

            foreach (var plane in _checkedAssigments)
            {
                plane.Disable();
            }

            foreach (var plane in _deletedAssigments)
            {
                plane.Disable();
            }
        }

        private void AddNewAssigment()
        {
            _addAssigmentButton.transform
                .DOPunchScale(Vector3.one * _buttonPunchScale, _buttonAnimationDuration, 1, 0.5f)
                .OnComplete(() =>
                {
                    _addAssigmentScreen.EnableScreen();
                    _screenVisabilityHandler.DisableScreen();
                });
        }

        private void EditAssigment(AssigmentPlane plane)
        {
            plane.transform
                .DOPunchScale(Vector3.one * 0.1f, 0.3f, 1, 0.5f)
                .OnComplete(() =>
                {
                    _editAssigmentScreen.EnableScreen(plane);
                    _screenVisabilityHandler.DisableScreen();
                });
        }

        private void UpdateAllPlanes()
        {
            _screenVisabilityHandler.EnableScreen();

            var datas = new List<AssigmentData.AssigmentData>();

            foreach (var plane in _currentAssigments)
            {
                if (plane.IsActive && plane.Data != null)
                {
                    plane.UpdateText();
                    datas.Add(plane.Data);

                    plane.transform.DOPunchScale(Vector3.one * 0.05f, 0.3f, 1, 0.5f);
                }
            }

            UpdatedDatas?.Invoke(datas);
        }

        private void OnBackClicked()
        {
            _backButtonClicked.transform
                .DOPunchScale(Vector3.one * _buttonPunchScale, _buttonAnimationDuration, 1, 0.5f)
                .OnComplete(() =>
                {
                    BackClicked?.Invoke();
                    _screenVisabilityHandler.DisableScreen();
                });
        }

        #region Animation Methods

        private void PrepareAssignmentForAnimation(Transform assignmentTransform)
        {
            CanvasGroup canvasGroup = assignmentTransform.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = assignmentTransform.gameObject.AddComponent<CanvasGroup>();
            }

            canvasGroup.alpha = 0f;
            assignmentTransform.localScale = Vector3.zero;

            Vector3 originalPosition = assignmentTransform.localPosition;
            assignmentTransform.localPosition = originalPosition + new Vector3(-50f, 0f, 0f);

            assignmentTransform.SetTag("OriginalPosition", originalPosition);
        }

        private Sequence AnimateAssignmentIn(Transform assignmentTransform)
        {
            Sequence sequence = DOTween.Sequence();

            Vector3 originalPosition = (Vector3)assignmentTransform.GetTag("OriginalPosition");
            CanvasGroup canvasGroup = assignmentTransform.GetComponent<CanvasGroup>();

            sequence.Join(
                assignmentTransform.DOLocalMove(originalPosition, _moveInDuration).SetEase(_animationEaseType));

            sequence.Join(assignmentTransform.DOScale(Vector3.one, _scaleInDuration).SetEase(_animationEaseType));

            sequence.Join(canvasGroup.DOFade(1f, _fadeInDuration).SetEase(Ease.InQuad));

            return sequence;
        }

        private void AnimateEmptyPlane()
        {
            Transform emptyTransform = _emptyPlane.transform;

            CanvasGroup canvasGroup = emptyTransform.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = emptyTransform.gameObject.AddComponent<CanvasGroup>();
            }

            canvasGroup.alpha = 0f;
            emptyTransform.localScale = Vector3.zero;

            Sequence emptySequence = DOTween.Sequence();

            emptySequence.Append(emptyTransform.DOScale(Vector3.one, _scaleInDuration).SetEase(_animationEaseType));
            emptySequence.Join(canvasGroup.DOFade(1f, _fadeInDuration).SetEase(Ease.InQuad));

            emptySequence.Append(emptyTransform.DOPunchScale(Vector3.one * 0.1f, 0.5f, 1, 0.5f));

            emptySequence.Play();
        }

        private void KillAllAnimations()
        {
            if (_showSequence != null)
            {
                _showSequence.Kill();
                _showSequence = null;
            }

            foreach (var plane in _currentAssigments.Concat(_checkedAssigments).Concat(_deletedAssigments))
            {
                DOTween.Kill(plane.transform);
            }

            DOTween.Kill(_addAssigmentButton.transform);
            DOTween.Kill(_backButtonClicked.transform);

            DOTween.Kill(_emptyPlane.transform);
        }

        #endregion
    }

    public static class TransformExtensions
    {
        private static readonly Dictionary<Transform, Dictionary<string, object>> Tags =
            new Dictionary<Transform, Dictionary<string, object>>();

        public static void SetTag(this Transform transform, string key, object value)
        {
            if (!Tags.ContainsKey(transform))
            {
                Tags[transform] = new Dictionary<string, object>();
            }

            Tags[transform][key] = value;
        }

        public static object GetTag(this Transform transform, string key)
        {
            if (Tags.ContainsKey(transform) && Tags[transform].ContainsKey(key))
            {
                return Tags[transform][key];
            }

            return null;
        }
    }
}