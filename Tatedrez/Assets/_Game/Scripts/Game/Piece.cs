using DG.Tweening;
using NaughtyAttributes;
using RaphaelHerve.Tatedrez.Enums;
using UnityEngine;

namespace RaphaelHerve.Tatedrez.Game
{
    public class Piece : MonoBehaviour
    {
        [SerializeField]
        private PieceType _pieceType;
        [SerializeField]
        private Transform _visual;

        [Header("Base renderer based on player")]
        [SerializeField]
        private Renderer _base;
        [SerializeField]
        private Material _player1BaseMaterial;
        [SerializeField]
        private Material _player2BaseMaterial;

        [Header("Sprite based on piece type")]
        [SerializeField]
        private SpriteRenderer _spriteRenderer;
        [SerializeField]
        private Sprite _player1Sprite;
        [SerializeField]
        private Sprite _player2Sprite;

        [Header("Movement parameters")]
        [SerializeField]
        private float _visualBoardHeightOffset = -.1f;
        [SerializeField]
        private float _movingHeight = .1f;

        [ShowNonSerializedField]
        private PlayerType _owner;
        [ShowNonSerializedField]
        private Tile _currentTile;

        // Movement
        private Vector3 _targetPosition;
        private Vector3 _currentMoveVelocity;
        private Vector3 _startPosition;

        private bool _isBeingDragged;
        private Tween _visualRotationTween;

        private float TargetVisualHeight => _isBeingDragged ? _movingHeight : IsPlacedOnTile ? 0f : _visualBoardHeightOffset;

        public PieceType PieceType => _pieceType;
        public PlayerType Owner => _owner;
        public Tile CurrentTile => _currentTile;
        public bool IsPlacedOnTile => _currentTile != null;
        public Vector2Int Coordinates => _currentTile != null ? _currentTile.Coordinates : default;
        public Vector3 PreviousPosition => _currentTile != null ? _currentTile.transform.position : _startPosition;

        private void Awake()
        {
            _startPosition = _targetPosition = transform.position;

            _visual.localPosition = new Vector3(0f, _visualBoardHeightOffset, 0f);

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
                Debug.LogError($"Can't set Piece's Owner to {PlayerType.None}");
                return;
            }

            _owner = owner;

            _visual.rotation = _owner.Rotation();

            _spriteRenderer.sprite = owner switch
            {
                PlayerType.Player1 => _player1Sprite,
                PlayerType.Player2 => _player2Sprite,
                _ => null
            };

            _base.sharedMaterial = owner switch
            {
                PlayerType.Player1 => _player1BaseMaterial,
                PlayerType.Player2 => _player2BaseMaterial,
                _ => null
            };
        }

        public void Reset()
        {
            RotateVisual(_owner.Rotation());
            _currentTile = null;
            MoveTo(PreviousPosition);
        }

        private void Update()
        {
            ProcessSmoothDamping();
            ProcessVisualHeight();
        }

        private void ProcessSmoothDamping()
        {
            float rotationY = _visual.localRotation.eulerAngles.y;
            _visual.up = _targetPosition - transform.position + Vector3.up * 2f;
            _visual.localRotation = Quaternion.Euler(_visual.localRotation.eulerAngles.x, rotationY, _visual.localRotation.eulerAngles.z);
            transform.position = Vector3.SmoothDamp(transform.position, _targetPosition, ref _currentMoveVelocity, .075f);
        }

        private void ProcessVisualHeight() => _visual.localPosition = new Vector3(0f, Mathf.Lerp(_visual.localPosition.y, TargetVisualHeight, Time.deltaTime * 10f), 0f);

        public void StartDragging() => _isBeingDragged = true;

        public void StopDragging() => _isBeingDragged = false;

        public void MoveTo(Vector3 position) => _targetPosition = position;

        // TODO add placement animation and feedbacks, maybe cancel out smooth damping too
        public void PlaceOnTile(Tile tile)
        {
            _currentTile = tile;
        }

        private void RotateVisual(Quaternion targetRotation)
        {
            _visualRotationTween?.Kill();
            _visualRotationTween = _visual.DORotateQuaternion(targetRotation, .25f);
        }
    }
}