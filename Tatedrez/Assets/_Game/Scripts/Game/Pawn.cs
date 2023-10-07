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
        private Sprite _player1Sprite;
        [SerializeField]
        private Sprite _player2Sprite;
        [ShowNonSerializedField]
        private PlayerType _owner = PlayerType.None;
        [ShowNonSerializedField]
        private Tile _currentTile = null;

        // Movement
        private Vector3 _targetPosition;
        private Vector3 _currentMoveVelocity;

        public PawnType PawnType => _pawnType;
        public PlayerType Owner => _owner;
        public Tile CurrentTile => _currentTile;
        public bool IsPlacedOnTile => _currentTile != null;
        public Vector2Int Coordinates => _currentTile != null ? _currentTile.Coordinates : default;

        private void Awake()
        {
            _targetPosition = transform.position;
        }

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
                PlayerType.Player1 => _player1Sprite,
                PlayerType.Player2 => _player2Sprite,
                _ => null,
            };
        }

        private void Update() => ProcessSmoothDamping();

        private void ProcessSmoothDamping() => transform.position = Vector3.SmoothDamp(transform.position, _targetPosition, ref _currentMoveVelocity, .05f/*, 10f, Time.deltaTime*/);

        public void MoveTo(Vector3 position) => _targetPosition = position;

        // TODO add placement animation and feedbacks, maybe cancel out smooth damping too
        public void PlaceOnTile(Tile tile)
        {
            _currentTile = tile;
        }
    }
}