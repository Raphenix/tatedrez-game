using static Lofelt.NiceVibrations.HapticPatterns;

namespace RaphaelHerve.Tatedrez.Feedbacks
{
    public static class PresetTypeExtensions
    {
        public static int Strength(this PresetType presetType)
            => presetType switch
            {
                PresetType.None => 1,
                PresetType.Selection => 2,
                PresetType.LightImpact => 3,
                PresetType.SoftImpact => 4,
                PresetType.MediumImpact => 5,
                PresetType.RigidImpact => 6,
                PresetType.HeavyImpact => 7,
                PresetType.Warning => 8,
                PresetType.Failure => 9,
                PresetType.Success => 10,
                _ => 0
            };
    }
}