using UnityEngine;

namespace ScriptableFiniteStateMachine
{
    [CreateAssetMenu(fileName = "Move", menuName = "ScriptableObjects/FiniteStateMachine/Actions/Move")]
    public class Move : ActionScriptableObject
    {
        const string NO_PATH = "{0} Has no path to move";
        const string MOVING = "Moving";
        [SerializeField] private float minDistanceToNode = 0.1f; 

        public override void Run(FiniteStateMachine fsm)
        {
            if (!fsm.PathControl.HasPath)
            {
                Debug.LogFormat(NO_PATH, fsm.gameObject.name);
                return;
            }

            TraversePath(fsm);
        }

        public void TraversePath(FiniteStateMachine fsm)
        {
            if(fsm.PathControl.nextNode == null)
            {
                fsm.PathControl.currentNodeIndex = 0;
                fsm.PathControl.currentNode = AstarPath.active.GetNearest(fsm.transform.position).node;
                fsm.PathControl.nextNode = fsm.PathControl.currentPath.path[0];
            }

            var distanceToNextWaypoint = Vector3.Distance(fsm.Rigidbody.position, fsm.PathControl.NextNodePosition);

            if(distanceToNextWaypoint < minDistanceToNode)
            {
                fsm.PathControl.currentNode = fsm.PathControl.nextNode;
                fsm.PathControl.currentNodeIndex = (fsm.PathControl.currentNodeIndex + 1) % fsm.PathControl.currentPath.vectorPath.Count;
                fsm.PathControl.nextNode = fsm.PathControl.currentPath.path[fsm.PathControl.currentNodeIndex];
            }

            RotateTowardsPoint(fsm.PathControl.NextNodePosition, fsm.Rigidbody);
            MoveForward(fsm, fsm.Rigidbody);   
            
        }

        public void MoveForward(FiniteStateMachine fsm, Rigidbody rigidBody)
        {
            rigidBody.MovePosition(rigidBody.position + rigidBody.transform.forward * fsm.MovementStats.MoveSpeed.Value * Time.deltaTime);
        }

        public void RotateTowardsPoint(Vector3 point, Rigidbody rigidbody)
        {
            rigidbody.transform.LookAt(point);
        }
    }
}