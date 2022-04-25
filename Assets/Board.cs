using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _SL
{
    public class Board : MonoBehaviour
    {
#if UNITY_EDITOR
        [Header("DEBUG")]
        [SerializeField]
        private bool showDebug = true;
        [Space]
#endif

        [Range(1, 50)]
        [SerializeField]
        private int collums = 10;
        [Range(1, 50)]
        [SerializeField]
        private int rows = 10;
        [SerializeField]
        private int cubeSize = 1;
        [SerializeField]
        private LayerMask moverLayerMask;

        private Vector2[] housePositions;

        public Vector2[] HousePositions => housePositions;

        private Vector2 DEBUG;

        private void Awake()
        {
            housePositions = GetGridHouses();
        }

        public int GetHouseNextToPosition(Vector2 position)
        {
            int result = 1;
            float lowestDistance = float.MaxValue;

            for (int currentHouse = 1; currentHouse < housePositions.Length + 1; currentHouse++)
            {
                float distance = Vector2.Distance(position, GetHousePosition(currentHouse));
                if (distance < lowestDistance)
                {
                    lowestDistance = distance;
                    result = currentHouse;
                }
            }

            return result;
        }

        public Vector2 GetHousePosition(int boardHouse)
        {
            return housePositions[boardHouse - 1];
        }

        public Vector2 GetHousePositionMovingBy(int currentBoardHouse, int boardHousesToMove)
        {
            return GetHousePosition(currentBoardHouse + boardHousesToMove);
        }

        public bool CheckHasMover(int boardHouseToCheck, out Vector2 moverEndPosition)
        {
            bool result = false;
            moverEndPosition = housePositions[boardHouseToCheck];

            Collider2D[] colliders = Physics2D.OverlapBoxAll(housePositions[boardHouseToCheck], Vector2.one * .5f, 0);
            if (colliders != null)
            {
                foreach (var collider in colliders)
                {
                    DEBUG = housePositions[boardHouseToCheck];

                    if (collider.TryGetComponent(out Mover mover))
                    {
                        result = true;
                        moverEndPosition = mover.EndPoint;

                        print("Mover: " + mover.name);
                        break;
                    }
                }
            }

            return result;
        }

        private Vector2[] GetGridHouses()
        {
            float cubeHalfSize = cubeSize * .5f;
            Vector2 offset = Vector2.one * cubeHalfSize + (Vector2)transform.position;
            List<Vector2> gridPositions = new List<Vector2>();

            for (int currentRow = 0; currentRow < rows; currentRow++)
            {
                if (currentRow % 2 == 0)
                {
                    for (int currentCollum = 0; currentCollum < collums; currentCollum++)
                    {
                        Vector2 position = new Vector2(currentCollum * cubeSize, currentRow * cubeSize) + offset;
                        gridPositions.Add(position);
                    }
                }
                else
                {
                    for (int currentCollum = collums - 1; currentCollum >= 0; currentCollum--)
                    {
                        Vector2 position = new Vector2(currentCollum * cubeSize, currentRow * cubeSize) + offset;
                        gridPositions.Add(position);
                    }
                }
            }

            return gridPositions.ToArray();
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawCube(DEBUG, Vector2.one * .5f);

            if (!showDebug) return;

            Color occupied = new Color(1, 0, 0, .35f);
            Color notOccupied = new Color(0, 1, 0, .35f);

            Gizmos.color = notOccupied;

            Vector2[] positions = GetGridHouses();
            foreach (var position in positions)
            {
                Gizmos.DrawCube(position, Vector2.one * cubeSize);
                Gizmos.DrawWireCube(position, Vector2.one * cubeSize);
            }
        }
#endif
    }
}
