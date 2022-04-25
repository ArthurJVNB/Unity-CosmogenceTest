using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _SL
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Dice : MonoBehaviour
    {
        [SerializeField]
        private Sprite[] faces;

        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public int RowDice()
        {
            int value = Random.Range(1, 7);
            spriteRenderer.sprite = faces[value - 1];
            return value;
        }
    }
}
