/// developed by pr3senty
/// 
/// the values from the GameObjectsToConvert array
/// must match the values from TypesOfCurseObjects


using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


public class CurseManager : MonoBehaviour
{
    public int TimeBeetweenStacks;
    public int CountStacksForInvisible;
    public float TimeBetweenDamage;
    public float BaseHealthDamage; // Base Curse Damage
    public float SpeedEffectRatio;
    public float SpeedStackRatio;

    public float HealthStackDamage;

    public float TeleportationDistance;
    public int CurseRadius;
    public float TeleportationTimeOut;

    public GameObject AntidotePrefab; // Antidote object prefab
    
    public GameObject[] GameObjectsToConvert;
    public int[] TypesOfCurseObjects;

    private List<CurseObject> CurseObjects = new List<CurseObject>(); // massive for all curse objects
    private System.Random rnd = new System.Random();


    void Start()
    /// initialize CurseObjects from list GameObjectsToConvert
    {
        if (GameObjectsToConvert.Length > 0)
        {
            for (int i = 0; i < GameObjectsToConvert.Length; i++)
            {
                CurseObject CurseObject = new CurseObject();
                CurseObject.gameObject = GameObjectsToConvert[i];

                var CurseArea = new GameObject("CurseArea");

                CurseArea.transform.parent = CurseObject.gameObject.transform;
                CurseArea.transform.position = CurseObject.gameObject.transform.position;
                CurseArea.tag = "CurseArea";

                CurseArea.AddComponent<CapsuleCollider>();

                CurseArea.GetComponent<CapsuleCollider>().radius = CurseRadius;
                CurseArea.GetComponent<CapsuleCollider>().isTrigger = true;

                CurseObject.CurseArea = CurseArea;
                CurseObject.TypeOfCurse = TypesOfCurseObjects[i];

                CurseObjects.Add(CurseObject);
            }
        }
    }

    private void Update()
    {
        if (CurseObjects.Count > 0)
        {
            for (int i = 0; i < CurseObjects.Count; i++)
            {
                var CurseObject = CurseObjects[i];

                if (CurseObject.Works)
                {
                    CurseObject.TimeAfterDamage += Time.deltaTime;

                    if (MathF.Round(CurseObject.TimeAfterTeleportation, 2) != TeleportationTimeOut)
                    {
                        CurseObject.TimeAfterTeleportation += Time.deltaTime;
                    }

                    ApplyCurse(CurseObject);

                    CheckStacks(CurseObject);
                }
            }
        }
    }

    //private void OnGUI()
    //{
    //    const int BETWEEN_LABELS = 20;
    //    const int WIDTH = 800;

    //    GUI.Box(new Rect(10, 10, WIDTH, 150), "Проклятия на сцене:");

    //    int x = 20;
    //    int y = 10;

    //    float totalHealthDamage = 0;
    //    float totalSpeedRatio = 1;
    //    for (int i = 0; i < CurseObjects.Count; i++)
    //    {
    //        string name = CurseObjects[i].gameObject.name;
    //        int stacks = CurseObjects[i].StacksNum;

    //        string text = GenerateText(CurseObjects[i], name, stacks);

    //        GUI.Label(new Rect(x, y + BETWEEN_LABELS, WIDTH, 20), text);

    //        if (CurseObjects[i].TypeOfCurse == 1 && CurseObjects[i].Works) { totalHealthDamage += BaseHealthDamage + CurseObjects[i].StacksNum * HealthStackDamage; }
    //        if (CurseObjects[i].TypeOfCurse == 2 && CurseObjects[i].Works) { totalSpeedRatio *= SpeedEffectRatio - CurseObjects[i].StacksNum * SpeedStackRatio; }

    //        y += BETWEEN_LABELS;
    //    }

