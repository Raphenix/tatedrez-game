using DG.Tweening;
using RaphaelHerve.Tatedrez.Enums;
using RaphaelHerve.Tatedrez.Game;
using UnityEngine;

namespace RaphaelHerve.Tatedrez.Camera
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField]
        private Transform _cameraTransform;

        [Header("Idle animation")]
        [SerializeField]
        private float _idleAngle = 50f;
        [SerializeField]
        private float _idleAnimationDuration = 20f;
        [SerializeField]
        private float _cameraTransformIdleDistance;

        [Header("Players angles")]
        [SerializeField]
        private float _defaultAngle = 90f;
        [SerializeField]
        private float _player1Angle = 75f;
        [SerializeField]
        private float _player2Angle = 105f;

        private float _cameraTransformGameDistance;
        private bool _performIdleAnimation;
        private Tween _rotationTween;
        private Tween _shakeTween;

        private void Awake()
        {
            _cameraTransformGameDistance = _cameraTransform.localPosition.z;
            _cameraTransform.localPosition = new Vector3(_cameraTransform.localPosition.x,
                                                         _cameraTransform.localPosition.y,
                                                         _cameraTransformIdleDistance);
        }

        private void Start()
        {
            GameManager.OnCurrentPlayerChanged += CurrentPlayerChanged;
            GameManager.OnGameStateChanged += GameStateChanged;
            GameManager.Board.OnPiecePlacedOnTile += OnPiecePlacedOnTile;

            transform.rotation = Quaternion.Euler(_idleAngle, 0f, 0f);

            GameStateChanged(GameState.None, GameState.None);
        }

        private void GameStateChanged(GameState from, GameState to)
        {
            _performIdleAnimation = to == GameState.None;
        }

        private void Update()
        {
            if (_performIdleAnimation)
            {
                transform.rotation = Quaternion.Euler(Mathf.Lerp(transform.rotation.eulerAngles.x, _idleAngle, Time.deltaTime),
                                                      transform.rotation.eulerAngles.y + 360f * Time.deltaTime / _idleAnimationDuration,
                                                      0f);
            }

            float cameraZ = Mathf.Lerp(_cameraTransform.localPosition.z,
                                       _performIdleAnimation ? _cameraTransformIdleDistance : _cameraTransformGameDistance,
                                       Time.deltaTime * 10f);

            _cameraTransform.localPosition = new Vector3(_cameraTransform.localPosition.x,
                                                         _cameraTransform.localPosition.y,
                                                         cameraZ);
        }

        private void OnDestroy()
        {
            GameManager.OnCurrentPlayerChanged -= CurrentPlayerChanged;

            if (GameManager.Board != null)
            {
                GameManager.Board.OnPiecePlacedOnTile += OnPiecePlacedOnTile;
            }
        }

        private void CurrentPlayerChanged(PlayerType from, PlayerType to)
        {
            if (to == PlayerType.None)
            {
                return;
            }

            float x = to switch
            {
                PlayerType.Player1 => _player1Angle,
                PlayerType.Player2 => _player2Angle,
                _ => _defaultAngle
            };

            Quaternion rotation = Quaternion.Euler(x, 0f, 0f);

            _rotationTween?.Kill();
            _rotationTween = transform.DOLocalRotateQuaternion(rotation, .25f);
        }

        private void OnPiecePlacedOnTile(Piece piece)
        {
            _shakeTween?.Kill();
            //_shakeTween = transform.DOJump(transform.position, .1f, 1, .1f);
            _shakeTween = transform.DOShakePosition(.1f, .1f, 100);
        }
    }
}