// Build archetypes a face belongs to. The Market weights its offers toward
// the tags already on the player's die. Assets store these as numbers: add new tags at the end.
public enum FaceTag
{
    Physical,
    Block,
    Lifesteal,
    Burn,
    Poison,
    Rage,
    Ritual,
    Bleed,
    Hammer,
    // Faces and items built around rerolls.
    Fortune,
    // Pragmatist faces: no archetype, they read states many builds produce (see GDD "Pragmatist Faces").
    General,
    // Retiarius: nets the enemy with Weak and Vulnerable (see GDD "Retiarius").
    Net,
    // Provocator: invites the enemy's attacks and punishes them (see GDD "Provocator").
    Riposte
}
