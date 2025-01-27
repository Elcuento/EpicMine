using System;
using System.Collections;
using CommonDLL.Static;
using TMPro;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class RedDotBaseView : MonoBehaviour
    {
        public Action<RedDotBaseView> OnChange;

        [SerializeField] protected GameObject SimpleDot;
        [SerializeField] protected GameObject NumericDot;
        [SerializeField] protected TextMeshProUGUI Number;

        public bool IsActive => gameObject.activeSelf && (SimpleDot.activeSelf || NumericDot.activeSelf);

        public void Show(int number = 1)
        {
            Hide();

            if (!App.Instance.Controllers.TutorialController.IsStepComplete(TutorialStepIds.ShowDailyTasks))
                return;

            if (number <= 0)
                return;

            if (number == 1)
            {
                SimpleDot.SetActive(true);
                return;
            }
            
            NumericDot.SetActive(true);
            Number.text = number.ToString();
            StartCoroutine(UpdateCanvas());
            OnChange?.Invoke(this);
        }

        public void Hide()
        {
            SimpleDot.SetActive(false);
            NumericDot.SetActive(false);
            Number.text = string.Empty;

            OnChange?.Invoke(this);
        }

        private IEnumerator UpdateCanvas()
        {
            Number.text += " ";
            yield return new WaitForEndOfFrame();
            Number.text = Number.text.Trim();
        }
    }
}