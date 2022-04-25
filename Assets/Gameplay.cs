using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _SL
{
    public class Gameplay : MonoBehaviour
    {
        [SerializeField]
        private Board board;
        [SerializeField]
        private GameObject player;
        [SerializeField]
        private GameObject npc;
        [SerializeField]
        private Dice dice;
        [Range(0f, 2f)]
        [SerializeField]
        private float pawnMovementSpeed = 1f;

        public Board Board => board;

        private Coroutine movementRoutine;
        private bool CanMove => movementRoutine == null;

        private enum Pawn
        {
            Player,
            NPC
        }

        private int currentPlayerBoardHouse = 1;
        private int currentNpcBoardHouse = 1;

        private void Awake()
        {
            if (board == null)
                board = FindObjectOfType<Board>();
        }

        private void Update()
        {
            if (Input.GetButtonDown("Jump"))
            {
                if (CanMove)
                {
                    MovePawn(Pawn.Player);
                    //StartCoroutine(MoveThroughAllPositions());
                }
            }
        }

        private IEnumerator MoveThroughAllPositions()
        {
            Vector2[] positions = board.HousePositions;
            for (int i = 0; i < positions.Length; i++)
            {
                player.transform.position = positions[i];
                print("House: " + board.GetHouseNextToPosition(player.transform.position));
                yield return new WaitForSeconds(.1f);
            }
        }

        private void SetPawnHouse(Pawn pawn, int house)
        {
            switch (pawn)
            {
                case Pawn.Player:
                    currentPlayerBoardHouse = house;
                    break;
                case Pawn.NPC:
                    currentNpcBoardHouse = house;
                    break;
                default:
                    currentPlayerBoardHouse = house;
                    break;
            }
        }

        private void MovePawn(Pawn pawn)
        {
            if (movementRoutine != null) StopCoroutine(movementRoutine);
            movementRoutine = StartCoroutine(MovePawnRoutine(pawn));
        }

        private IEnumerator MovePawnRoutine(Pawn pawn)
        {
            int currentPawnHouse;
            GameObject currentGameObject;

            switch (pawn)
            {
                case Pawn.Player:
                    currentPawnHouse = currentPlayerBoardHouse;
                    currentGameObject = player;
                    break;
                case Pawn.NPC:
                    currentPawnHouse = currentNpcBoardHouse;
                    currentGameObject = npc;
                    break;
                default:
                    currentPawnHouse = currentPlayerBoardHouse;
                    currentGameObject = player;
                    break;
            }

            int value = dice.RowDice();
            //yield return new WaitForSeconds(1f);

            Vector2 positionToMove = board.GetHousePositionMovingBy(currentPawnHouse, value);
            currentPawnHouse += value;

            print("Current player = " + currentGameObject.name + " | Dice = " + value + " | Position to move = " + positionToMove);

            while (Vector2.Distance(currentGameObject.transform.position, positionToMove) > .1f)
            {
                currentGameObject.transform.position = Vector2.Lerp(currentGameObject.transform.position, positionToMove, pawnMovementSpeed * Time.deltaTime);
                yield return null;
            }

            print("REACHED");
            currentGameObject.transform.position = positionToMove;

            yield return new WaitForSeconds(.5f);

            if (board.CheckHasMover(currentPawnHouse, out Vector2 moverEndPosition))
            {
                int previousPawnHouse = currentPawnHouse;
                currentPawnHouse = board.GetHouseNextToPosition(moverEndPosition);

                print("SPECIAL HOUSE at " + previousPawnHouse + ". MOVING " + currentGameObject.name + " to HOUSE " + currentPawnHouse);

                while (Vector2.Distance(currentGameObject.transform.position, moverEndPosition) > .1f)
                {
                    currentGameObject.transform.position = Vector2.Lerp(currentGameObject.transform.position, moverEndPosition, pawnMovementSpeed * Time.deltaTime);
                    yield return null;
                }

                currentGameObject.transform.position = moverEndPosition;
            }

            SetPawnHouse(pawn, currentPawnHouse);
            movementRoutine = null;
        }
    }
}
