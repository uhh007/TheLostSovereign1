using UnityEngine;

[CreateAssetMenu(fileName = "Characteristics", menuName = "Movement/MovementCharacteristics", order = 1)]
public class MovementCharacteristics : ScriptableObject
{
    [SerializeField] private bool _visibleCursor = false;
    [SerializeField] private float _movementSpeed = 1f;
    [SerializeField] private float _runSpeed = 2f;
    [SerializeField] private float _angularSpeed = 150f;
    [SerializeField] private float _gravity = 1.7f;
    [SerializeField] private float _jump = 5f;

    public bool VisibleCursor => _visibleCursor;

    public float MovementSpeed => _movementSpeed;

    public float RunSpeed => _runSpeed;

    public float AngularSpeed => _angularSpeed;

    public float Gravity => _gravity / 10f;

    public float Jump => _jump / 100f;
}