namespace ScriptableFiniteStateMachine
{
    [System.Serializable]
    public class StateSerializeObject
    {
        public StateScriptableObject state;
        public bool isDefaultState = false;
        public string Name => state.name;
    }
}