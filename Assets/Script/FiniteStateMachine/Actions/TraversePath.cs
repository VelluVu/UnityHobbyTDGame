using UnityEngine;

namespace TheTD.ScriptableFiniteStateMachine
{
    [CreateAssetMenu(fileName = "TraversePath", menuName = "ScriptableObjects/FiniteStateMachine/Actions/TraversePath")]
    public class TraversePath : ActionScriptableObject
    {
        const string NO_PATH = "{0} Has no path to move";
        const string MOVING = "Moving";
        [SerializeField] private float minDistanceToNode = 0.1f;
        [SerializeField, Range(-1, 1)] private float minValueForRightDirection = 0.95f;

        public override void Act(FiniteStateMachine fsm)
        {
            if (!fsm.PathControl.HasPath)
            {
                Debug.LogFormat(NO_PATH, fsm.gameObject.name);
                return;
            }

            Traverse(fsm);
        }

        public void Traverse(FiniteStateMachine fsm)
        {
            if (fsm.PathControl.nextNode == null)
            {
                fsm.PathControl.currentNodeIndex = 0;
                fsm.PathControl.currentNode = AstarPath.active.GetNearest(fsm.transform.position).node;
                fsm.PathControl.nextNode = fsm.PathControl.currentPath.path[0];
            }

            var distanceToNextWaypoint = Vector3.Distance(fsm.Rigidbody.position, fsm.PathControl.NextNodePosition);

            if (distanceToNextWaypoint < minDistanceToNode)
            {
                fsm.PathControl.currentNode = fsm.PathControl.nextNode;
                fsm.PathControl.currentNodeIndex = (fsm.PathControl.currentNodeIndex + 1) % fsm.PathControl.currentPath.vectorPath.Count;
                fsm.PathControl.nextNode = fsm.PathControl.currentPath.path[fsm.PathControl.currentNodeIndex];
            }

            RotateTowardsPoint(fsm.PathControl.NextNodePosition, fsm.Rigidbody, fsm.MovementStats.TurnSpeed.Value);
            MoveForward(fsm, fsm.Rigidbody);

        }

        public void MoveForward(FiniteStateMachine fsm, Rigidbody rigidBody)
        {
            rigidBody.MovePosition(rigidBody.position + rigidBody.transform.forward * fsm.MovementStats.MoveSpeed.Value * Time.deltaTime);
        }

        public void RotateTowardsPoint(Vector3 point, Rigidbody rigidbody, float turnSpeed)
        {
            var toPointDirection = (point - rigidbody.position).normalized;
            if(IsRightDirection(rigidbody.transform.forward, toPointDirection)) return;
            var angleDir = AngleDirection(rigidbody.transform.forward, toPointDirection, rigidbody.transform.up);
            var eulerangleVelocity = new Vector3(0f, turnSpeed * angleDir, 0f);
            var deltaRotation = Quaternion.Euler(eulerangleVelocity * Time.fixedDeltaTime);
            rigidbody.MoveRotation(rigidbody.rotation * deltaRotation);
            //rigidbody.transform.LookAt(point);
        }

        private bool IsRightDirection(Vector3 forward, Vector3 direction)
        {
            var dot = Vector3.Dot(forward, direction);
            return dot > minValueForRightDirection;
        }

        private float AngleDirection(Vector3 forwardDirection, Vector3 targetDirection, Vector3 up)
        {
            Vector3 perpendicular = Vector3.Cross(forwardDirection, targetDirection);
            float dot = Vector3.Dot(perpendicular, up);
            //Debug.Log(dot);
            if (dot > 0.0f)
            {
                return 1.0f;
            }
            else if (dot < 0.0f)
            {
                return -1.0f;
            }
            else
            {
                return 0.0f;
            }
        }
    }
}