using UnityEngine;

namespace TheTD.Building
{
    [System.Serializable]
    public class BuildSpot
    {
        public Vector3 CenterPositionLocal { get => new Vector3(BottomLeftLocalPosition.x + Size * 0.5f, BottomLeftLocalPosition.y, BottomLeftLocalPosition.z + Size * 0.5f); }
        public Vector3 CenterPositionInWorld { get => new Vector3(BottomLeftPositionInWorld.x + Size * 0.5f, BottomLeftPositionInWorld.y, BottomLeftPositionInWorld.z + Size * 0.5f); }
        public Vector3 BottomRightCornerPositionInWorld { get => new Vector3(BottomLeftPositionInWorld.x + Size, BottomLeftPositionInWorld.y, BottomLeftPositionInWorld.z); }
        public Vector3 TopRightCornerPositionInWorld { get => new Vector3(BottomLeftPositionInWorld.x + Size, BottomLeftPositionInWorld.y, BottomLeftPositionInWorld.z + Size); }
        public Vector3 TopLeftCornerPositionInWorld { get => new Vector3(BottomLeftPositionInWorld.x, BottomLeftPositionInWorld.y, BottomLeftPositionInWorld.z + Size); }
        public float Height { get => CenterPositionInWorld.y + Size; }
   
        private Construction building;
        public Construction Building { get => building; set => building = value; }

        [SerializeField] private bool isOccupied;
        public bool IsOccupied { get => isOccupied; set => isOccupied = value; }

        [SerializeField] private float size;
        public float Size { get => size; private set => size = value; }

        [SerializeField] private Vector3 bottomLeftLocalPosition;
        public Vector3 BottomLeftLocalPosition { get => bottomLeftLocalPosition; private set => bottomLeftLocalPosition = value; }

        [SerializeField] private Vector3 bottomLeftLocalPositionInWorld;
        public Vector3 BottomLeftPositionInWorld { get => bottomLeftLocalPositionInWorld; private set => bottomLeftLocalPositionInWorld = value; }

        public BuildSpot(Vector3 localPosition, Vector3 worldPosition, bool isOccupied, float size)
        {
            BottomLeftLocalPosition = localPosition;
            BottomLeftPositionInWorld = worldPosition;
            IsOccupied = isOccupied;
            Size = size;
        }

        public bool IsPositionInBounds(Vector3 position)
        {
            Vector3 startPosition = BottomLeftPositionInWorld;
            Vector3 endPosition = TopRightCornerPositionInWorld;
            var isBiggerThanStart = (position.x > startPosition.x) && (position.z > startPosition.z) && (position.y >= startPosition.y);
            var isSmallerThanEnd = (position.x < endPosition.x) && (position.z < endPosition.z) && (position.y < Height);
            var isInBounds = isBiggerThanStart && isSmallerThanEnd;
            return isInBounds;
        }
    }
}