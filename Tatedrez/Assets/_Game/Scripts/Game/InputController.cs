using NaughtyAttributes;
using RaphaelHerve.Tatedrez.Enums;
using UnityEngine;

namespace RaphaelHerve.Tatedrez.Game
{
    public class InputController : MonoBehaviour
    {
        [ShowNonSerializedField]
        private InputMode _inputMode;
        [SerializeField]
        private Collider _defaultRaycastPlane;
        [SerializeField]
        private LayerMask _tileLayerMask;
        [SerializeField]
        private LayerMask _pawnLayerMask;

        private Camera _mainCamera;
        private Pawn _selectedPawn;
        private Tile _selectedTile;

        public InputMode InputMode
        {
            get => _inputMode;
            set => _inputMode = value;
        }

        private void Awake()
        {
            _mainCamera = Camera.main;

            GameManager.OnGameStateChanged += GameStateChanged;
        }

        private void OnDestroy()
        {
            GameManager.OnGameStateChanged -= GameStateChanged;
        }

        private void GameStateChanged(GameState from, GameState to)
        {
            InputMode = to switch
            {
                GameState.PiecePlacement => InputMode.PiecePlacement,
                GameState.Dynamic => InputMode.Dynamic,
                _ => InputMode.None,
            };
        }

        private void Update()
        {
            // No input mode
            if (InputMode == InputMode.None)
            {
                return;
            }

            // Press
            if (Input.GetMouseButtonDown(0))
            {
                HandleInputPress();
            }
            // Drag
            else if (Input.GetMouseButton(0))
            {
                HandleInputDrag();
            }
            // Release
            else if (Input.GetMouseButtonUp(0))
            {
                HandleInputRelease();
            }
        }

        private void HandleInputPress()
        {
            // All inputs start with selecting a pawn
            if (!TryGetPawnOnRaycast(out Pawn pawn))
            {
                Debug.Log("No pawn found");
                return;
            }

            // Can't select other player's pawn
            if (pawn.Owner != GameManager.CurrentPlayer)
            {
                return;
            }

            // Pawn is already placed on board
            if (InputMode == InputMode.PiecePlacement && pawn.IsPlacedOnTile)
            {
                return;
            }

            _selectedPawn = pawn;
        }

        private void HandleInputDrag()
        {
            // No selected pawn
            if (_selectedPawn == null)
            {
                return;
            }

            // Raycasting on a tile, snap to it
            // TODO check if movement possible
            if (TryGetTileOnRaycast(out Tile tile) && GameManager.Board.CanPlacePawnOnTile(_selectedPawn, tile))
            {
                _selectedTile = tile;
                _selectedPawn.MoveTo(tile.transform.position);
                return;
            }

            _selectedTile = null;

            // Raycast on default plane, simply move around
            if (RaycastDefaultPlane(out RaycastHit hitInfo))
            {
                _selectedPawn.MoveTo(hitInfo.point);
                return;
            }
        }

        private void HandleInputRelease()
        {
            // No selected pawn
            if (_selectedPawn == null)
            {
                return;
            }

            if (_selectedTile != null)
            {
                GameManager.Board.PlacePawnOnTile(_selectedPawn, _selectedTile);
            }
            else
            {
                _selectedPawn.MoveTo(_selectedPawn.CurrentTile.transform.position);
            }

            _selectedPawn = null;
        }

        private bool RaycastDefaultPlane(out RaycastHit hitInfo) => _defaultRaycastPlane.Raycast(_mainCamera.ScreenPointToRay(Input.mousePosition), out hitInfo, float.MaxValue);

        private bool TryGetComponentOnRaycast<T>(int layerMask, out T result) where T : Component
        {
            if (!Physics.Raycast(_mainCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo, float.MaxValue, layerMask))
            {
                result = default;
                return false;
            }

            return hitInfo.transform.TryGetComponent(out result);
        }

        private bool TryGetTileOnRaycast(out Tile tile) => TryGetComponentOnRaycast(_tileLayerMask, out tile);

        private bool TryGetPawnOnRaycast(out Pawn pawn) => TryGetComponentOnRaycast(_pawnLayerMask, out pawn);
    }
}