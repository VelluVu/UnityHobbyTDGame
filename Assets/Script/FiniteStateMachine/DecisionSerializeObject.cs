namespace TheTD.ScriptableFiniteStateMachine
{
    [System.Serializable]
    public class DecisionSeriliazeObject
    {
        public DecisionScriptableObject condition;
        public string Name => condition.name;

    }
}