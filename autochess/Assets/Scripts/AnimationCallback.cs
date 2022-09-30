using UnityEngine;
using UnityEngine.Events;

public class AnimationCallback : MonoBehaviour
{
    public UnityEvent animationStartCallback = new UnityEvent();
    public UnityEvent animationImpactCallback = new UnityEvent();
    public UnityEvent animationEndCallback = new UnityEvent();
    public void AnimationStartCallback() {
        animationStartCallback.Invoke();
    }
    public void AnimationImpactCallback() {
        animationImpactCallback.Invoke();
    }
    public void AnimationEndCallback() {
        animationEndCallback.Invoke();
    }
}
