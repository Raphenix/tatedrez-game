using NaughtyAttributes;
using RaphaelHerve.Tatedrez.Enums;
using UnityEngine;

namespace RaphaelHerve.Tatedrez.Game
{
    public class Pawn : MonoBehaviour
    {
        [SerializeField]
        private PawnType _pawnType = PawnType.None;
        [SerializeField]
        private SpriteRenderer _spriteRenderer;
        [SerializeField]
        private Sprite _playerASprite;
        [SerializeField]
        private Sprite _playerBSprite;
        [ShowNonSerializedField]
        private PlayerType _owner = PlayerType.None;
        [ShowNonSerializedField]
        private bool _isPlacedOnBoard = false;

        public PlayerType Owner => _owner;
        public bool IsPlacedOnBoard => _isPlacedOnBoard;

        public void Init(PlayerType owner)
        {
            if (owner == PlayerType.None)
            {
                Debug.LogError($"Can't set Pawn's Owner to {PlayerType.None}");
                return;
            }

            _owner = owner;

            _spriteRenderer.sprite = owner switch
            {
                PlayerType.PlayerA => _playerASprite,
                PlayerType.PlayerB => _playerBSprite,
                _ => null,
            };
        }
    }
}