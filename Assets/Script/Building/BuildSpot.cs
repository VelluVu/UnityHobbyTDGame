using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TheTD.Building
{
    [System.Serializable]
    public class BuildSpot
    {
        public bool HasOccupiedNeighbours { get => GetHasOccupiedNeighbours(); }
        public float Height { get => CenterPositionInWorld.y + Size; }
        public Vector3 CenterPositionLocal { get => new Vector3(BottomLeftLocalPosition.x + Size * 0.5f, BottomLeftLocalPosition.y, BottomLeftLocalPosition.z + Size * 0.5f); }
        public Vector3 CenterPositionInWorld { get => new Vector3(BottomLeftPositionInWorld.x + Size * 0.5f, BottomLeftPositionInWorld.y, BottomLeftPositionInWorld.z + Size * 0.5f); }
        public Vector3 BottomRightCornerPositionInWorld { get => new Vector3(BottomLeftPositionInWorld.x + Size, BottomLeftPositionInWorld.y, BottomLeftPositionInWorld.z); }
        public Vector3 TopRightCornerPositionInWorld { get => new Vector3(BottomLeftPositionInWorld.x + Size, BottomLeftPositionInWorld.y, BottomLeftPositionInWorld.z + Size); }
        public Vector3 TopLeftCornerPositionInWorld { get => new Vector3(BottomLeftPositionInWorld.x, BottomLeftPositionInWorld.y, BottomLeftPositionInWorld.z + Size); }

        [SerializeField]private Bounds _bounds;
        public Bounds Bounds { get => _bounds = (_bounds != null || _bounds.size != Vector3.zero) ? _bounds : new Bounds(CenterPositionInWorld + Vector3.up * 0.5f, new Vector3(Size, Size, Size)); }

        [SerializeField] private Vector2Int _gridPosition;
        public Vector2Int GridPosition { get => _gridPosition; private set => _gridPosition = value; }

        public Dictionary<Vector2Int, BuildSpot> NeighbourBuildSpots { get; private set; }

        private Construction _construction;
        public Construction Construction { get => _construction; set => _construction = value; }

        [SerializeField] private bool _isOccupied;
        public bool IsOccupied { get => _isOccupied; set => _isOccupied = value; }

        [SerializeField] private bool _isInvalidSpot;
        public bool IsInvalidSpot { get => _isInvalidSpot; set => _isInvalidSpot = value; }

        [SerializeField] private float size;
        public float Size { get => size; private set => size = value; }

        [SerializeField] private Vector3 bottomLeftLocalPosition;
        public Vector3 BottomLeftLocalPosition { get => bottomLeftLocalPosition; private set => bottomLeftLocalPosition = value; }

        [SerializeField] private Vector3 bottomLeftLocalPositionInWorld;
        public Vector3 BottomLeftPositionInWorld { get => bottomLeftLocalPositionInWorld; private set => bottomLeftLocalPositionInWorld = value; }

        public BuildSpot(Vector2Int gridPosition, Vector3 localPosition, Vector3 worldPosition, bool isOccupied, float size)
        {
            GridPosition = gridPosition;
            BottomLeftLocalPosition = localPosition;
            BottomLeftPositionInWorld = worldPosition;
            IsOccupied = isOccupied;
            Size = size;              
            _bounds = new Bounds(CenterPositionInWorld + Vector3.up * 0.5f, new Vector3(Size * 0.99f, Size, Size * 0.99f));
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

        public void SetNeighbourBuildSpots(Dictionary<Vector2Int, BuildSpot> neighbourSpots)
        {
            if (NeighbourBuildSpots != null) return;
            NeighbourBuildSpots = neighbourSpots;
        }

        public List<BuildSpot> GetNeighboursWithConstructions()
        {
            return NeighbourBuildSpots.Values.ToList().FindAll(neighbour => neighbour.IsOccupied);
        }

        public List<BuildSpot> GetNeighboursMarkedInvalidWithNoConstruction()
        {
            return NeighbourBuildSpots.Values.ToList().FindAll(neighbour => neighbour.IsInvalidSpot && !neighbour.IsOccupied);
        }

        private bool GetHasOccupiedNeighbours()
        {
            if (NeighbourBuildSpots.Count < 8)
            {
                return true;
            }

            foreach (var neighbour in NeighbourBuildSpots.Values)
            {
                if (neighbour.IsOccupied || neighbour.IsInvalidSpot) return true;
            }

            return false;
        }
    }
}