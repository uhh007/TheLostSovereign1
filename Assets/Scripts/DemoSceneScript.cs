using UnityEngine;

public class DemoSceneScript : MonoBehaviour
{
    [SerializeField] private float PlayerMustKills;
    [SerializeField] private int FontSize;

    private GUIStyle textStyle;

    public static bool GameRun = true;
    public static bool Button = false;

    private void Start()
    {
        Player.gameObject = gameObject;
        textStyle = new GUIStyle
        {
            font = Font.CreateDynamicFontFromOSFont("Roboto-Black", FontSize)
        };
        textStyle.normal.textColor = Color.white;
        Checkpoint.CheckpointInitialize();
        Checkpoint.SaveGame();
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(0, Screen.height - 100, 1000, FontSize), $"�������� {Player.Health}", textStyle);
        GUI.Label(new Rect(0, Screen.height - 50, 1000, FontSize), $"������� {Player.Stamina}", textStyle);
        GUI.Label(new Rect(0, Screen.height - 150, 1000, FontSize), $"������ {Player.Money}", textStyle);
        GUI.Label(new Rect(0, 100, 1000, FontSize), $"�������: {Player.PlayerLevel}", textStyle);

        if (Player.isDied)
        {
            Time.timeScale = 0;
            GUI.Label(new Rect(Screen.width / 2, Screen.height / 2, 1000, FontSize), "�� ������", textStyle);
            if (GUI.Button(new Rect(Screen.width / 2, Screen.height / 2 + 100, 1000, FontSize), "��������� ��������", textStyle))
            {
                Player.Money -= 150f;
                Button = true;
                Time.timeScale = 1;
            }
        }

        if (Player.PlayerKills == PlayerMustKills)
        {
            GUI.Label(new Rect((Screen.width / 2) - 160, (Screen.height / 2) - 30, 1000, FontSize), "���������� ���������", textStyle);
        }
    }

    private void Update()
    {
        if (Button)
        {
            Checkpoint.LoadGame();
            Checkpoint.LoadPlayer();
            Player.isDied = false;
            Button = false;
        }
    }
}