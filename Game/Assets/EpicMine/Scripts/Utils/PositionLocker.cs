using UnityEngine;

public class PositionLocker : MonoBehaviour
{

    public Vector3 Position;

    public void Update()
    {
        transform.localPosition = Position;
    }
}
