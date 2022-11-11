using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// основной скрипт для работы с обьектами в меню

public class MenuController : MonoBehaviour
    {
    [SerializeField] Image soundButton;// картинка кнопки звука
    [SerializeField] Sprite[] soundButtonImages;// картинки для смены состояния картинка кнопки звука
    [SerializeField] Text joyPadText;// текст состояния настройки джойпада
    void Start()
        {
        changeSoundButton();// настроим кнопку звука, в соответствии с настройками звука выбранными игроком
        if (!Application.isMobilePlatform) // если игра запущена на компе
            joyPadText.gameObject.SetActive(false);// скроем настройку джойстика
        else
            changeJoyPadText();// выведем настройку джойстика
        }
    public void SoundOnOff() // переключатель состояния кнопки звука
        {
        PlayerPrefs.SetInt("sounds", PlayerPrefs.GetInt("sounds", 1) == 1 ? 0 : 1);// получим и сменим состояние звука и сохраним его
        changeSoundButton();// сменим состояние кнопки
		}
    public void ToInfo() // переход на сцену информации о игре
        {
        SceneManager.LoadScene("InfoScene");
		}
    public void ToSwitchLevel() // переход на сцену выбора уровней в игре
        {
        SceneManager.LoadScene("SwitchLevelScene");
		}
    public void ExitGame() // выход из игры
        {
        Application.Quit();
		}
    public void ChangeJoyPad() // переключение настроек положения джойпада
        {
        PlayerPrefs.SetInt("joypad", PlayerPrefs.GetInt("joypad", 0) == 0 ? 1 : 0);// получим, инвертируем и сменим состояние положение джойпада
        changeJoyPadText();// сменим текстовое состояние джойпада
        }
    private void changeSoundButton() // смена картинки состояния кнопки звука
        {
        soundButton.sprite = soundButtonImages[(int) PlayerPrefs.GetInt("sounds", 1)];
        }
    private void changeJoyPadText() // смена текстового состояния положения джойстика
        {
        joyPadText.text = "Джойстик: " + (PlayerPrefs.GetInt("joypad", 0) == 0 ? "слева" : "справа"); 
		}
    }