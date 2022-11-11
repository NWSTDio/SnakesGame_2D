using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// ������ ����������� ����

public class GameController : MonoBehaviour
	{
	public enum GAMESTATE { RUNNING, PAUSED, GAME_OVER } // ���� ��������� ����
	private GAMESTATE _state;// ��������� ����
	public GAMESTATE State // ��������� � ��������� ��������� ����
		{
		get { return _state; } // ������� ��������� ����
		set 
			{
			_state = value;// ��������� ���������
			if (_state == GAMESTATE.GAME_OVER) // ���� ���������� ��������� ����� ����
				_gameOver();// ������� �� ����
			}
		}

	[SerializeField] GameObject gamePanel, gameOverPanel, pausedPanel;// ������: �������, ����� ���� � �����
	[SerializeField] Text scoreText, bestScoreText;// ��������� � ������� ��������� (��������� ������ �� ������ ���������)

	[SerializeField] GameObject bonusPanel, bonusStatus;// ������ ������, � ��������� ������ (���������� ����� ������ ����� ������ ������� ����)
	[SerializeField] PlayerScript player;// ������ ������
	[SerializeField] GameObject joyPadPanel, rightJoyPadPanel;// ����� � ������ ��������

	int score = 0;// ������� ����
	AudioSource source;// ��� ������ � ������� �������

	void Awake()
		{
		if (Data.level == 0) // ���� ������� �� ������, �� �� ��������� ������� ����� �������, ����������
			SceneManager.LoadScene("MenuScene");// �������� � ����
		}

	void Start()
		{
		SetScore(0);// �������� �����
		if (!Application.isMobilePlatform) // ���� �� �����
			{
			// ������ ��� ���������
			joyPadPanel.SetActive(false);
			rightJoyPadPanel.SetActive(false);
			}
		else
			{
			int dir = PlayerPrefs.GetInt("joypad", 0);// ������� ��������� ���������
			if (dir == 0) // ���� � ����
				rightJoyPadPanel.SetActive(false);// ������ ������ ��������
			else // �����
				joyPadPanel.SetActive(false);// ������ ����� ��������
			}
		if (PlayerPrefs.GetInt("sounds", 1) == 1) // ���� ����� ���������
			{
			source = GetComponent<AudioSource>();// ������� ��������� ��� ������ �� ������
			source.Play();// �������� ���� ������� ������� � ��� (������� ������)
			}
		}

	public void PauseGame() // ����� �������� �� ����� � ����
		{
		State = GAMESTATE.PAUSED;
		gamePanel.SetActive(false);
		pausedPanel.SetActive(true);
		gameOverPanel.SetActive(false);
		}
	public void PlayGame() // ����� �������� ���������� ���� �� ������ �����
		{
		State = GAMESTATE.RUNNING;
		gamePanel.SetActive(true);
		gameOverPanel.SetActive(false);
		pausedPanel.SetActive(false);
		}
	public void NewGame() // ����� �������� ����� ���� �� ������ ����� ����
		{
		SceneManager.LoadScene("GameScene");
		}
	public void ToMenu() // ����� �������� ���� �� ������ ����� ����
		{
		SceneManager.LoadScene("MenuScene");
		}
	public void SetScore(int score) // �������� �����
		{
		this.score += score;// �������� ����
		int best = PlayerPrefs.GetInt("best_" + Data.level, 0);// ������� ������� ��������� ��� �������� ������
		if (this.score >= best) // ���� ������� �������� ����� ����, �������� ����������
			PlayerPrefs.SetInt("best_" + Data.level, this.score);// ������� ������� ���������
		scoreText.text = "" + this.score;// ������� ��������� �������� �������� �������� �����
		}
	public void ShowBonusBar() // ����� ����� ����, ����� ������ ������� ����
		{
		bonusPanel.SetActive(true);// ����� ����� ������
		RectTransform rect = bonusStatus.GetComponent<RectTransform>() as RectTransform;// ��������� ��� ������ � ��������� �������
		rect.sizeDelta = new Vector2(512, rect.sizeDelta.y);// ������ ������� ����� ������
		}
	public void UpdateBonusBar(float size) // ���������� ����� ���� �� ������� ������
		{
		RectTransform rect = bonusStatus.GetComponent<RectTransform>() as RectTransform;// ��������� ��� ������ � ��������� �������
		rect.sizeDelta = new Vector2(size, rect.sizeDelta.y);// ��������� ����� �������
		}
	public void HideBonusBar() // ������ ����� ���
		{
		bonusPanel.SetActive(false);// ������� ����� ������
		}
	public void JoyPadClicked(int direction) // ����� ������� �� ������ ���������
		{
		player.Move((PlayerScript.DIRECTION) direction);// ������� ������ � ��� ��� ��������
		}
	private void _gameOver() // ����� ����
		{
		gamePanel.SetActive(false);// ������ ������� ������
		pausedPanel.SetActive(false);// ������ ������ �����
		gameOverPanel.SetActive(true);// ������� ������ ����� ����
		bestScoreText.text = "Your Score: " + score + "\n" + "Your Best Score: " + PlayerPrefs.GetInt("best_" + Data.level, 0);// ������� ��������� � ������� ���������
		}
	}