using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// ������ ��� ������ �� ������ ������ ������� � ������ � �� ���������

public class SwitchLevelController : MonoBehaviour
    {
    [SerializeField] GameObject levelPanel, infoPanel;// ������ ������� � ���� ������ (����� ������� �������)
    [SerializeField] Text levelText, bestScoreText;// ����� ������ � ������� ��������� (��� ���� ������)
    public void SwitchLevel(int level) // ����� ������ � ������ �������
        {
        Data.level = level;// �������� �������� ������ ������ � ����� �����
        levelPanel.SetActive(false);// ������ ������ ������ �������
        infoPanel.SetActive(true);// ������� ������ ���������� � ������ � ����������� ������ ����
        levelText.text = "Level " + level;// ����������� ���������� ������
        bestScoreText.text = "Your Best Score: " + PlayerPrefs.GetInt("best_" + level, 0);// ������� ��������� ��� ���������� ������
        }
    public void HideInfo() // ������ ������ ���������� (������� ������ ������ �� ������ ����)
        {
        infoPanel.SetActive(false);// ������ ������ ����������
        levelPanel.SetActive(true);// �������� ������ ������ ������
		}
    public void StartGame() // ����� ������ (� ���� ������)
        {
        SceneManager.LoadScene("GameScene");
		}
    public void ToMenu() // �������� � ����
        {
        SceneManager.LoadScene("MenuScene");
        }
    }