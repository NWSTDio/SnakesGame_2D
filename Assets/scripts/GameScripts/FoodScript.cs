using UnityEngine;

// ������ ��� ������ � ���������� ��������� ��� ��� ������

public class FoodScript : MonoBehaviour
    {
    private Animator animator;// ��� ������ � ���������
    public enum States { EAT, INVISE, LOW_SPEED, CUTE } // ��������� ������� (���, �����������, ����� �������� �� 2, ��������� ����� ����)
    // ����� � ��� �������� ���������� ����� �������� ���������� State: GetComponent<FoodScript>().State = FoodScrip.States.INVISE
    public States State // ��� ������ � ���������� �������� (��� �������� � ���� ��� ������)
        {
        get { return (States)animator.GetInteger("state"); }
        set { animator.SetInteger("state", (int)value); }
        }
    private void Awake()
        {
        animator = GetComponent<Animator>();
        }
    }