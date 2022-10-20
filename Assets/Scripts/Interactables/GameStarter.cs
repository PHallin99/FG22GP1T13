using Managers;
using UnityEngine;

namespace Interactables
{
    public class GameStarter : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player"))
                return;
            GameManager.Instance.StartGame();
        }
    }
}