using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DifficultyHandler : MonoBehaviour {
    public Toggle m_EasyToggle;
    public Toggle m_MediumToggle;
    public Toggle m_HardToggle;
    public Toggle m_CustomToggle;

    public UnityEngine.UI.InputField inputFieldTries;
    public UnityEngine.UI.InputField inputFieldTryMoves;

    public MCTSIA m_MCTSIA;

    [Header("Settings")]
    public int EasyTries = 100;
    public int EasyMoves = 1;
    public int MediumTries = 500;
    public int MediumMoves = 10;
    public int HardTries = 1000;
    public int HardMoves = 50;


    private enum Difficulty
    {
        EASY,
        MEDIUM,
        HARD,
        CUSTOM
    }

    private void Start()
    {
        m_EasyToggle.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<bool>(
            (val) => {
                if (val)
                    SetDifficulty(Difficulty.EASY);
            })
        );

        m_MediumToggle.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<bool>(
            (val) => {
                if (val)
                    SetDifficulty(Difficulty.MEDIUM);
            })
        );

        m_HardToggle.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<bool>(
            (val) => {
                if (val)
                    SetDifficulty(Difficulty.HARD);
            })
        );

        inputFieldTries.onEndEdit.AddListener(new UnityEngine.Events.UnityAction<string>((s) => {
            m_CustomToggle.isOn = true;
        }));

        inputFieldTryMoves.onEndEdit.AddListener(new UnityEngine.Events.UnityAction<string>((s) => {
            m_CustomToggle.isOn = true;
        }));

        m_MediumToggle.isOn = true;
    }

    private void SetDifficulty(Difficulty _difficulty)
    {
        switch(_difficulty)
        {
            case Difficulty.EASY:
                inputFieldTries.text = EasyTries.ToString();
                inputFieldTryMoves.text = EasyMoves.ToString();
                break;
            case Difficulty.MEDIUM:
                inputFieldTries.text = MediumTries.ToString();
                inputFieldTryMoves.text = MediumMoves.ToString();
                break;
            case Difficulty.HARD:
                inputFieldTries.text = HardTries.ToString();
                inputFieldTryMoves.text = HardMoves.ToString();
                break;
            default:
                break;
        }

        m_MCTSIA.SetTries();
        m_MCTSIA.SetTryMoves();
    }
    
}
