using UnityEngine;
using TMPro;
public class UIPlayerStats : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI lengthText;

    //Send change length event based off ChangeLength text
    private void OnEnable()
    {
        PlayerLength.ChangedLengthEvent += ChangeLengthText;
    }

    //Send change length event based off ChangeLength text
    private void OnDisable()
    {
        PlayerLength.ChangedLengthEvent -= ChangeLengthText;
    }

    //Change the text of length change to string to output
    private void ChangeLengthText(ushort length)
    {
        lengthText.text = length.ToString();
    }
}
