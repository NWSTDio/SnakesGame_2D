using UnityEngine;
using UnityEngine.UI;

// подсвечивает нажатую кнопку на джойстике, в игре

public class ButtonClickedScript : MonoBehaviour
    {
    [SerializeField] Image img;// изображение кнопки джойстика
    public void ButtonDown()
        {
        img.color = new Color(1, 1, 1, .4f);// уменьшим прозрачность
		}
    public void ButtonUp()
        {
        img.color = new Color(1, 1, 1, .2f);// увеличим прозрачность
        }
    }