    //    GUI.Label(new Rect(x, y + BETWEEN_LABELS * 2, WIDTH, 20), "Общий урон здоровью: " + totalHealthDamage);
    //    GUI.Label(new Rect(x, y + BETWEEN_LABELS * 3, WIDTH, 20), "Конечный коэффицент скорости игрока " + totalSpeedRatio);
    //}

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("CurseArea"))
        {
            var CurseObject = FindCurseObject(other.gameObject.transform.parent.gameObject);

            if (CurseObject != null)
            {
                CurseObject.TimeAfterStack += Time.deltaTime;

                if (MathF.Round(CurseObject.TimeAfterStack, 2) == TimeBeetweenStacks)
                {
                    CurseObject.TimeAfterStack = 0f;
                    CurseObject.StacksNum += 1;
                }

                if ((MathF.Round(CurseObject.TimeAfterTeleportation, 2) == TeleportationTimeOut) && (Vector3.Distance(CurseObject.gameObject.transform.position, transform.position) <= TeleportationDistance))
                {
                    CurseObject.TimeAfterTeleportation = 0f;
                    TeleportCurse(CurseObject);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("CurseArea"))
        {
            var CurseObject = FindCurseObject(other.gameObject.transform.parent.gameObject);
            if (!CurseObject.Works)
            {
                CurseObject.Works = true;
                CurseObject.CurseEnterTime = (int)Time.time; // Time when player enter into CaplsuleCollider
                HealthBoxPosition(AntidotePrefab, CurseObject);
            }
        }

        if (other.gameObject.CompareTag("AntidoteObject")) 
        {
            var CurseObject = FindCurseObject(other.gameObject.transform.parent.gameObject);
            RemoveCurse(CurseObject);
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("CurseArea"))
        {
            var CurseObject = FindCurseObject(other.gameObject.transform.parent.gameObject);

            CurseObject.Works = false;
            CurseObject.TimeAfterStack = 0f;

            Destroy(CurseObject.AntidoteObject);
        }
    }

    private void HealthBoxPosition(GameObject AntidotePrefab, CurseObject CurseObject)
    /// Calculates the position of AntidoteObjects and spawn it there
    {
        int x;
        int z;

        float CurseAreaPositionX = CurseObject.CurseArea.transform.position.x;
        float CurseAreaPositionZ = CurseObject.CurseArea.transform.position.z;

        (x, z) = RandomDotInCircle(Math.PI / 2, CurseRadius);

        int QuarterOfCircle = WherePlayerStay(CurseAreaPositionX, CurseAreaPositionZ);

        if (QuarterOfCircle == 1)
        {
            HealthBoxSpawn(AntidotePrefab, CurseObject, CurseAreaPositionX - x, CurseObject.gameObject.transform.position.y, CurseAreaPositionZ + z);
        }
        else if (QuarterOfCircle == 2)
        {
            HealthBoxSpawn(AntidotePrefab, CurseObject, CurseAreaPositionX - x, CurseObject.gameObject.transform.position.y, CurseAreaPositionZ - z);
        }
        else if (QuarterOfCircle == 3)
        {
            HealthBoxSpawn(AntidotePrefab, CurseObject, CurseAreaPositionX + x, CurseObject.gameObject.transform.position.y, CurseAreaPositionZ - z);
        }
        else if (QuarterOfCircle == 4)
        {
            HealthBoxSpawn(AntidotePrefab, CurseObject, CurseAreaPositionX + x, CurseObject.gameObject.transform.position.y, CurseAreaPositionZ + z);
        }
        else
        {
            print(QuarterOfCircle);
            print(CurseAreaPositionX + " " + CurseAreaPositionZ);
            print(transform.position.x + " " + transform.position.z);
        }
    }

    private void HealthBoxSpawn(GameObject AntidotePrefab, CurseObject CurseObject, float x, float y, float z)
    /// Spawn AntidoteObjects
    {
        var AntidoteObject = Instantiate(AntidotePrefab, new Vector3(x, y + 1, z), Quaternion.identity);

        if (CurseObject.Invisible)
        {
            Destroy(AntidoteObject.GetComponent<MeshFilter>());
        }

        AntidoteObject.transform.parent = CurseObject.gameObject.transform;
        AntidoteObject.tag = "AntidoteObject";
        print("YES");

        CurseObject.AntidoteObject = AntidoteObject;
    }

    private void RemoveCurse(CurseObject CurseObject)
    /// Remove Curse from object
    {
        CurseObject.Works = false;

        //if (CurseObject.TypeOfCurse == 2)
        //{
        //    FirstPersonController.SpeedRatio = 1f;
        //}

        Destroy(CurseObject.CurseArea);
        Destroy(CurseObject.AntidoteObject);
        CurseObjects.Remove(CurseObject);
    }

    private (int, int) RandomDotInCircle(double CircleLength, int Radius)
    /// Return random coordinates (x, y) in circle
    {
        var theta = rnd.NextDouble() * CircleLength; 
        var r = Math.Sqrt(rnd.NextDouble()) * Radius;
        return ((int)(r * Math.Cos(theta)), (int)(r * Math.Sin(theta)));
    }

    private int WherePlayerStay(float CurseAreaPositionX, float CurseAreaPositionZ)
    /// Returns the quarter circle where the player is standing
    {
        if ((transform.position.x >= CurseAreaPositionX && transform.position.x <= CurseAreaPositionX + CurseRadius) && (transform.position.z >= CurseAreaPositionZ - CurseRadius && transform.position.z <= CurseAreaPositionZ))
        {
            return 1;
        }
        else if ((transform.position.x >= CurseAreaPositionX && transform.position.x <= CurseAreaPositionX + CurseRadius) && (transform.position.z >= CurseAreaPositionZ && transform.position.z <= CurseAreaPositionZ + CurseRadius))
        {
            return 2;
        }
        else if ((transform.position.x >= CurseAreaPositionX - CurseRadius && transform.position.x <= CurseAreaPositionX) && (transform.position.z >= CurseAreaPositionZ && transform.position.z <= CurseAreaPositionZ + CurseRadius))
        {
            return 3;
        }
        else if ((transform.position.x >= CurseAreaPositionX - CurseRadius && transform.position.x <= CurseAreaPositionX) && (transform.position.z >= CurseAreaPositionZ - CurseRadius && transform.position.z <= CurseAreaPositionZ))
        {
            return 4;
        }
        return rnd.Next(1, 4);
    }

    private void TeleportCurse(CurseObject CurseObject)
    /// Teleport CurseObject
    {
        (int x, int z) = RandomDotInCircle(Math.PI / 2, CurseRadius);

        Destroy(CurseObject.AntidoteObject);

        int QuarterOfCircle = WherePlayerStay(CurseObject.CurseArea.transform.position.x, CurseObject.CurseArea.transform.position.z);

        if (QuarterOfCircle == 1)
        {
            CurseObject.gameObject.transform.Translate(-x, 0, z);
        }
        else if (QuarterOfCircle == 2)
        {
            CurseObject.gameObject.transform.Translate(-x, 0, -z);
        }
        else if (QuarterOfCircle == 3)
        {
            CurseObject.gameObject.transform.Translate(x, 0, -z);
        }
        if (QuarterOfCircle == 4)
        {
            CurseObject.gameObject.transform.Translate(x, 0, z);
        }

        HealthBoxPosition(AntidotePrefab, CurseObject);
    }

    private CurseObject FindCurseObject(GameObject obj)
    /// Returns CurseObject by its GameObject
    {
        for (int i = 0; i < CurseObjects.Count; i++)
        {
            if (CurseObjects[i].gameObject == obj)
            {
                return CurseObjects[i];
            }
        }

        return null;
    }

    private void ApplyCurse(CurseObject CurseObject)
    /// Applies the effect of the curse
    {
        if (CurseObject.TypeOfCurse == 1)
        {
            HealthCurse(CurseObject);
        }
        //if (CurseObject.TypeOfCurse == 2)
        //{
        //    SpeedCurse(CurseObject);
        //}
    }

    public void HealthCurse(CurseObject CurseObject)
    {
        if (CurseObject.TypeOfCurse == 1)
        {
            if (MathF.Round(CurseObject.TimeAfterDamage, 2) == TimeBetweenDamage)
            {
                CurseObject.TimeAfterDamage = 0f;
                Player.Health -= BaseHealthDamage + HealthStackDamage * CurseObject.StacksNum;
            }
        }
    }

    //public void SpeedCurse(CurseObject CurseObject)
    //{
    //    if (CurseObject.TypeOfCurse == 2)
    //    {
    //        FirstPersonController.SpeedRatio = SpeedEffectRatio - SpeedStackRatio * CurseObject.StacksNum;
    //    }
    //}

    public string GenerateText(CurseObject CurseObject, string name, int stacks)
    {
        if (CurseObject.TypeOfCurse == 1)
        {
            if (CurseObject.Works)
            {
                if (!CurseObject.SeeNegativeEffect)
                {
                    return ("Объект: " + name + " || Скрыто");
                }

                var damage = BaseHealthDamage + HealthStackDamage * stacks;
                return ("Объект: " + name + " || Тип проклятия: Здоровье || Состояние: Активно ||" + " Кол-во стаков: " + stacks + " || Урон: " + damage);
            }
            else
            {
                return ("Объект: " + name + " || Тип проклятия: Здоровье || Состояние: Спит");
            }
        }
        else if (CurseObject.TypeOfCurse == 2)
        {
            if (CurseObject.Works)
            {
                if (!CurseObject.SeeNegativeEffect)
                {
                    return ("Объект: " + name + " || Скрыто");
                }

                var speedRatio = SpeedEffectRatio - SpeedStackRatio * stacks;
                return ("Объект: " + name + " || Тип проклятия: Скорость || Состояние: Активно || Кол-во стаков: " + stacks + " || Коэффицент скорости игрока: " + speedRatio);
            }
            else
            {
                return ("Объект: " + name + " || Тип проклятия: Скорость || Состояние: Спит || Коэффицент скорости игрока: 1");
            }
        }
        return " ";
    }

    private void CheckStacks(CurseObject CurseObject)
    /// Checking count of stacks for invisible and opportunity to see the negative effect
    {
        if (!CurseObject.Invisible && CurseObject.StacksNum == CountStacksForInvisible)
        {
            CurseObject.Invisible = true;


            Destroy(CurseObject.gameObject.GetComponent<MeshFilter>());
            Destroy(CurseObject.AntidoteObject.GetComponent<MeshFilter>());
        }
        else if (CurseObject.SeeNegativeEffect && CurseObject.StacksNum == CountStacksForInvisible * 2)
        {
            CurseObject.SeeNegativeEffect = false;
        }
    }
}

