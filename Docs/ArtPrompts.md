# Art Prompts — Enemies and Bosses

Prompts for still enemy sprites made with Gemini, then cleaned by a script (true pixel grid, magenta removed, shared palette). Animations are made with PixelLab instead (see DevLog).

## How to use

1. Open a **new chat for every image**, so earlier images don't leak in (the first animation test drew the Minotaur instead of the gladiator).
2. Attach the player gladiator as the style reference (`east.png` from PixelLab).
3. Paste the **Style block** first, then the enemy's **Subject** line, both in the same message.
4. Save the result as `<EnemyAssetName>.png` (e.g. `ArenaWolf.png`), the same name as the asset in `Assets/Enemies/`.

If a result is off, ask in the same chat for one change at a time ("make the shield round", "face left"), not a new prompt.

## Style block (paste every time)

```
Pixel art sprite of an enemy for a 2D turn-based gladiator roguelite set in an
ancient Roman arena. Full body, side view, facing LEFT, standing in an idle
combat stance, feet at the bottom of the image.
Match the art style, outline thickness, shading and proportions of the attached
reference character exactly.
Strict pixel art: hard pixel edges, no anti-aliasing, no blur, no gradients,
no soft glow. Limited palette of about 16-20 colors: warm terracotta, bronze,
dark brown, bone white, olive, deep red, with dark brown outlines.
One single character only, centered, plain solid magenta background (#FF00FF)
filling the whole image, no ground, no shadow, no text, no frame, no border.
```

## Subjects

Size words keep the enemies in scale with each other: the gladiator is "human-sized".

### Weak (battles 1–3)

| Asset | Subject |
|---|---|
| Tiro | `Subject: a nervous young recruit gladiator, human-sized, thin, plain tunic, simple iron helmet without crest, short wooden practice sword and a small battered wooden shield.` |
| ArenaWolf | `Subject: a lean grey arena wolf, smaller than a human, snarling, bristling fur, a spiked bronze collar, crouched ready to bite.` |
| Murmillo | `Subject: a heavy murmillo gladiator, human-sized, broad-brimmed bronze helmet with a tall fish-shaped crest and grille visor, large rectangular red scutum shield held in front, short gladius, padded arm guard.` |
| Thraex | `Subject: a thraex gladiator, human-sized, bronze helmet with a griffin crest, small square shield, curved sica dagger dripping green poison, tall greaves on both legs.` |
| Lampadarius | `Subject: a torch-bearer gladiator, human-sized, light leather armor, open bronze helmet, a burning torch held forward with pixel-art orange flames, a short sword at the belt.` |

### Medium (battles 4–7)

| Asset | Subject |
|---|---|
| ArenaLion | `Subject: a big maned arena lion, about as tall as a human at the shoulder, scarred, roaring with open jaws, claws out, a broken chain hanging from its collar.` |
| Dimachaerus | `Subject: a dimachaerus gladiator, human-sized, two swords (one in each hand, one held high, one low in a parry), no shield, light scale armor on the shoulders, visored bronze helmet.` |
| Retiarius | `Subject: a retiarius gladiator, human-sized, no helmet, bare-chested, a tall bronze shoulder guard on one arm, a long trident in one hand and a weighted fishing net in the other.` |
| Secutor | `Subject: a secutor gladiator, human-sized, smooth egg-shaped bronze helmet with tiny eye holes, large curved rectangular shield, short gladius, stocky and grim.` |
| Venefica | `Subject: a hooded poisoner woman, human-sized, dark olive robes, a blowpipe in one hand and a small clay vial of green poison in the other, a thin wisp of green smoke.` |

### Strong (battles 9–10)

| Asset | Subject |
|---|---|
| Bestiarius | `Subject: a beast-master gladiator, human-sized, wearing a wolf pelt over his head and shoulders, a long leather whip in one hand, a short spear in the other, scars all over.` |
| Essedarius | `Subject: a chariot gladiator riding a small two-wheeled war chariot pulled by one horse, wheels with iron spikes, holding the reins and a whip, the whole group fits in the image.` |
| HoplomachusChampion | `Subject: a champion hoplomachus gladiator, human-sized but tall and proud, large round bronze shield with an eagle emblem, long spear, crested helmet with a red plume, gleaming armor.` |
| WarElephant | `Subject: a huge armored war elephant, three times taller than a human, bronze plates on its head and flanks, tusks with metal caps, a small empty tower on its back.` |

### Bosses

The Minotaur is done (see the art comparison page). Bosses are bigger than everyone else.

| Asset | Subject |
|---|---|
| Medusa | `Subject: Medusa, a boss: a woman's upper body on a long coiled green serpent tail instead of legs, hair made of living hissing snakes, glowing pale green eyes, skin turning to grey stone in patches, fragments of a stone statue at her tail. Twice the height of a human.` |
| Hydra | `Subject: the Hydra's body, a boss: a huge dark green scaled reptile body with a heavy tail and clawed legs, its many necks cut off short as stumps (the heads are drawn separately). Three times the size of a human.` |
| HydraHead | `Subject: one Hydra head on a long curved neck rising from below the image, dark green scales, open jaws with fangs and dripping acid, the neck cut off at the bottom edge of the image.` |
