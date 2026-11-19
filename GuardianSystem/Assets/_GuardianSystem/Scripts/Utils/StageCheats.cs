using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class StageCheats : MonoBehaviour
{
    private const int MaxCodeLength = 8;

    private string typedText = "";
    private Keyboard keyboard;
    private string currentScene;

    private void OnEnable()
    {
        currentScene = SceneManager.GetActiveScene().name;

        keyboard = Keyboard.current;

        if (keyboard != null)
            keyboard.onTextInput += HandleTextInput;
    }

    private void OnDisable()
    {
        if (keyboard != null)
            keyboard.onTextInput -= HandleTextInput;

        typedText = "";
    }

    private void HandleTextInput(char character)
    {
        if (!char.IsLetter(character))
        {
            typedText = "";
            return;
        }

        typedText += char.ToUpperInvariant(character);

        if (typedText.Length > MaxCodeLength)
            typedText = typedText.Substring(
                typedText.Length - MaxCodeLength
            );

        if (currentScene == "StageScene")
        {
            //In Game commands
            if (typedText.EndsWith("GLOBAL"))
            {
                SetPopularity(0f);
                typedText = "";
            }
            else if (typedText.EndsWith("SOLUTION"))
            {
                SetPopularity(100f);
                typedText = "";
            }else if (typedText.EndsWith("FIAP"))
            {
                SetPopularity(50f);
                typedText = "";
            }
        }
        else if (currentScene == "MainMenuScene")
        {
            //Main Menu commands
            if (typedText.EndsWith("BOOT"))
            {
                CreateSave(0);
            }
            else if (typedText.EndsWith("ALERT"))
            {
                CreateSave(1);
            }
            else if (typedText.EndsWith("CRISIS"))
            {
                CreateSave(2);
            }
        }
    }

    private void SetPopularity(float target)
    {
        var population = GameManager.Instance.populationModel;

        population.AdjustPopularity(
            target - population.Popularity
        );

        Debug.Log($"[CHEAT] Popularidade definida para {target}");
    }

    private void CreateSave(int dayIndex)
    {
        SaveData.currentPlayerData = new PlayerData
        {
            lastDayNumber = dayIndex,
            lastPopularityValue = 50f
        };

        SaveData.SaveGame();

        typedText = "";

        Debug.Log($"[CHEAT] Save criado para o Dia {dayIndex + 1}");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}