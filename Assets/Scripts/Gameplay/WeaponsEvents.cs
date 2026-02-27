using System;
using UnityEngine;

public class WeaponsEvents : MonoBehaviour
{
    public event Action onAttackFinished;
    public event Action onAnimationFinished;
    public event Action onAttackStarted;

    public void OnAttackFinished()
    {
        onAttackFinished?.Invoke();
    }

    public void OnAnimationFinished()
    {
        onAnimationFinished?.Invoke();
    }

    public void OnAttackStarted()
    {
        onAttackStarted?.Invoke();
    }
}
