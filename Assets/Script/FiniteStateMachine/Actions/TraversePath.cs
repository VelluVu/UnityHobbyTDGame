using UnityEngine;

namespace TheTD.ScriptableFiniteStateMachine
{
    [CreateAssetMenu(fileName = "TraversePath", menuName = "ScriptableObjects/FiniteStateMachine/Actions/TraversePath")]
    public class TraversePath : ActionScriptableObject
    {
        const string NO_PATH = "{0} Has no path to move";
        const string MOVING = "Moving";
        [SerializeField] private float minDistanceToNode = 0.1f;
        [SerializeField, Range(-1, 1)] private float minValueForCorrectDirection = 0.991f;

        public override void Act(FiniteStateMachine fsm)
        {
            if (!fsm.PathControl.HasPath)
            {
                return;
            }

            Traverse(fsm);
        }

        public void Traverse(FiniteStateMachine fsm)
        {
            if (fsm.PathControl.IsNewPath)
            {
                fsm.PathControl.currentNodeIndex = 0;
                fsm.PathControl.currentNode = (Vector3)AstarPath.active.GetNearest(fsm.transform.position).node.position;
                fsm.PathControl.nextNode = fsm.PathControl.currentPath[0];
                fsm.PathControl.IsNewPath = false;
            }

            var distanceToNextWaypoint = Vector3.Distance(fsm.Rigidbody.position, fsm.PathControl.nextNode);

            if (distanceToNextWaypoint < minDistanceToNode)
            {
                fsm.PathControl.currentNode = fsm.PathControl.nextNode;
                fsm.PathControl.currentNodeIndex = (fsm.PathControl.currentNodeIndex + 1) % fsm.PathControl.currentPath.Count;
                fsm.PathControl.nextNode = fsm.PathControl.currentPath[fsm.PathControl.currentNodeIndex];
            }
      
            fsm.IsTurning = RotateTowardsPoint(fsm.PathControl.nextNode, fsm.Rigidbody, fsm.MovementStats.TurnSpeed.Value);
            Move(fsm, fsm.Rigidbody);

        }

        public void Move(FiniteStateMachine fsm, Rigidbody rigidBody)
        {
            //if (fsm.IsTurning) return;
            if(!fsm.IsGrounded)
            {
                rigidBody.useGravity = true;
                return;
            }
            
            fsm.moveDirection = fsm.Rigidbody.transform.forward;
            rigidBody.useGravity = !fsm.IsOnSlope;
            Debug.DrawLine(fsm.transform.position, fsm.transform.position + fsm.moveDirection, Color.blue, 1f);
            float speed = fsm.MovementStats.MoveSpeed.Value;
            fsm.Velocity = fsm.moveDirection * speed;
            
            if(fsm.IsOnSlope)
            {
                fsm.Velocity = Vector3.ProjectOnPlane(fsm.Velocity, fsm.slopeHit.normal);
                //velocity += Vector3.down * 0.1f;
            }
            
            rigidBody.AddForce(fsm.Velocity, ForceMode.VelocityChange);
        }

       

        public bool RotateTowardsPoint(Vector3 point, Rigidbody rigidbody, float turnSpeed)
        {
            point = new Vector3(point.x, rigidbody.transform.position.y, point.z);
            var toPointDirection = (point - rigidbody.position).normalized;

            if (IsCorrectDirection(rigidbody.transform.forward, toPointDirection)) return false;

            var angleDir = AngleDirection(rigidbody.transform.forward, toPointDirection, rigidbody.transform.up);
            var eulerangleVelocity = new Vector3(0f, turnSpeed * angleDir, 0f);
            var deltaRotation = Quaternion.Euler(eulerangleVelocity * Time.deltaTime);
            rigidbody.MoveRotation(rigidbody.rotation * deltaRotation);
            return true;
        }

        private bool IsCorrectDirection(Vector3 forward, Vector3 direction)
        {
            var dot = Vector3.Dot(forward, direction);
            return dot > minValueForCorrectDirection;
        }

        private float AngleDirection(Vector3 forwardDirection, Vector3 targetDirection, Vector3 up)
        {
            Vector3 perpendicular = Vector3.Cross(forwardDirection, targetDirection);
            float dot = Vector3.Dot(perpendicular, up);

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