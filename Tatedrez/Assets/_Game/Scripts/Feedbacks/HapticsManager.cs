using RaphaelHerve.Tatedrez.Enums;
using RaphaelHerve.Tatedrez.Game;
using System.Collections.Generic;
using UnityEngine;
using static Lofelt.NiceVibrations.HapticPatterns;

namespace RaphaelHerve.Tatedrez.Feedbacks
{
    public class HapticsManager : MonoBehaviour
    {
        private List<PresetType> _concurrentHaptics = new();

        private void Start()
        {
            GameManager.OnGameStateChanged += GameStateChanged;
            GameManager.Board.OnPiecePlacedOnTile += PiecePlacedOnTile;
            InputController.OnStartMovingPiece += StartMovingPiece;
            InputController.OnCancelledMove += CancelledMove;
            InputController.OnInvalidMove += InvalidMove;
        }

        private void LateUpdate()
        {
            if (_concurrentHaptics.Count == 0)
            {
                return;
            }

            if (_concurrentHaptics.Count == 1)
            {
                PlayPreset(_concurrentHaptics[0]);
                _concurrentHaptics.Clear();
                return;
            }

            PresetType strongestPresetType = PresetType.None;

            foreach (PresetType presetType in _concurrentHaptics)
            {
                if (presetType.Strength() < strongestPresetType.Strength())
                {
                    continue;
                }

                strongestPresetType = presetType;
            }

            PlayPreset(strongestPresetType);
            _concurrentHaptics.Clear();
        }

        private void GameStateChanged(GameState from, GameState to)
        {
            PresetType presetType = to switch
            {
                GameState.None => PresetType.Selection,
                GameState.PiecePlacement => PresetType.Selection,
                GameState.GameOver => PresetType.Success,
                _ => PresetType.None
            };

            _concurrentHaptics.Add(presetType);
        }

        private void PiecePlacedOnTile(Piece piece)
            => _concurrentHaptics.Add(PresetType.LightImpact);

        private void StartMovingPiece(Piece piece)
            => _concurrentHaptics.Add(PresetType.Selection);

        private void CancelledMove(Piece obj)
            => _concurrentHaptics.Add(PresetType.Selection);

        private void InvalidMove(Piece obj)
            => _concurrentHaptics.Add(PresetType.Warning);
    }
}