using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// �������� ������ ��� ������ � ��������� � ����

public class MenuController : MonoBehaviour
    {
    [SerializeField] Image soundButton;// �������� ������ �����
    [SerializeField] Sprite[] soundButtonImages;// �������� ��� ����� ��������� �������� ������ �����
    [SerializeField] Text joyPadText;// ����� ��������� ��������� ��������
    void Start()
        {
        changeSoundButton();// �������� ������ �����, � ������������ � ����������� ����� ���������� �������
        if (!Application.isMobilePlatform) // ���� ���� �������� �� �����
            joyPadText.gameObject.SetActive(false);// ������ ��������� ���������
        else
            changeJoyPadText();// ������� ��������� ���������
        }
    public void SoundOnOff() // ������������� ��������� ������ �����
        {
        PlayerPrefs.SetInt("sounds", PlayerPrefs.GetInt("sounds", 1) == 1 ? 0 : 1);// ������� � ������ ��������� ����� � �������� ���
        changeSoundButton();// ������ ��������� ������
		}
    public void ToInfo() // ������� �� ����� ���������� � ����
        {
        SceneManager.LoadScene("InfoScene");
		}
    public void ToSwitchLevel() // ������� �� ����� ������ ������� � ����
        {
        SceneManager.LoadScene("SwitchLevelScene");
		}
    public void ExitGame() // ����� �� ����
        {
        Application.Quit();
		}
    public void ChangeJoyPad() // ������������ �������� ��������� ��������
        {
        PlayerPrefs.SetInt("joypad", PlayerPrefs.GetInt("joypad", 0) == 0 ? 1 : 0);// �������, ����������� � ������ ��������� ��������� ��������
        changeJoyPadText();// ������ ��������� ��������� ��������
        }
    private void changeSoundButton() // ����� �������� ��������� ������ �����
        {
        soundButton.sprite = soundButtonImages[(int) PlayerPrefs.GetInt("sounds", 1)];
        }
    private void changeJoyPadText() // ����� ���������� ��������� ��������� ���������
        {
        joyPadText.text = "��������: " + (PlayerPrefs.GetInt("joypad", 0) == 0 ? "�����" : "������"); 
		}
    }