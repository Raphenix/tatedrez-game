using NaughtyAttributes;
using UnityEngine;

namespace RaphaelHerve.Tatedrez.Game
{
    public class Tile : MonoBehaviour
    {
        [SerializeField]
        private GameObject _highlight;
        [ShowNonSerializedField]
        private Vector2Int _coordinates;
        [ShowNonSerializedField]
        private Piece _piece;

        public Vector2Int Coordinates => _coordinates;
        public bool HasPieceOn => _piece != null;

        public void Init(int x, int y)
        {
            _coordinates = new Vector2Int(x, y);
            HideHighlight();
        }

        public void PlacePieceOn(Piece piece)
        {
            if (_piece != null)
            {
                Debug.LogError($"Tile already has {_piece} on", _piece);
                return;
            }

            _piece = piece;
        }

        public void RemovePiece(Piece piece)
        {
            if (_piece == null)
            {
                Debug.LogError("Tile doesn't contains any piece", this);
                return;
            }

            if (piece != _piece)
            {
                Debug.LogError($"Tile doesn't contain {piece}", this);
                return;
            }

            _piece = null;
        }

        public void ShowHighlight()
        {
            _highlight?.SetActive(true);
        }

        public void HideHighlight()
        {
            _highlight?.SetActive(false);
        }
    }
}