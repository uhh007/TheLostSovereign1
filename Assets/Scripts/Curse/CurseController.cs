using System;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class CurseController : MonoBehaviour
{
    [SerializeField] private GameObject TextCurseTrue; //������� ������ ������ �������� ���������
    [SerializeField] private float DamageRatio = 30f; //����������� ����� ��� �����������
    [SerializeField] private GameObject[] AntidoteCube = new GameObject[2]; //������� ������� ����������� (0 - �������� 1 - ��������)
    [SerializeField] private GameObject[] CurseArea = new GameObject[2]; //������� ������� �������� ��������� (0 - �������� 1 - ��������)
    [SerializeField] private GameObject[] CurseBox = new GameObject[2]; //������� ������� ���������� ��������� (0 - �������� 1 - ��������)
    [SerializeField] private float TeleportationDistance = 1f; //���������� ������ � ������� ��� ������������
    [SerializeField] private float TeleportationTimeOut = 0f; //����� �� ��������� ��������� ������������
    [SerializeField] private float StackTime = 10f; //����� ������� �����
    [SerializeField] private float DamageTime = 1f; //����� ����� ���������� �����
    [SerializeField] private float DamageHealth = 5f; //������� �������� �����

    private float[] CurseAreaRadius = new float[2]; //������ ��������� ������������ (0 - �������� 1 - ��������)
    private float[] CurseTimer = new float[3]; //���������� �������� �������� ������� (0 - ������������ ��������� 1 - ����� ������ 2 - ���������� �����)
    private bool[] PlayerImmunity = {false, false}; //������� ���������� � ��������� (0 - �������� 1 - ��������)
    private float StackRatio;
    private bool CurseWorks = false;
    private Text txtcrstrue;
    private bool HealthSpawn = false;
    System.Random rnd = new();

    private void Start()
    {
        CurseTimer[0] = TeleportationTimeOut;
        CurseTimer[1] = StackTime;
        StackRatio = DamageHealth;
        CapsuleCollider crsareaht = CurseArea[0].GetComponent<CapsuleCollider>();
        CurseAreaRadius[0] = crsareaht.radius;
        CapsuleCollider crsareasp = CurseArea[1].GetComponent<CapsuleCollider>();
        CurseAreaRadius[1] = crsareasp.radius;
        txtcrstrue = TextCurseTrue.GetComponent<Text>();
    }

    private void Update()
    {
        //Debug.Log(rnd.NextDouble());
        if (CurseTimer[0] > 0) CurseTimer[0] -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        if (Vector3.Distance(CurseBox[0].transform.position, transform.position) < TeleportationDistance)
        {
            CurseTeleportation(CurseAreaRadius[0], CurseArea[0]);
        }
        if (Vector3.Distance(CurseBox[1].transform.position, transform.position) < TeleportationDistance)
        {
            CurseTeleportation(CurseAreaRadius[1], CurseArea[1]);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("CurseHealth") && !PlayerImmunity[0])
        {
            HealthBoxPosition(AntidoteCube[0], CurseBox[0], CurseAreaRadius[0]);
            txtcrstrue.text = "��������� �������� ���������";
            CurseWorks = true;
            CurseTimer[2] -= Time.deltaTime;
            if (CurseTimer[2] <= 0)
            {
                CurseTimer[2] = DamageTime;
                FirstPersonController.Health -= DamageHealth;
            }
            if (CurseTimer[1] > 0) CurseTimer[1] -= Time.deltaTime;
            if (CurseTimer[1] <= 0)
            {
                DamageHealth += StackRatio;
                CurseTimer[1] = StackTime;
            }
        }
        if (other.gameObject.CompareTag("CurseSpeed") && !PlayerImmunity[1])
        {
            HealthBoxPosition(AntidoteCube[1], CurseBox[1], CurseAreaRadius[1]);
            txtcrstrue.text = "��������� �������� ���������";
            CurseWorks = true;
            FirstPersonController.SpeedRatio = Vector3.Distance(CurseBox[1].transform.position, transform.position) / DamageRatio;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("CurseHealth"))
        {
            txtcrstrue.text = "��������� �� ���������";
            CurseWorks = false;
        }
        if (other.gameObject.CompareTag("CurseSpeed"))
        {
            txtcrstrue.text = "��������� �� ���������";
            FirstPersonController.SpeedRatio = 1f;
            CurseWorks = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("SpeedAntidoteCube"))
        {
            txtcrstrue.text = "��������� �� ���������";
            FirstPersonController.SpeedRatio = 1f;
            PlayerImmunity[1] = true;
            Destroy(other.gameObject);
        }
        if (other.gameObject.CompareTag("HealthAntidoteCube"))
        {
            txtcrstrue.text = "��������� �� ���������";
            PlayerImmunity[0] = true;
            Destroy(other.gameObject);
        }
    }

    private void HealthBoxPosition(GameObject AntidoteBox,GameObject CurseBox, float CurseAreaRadius)
    {
        float x;
        float z;
        if (transform.position.x >= CurseBox.transform.position.x && transform.position.z >= CurseBox.transform.position.z && CurseWorks)
        {
            z = CurseBox.transform.position.z - (CurseAreaRadius / 2);
            x = CurseBox.transform.position.x - (CurseAreaRadius / 2);
            HealthBoxSpawn(AntidoteBox,x, CurseBox.transform.position.y, z);
        }
        if (transform.position.x <= CurseBox.transform.position.x && transform.position.z >= CurseBox.transform.position.z && CurseWorks)
        {
            z = CurseBox.transform.position.z + (CurseAreaRadius / 2);
            x = CurseBox.transform.position.x - (CurseAreaRadius / 2);
            HealthBoxSpawn(AntidoteBox, x, CurseBox.transform.position.y, z);
        }
        if (transform.position.x <= CurseBox.transform.position.x && transform.position.z <= CurseBox.transform.position.z && CurseWorks)
        {
            z = CurseBox.transform.position.z + (CurseAreaRadius / 2);
            x = CurseBox.transform.position.x + (CurseAreaRadius / 2);
            HealthBoxSpawn(AntidoteBox, x, CurseBox.transform.position.y, z);
        }
        if (transform.position.x >= CurseBox.transform.position.x && transform.position.z <= CurseBox.transform.position.z && CurseWorks)
        {
            z = CurseBox.transform.position.z - (CurseAreaRadius / 2);
            x = CurseBox.transform.position.x + (CurseAreaRadius / 2);
            HealthBoxSpawn(AntidoteBox, x, CurseBox.transform.position.y, z);
        }
    }

    private void HealthBoxSpawn(GameObject Antidote, float x, float y, float z)
    {
        if (!HealthSpawn)
        {
            Instantiate(Antidote, new Vector3(x, y, z), Quaternion.identity);
            HealthSpawn = true;
        }
    }

    void CurseTeleportation(float CurseRadius, GameObject CurseArea)
    {
        if (CurseTimer[0] <= 0)
        {
            //float theta = (float)((double)rnd.Next() / int.MaxValue * 2 * Math.PI);
            float theta = (float)(rnd.NextDouble() * 2 * Math.PI);
            float x = (float)(rnd.NextDouble() * Math.Cos(theta));
            float z = (float)(rnd.NextDouble() * Math.Sin(theta));

            CurseArea.transform.position = new Vector3(CurseArea.transform.position.x + x, CurseArea.transform.position.y, CurseArea.transform.position.z + z);
            Destroy(GameObject.FindGameObjectWithTag("HealthAntidoteCube"));
            HealthSpawn = false;
            CurseTimer[0] = TeleportationTimeOut;
        }
    }
}