using UnityEngine;

namespace RaphaelHerve.Tatedrez.Enums
{
    public enum PlayerType
    {
        None = 0,

        Player1 = 1,
        Player2 = 2,
    }

    public static class PlayerTypeExtensions
    {
        public static string Name(this PlayerType playerType)
            => playerType switch
            {
                PlayerType.Player1 => "Player 1",
                PlayerType.Player2 => "Player 2",
                _ => ""
            };

        public static PlayerType OtherPlayer(this PlayerType playerType)
            => playerType switch
            {
                PlayerType.Player1 => PlayerType.Player2,
                PlayerType.Player2 => PlayerType.Player1,
                _ => PlayerType.None
            };

        public static Quaternion Rotation(this PlayerType playerType)
            => playerType switch
            {
                PlayerType.Player1 => Quaternion.LookRotation(Vector3.forward),
                PlayerType.Player2 => Quaternion.LookRotation(Vector3.back),
                _ => Quaternion.identity
            };
    }
}