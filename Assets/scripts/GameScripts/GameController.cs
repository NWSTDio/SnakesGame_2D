using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// скрипт контроллера игры

public class GameController : MonoBehaviour
	{
	public enum GAMESTATE { RUNNING, PAUSED, GAME_OVER } // типы состояний игры
	private GAMESTATE _state;// состояние игры
	public GAMESTATE State // установка и получения состояния игры
		{
		get { return _state; } // возврат состояния игры
		set 
			{
			_state = value;// установка состояния
			if (_state == GAMESTATE.GAME_OVER) // если установили состояние конец игры
				_gameOver();// сообщим об этом
			}
		}

	[SerializeField] GameObject gamePanel, gameOverPanel, pausedPanel;// панели: игровая, конца игры и паузы
	[SerializeField] Text scoreText, bestScoreText;// результат и лучьший результат (текстовый обьект на панели проигрыша)

	[SerializeField] GameObject bonusPanel, bonusStatus;// панель бонуса, и состояние бонуса (появляется шкала вверху когда сьесть красный ромб)
	[SerializeField] PlayerScript player;// скрипт игрока
	[SerializeField] GameObject joyPadPanel, rightJoyPadPanel;// левый и правый джойстик

	int score = 0;// текущие очки
	AudioSource source;// для работы с фоновой музыкой

	void Awake()
		{
		if (Data.level == 0) // если уровень не указан, то мы запустили игровую сцену миновав, предыдущие
			SceneManager.LoadScene("MenuScene");// вернемся в меню
		}

	void Start()
		{
		SetScore(0);// устновка очков
		if (!Application.isMobilePlatform) // если на компе
			{
			// скроем оба джойстика
			joyPadPanel.SetActive(false);
			rightJoyPadPanel.SetActive(false);
			}
		else
			{
			int dir = PlayerPrefs.GetInt("joypad", 0);// получим положение джойстика
			if (dir == 0) // если с лева
				rightJoyPadPanel.SetActive(false);// скроем правый джойстик
			else // иначе
				joyPadPanel.SetActive(false);// скроем левый джойстик
			}
		if (PlayerPrefs.GetInt("sounds", 1) == 1) // если звуки разрешены
			{
			source = GetComponent<AudioSource>();// получим компонент для работы со звуком
			source.Play();// стартуем звук который заложен в нем (фоновая музыка)
			}
		}

	public void PauseGame() // когда нажимаем на паузу в игре
		{
		State = GAMESTATE.PAUSED;
		gamePanel.SetActive(false);
		pausedPanel.SetActive(true);
		gameOverPanel.SetActive(false);
		}
	public void PlayGame() // когда нажимаем продолжить игру на панели паузы
		{
		State = GAMESTATE.RUNNING;
		gamePanel.SetActive(true);
		gameOverPanel.SetActive(false);
		pausedPanel.SetActive(false);
		}
	public void NewGame() // когда нажимаем новая игры на панели конца игры
		{
		SceneManager.LoadScene("GameScene");
		}
	public void ToMenu() // когда нажимаем меню на панели конца игры
		{
		SceneManager.LoadScene("MenuScene");
		}
	public void SetScore(int score) // устновка очков
		{
		this.score += score;// увеличим очки
		int best = PlayerPrefs.GetInt("best_" + Data.level, 0);// получим лучьший результат для текущего уровня
		if (this.score >= best) // если текущее значение очков выше, лучьшего результата
			PlayerPrefs.SetInt("best_" + Data.level, this.score);// обновим лучьший результат
		scoreText.text = "" + this.score;// обновим текстовое значение текущего значения очков
		}
	public void ShowBonusBar() // показ бонус бара, когда сьесть красный ромб
		{
		bonusPanel.SetActive(true);// показ бонус панели
		RectTransform rect = bonusStatus.GetComponent<RectTransform>() as RectTransform;// компонент для работы с размерами обьекта
		rect.sizeDelta = new Vector2(512, rect.sizeDelta.y);// вернем размеры бонус панели
		}
	public void UpdateBonusBar(float size) // обновление бонус бара из скрипта игрока
		{
		RectTransform rect = bonusStatus.GetComponent<RectTransform>() as RectTransform;// компонент для работы с размерами обьекта
		rect.sizeDelta = new Vector2(size, rect.sizeDelta.y);// установим новые размеры
		}
	public void HideBonusBar() // скрыть бонус бар
		{
		bonusPanel.SetActive(false);// спрячем бонус панель
		}
	public void JoyPadClicked(int direction) // когда кликаем по кнопке джойстика
		{
		player.Move((PlayerScript.DIRECTION) direction);// сообщим игроку о том что кликнули
		}
	private void _gameOver() // конец игры
		{
		gamePanel.SetActive(false);// скроем игровую панель
		pausedPanel.SetActive(false);// скроем панель паузы
		gameOverPanel.SetActive(true);// покажем панель конца игры
		bestScoreText.text = "Your Score: " + score + "\n" + "Your Best Score: " + PlayerPrefs.GetInt("best_" + Data.level, 0);// выведем результат и лучьший результат
		}
	}