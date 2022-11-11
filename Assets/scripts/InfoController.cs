using UnityEngine;
using UnityEngine.SceneManagement;

// функция выхода из сцены инфо в меню

public class InfoController : MonoBehaviour
	{
	public void ToMenu()
		{
		SceneManager.LoadScene("MenuScene");
		}
	}