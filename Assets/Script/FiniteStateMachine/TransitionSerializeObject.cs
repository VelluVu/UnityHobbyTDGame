namespace TheTD.ScriptableFiniteStateMachine
{
    [System.Serializable]
    public class TransitionSeriliazeObject
    {
        public TransitionScriptableObject transition;
        public string Name => transition.name;
    }
}