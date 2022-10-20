using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrashCounter : MonoBehaviour
{
    [SerializeField] private Text counterText;

    void Update()
    {
        counterText.text= GameManager.Instance.TrashLeft().ToString();
    }
}
