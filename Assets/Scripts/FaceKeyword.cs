// Rules on a whole die face, which affect every action on it (see GDD "Face Keywords").
// Assets store these as numbers: always add new keywords at the end.
public enum FaceKeyword
{
    // Usable once per battle, whichever action is chosen; afterwards every action on the face is Change.
    Consumable,
    // Every turn the face isn't the one you act with, it gains +1 value for the battle.
    Charge
}
