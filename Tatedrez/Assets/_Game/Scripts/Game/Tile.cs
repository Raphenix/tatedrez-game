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
        private Pawn _pawn;

        public Vector2Int Coordinates => _coordinates;
        public bool HasPawnOn => _pawn != null;

        public void Init(int x, int y)
        {
            _coordinates = new Vector2Int(x, y);
            HideHighlight();
        }

        public void PlacePawnOn(Pawn pawn)
        {
            if (_pawn != null)
            {
                Debug.LogError($"Tile already has {_pawn} on", _pawn);
                return;
            }

            _pawn = pawn;
        }

        public void RemovePawn(Pawn pawn)
        {
            if (_pawn == null)
            {
                Debug.LogError("Tile doesn't contains any pawn", this);
                return;
            }

            if (pawn != _pawn)
            {
                Debug.LogError($"Tile doesn't contain {pawn}", this);
                return;
            }

            _pawn = null;
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