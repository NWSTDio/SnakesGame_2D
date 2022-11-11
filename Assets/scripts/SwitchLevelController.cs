using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// скрипт дл€ работы со сценой выбора уровней и работы с ее обьектами

public class SwitchLevelController : MonoBehaviour
    {
    [SerializeField] GameObject levelPanel, infoPanel;// панель уровней и инфо панель (когда выбрать уровень)
    [SerializeField] Text levelText, bestScoreText;// текст уровн€ и лучьший результат (дл€ инфо панели)
    public void SwitchLevel(int level) // выбор уровн€ с панели уровней
        {
        Data.level = level;// сохраним значение выбора уовней в общий класс
        levelPanel.SetActive(false);// уберем панель выбора уровней
        infoPanel.SetActive(true);// покажем панель информации о уровне и возможности начать игру
        levelText.text = "Level " + level;// отображение выбранного уровн€
        bestScoreText.text = "Your Best Score: " + PlayerPrefs.GetInt("best_" + level, 0);// лучьший результат дл€ выбранного уровн€
        }
    public void HideInfo() // пр€чем панель информации (крестик справа сферзу на панели инфо)
        {
        infoPanel.SetActive(false);// скрыть панель информации
        levelPanel.SetActive(true);// показать панель выбора уровн€
		}
    public void StartGame() // старт уровн€ (в инфо панели)
        {
        SceneManager.LoadScene("GameScene");
		}
    public void ToMenu() // вернутс€ в меню
        {
        SceneManager.LoadScene("MenuScene");
        }
    }