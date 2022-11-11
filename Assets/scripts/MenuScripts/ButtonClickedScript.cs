using UnityEngine;
using UnityEngine.UI;

// ������������ ������� ������ �� ���������, � ����

public class ButtonClickedScript : MonoBehaviour
    {
    [SerializeField] Image img;// ����������� ������ ���������
    public void ButtonDown()
        {
        img.color = new Color(1, 1, 1, .4f);// �������� ������������
		}
    public void ButtonUp()
        {
        img.color = new Color(1, 1, 1, .2f);// �������� ������������
        }
    }