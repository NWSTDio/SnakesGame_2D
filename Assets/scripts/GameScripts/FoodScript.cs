using UnityEngine;

// скрипт для работы с отдельными обьектами еды для змейки

public class FoodScript : MonoBehaviour
    {
    private Animator animator;// для работы с анимацией
    public enum States { EAT, INVISE, LOW_SPEED, CUTE } // состояния бонусов (еда, невидимость, сброс скорости на 2, обрезание части змеи)
    // смена и тип анимации происходят когда вызывают переменную State: GetComponent<FoodScript>().State = FoodScrip.States.INVISE
    public States State // для работы с сотояниями анимации (тип анимации и есть тип бонуса)
        {
        get { return (States)animator.GetInteger("state"); }
        set { animator.SetInteger("state", (int)value); }
        }
    private void Awake()
        {
        animator = GetComponent<Animator>();
        }
    }