using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/*

    � ������ ������������� ����, ��������� ������� �������� ������
    ������ �������� �� ������ ��������, �� ��� ������ ������������
    ������� ������ ������� ������ - �����, � ������ ��������� � 
    ����� ������, �� ��������� � ��������� �����������, �� ����������
    � ������, ������� ��� ������� � ������� ����� ������������

    ���: ��� ��� ������ ��� ������� ����
    ������� �����, ������ ���, ���������� �����, �.�. ���� ���� ������ �������� � ������ ����,
    �� ��� ���� �� �������� ����� ������� � �����
    ������ ����� �������� ������ ���� �� 1 �� 10 �� ���, �� ��� ������� ��� ������ ���� �� ����� 15 ���������
    ����� ����� �������� �������� ���� �� 2
    ������� ����� ���� ����������� ���� �� 10 ������
    �.�. ������������ ��������� ������ ��� ������, �� �� ����� ������� ���� ����� ���� �������� � ���� ��� � �����,
    ����� �� ������ ������ �� ������

*/

public class PlayerScript : MonoBehaviour
    {
    [SerializeField] GameController controller;// ���������� ����
    public enum DIRECTION { LEFT, RIGHT, UP, DOWN } // ��� ������ � ������������
    DIRECTION eyeDir = DIRECTION.RIGHT;// ����������� �������
    DIRECTION moveDir = DIRECTION.RIGHT;// ����������� ��������
    [SerializeField] GameObject bodyPartPrefab, spawnPoint;// ���� ���� � ����� �����
    [SerializeField] GameObject foodsContainer, foodPrefab;// ��������� ��� � ��������� ���
    [SerializeField] GameObject wallsContainer, wallPrefab;// ��������� ���� � ��������� �����
    float scaleX, scaleY;// ������� ������ �������
    float moveTimer = 0, speed = 5.0f;// ��� �������� ���� ��� �������������� ������������
    List<GameObject> snakes;// ����� ����
    List<GameObject> foods;// ��� �� ������
    List<GameObject> walls;// �����
    [SerializeField] Color colorHead, colorBody;// ���� ������ � ����
    // ������ ��� �������� �������
    [SerializeField] GameObject eatExplousionPrefab, inviseExplousionPrefab, lowSpeedExplousionPrefab, cuteExplousionPrefab;

    float bonusTimer = 0, bonusTime = 0;// ������� ����� ������ � ���� ����� ������ (��� �������� �����)
    bool invise = false;// ����������� ������
    bool newBonus = false, isBonus = false;// ����� �� �����  ����� � ���� �� ������ �������� ����� (������� ����)

    [SerializeField] AudioClip clip;// ���� �������� ������
    AudioSource source;// ��� ������ �� �������

    void Start()
        {
        snakes = new List<GameObject>();// ���� ������
        foods = new List<GameObject>();// ��� ���
        walls = new List<GameObject>();// ��������
        // ������� ������
        GameObject temp = Instantiate(bodyPartPrefab, spawnPoint.transform.position, Quaternion.identity) as GameObject;
        temp.GetComponent<SpriteRenderer>().color = colorHead;// ���� ������
        temp.transform.SetParent(transform);// ������ �� �������� �������� ������� ���� �� �������� ����
        snakes.Add(temp);// ������� ������ � ������ �������� ����� ����
        // ������� ������� ��������
        scaleX = temp.transform.localScale.x;
        scaleY = temp.transform.localScale.y;
        genWalls();// ����������� �����
        // ������� ��� ����� ����, ��������� ����������� ����� �������
        addPart(DIRECTION.RIGHT);
        addPart(DIRECTION.RIGHT);
        addFood(FoodScript.States.EAT);// ������� ���
        invise = false;// �������� ����������� ����
        source = GetComponent<AudioSource>();// ������� ��������� ��� ������ �� �������
        }
    void Update()
        {
        if (controller.State == GameController.GAMESTATE.RUNNING) // ���� ���� �� ��������
            {
            if (isBonus) // ���� ���������� ����� �����������
                {
                if (bonusTimer < bonusTime) // ���� ����� ������ �� ��������
                    {
                    // ������� ����� ����� ������� ������ ��� ����� ����
                    bonusTimer += Time.deltaTime;// ������� �����
                    float tmp = bonusTime - bonusTimer;// ������� ���������� ������� �� �������
                    float per = tmp * 100 / bonusTime;// ���������� �����������
                    float size = per * 512 / 100;// ������ �������� � ������������ � % ������������ ������������ �������
                    controller.UpdateBonusBar(size);// ������� ����� ���
                    }
                else
                    {
                    bonusTime = 0;// ������� ������ ������
                    bonusTimer = 0;// ������� ����� ������
                    controller.HideBonusBar();// ������� ����� ���
                    invise = false;// ������ ����������� ����
                    isBonus = false;// ������ ��� ����� �� �������
                    }
                }
            if (Data.DEBUG_MODE) // ����� �������
                {
                bool isKeyDown = false;// ���� �� ������� �������
                // ��������� ����������� ������� � �������������� �����������
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
                if (isKeyDown) // ���� ������ �������
                    {
                    moveDir = eyeDir;// ��������� ��������
                    addPart(moveDir);// ������� ������
                    if (!checkCollisionFood()) // ���� ��� ������������ � ����
                        removePart();// ������ �����
                    if (checkCollisionSnake()) // ���� ���� ����������� � ����� ��� ������
                        controller.State = GameController.GAMESTATE.GAME_OVER;
                    }
                }
            else
                {
                // ��������� ����������� �������
                if (Input.GetKeyDown(KeyCode.A) && eyeDir != DIRECTION.LEFT && eyeDir != DIRECTION.RIGHT && moveDir != DIRECTION.RIGHT)
                    eyeDir = DIRECTION.LEFT;
                if (Input.GetKeyDown(KeyCode.D) && eyeDir != DIRECTION.RIGHT && eyeDir != DIRECTION.LEFT && moveDir != DIRECTION.LEFT)
                    eyeDir = DIRECTION.RIGHT;
                if (Input.GetKeyDown(KeyCode.W) && eyeDir != DIRECTION.UP && eyeDir != DIRECTION.DOWN && moveDir != DIRECTION.DOWN)
                    eyeDir = DIRECTION.UP;
                if (Input.GetKeyDown(KeyCode.S) && eyeDir != DIRECTION.DOWN && eyeDir != DIRECTION.UP && moveDir != DIRECTION.UP)
                    eyeDir = DIRECTION.DOWN;
                if (moveTimer >= 1f) // ���� ������ ���
                    {
                    moveDir = eyeDir;// �������� ����������� ��������
                    moveTimer = 0;// ������� nqvth ifuf
                    addPart(moveDir);// ������� ������
                    if (!checkCollisionFood()) // ���� �� ���� ������������ � ����
                        removePart();// ������ �����
                    if (checkCollisionSnake()) // ���� ���� ������� ���� ��� �����
                        controller.State = GameController.GAMESTATE.GAME_OVER;
                    }
                else moveTimer += speed * Time.deltaTime;// �������� ������
                }
            }
        if (Data.DEBUG_MODE && Input.GetKeyUp(KeyCode.Space)) // � ������ ������� ��� ������� ������� ������
            SceneManager.LoadScene("GameScene");// ���������� �����
        if(newBonus) // ���� ��������� �������� ������ ������
            {
            if (Random.Range(0, 3) == 0) // ���� ��� �������� �����, ������ ��������
                addFood(FoodScript.States.LOW_SPEED);
            else if (Random.Range(0, 3) == 0) // ���� ��� �������� ����� ��������� ������
                addFood(FoodScript.States.CUTE);
            else
                addFood(FoodScript.States.INVISE);// ����� �������� ����� ����������� ������
            newBonus = false;// ������ ��� ����� ����� �� �����
			}
        }

    public void Move(DIRECTION dir) // �������� ������ ����� ��������
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
    private void addPart(DIRECTION dir) // ���������� ������ � �������������� �����������
        {
        // ���������� ������� ������ � ����� ����
        snakes[snakes.Count - 1].GetComponent<SpriteRenderer>().color = colorBody;
        // ������� ���������� ������� ������ ����
        Vector3 pos = Round(snakes[snakes.Count - 1].transform.position, 1);
        // ������� ������ � �������� �����������
        if (eyeDir == DIRECTION.LEFT)
            pos.x -= scaleX;
        else if (eyeDir == DIRECTION.RIGHT)
            pos.x += scaleX;
        else if (eyeDir == DIRECTION.UP)
            pos.y += scaleY;
        else if (eyeDir == DIRECTION.DOWN)
            pos.y -= scaleY;
        // ���� ������� ������ �� ��� X
        if (pos.x < -Data.WIDTH / 2 * scaleX)
            pos.x = Data.WIDTH / 2 * scaleX;
        if (pos.x > Data.WIDTH / 2 * scaleX)
            pos.x = -Data.WIDTH / 2 * scaleX;
        // ���� ������� ������ �� ��� Y
        if (pos.y < -Data.HEIGHT / 2 * scaleY)
            pos.y = Data.HEIGHT / 2 * scaleY;
        if (pos.y > Data.HEIGHT / 2 * scaleY)
            pos.y = -Data.HEIGHT / 2 * scaleY;
        pos = Round(pos, 1);// �������� �������, �� ���� ������
        // �������� ������
        GameObject temp = Instantiate(bodyPartPrefab, pos, Quaternion.identity) as GameObject;
        temp.transform.SetParent(transform);
        temp.GetComponent<SpriteRenderer>().color = colorHead;
        snakes.Add(temp);// ������� �� � ������
        }
    private void removePart() // �������� ������
        {
        GameObject tail = snakes[0];// ������� ������ �������
        Destroy(tail);// ������ ��� ������
        snakes.RemoveAt(0);// ������ ��� �� ������
		}
    private void addFood(FoodScript.States state) // ���������� ���
        {
        // ���� ����� ���� � ������ ���� ������ ��� ������ �� ����
        if (snakes.Count + walls.Count >= Data.WIDTH * Data.HEIGHT)
            return;
        List<Vector3> addr = new List<Vector3>();// ������ �������� ���������
        foreach(GameObject obj in snakes) // ������� ��� ����� ����
            addr.Add(obj.transform.position);
        foreach (GameObject obj in walls) // ������� ��� ����� ����
            addr.Add(obj.transform.position);
        foreach (GameObject obj in foods) // ������� ��� ����� ���
            addr.Add(obj.transform.position);
        List<Vector3> emptyAddr = new List<Vector3>();// ������ �������� ���������
        Vector3 pos;
        // ������� �� ����� ����
        for(int i = -Data.WIDTH / 2; i <= Data.WIDTH / 2; i++)
            for(int j = -Data.HEIGHT / 2; j <= Data.HEIGHT / 2; j++)
                {
                pos = Round(new Vector3(i * scaleY, j * scaleX, 0), 1);
                if (addr.Contains(pos)) // ���� ��������� ����� ���� � ��������� ������
                    continue;// ���� ��� ���
                emptyAddr.Add(pos);// ������� ����� � ������ ���������
				}
        if (emptyAddr.Count <= 1) // ���� ��������� ������� ����
            return;
        pos = Round(emptyAddr[(int)Random.Range(0, emptyAddr.Count)], 1);
        // �������� � ������� ��� � ������
        GameObject food = Instantiate(foodPrefab, pos, Quaternion.identity) as GameObject;
        food.transform.SetParent(foodsContainer.transform);
        food.GetComponent<FoodScript>().State = state;
        foods.Add(food);
		}
    private bool checkCollisionFood() // �������� ������������ � ����
        {
        GameObject head = snakes[snakes.Count - 1];// ������
        Vector3 pos = Round(head.transform.position, 1);// ���������� ������
        FoodScript foodScript;// ��� ������� ������ � �����������
        foreach(GameObject food in foods) // ������� �� ���� ���
            {
            Vector3 ppos = Round(food.transform.position, 1);
            if (Equals(pos, ppos)) // ���� ���������� �����
                {
                foodScript = food.GetComponent<FoodScript>() as FoodScript;// ������� ���������
                if (foodScript.State == FoodScript.States.INVISE) // ���� ��� - ����� �����������
                    {
                    Instantiate(inviseExplousionPrefab, ppos, Quaternion.identity);// ������
                    controller.ShowBonusBar();// ������� ����� ���
                    controller.SetScore(5);// �������� ����
                    bonusTime = 10.0f;// ����� ������
                    invise = true;// ������� �����������
                    isBonus = true;// ������ ��� �������� �����
                    }
                else if (foodScript.State == FoodScript.States.LOW_SPEED) // ���� ��� - ����� ����������� ��������
                    {
                    Instantiate(lowSpeedExplousionPrefab, ppos, Quaternion.identity);
                    controller.SetScore(10);// �������� ����
                    speed -= 2.0f;// �������� ��������
                    if (speed <= 5f) // ���� ����� �������� ������ �����������
                        speed = 5f;// ����������� ��������
                    }
                else if (foodScript.State == FoodScript.States.CUTE) // ���� ��� - ������ ����� ����
                    {
                    Instantiate(cuteExplousionPrefab, ppos, Quaternion.identity);
                    controller.SetScore(20);// �������� ����
                    if (snakes.Count >= 15) // ���� ������ ������ ����� ��� ����� 15
                        {
                        int rand = Random.Range(1, 11);// ������ ������� ������ ���� ������
                        for (int i = 0; i < rand; i++)
                            removePart();// ������ ����� ����
						}
                    }
                else
                    {
                    controller.SetScore(1);// �������� ����
                    if (Random.Range(0, 10) == 0 && foods.Count <= 5) // ���� ����� ���� � ������� ������ 6
                        newBonus = true;// ����� �������� ����� �����
                    Instantiate(eatExplousionPrefab, ppos, Quaternion.identity);// ������
                    addFood(FoodScript.States.EAT);// ������� ����� ���
                    speed += .2f;// �������� ��������
                    }
                Destroy(food);// ������ ���
                foods.Remove(food);// ������ �� �� ������
                if(PlayerPrefs.GetInt("sounds", 1) == 1) // ���� ����� ���������
                    source.PlayOneShot(clip);// ��������� ���� �������� ���
                if (foodScript.State == FoodScript.States.EAT) // ���� ��� ����� - ���
                    return true;// ������ ��� ���� ������������, ��� ������ �������� ������
                return false;// ������������ �� ����
				}
			}
        return false;// ������� �� ����
		}
    private bool checkCollisionSnake() // ������������ ������ ���� �
        {
        if (!invise) // ���� ����������� �� �������
            {
            GameObject head = snakes[snakes.Count - 1];// ������ ����
            Vector3 pos = head.transform.position;
            for (int i = 0; i < snakes.Count - 2; i++) // ����� �����
                {
                Vector3 ppos = Round(snakes[i].transform.position, 1);
                if (Equals(pos, ppos))
                    return true;
                }
            foreach (GameObject wall in walls) // �� �������
                {
                Vector3 ppos = Round(wall.transform.position, 1);
                if (Equals(pos, ppos))
                    return true;
                }
            }
        return false;// ������������ �� ����
		}
    private void genWalls() // ���������� ����� �� ������ �����
        {
        TextAsset text = Resources.Load("level/" + Data.level) as TextAsset;// ������� ������ �����
        string test = text.text;// ������� ����� �� ������ �����
        GameObject wall;// ��� �������� �������� �����
        int x = -Data.WIDTH / 2, y = Data.HEIGHT / 2;// ���������� �������� �������
        // ������� �� ���� �������� �����
        foreach (char ch in test)
            {
            if(ch.Equals('\n')) // ���� ������: ������� ������
                {
                x = -Data.WIDTH / 2;// ������� ���������� �� ��� X
                y--;// ��������� ���� �� ��� Y
                continue;
				}
            if(ch.Equals('1')) // ���� ������: 1
                {
                // ������� ���� �����
                Vector3 pos = Round(new Vector3(x * scaleX, y * scaleY, 0), 1);// ����������
                wall = Instantiate(wallPrefab, pos, Quaternion.identity);
                wall.transform.SetParent(wallsContainer.transform);
                walls.Add(wall);
                }
            x++;// ������������ �� ����. ���������� �� ��� X
            }
		}
    private Vector3 Round(Vector3 vector, int diapason) // ���������� ��������� �������
        {
        vector.x = (float)System.Math.Round(vector.x, diapason);
        vector.y = (float)System.Math.Round(vector.y, diapason);
        return vector;
		}
    private bool Equals(Vector3 vec_1, Vector3 vec_2) // ��������� ��������
        {
        if (Mathf.Approximately(vec_1.x, vec_2.x) && Mathf.Approximately(vec_1.y, vec_2.y))
            return true;
        return false;
		}
	}