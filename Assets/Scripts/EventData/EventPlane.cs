using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EventData
{
    public class EventPlane : MonoBehaviour
    {
        [SerializeField] private Color _completedColor;
        [SerializeField] private Color _defaultColor;
        [SerializeField] private Sprite _nextSprite;
        [SerializeField] private Sprite _completedSprite;
        [SerializeField] private Sprite _plannedSprite;
        [SerializeField] private Sprite _examSprite;
        [SerializeField] private TMP_Text _time;
        [SerializeField] private TMP_Text _name;
        [SerializeField] private TMP_Text _examPersentage;
        [SerializeField] private GameObject _examPersentagePlane;
        [SerializeField] private Image _image;

        public bool IsCompleted { get; private set; }
        public bool IsActive { get; private set; }
        public EventData EventData { get; private set; }

        public void Enable()
        {
            gameObject.SetActive(true);
            _examPersentagePlane.gameObject.SetActive(false);
            IsActive = true;
        }

        public void Disable()
        {
            gameObject.SetActive(false);
            IsActive = false;
        }

        public void SetData(EventData data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            EventData = data;

            _name.text = EventData.Name;
            if (!string.IsNullOrEmpty(EventData.Note))
            {
                _name.text += $"<br>{EventData.Note}";
            }
            
            _time.text = $"{EventData.TimeHr}:{EventData.TimeMin:00}";

            if (EventData.DurationHr > 0 || EventData.DurationMin > 0)
            {
                int finalTimeHr = EventData.TimeHr + EventData.DurationHr;
                int finalTimeMin = EventData.TimeMin + EventData.DurationMin;

                if (finalTimeMin >= 60)
                {
                    finalTimeHr += finalTimeMin / 60;
                    finalTimeMin %= 60;
                }

                _time.text += $" - {finalTimeHr}:{finalTimeMin:00}";
            }
            
            if (EventData.IsExam)
            {
                _image.sprite = _examSprite;
                
                if(EventData.ExamData == null)
                    return;
                
                _examPersentagePlane.gameObject.SetActive(true);
                
                float completionPercentage = EventData.ExamData.GetCompletionPercentage();
                _examPersentage.text = $"{completionPercentage:F0}%";
            }
            else
            {
                _examPersentagePlane.gameObject.SetActive(false);
            }
        }

        public void SetNextSprite()
        {
            _image.sprite = _nextSprite;
            _time.color = _defaultColor;
            _name.color = _defaultColor;
        }

        public void SetCompleted()
        {
            _image.sprite = _completedSprite;
            _time.color = _completedColor;
            _name.color = _completedColor;
            _examPersentagePlane.gameObject.SetActive(false);
        }

        public void SetPlannedSprite()
        {
            _image.sprite = _plannedSprite;
            _time.color = _defaultColor;
            _name.color = _defaultColor;
        }
    }
}
