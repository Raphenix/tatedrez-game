using DG.Tweening;
using NaughtyAttributes;
using RaphaelHerve.Tatedrez.Enums;
using UnityEngine;

namespace RaphaelHerve.Tatedrez.Game
{
    public class Pawn : MonoBehaviour
    {
        [SerializeField]
        private PawnType _pawnType;
        [SerializeField]
        private Transform _visual;
        [SerializeField]
        private SpriteRenderer _spriteRenderer;
        [SerializeField]
        private Sprite _player1Sprite;
        [SerializeField]
        private Sprite _player2Sprite;
        [ShowNonSerializedField]
        private PlayerType _owner;
        [ShowNonSerializedField]
        private Tile _currentTile;

        // Movement
        private Vector3 _targetPosition;
        private Vector3 _currentMoveVelocity;
        private Vector3 _startPosition;

        private Tween _visualRotationTween;

        public PawnType PawnType => _pawnType;
        public PlayerType Owner => _owner;
        public Tile CurrentTile => _currentTile;
        public bool IsPlacedOnTile => _currentTile != null;
        public Vector2Int Coordinates => _currentTile != null ? _currentTile.Coordinates : default;
        public Vector3 PreviousPosition => _currentTile != null ? _currentTile.transform.position : _startPosition;

        private void Awake()
        {
            _startPosition = _targetPosition = transform.position;
            GameManager.OnCurrentPlayerChanged += GameManager_OnCurrentPlayerChanged;
        }

        private void GameManager_OnCurrentPlayerChanged(PlayerType from, PlayerType to)
        {
            if (IsPlacedOnTile)
            {
                RotateVisual(to.Rotation());
            }
        }

        public void Init(PlayerType owner)
        {
            if (owner == PlayerType.None)
            {
                Debug.LogError($"Can't set Pawn's Owner to {PlayerType.None}");
                return;
            }

            _owner = owner;

            _visual.rotation = _owner.Rotation();

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

        private void RotateVisual(Quaternion targetRotation)
        {
            _visualRotationTween?.Kill();
            _visualRotationTween = _visual.DORotateQuaternion(targetRotation, .5f);
        }
    }
}