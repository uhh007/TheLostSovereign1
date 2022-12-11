using UnityEngine;

public class ImprovementMenuController : MonoBehaviour
{
    public KeyCode ImprovementKey;
    public Texture2D SkillTree;
    public Material TextColor;
    public int FontSize;
    public int ButtonSize;

    private GUIStyle textStyle;

    private bool Open = false;
    private int SkillPoints = Player.SkillPoints;

    private float TotalIncreasingHealth;
    private float TotalIncreasingStamina;
    private float TotalIncreasingDamage;
    private float TotalIncreasingCriticalChance;

    private void Start()
    {
        textStyle = new GUIStyle
        {
            font = Font.CreateDynamicFontFromOSFont("Roboto-Black", FontSize)
        };
        textStyle.normal.textColor = Color.white;
        ResetValues();
    }

    private void Update()
    {
        if (Input.GetKeyDown(ImprovementKey)) Open = !Open;
        if (Open) { Time.timeScale = 0;  }
        if (!Open) { Time.timeScale = 1; }
    }

    private void OnGUI()
    {
        if (Open)
        {
            int x = (Screen.width - SkillTree.width) / 2;
            int y = (Screen.height - SkillTree.height) / 2;
            GUI.DrawTexture(new Rect(x, y, SkillTree.width, SkillTree.height), SkillTree);
            GUI.Label(new Rect(x + 30, y + 30, 1000, FontSize), $"Очков: {SkillPoints}", textStyle);
            ControllButtons(x, y);
            DisplayOnScreen(x, y);
        }
    }

    private void DisplayOnScreen(int x, int y)
    {
        GUI.Label(new Rect(x + 105, y + 200, 1000, FontSize), $"{Player.FULL_HP + TotalIncreasingHealth}", textStyle);
        GUI.Label(new Rect(x + 250, y + 200, 1000, FontSize), $"{Player.FULL_STAMINA + TotalIncreasingStamina}", textStyle);
        GUI.Label(new Rect(x + 400, y + 200, 1000, FontSize), $"{Player.Damage + TotalIncreasingDamage}", textStyle);
        GUI.Label(new Rect(x + 550, y + 200, 1000, FontSize), $"{Player.CriticalDamageChance + TotalIncreasingCriticalChance}", textStyle);
    }

    private void ControllButtons(int x, int y)
    {
        CheckIncreaseButtons(x, y);
        CheckDecreaseButtons(x, y);
        if (GUI.Button(new Rect(x + 500, y + 30, 1000, FontSize), "Подтвердить", textStyle))
        {
            Player.FULL_HP += TotalIncreasingHealth;
            Player.FULL_STAMINA += TotalIncreasingStamina;
            Player.Damage += TotalIncreasingDamage;
            Player.CriticalDamageChance += TotalIncreasingCriticalChance;
            ResetValues();
            Open = false;
        }
    }

    private void CheckIncreaseButtons(int x, int y)
    {
        if (GUI.Button(new Rect(x + 150, y + 290, ButtonSize, FontSize), "+", textStyle) && CanImprove())
        {
            TotalIncreasingHealth += 5;
            SkillPoints -= 1;
        }
        if (GUI.Button(new Rect(x + 300, y + 290, ButtonSize, FontSize), "+", textStyle) && CanImprove())
        {
            TotalIncreasingStamina += 5;
            SkillPoints -= 1;
        }
        if (GUI.Button(new Rect(x + 450, y + 290, ButtonSize, FontSize), "+", textStyle) && CanImprove())
        {
            TotalIncreasingDamage += 5;
            SkillPoints -= 1;
        }
        if (GUI.Button(new Rect(x + 600, y + 290, ButtonSize, FontSize), "+", textStyle) && CanImprove())
        {
            TotalIncreasingCriticalChance += 0.1f;
            SkillPoints -= 1;
        }
    }

    private void CheckDecreaseButtons(int x, int y)
    {
        if (GUI.Button(new Rect(x + 100, y + 287, ButtonSize, FontSize), "-", textStyle) && (TotalIncreasingHealth > 0))
        {
            TotalIncreasingHealth -= 5;
            SkillPoints += 1;
        }
        if (GUI.Button(new Rect(x + 250, y + 287, ButtonSize, FontSize), "-", textStyle) && (TotalIncreasingStamina > 0))
        {
            TotalIncreasingStamina -= 5;
            SkillPoints += 1;
        }
        if (GUI.Button(new Rect(x + 400, y + 287, ButtonSize, FontSize), "-", textStyle) && (TotalIncreasingDamage > 0))
        {
            TotalIncreasingDamage -= 5;
            SkillPoints += 1;
        }
        if (GUI.Button(new Rect(x + 540, y + 287, ButtonSize, FontSize), "-", textStyle) && (TotalIncreasingCriticalChance > 0))
        {
            TotalIncreasingCriticalChance -= 0.1f;
            SkillPoints += 1;
        }
    }

    private bool CanImprove()
    {
        return SkillPoints > 0;
    }

    private void ResetValues()
    {
        TotalIncreasingHealth = 0;
        TotalIncreasingStamina = 0;
        TotalIncreasingDamage = 0;
        TotalIncreasingCriticalChance = 0;
    }
}