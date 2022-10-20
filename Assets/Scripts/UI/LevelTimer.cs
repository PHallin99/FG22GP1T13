using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class LevelTimer : MonoBehaviour
    {
        [SerializeField] private Text minuteTens;
        [SerializeField] private Text minuteUnits;
        [SerializeField] private Text secondTens;
        [SerializeField] private Text secondUnits;

        private void Update()
        {
            var secondsLeft = GameManager.Instance.SecondsLeft();
            minuteTens.text = ((int)(secondsLeft / 600 % 6)).ToString();
            minuteUnits.text = ((int)(secondsLeft / 60 % 10)).ToString();
            secondTens.text = ((int)(secondsLeft / 10 % 6)).ToString();
            secondUnits.text = ((int)(secondsLeft % 10)).ToString();
        }
    }
}