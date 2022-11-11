using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/*

    в данной интерпретации игры, создается иллюзия движения змейки
    змейка состоити из списка обьектов, мы при каждом передвижении
    убираем первый элемент списка - хвост, а голову добавляем в 
    конец списка, со смещением в указанном направлении, от последнего
    в списке, который был головой в прошлом кадре передвижения

    Еда: это все бонусы что поедает змея
    зеленый бонус, просто еда, негативный бонус, т.к. изза него растет скорость и размер змеи,
    но без него не получить новых бонусов и очков
    желтый бонус уменьшит размер змеи от 1 до 10 за раз, но при условии что размер змеи не менее 15 элементов
    синий бонус уменьшит скорость змеи на 2
    красный бонус дает невидимость змеи на 10 секунд
    т.к. столкновения считаются только для головы, то не будет проблем если часть тела окажется в теле или в стене,
    иначе от бонуса небыло бы смысла

*/

public class PlayerScript : MonoBehaviour
    {
    [SerializeField] GameController controller;// контроллер игры
    public enum DIRECTION { LEFT, RIGHT, UP, DOWN } // для работы с направлением
    DIRECTION eyeDir = DIRECTION.RIGHT;// направление взгляда
    DIRECTION moveDir = DIRECTION.RIGHT;// направление движения
    [SerializeField] GameObject bodyPartPrefab, spawnPoint;// тело змеи и спавн поинт
    [SerializeField] GameObject foodsContainer, foodPrefab;// хранилище еды и экземпляр еды
    [SerializeField] GameObject wallsContainer, wallPrefab;// хранилище стен и экземпляр стены
    float scaleX, scaleY;// размеры ячейки обьекта
    float moveTimer = 0, speed = 5.0f;// для просчета шага при автоматическом передвижении
    List<GameObject> snakes;// части змеи
    List<GameObject> foods;// еда на экране
    List<GameObject> walls;// стены
    [SerializeField] Color colorHead, colorBody;// цвет головы и тела
    // эфекты при поедании бонусов
    [SerializeField] GameObject eatExplousionPrefab, inviseExplousionPrefab, lowSpeedExplousionPrefab, cuteExplousionPrefab;

    float bonusTimer = 0, bonusTime = 0;// текущее время бонуса и макс время бонуса (для красного ромба)
    bool invise = false;// невидимость игрока
    bool newBonus = false, isBonus = false;// нужен ли новый  бонус и есть ли сейчас активный бонус (красный ромб)

    [SerializeField] AudioClip clip;// звук поедания бонуса
    AudioSource source;// для работы со звуками

    void Start()
        {
        snakes = new List<GameObject>();// тело змейки
        foods = new List<GameObject>();// вся еда
        walls = new List<GameObject>();// преграды
        // создаем голову
        GameObject temp = Instantiate(bodyPartPrefab, spawnPoint.transform.position, Quaternion.identity) as GameObject;
        temp.GetComponent<SpriteRenderer>().color = colorHead;// цвет головы
        temp.transform.SetParent(transform);// укажем ее потомком текущего обьекта дабы не засорять игру
        snakes.Add(temp);// добавим голову в список обьектов части змеи
        // получим размеры обьектов
        scaleX = temp.transform.localScale.x;
        scaleY = temp.transform.localScale.y;
        genWalls();// сгенерируем стены
        // добавим две части змеи, последняя добавленная будет головой
        addPart(DIRECTION.RIGHT);
        addPart(DIRECTION.RIGHT);
        addFood(FoodScript.States.EAT);// добавим еду
        invise = false;// отключим невидимость змеи
        source = GetComponent<AudioSource>();// получим компонент для работы со звуками
        }
    void Update()
        {
        if (controller.State == GameController.GAMESTATE.RUNNING) // если игра не окончена
            {
            if (isBonus) // если установлен бонус невидимости
                {
                if (bonusTimer < bonusTime) // если время бонуса не окончено
                    {
                    // просчет длины линии статуса бонуса для бонус бара
                    bonusTimer += Time.deltaTime;// обновим время
                    float tmp = bonusTime - bonusTimer;// получим оставшуюся разницу во времени
                    float per = tmp * 100 / bonusTime;// процентное соотношение
                    float size = per * 512 / 100;// длинна картинки в соответствии с % соотношением оставшиегося времени
                    controller.UpdateBonusBar(size);// обновим бонус бар
                    }
                else
                    {
                    bonusTime = 0;// сбросим таймер бонуса
                    bonusTimer = 0;// сбросим время бонуса
                    controller.HideBonusBar();// спрячем бонус бар
                    invise = false;// уберем невидимость змеи
                    isBonus = false;// укажем что бонус не активен
                    }
                }
            if (Data.DEBUG_MODE) // режим отладки
                {
                bool isKeyDown = false;// было ли нажатие клавиши
                // установим направление взгляда в соотвествующем направлении
                if (Input.GetKeyDown(KeyCode.A) && eyeDir != DIRECTION.RIGHT && moveDir != DIRECTION.RIGHT)
                    {
                    eyeDir = DIRECTION.LEFT;
                    isKeyDown = true;
                    }
                if (Input.GetKeyDown(KeyCode.D) && eyeDir != DIRECTION.LEFT && moveDir != DIRECTION.LEFT)
                    {
                    eyeDir = DIRECTION.RIGHT;
                    isKeyDown = true;
                    }
                if (Input.GetKeyDown(KeyCode.W) && eyeDir != DIRECTION.DOWN && moveDir != DIRECTION.DOWN)
                    {
                    eyeDir = DIRECTION.UP;
                    isKeyDown = true;
                    }
                if (Input.GetKeyDown(KeyCode.S) && eyeDir != DIRECTION.UP && moveDir != DIRECTION.UP)
                    {
                    eyeDir = DIRECTION.DOWN;
                    isKeyDown = true;
                    }
                if (isKeyDown) // если нажали клавишу
                    {
                    moveDir = eyeDir;// установим движение
                    addPart(moveDir);// добавим голову
                    if (!checkCollisionFood()) // если нет столкновений с едой
                        removePart();// удалим хвост
                    if (checkCollisionSnake()) // если змея столкнулась с собой или стеной
                        controller.State = GameController.GAMESTATE.GAME_OVER;
                    }
                }
            else
                {
                // установим направление взгляда
                if (Input.GetKeyDown(KeyCode.A) && eyeDir != DIRECTION.LEFT && eyeDir != DIRECTION.RIGHT && moveDir != DIRECTION.RIGHT)
                    eyeDir = DIRECTION.LEFT;
                if (Input.GetKeyDown(KeyCode.D) && eyeDir != DIRECTION.RIGHT && eyeDir != DIRECTION.LEFT && moveDir != DIRECTION.LEFT)
                    eyeDir = DIRECTION.RIGHT;
                if (Input.GetKeyDown(KeyCode.W) && eyeDir != DIRECTION.UP && eyeDir != DIRECTION.DOWN && moveDir != DIRECTION.DOWN)
                    eyeDir = DIRECTION.UP;
                if (Input.GetKeyDown(KeyCode.S) && eyeDir != DIRECTION.DOWN && eyeDir != DIRECTION.UP && moveDir != DIRECTION.UP)
                    eyeDir = DIRECTION.DOWN;
                if (moveTimer >= 1f) // если прошли шаг
                    {
                    moveDir = eyeDir;// запомним направление движения
                    moveTimer = 0;// сбросим nqvth ifuf
                    addPart(moveDir);// добавим голову
                    if (!checkCollisionFood()) // если не было столкновений с едой
                        removePart();// удалим хвост
                    if (checkCollisionSnake()) // если змея укусила себя или стену
                        controller.State = GameController.GAMESTATE.GAME_OVER;
                    }
                else moveTimer += speed * Time.deltaTime;// увеличим таймер
                }
            }
        if (Data.DEBUG_MODE && Input.GetKeyUp(KeyCode.Space)) // в режиме отладки при нажатии клавиши пробед
            SceneManager.LoadScene("GameScene");// перегрузим сцену
        if(newBonus) // если разрешено создание нового бонуса
            {
            if (Random.Range(0, 3) == 0) // шанс что создадим бонус, меньше скорости
                addFood(FoodScript.States.LOW_SPEED);
            else if (Random.Range(0, 3) == 0) // шанс что создадим бонус обрезания змейки
                addFood(FoodScript.States.CUTE);
            else
                addFood(FoodScript.States.INVISE);// иначе создадим бонус невидимости змейки
            newBonus = false;// укажем что новый бонус не нужен
			}
        }

    public void Move(DIRECTION dir) // движение змейки через джойстик
        {
        if (dir == DIRECTION.LEFT && eyeDir != DIRECTION.LEFT && eyeDir != DIRECTION.RIGHT && moveDir != DIRECTION.RIGHT)
            eyeDir = DIRECTION.LEFT;
        if (dir == DIRECTION.RIGHT && eyeDir != DIRECTION.RIGHT && eyeDir != DIRECTION.LEFT && moveDir != DIRECTION.LEFT)
            eyeDir = DIRECTION.RIGHT;
        if (dir == DIRECTION.UP && eyeDir != DIRECTION.UP && eyeDir != DIRECTION.DOWN && moveDir != DIRECTION.DOWN)
            eyeDir = DIRECTION.UP;
        if (dir == DIRECTION.DOWN && eyeDir != DIRECTION.DOWN && eyeDir != DIRECTION.UP && moveDir != DIRECTION.UP)
            eyeDir = DIRECTION.DOWN;
        }
    private void addPart(DIRECTION dir) // добавление головы в соответсвующем направлении
        {
        // перекрасим прошлую голову в белый цвет
        snakes[snakes.Count - 1].GetComponent<SpriteRenderer>().color = colorBody;
        // получим координаты прошлой головы змеи
        Vector3 pos = Round(snakes[snakes.Count - 1].transform.position, 1);
        // сместим голову в заданном направлении
        if (eyeDir == DIRECTION.LEFT)
            pos.x -= scaleX;
        else if (eyeDir == DIRECTION.RIGHT)
            pos.x += scaleX;
        else if (eyeDir == DIRECTION.UP)
            pos.y += scaleY;
        else if (eyeDir == DIRECTION.DOWN)
            pos.y -= scaleY;
        // если граница экрана по оси X
        if (pos.x < -Data.WIDTH / 2 * scaleX)
            pos.x = Data.WIDTH / 2 * scaleX;
        if (pos.x > Data.WIDTH / 2 * scaleX)
            pos.x = -Data.WIDTH / 2 * scaleX;
        // если граница экрана по оси Y
        if (pos.y < -Data.HEIGHT / 2 * scaleY)
            pos.y = Data.HEIGHT / 2 * scaleY;
        if (pos.y > Data.HEIGHT / 2 * scaleY)
            pos.y = -Data.HEIGHT / 2 * scaleY;
        pos = Round(pos, 1);// округлим позицию, на всяк случай
        // создадим голову
        GameObject temp = Instantiate(bodyPartPrefab, pos, Quaternion.identity) as GameObject;
        temp.transform.SetParent(transform);
        temp.GetComponent<SpriteRenderer>().color = colorHead;
        snakes.Add(temp);// добавим ее в список
        }
    private void removePart() // удаление хвоста
        {
        GameObject tail = snakes[0];// получим первый элемент
        Destroy(tail);// удалим его обьект
        snakes.RemoveAt(0);// удалим его из списка
		}
    private void addFood(FoodScript.States state) // добавление еды
        {
        // если сумма стен и частей змеи больше чем клеток на поле
        if (snakes.Count + walls.Count >= Data.WIDTH * Data.HEIGHT)
            return;
        List<Vector3> addr = new List<Vector3>();// список закрытых координат
        foreach(GameObject obj in snakes) // добавим все части змеи
            addr.Add(obj.transform.position);
        foreach (GameObject obj in walls) // добавим все части стен
            addr.Add(obj.transform.position);
        foreach (GameObject obj in foods) // добавим все части еды
            addr.Add(obj.transform.position);
        List<Vector3> emptyAddr = new List<Vector3>();// список открытых координат
        Vector3 pos;
        // пройдем по всему полю
        for(int i = -Data.WIDTH / 2; i <= Data.WIDTH / 2; i++)
            for(int j = -Data.HEIGHT / 2; j <= Data.HEIGHT / 2; j++)
                {
                pos = Round(new Vector3(i * scaleY, j * scaleX, 0), 1);
                if (addr.Contains(pos)) // если указанный адрес есть в запретном списке
                    continue;// ищем еще раз
                emptyAddr.Add(pos);// добавим адрес в список доступных
				}
        if (emptyAddr.Count <= 1) // если доступных адресов мало
            return;
        pos = Round(emptyAddr[(int)Random.Range(0, emptyAddr.Count)], 1);
        // создадим и добавим еду в список
        GameObject food = Instantiate(foodPrefab, pos, Quaternion.identity) as GameObject;
        food.transform.SetParent(foodsContainer.transform);
        food.GetComponent<FoodScript>().State = state;
        foods.Add(food);
		}
    private bool checkCollisionFood() // проверка столкновения с едой
        {
        GameObject head = snakes[snakes.Count - 1];// голова
        Vector3 pos = Round(head.transform.position, 1);// координаты головы
        FoodScript foodScript;// для быстрой работы с компонентом
        foreach(GameObject food in foods) // пройдем по всей еде
            {
            Vector3 ppos = Round(food.transform.position, 1);
            if (Equals(pos, ppos)) // если координаты равны
                {
                foodScript = food.GetComponent<FoodScript>() as FoodScript;// получим компонент
                if (foodScript.State == FoodScript.States.INVISE) // если еда - бонус невидимости
                    {
                    Instantiate(inviseExplousionPrefab, ppos, Quaternion.identity);// эффект
                    controller.ShowBonusBar();// покажем бонус бар
                    controller.SetScore(5);// увеличим очки
                    bonusTime = 10.0f;// время бонуса
                    invise = true;// включим невидимость
                    isBonus = true;// укажем что работает бонус
                    }
                else if (foodScript.State == FoodScript.States.LOW_SPEED) // если еда - бонус уменбьшения скорости
                    {
                    Instantiate(lowSpeedExplousionPrefab, ppos, Quaternion.identity);
                    controller.SetScore(10);// увеличим очки
                    speed -= 2.0f;// уменьшим скорость
                    if (speed <= 5f) // если новая скорость меньше минимальной
                        speed = 5f;// минимальное значение
                    }
                else if (foodScript.State == FoodScript.States.CUTE) // если еда - убрать часть змеи
                    {
                    Instantiate(cuteExplousionPrefab, ppos, Quaternion.identity);
                    controller.SetScore(20);// увеличим очки
                    if (snakes.Count >= 15) // если размер змейки более или равно 15
                        {
                        int rand = Random.Range(1, 11);// укажем сколько частей змеи убрать
                        for (int i = 0; i < rand; i++)
                            removePart();// уберем часть змеи
						}
                    }
                else
                    {
                    controller.SetScore(1);// увеличим очки
                    if (Random.Range(0, 10) == 0 && foods.Count <= 5) // если выпал шанс и бонусов меньше 6
                        newBonus = true;// можно добавить новый бонус
                    Instantiate(eatExplousionPrefab, ppos, Quaternion.identity);// эффект
                    addFood(FoodScript.States.EAT);// добавим новую еду
                    speed += .2f;// увеличим скорость
                    }
                Destroy(food);// удалим еду
                foods.Remove(food);// уберем ее из списка
                if(PlayerPrefs.GetInt("sounds", 1) == 1) // если звуки разрешены
                    source.PlayOneShot(clip);// проиграем звук поедания еды
                if (foodScript.State == FoodScript.States.EAT) // если был бонус - еда
                    return true;// укажем что было столкновение, для отмены удаления хвоста
                return false;// столкновений не было
				}
			}
        return false;// колизии не было
		}
    private bool checkCollisionSnake() // столкновение головы змеи с
        {
        if (!invise) // если невидимость не активна
            {
            GameObject head = snakes[snakes.Count - 1];// голова змеи
            Vector3 pos = head.transform.position;
            for (int i = 0; i < snakes.Count - 2; i++) // самой собой
                {
                Vector3 ppos = Round(snakes[i].transform.position, 1);
                if (Equals(pos, ppos))
                    return true;
                }
            foreach (GameObject wall in walls) // со стенами
                {
                Vector3 ppos = Round(wall.transform.position, 1);
                if (Equals(pos, ppos))
                    return true;
                }
            }
        return false;// столкновения не было
		}
    private void genWalls() // генерируем стену из данных фвйла
        {
        TextAsset text = Resources.Load("level/" + Data.level) as TextAsset;// получим данные файла
        string test = text.text;// получим текст из данных файла
        GameObject wall;// для создания обьектов стены
        int x = -Data.WIDTH / 2, y = Data.HEIGHT / 2;// координаты создания обьекта
        // пройдем по всем символам файла
        foreach (char ch in test)
            {
            if(ch.Equals('\n')) // если символ: перенос строки
                {
                x = -Data.WIDTH / 2;// сбросим координату по оси X
                y--;// опустимся ниже по оси Y
                continue;
				}
            if(ch.Equals('1')) // если символ: 1
                {
                // добавим блок стены
                Vector3 pos = Round(new Vector3(x * scaleX, y * scaleY, 0), 1);// координаты
                wall = Instantiate(wallPrefab, pos, Quaternion.identity);
                wall.transform.SetParent(wallsContainer.transform);
                walls.Add(wall);
                }
            x++;// передвинемся на след. координату по оси X
            }
		}
    private Vector3 Round(Vector3 vector, int diapason) // округление координат вектора
        {
        vector.x = (float)System.Math.Round(vector.x, diapason);
        vector.y = (float)System.Math.Round(vector.y, diapason);
        return vector;
		}
    private bool Equals(Vector3 vec_1, Vector3 vec_2) // сравнение векторов
        {
        if (Mathf.Approximately(vec_1.x, vec_2.x) && Mathf.Approximately(vec_1.y, vec_2.y))
            return true;
        return false;
		}
	}