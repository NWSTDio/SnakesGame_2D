using UnityEngine;
using UnityEngine.SceneManagement;

// ������� ������ �� ����� ���� � ����

public class InfoController : MonoBehaviour
	{
	public void ToMenu()
		{
		SceneManager.LoadScene("MenuScene");
		}
	}