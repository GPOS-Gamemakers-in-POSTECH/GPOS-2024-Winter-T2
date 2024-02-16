using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scrt_lever : MonoBehaviour
{
    public int floorLoc = 0; //��� �ִ� �ٴ��� ��ġ, 0: �ٴ�, 1: ���ʺ� 2: õ�� 3: �����ʺ�
    public int leverCode = 0; //0: ������ ���ִ� ����  1: ������ ����� ����
    public int remoteFloorLoc = 0; //���ְų� ���� ������Ʈ�� ���� �ٴ��� ��ġ
    public float obx; //�ش� ������ ������ ����ų� ������ �ش�Ǵ� ��ġ�� ���� ��ǥ��
    public float oby;
    public GameObject whatObj; //�� ���ְ� ������ ����

    SpriteRenderer spriteRenderer;
    Animator animator;

    GameObject relatedObj;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        gameObject.tag = "object";

        if (floorLoc != 0) { spriteRenderer.transform.localRotation = Quaternion.Euler(0f, 0f, (4 - floorLoc) * 90f); }

        if(leverCode == 0)
        {
            relatedObj = Instantiate(whatObj, new Vector3(obx, oby, 0f), Quaternion.Euler(0f, 0f, (4 - remoteFloorLoc) * 90f));
            animator.SetBool("bool_On", false);
        }
        else
        {
            animator.SetBool("bool_On", true);
        }
    }

    void Update()
    {
        
    }

    public void interAct() //��ȣ�ۿ��� �� �� �ش� ������ ȣ���ϸ� ��
    {
        if (leverCode == 0) //����
        {
            Destroy(relatedObj);
            leverCode = 1;
            animator.SetBool("bool_On", false);
        }
        else //����
        {
            relatedObj = Instantiate(whatObj, new Vector3(obx, oby, 0f), Quaternion.Euler(0f, 0f, (4 - remoteFloorLoc) * 90f));
            leverCode = 0;
            animator.SetBool("bool_On", true);
        }
    }
}
