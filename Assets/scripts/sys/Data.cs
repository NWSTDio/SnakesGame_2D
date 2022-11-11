// здесь хранятся все значения которые нужны для перехода между сценами, и не нуждаются в сохранении
// в PlayerPrefs

public class Data
    {
    public static int level = 0;// храним значение выбранного уровня
    public static int WIDTH = 30, HEIGHT = 16;// размеры карты, по полам в разные стороны (-15 на 15 по оси X)
    public static bool DEBUG_MODE = false;// режим отладки
    }