using UnityEngine;
using UnityEngine.UI;

public class DialectPair : MonoBehaviour
{
    #region Private Fields

    [SerializeField] private Text dialectismMeaning;
    [SerializeField] private Text dialectismName;
    public Text DialectismMeaning { get => dialectismMeaning; set => dialectismMeaning = value; }
    public Text DialectismName { get => dialectismName; set => dialectismName = value; }
    #endregion Private Fields

}