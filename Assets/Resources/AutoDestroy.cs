using UnityEngine;

/// <summary>
/// For development, in future is will be pool
/// </summary>
public class AutoDestroy : MonoBehaviour
{
    public float delay = 2.5f;
    
    private void Awake()
    {
        Invoke(nameof(ForceDestroy), delay);
    }

    private void ForceDestroy()
    {
        Destroy(gameObject);
    }
}