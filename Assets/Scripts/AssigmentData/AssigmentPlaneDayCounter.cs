using System;
using TMPro;
using UnityEngine;

namespace AssigmentData
{
    public class AssigmentPlaneDayCounter : MonoBehaviour
    {
        [SerializeField] private Color _criticalColor;
        [SerializeField] private Color _middleColor;
        [SerializeField] private Color _longTimeColor;

        [SerializeField] private TMP_Text _text;

        public void CalculateTime(DateTime dueTime)
        {
            DateTime currentTime = DateTime.Now;
            TimeSpan timeDifference = dueTime - currentTime;

            if (timeDifference.TotalDays > 2)
            {
                _text.text = $"{(int)timeDifference.TotalDays} days left";
                _text.color = _longTimeColor;
            }
            else if (timeDifference.TotalDays >= 1)
            {
                _text.text = $"{(int)timeDifference.TotalDays} days left";
                _text.color = _middleColor;
            }
            else if (timeDifference.TotalHours >= 1)
            {
                _text.text = $"{(int)timeDifference.TotalHours} hours left";
                _text.color = _criticalColor;
            }
            else
            {
                _text.text = "<1 hour left";
                _text.color = _criticalColor;
            }
        }
    }
}