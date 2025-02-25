using NaughtyAttributes;
using RaphaelHerve.Tatedrez.Enums;
using System;
using System.Collections.Generic;
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
        private LayerMask _pieceLayerMask;

        private UnityEngine.Camera _mainCamera;
        private Piece _selectedPiece;
        private Tile _selectedTile;
        private List<Tile> _possibleMoves = new();

        public static event Action<Piece> OnStartMovingPiece;
        public static event Action<Piece> OnCancelledMove;
        public static event Action<Piece> OnInvalidMove;

        public InputMode InputMode
        {
            get => _inputMode;
            set => _inputMode = value;
        }

        private void Awake()
        {
            _mainCamera = UnityEngine.Camera.main;

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
            // All inputs start with selecting a piece
            if (!TryGetPieceOnRaycast(out Piece piece))
            {
                return;
            }

            // Can't select other player's piece
            if (piece.Owner != GameManager.CurrentPlayer)
            {
                return;
            }

            // Piece is already placed on board
            if (InputMode == InputMode.PiecePlacement && piece.IsPlacedOnTile)
            {
                return;
            }

            _selectedPiece = piece;
            _selectedPiece.StartDragging();
            OnStartMovingPiece(piece);

            // When selecting a piece, highlight its possible moves
            if (InputMode == InputMode.Dynamic)
            {
                _possibleMoves = GameManager.Board.GetMoves(piece.PieceType, piece.Coordinates);
                GameManager.Board.ShowTilesHighlight(_possibleMoves);
            }
        }

        private void HandleInputDrag()
        {
            // No selected piece
            if (_selectedPiece == null)
            {
                return;
            }

            // Raycasting on a valid tile, snap to it
            if (TryGetTileOnRaycast(out Tile tile) && GameManager.Board.CanPlacePieceOnTile(_selectedPiece, tile))
            {
                _selectedTile = tile;
                _selectedPiece.MoveTo(tile.transform.position);
                return;
            }

            _selectedTile = null;

            // Raycast on default plane, simply move around
            if (RaycastDefaultPlane(out RaycastHit hitInfo))
            {
                _selectedPiece.MoveTo(hitInfo.point);
                return;
            }
        }

        private void HandleInputRelease()
        {
            // No selected piece
            if (_selectedPiece == null)
            {
                return;
            }

            // Invalid move, move piece back to its previous position
            if (_selectedTile == null || !GameManager.Board.TryPlacePieceOnTile(_selectedPiece, _selectedTile))
            {
                if (_selectedTile == null)
                {
                    OnCancelledMove?.Invoke(_selectedPiece);
                }
                else
                {
                    OnInvalidMove?.Invoke(_selectedPiece);
                }

                _selectedPiece.MoveTo(_selectedPiece.PreviousPosition);
            }

            _possibleMoves.Clear();
            GameManager.Board.HideTilesHighlights();

            _selectedPiece.StopDragging();
            _selectedPiece = null;
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

        private bool TryGetPieceOnRaycast(out Piece piece) => TryGetComponentOnRaycast(_pieceLayerMask, out piece);
    }
}