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

## The Beastmaster's war dog (the player's side)

The beast stands beside the player, so it faces RIGHT: change "facing LEFT" in the style block to "facing RIGHT". Save it as `WarDog.png`.

`Subject: a Roman war dog (molossus), smaller than a human, muscular and broad-chested, short brown and tan coat, a spiked bronze collar and a leather harness, standing alert and ready to lunge, facing right.`

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

# Icons

Two sheets of 12 icons, each drawn as ONE image, so every icon shares the style and the size. The cleanup script cuts the grid into separate icons in the order below (left to right, top to bottom).

## How to use

1. A **new chat** for each sheet; no reference image (an attached character tends to get drawn into the cells).
2. Paste the **Icon style block**, then the sheet's list, in one message.
3. If a cell has a label, a frame or two icons, ask in the same chat to fix that cell only.

## Icon style block

```
A pixel art icon sheet for a Roman gladiator game: exactly 12 icons in a grid of
4 columns and 3 rows, equal square cells, evenly spaced, every icon centered in
its cell and filling about 80% of it, all the same size and weight.
Strict pixel art like 32x32 game icons: hard pixel edges, no anti-aliasing, no
blur, no gradients, no glow, a 2-pixel dark brown outline around every icon,
bold readable silhouettes that work at small size, warm palette (bronze, bone
white, terracotta, deep red) plus the colors named below.
Plain solid magenta background (#FF00FF) in every cell, no grid lines, no
frames, no text, no letters, no numbers, no labels.
Icons, in this order (left to right, top to bottom):
```

## Sheet 1: statuses

```
1. Block: a round bronze shield seen from the front
2. Fortify: a short grey stone wall with battlements
3. Bleed: a single deep red blood drop
4. Burn: an orange and yellow flame
5. Poison: a bright green drop with a tiny skull inside
6. Rage: a clenched red fist
7. Ritual: a purple eye inside a small triangle
8. Vulnerable: a bronze shield with a big crack through it
9. Weak: a sword broken in two pieces
10. Entangled: a tangled brown rope net
11. Encourage: a green laurel wreath
12. Fire Shield: a bronze shield wrapped in orange flames
```

## Sheet 2: actions and screen

```
1. Damage: a short Roman sword (gladius) pointing up
2. Heal: a red heart with a small white cross
3. Magic: a rolled parchment scroll with a purple seal
4. Gold: a gold Roman coin with a laurel profile
5. Health: a plain red heart
6. Reroll: a white six-sided die with a curved arrow around it
7. Threat: a red exclamation mark on a bronze triangle
8. Charge: a war hammer with a small yellow lightning bolt
9. Petrified: a grey cracked stone block
10. Consumable: a cracked clay bottle
11. Change: two curved arrows chasing each other in a circle
12. Target: a gold crosshair ring with four short lines
```

## Sheet 3: Provocator statuses (3 icons)

For this sheet, change "exactly 12 icons in a grid of 4 columns and 3 rows" in the style block to "exactly 3 icons in a row". Until they exist, the status chips show the word.

```
1. Riposte: a sword pointing back toward the viewer with a curved return arrow
2. Vigil: a small lit oil lamp
3. Reversal: a round shield with a curved arrow bouncing off it
```

## Sheet 4: Hound (1 icon)

For this one, change "exactly 12 icons in a grid of 4 columns and 3 rows" in the style block to "exactly 1 icon".

```
1. Hound: a brown dog's paw print
```

# Face emblems

Every die face gets its own emblem, so a player who knows the game recognizes a face before reading it (GDD "Visual Style", "Face tiles"). Gemini draws only the object; the tile's ground color (the family), its rim (the rarity) and the value are drawn by the game, so the color system always stays consistent.

## Rules for every emblem

- **One object, named by the face:** the picture reminds you of the name (Anvil is an anvil, Exsanguinate a chalice of blood). Names and pictures help each other to be remembered.
- **A bold silhouette, no fine detail:** it must still read on a 46-pixel die net tile.
- **Never a status icon:** a face is what it *is*, a status is what *happens*. Serrated Blade is a saw-toothed blade, not a blood drop.
- **Distinct within its family:** faces of one family share a ground color, so their silhouettes must differ most.

## Families (ground color of the tile, drawn by the game)

The family is the face's first tag other than Physical; Lifesteal counts as Bleed. The starting faces (Sword, Shield, Health Potion) stay plain ivory.

| Family | Ground | Faces |
|---|---|---|
| Bleed | dark red | Serrated Blade, Lacerate, Open Wound, Exsanguinate, Bloodscent, Bite |
| Burn | orange | Ember, Fuel, Wildfire, Detonate, Cauterize, Flame Ward |
| Poison | poison green | Venom, Toxic Blade, Miasma, Contagion, Plague |
| Ritual | purple | Incantation, Dark Communion, Blood Rite, Witch's Curse |
| Rage | crimson brown | Fury, Bloodrage, Unleash, Frenzy, Twin Axes, Cleave, Rally, Battle Hymn, Iron Rage, Crowd's Favor |
| Block | steel blue | Shield Burst, Spiked Shield, Shield Bash |
| Hammer | iron grey | War Hammer, Maul, Anvil, Earthshaker, Follow Through, Steady Hand |
| Fortune | gold | Encore, Fate's Hand, Gambit, Hedge, Mirror Blade, Opening Gambit, Ricochet, Sidestep, Turnabout, Lucky Buckler, Fate's Edge, Gambler's Strike, Patient Blade |
| General | plain bone | Catalyst, Drill Sergeant, Exploit, Feint, Last Stand, Mimic, Sharpshooter, Spare Blade, Throwing Knife, Veteran |
| Net (Retiarius) | sea teal | Net, Trident, Harpoon, Tangle, Snare, Haul In |
| Riposte (Provocator) | wine | Parry, Bait, Grudge, Vigil, Wrath, Perfect Counter, Provoke |
| Hound (Beastmaster) | leather brown | Whistle, Command, Guard Dog, Feed, Bloodhound, Sic 'Em |

Rarity is the rim: Common bronze, Rare silver, Epic gold. An enchanted face gets a purple seal.

## How to use

Same as the icon sheets: a new chat per sheet, no reference image, the **Emblem style block** and the sheet's list in one message. If a cell comes out with a frame, a background shape or a label, ask to fix that cell only.

## Emblem style block

```
A pixel art sheet of item emblems for a Roman gladiator dice game: exactly 12
emblems in a grid of 4 columns and 3 rows, equal square cells, evenly spaced,
every emblem centered in its cell and filling about 75% of it, all the same
size and weight.
Strict pixel art like 48x48 game sprites: hard pixel edges, no anti-aliasing,
no blur, no gradients, no glow, a 2-pixel dark brown outline around every
emblem, one bold object per cell with a clear silhouette that reads at small
size, ancient Roman look, natural colors for each object.
Plain solid magenta background (#FF00FF) in every cell, no circles or tiles
behind the objects, no grid lines, no frames, no text, no letters, no numbers.
Emblems, in this order (left to right, top to bottom):
```

## Sheet 1: Bleed and Burn

```
1. Serrated Blade: a short blade with big saw teeth along its edge
2. Lacerate: three parallel claw slashes torn through a strip of leather
3. Open Wound: a torn, unrolling red cloth bandage
4. Exsanguinate: a golden chalice overflowing with dark red blood
5. Bloodscent: a snarling hound's head with red scent wisps at its nose
6. Bite: wide open jaws with sharp fangs
7. Ember: a single glowing orange coal held in iron tongs
8. Fuel: a clay oil amphora with oil dripping from its spout
9. Wildfire: a burning torch whose flame splits into three tongues
10. Detonate: a clay pot bursting apart with fire and shards
11. Cauterize: a red-hot iron branding rod
12. Flame Ward: a bronze brazier on a tripod with a low fire
```

## Sheet 2: Poison, Ritual and the starting faces

```
1. Venom: a coiled cobra with dripping fangs
2. Toxic Blade: a dagger with green liquid running down its blade
3. Miasma: a cracked clay jar leaking thick green fog
4. Contagion: a grey rat with glowing green eyes
5. Plague: a hooded skull breathing green mist
6. Incantation: a lit candle with a purple flame
7. Dark Communion: an empty dark hood with two glowing purple eyes
8. Blood Rite: a curved ritual knife over a small bowl of blood
9. Witch's Curse: a straw doll pierced by a long needle
10. Sword: a plain Roman gladius lying horizontally, hilt on the left
11. Shield: a tall rectangular red Roman scutum with a bronze boss
12. Health Potion: a round glass flask of red liquid with a cork
```

## Sheet 3: Rage and Block

```
1. Fury: a snorting red bull's head
2. Bloodrage: a red heart pierced by a dagger
3. Unleash: a heavy iron chain snapping in the middle
4. Frenzy: a small whirlwind with a blade spinning inside
5. Twin Axes: two crossed hand axes
6. Cleave: a broad cleaver axe with a wide blade
7. Rally: a curved bronze Roman war horn
8. Battle Hymn: a wooden lyre with strings
9. Iron Rage: a horned iron helmet with glowing red eye slits
10. Crowd's Favor: a raised hand giving a thumbs up
11. Shield Burst: a round shield shattering outward into shards
12. Spiked Shield: a round shield covered in big iron spikes
```

## Sheet 4: Block, Hammer and General

```
1. Shield Bash: a shield striking forward with impact lines
2. War Hammer: a long war hammer with a spike on the back of its head
3. Maul: a huge maul with a big square stone head
4. Anvil: a black iron anvil
5. Earthshaker: a fist punching the ground, the ground cracking around it
6. Follow Through: a hammer mid-swing with a curved motion arc
7. Steady Hand: an open palm held flat with a plumb line hanging from it
8. Catalyst: a glass alembic bubbling with a yellow liquid
9. Drill Sergeant: a centurion's helmet with a sideways red crest
10. Exploit: a dagger pointing into a crack in a bronze breastplate
11. Feint: a smiling theater mask
12. Last Stand: a torn war banner still standing planted in the sand
```

## Sheet 5: General and Fortune

```
1. Mimic: a green chameleon
2. Sharpshooter: a short bow with an arrow nocked
3. Spare Blade: a dagger in a belt sheath
4. Throwing Knife: three throwing knives fanned out
5. Veteran: a dented, scarred old gladiator helmet
6. Encore: a single red rose thrown on the sand
7. Fate's Hand: a hand holding a single die
8. Gambit: two dice tumbling
9. Hedge: a short wooden palisade fence
10. Mirror Blade: a sword reflected in a round polished bronze mirror
11. Opening Gambit: a wooden gate swinging open with light behind it
12. Ricochet: a sling stone bouncing off a wall with an arc line
```

## Sheet 6: Fortune (6 emblems)

For this sheet, change "exactly 12 emblems in a grid of 4 columns and 3 rows" in the style block to "exactly 6 emblems in a grid of 3 columns and 2 rows".

```
1. Sidestep: a sandal footprint with motion lines
2. Turnabout: a silver coin flipping in the air
3. Lucky Buckler: a small round buckler with a four-leaf clover painted on it
4. Fate's Edge: a sword with dice pips running along its blade
5. Gambler's Strike: a pair of Roman knucklebones (astragali)
6. Patient Blade: a sheathed sword leaning on an hourglass
```

## Sheet 7: Retiarius (6 emblems)

For this sheet, change "exactly 12 emblems in a grid of 4 columns and 3 rows" in the style block to "exactly 6 emblems in a grid of 3 columns and 2 rows". Save them as `Assets/Art/Faces/Net.png`, `Trident.png`, `Harpoon.png`, `Tangle.png`, `Snare.png` and `HaulIn.png`.

```
1. Net: a round fishing net with lead weights along its edge, spread open
2. Trident: a three-pronged Roman fishing trident standing upright
3. Harpoon: a barbed harpoon head on a short shaft with a coiled rope
4. Tangle: a knotted mass of rope twisted into a tight tangle
5. Snare: a rope noose snare tied to a bent wooden stake
6. Haul In: two hands pulling a rope taut, a full net hanging from it
```

## Sheet 8: Provocator (7 emblems)

Gemini tends to fill a whole 4 x 3 sheet anyway; only the first seven cells are used. Save them as `Assets/Art/Faces/Parry.png`, `Bait.png`, `Grudge.png`, `Vigil.png`, `Wrath.png`, `PerfectCounter.png` and `Provoke.png`.

```
1. Parry: a short sword held crosswise, deflecting a spear tip
2. Bait: a raw piece of meat hanging from a hook on a chain
3. Grudge: a clenched fist wrapped in a bloodied cloth bandage
4. Vigil: a bronze helmet with a single lit oil lamp beside it
5. Wrath: a bull's horn helmet crest with red steam rising
6. Perfect Counter: two crossed swords with a bright spark where they meet
7. Provoke: a gladiator's open hand beckoning, palm up, fingers curling
```

## Sheet 9: Beastmaster (6 emblems)

Only the first six cells are used. Save them as `Assets/Art/Faces/Whistle.png`, `Command.png`, `GuardDog.png`, `Feed.png`, `Bloodhound.png` and `SicEm.png`.

```
1. Whistle: a small bronze whistle on a leather cord
2. Command: a pointing hand with a leather wrist strap
3. Guard Dog: a spiked bronze dog collar lying in a circle
4. Feed: a big meat bone with a bite taken out of it
5. Bloodhound: a dog's nose sniffing a single red drop
6. Sic 'Em: a snarling dog's head with bared fangs, side view
```

# Items, die enchantments and face enchantments

Same rules as the face emblems (one object named by the thing, a bold silhouette, never a status icon), with one look per kind so they never mix up:

- **Items:** the object alone, like the face emblems.
- **Die enchantments:** the motif in relief on a round bronze medallion (they are bound to the die's places, like a stamp on the die).
- **Face enchantments:** the motif pressed into a glowing purple wax seal (the purple seal an enchanted face carries).

A new chat per sheet, no reference image, the kind's style block and the sheet's list in one message.

## Item style block

```
A pixel art sheet of item emblems for a Roman gladiator dice game: exactly 12
emblems in a grid of 4 columns and 3 rows, equal square cells, evenly spaced,
every emblem centered in its cell and filling about 75% of it, all the same
size and weight.
Strict pixel art like 48x48 game sprites: hard pixel edges, no anti-aliasing,
no blur, no gradients, no glow, a 2-pixel dark brown outline around every
emblem, one bold object per cell with a clear silhouette that reads at small
size, ancient Roman look, natural colors for each object.
Plain solid magenta background (#FF00FF) in every cell, no circles or tiles
behind the objects, no grid lines, no frames, no text, no letters, no numbers.
Emblems, in this order (left to right, top to bottom):
```

### Items 1

```
1. Alchemist's Belt: a leather belt with three small glass vials tucked in it
2. Ambusher's Knife: a short curved dagger half hidden in a dark cloth
3. Balance Scales: a large standing bronze balance with two pans
4. Barbed Hook: an iron hook with backward barbs on a short chain
5. Berserker Torc: a thick twisted bronze neck ring with red gem ends
6. Blood Pact: a rolled contract sealed with a red blood drop of wax
7. Bone Fetish: a small charm of tied bones and feathers
8. Censer: a hanging bronze incense burner with green smoke
9. Challenger's Mark: a red cloth glove thrown down
10. Compass: a bronze compass rose with a needle
11. Drill Banner: a small square Roman legion standard with an eagle on top
12. Haggler's Scale: a merchant's hand scale with gold coins on one pan
```

### Items 2

```
1. Herald's Horn: a long straight bronze trumpet with a red banner
2. Hourglass: a wooden hourglass with golden sand
3. Iron Bracers: a pair of riveted iron forearm guards
4. Laurel Wreath: a golden victor's crown of laurel leaves on a red cushion
5. Loaded Die: a single white die with a dark lead weight showing inside
6. Lucky Charm: an iron horseshoe tied with a red ribbon
7. Merchant's Purse: a fat leather coin purse with a gold drawstring
8. Motley Cloak: a patchwork cloak of many colored squares
9. Plague Mask: a long-beaked leather mask with round glass eyes
10. Rabbit's Foot: a white rabbit's foot on a leather cord
11. Scribe's Quill: a feather quill beside a small ink pot
12. Second Blade: two short swords, one behind the other
```

### Items 3 (8 emblems)

In the style block, write "exactly 8 emblems in a grid of 4 columns and 2 rows" instead of 12 in 4 by 3.

```
1. Smelling Salts: a small corked bottle with white crystals
2. Smoldering Gauntlet: a leather glove with glowing embers on the knuckles
3. Spiked Greaves: iron shin guards covered in short spikes
4. Tinder Box: an open wooden box with flint, steel and a spark
5. Tower Helm: a tall closed bronze helmet like a little tower
6. Whetstone: a grey sharpening stone with a blade being honed on it
7. Witch's Knot: a tangled knot of black and green cords
8. (spare) a coiled leather whip
```

## Die enchantment style block

```
A pixel art sheet of medallion emblems for a Roman gladiator dice game: exactly
8 emblems in a grid of 4 columns and 2 rows, equal square cells, evenly spaced,
every emblem centered in its cell and filling about 80% of it, all the same
size.
Every emblem is a round bronze medallion with a thick rim, and the motif raised
on it in relief, in warm bronze and gold tones, so all of them clearly belong
together; the motif must differ strongly from medallion to medallion.
Strict pixel art like 48x48 game sprites: hard pixel edges, no anti-aliasing,
no blur, no gradients, no glow, a 2-pixel dark brown outline around every
medallion.
Plain solid magenta background (#FF00FF) in every cell, no grid lines, no
frames, no text, no letters, no numbers.
Medallions, in this order (left to right, top to bottom):
```

### Die enchantments 1 (8)

```
1. Balance: a balance beam tipped up on one side
2. Burnout: a candle burning down to a stub with a big flame
3. Center of Gravity: a heavy round weight hanging in the middle
4. Heavy Core: an iron ball inside a cracked shell
5. Honed Edge: a blade edge with a bright gleam
6. Keystone: the keystone at the top of a stone arch
7. Kindred: two hands clasped together
8. Low Guard: a shield held low with a downward arrow
```

### Die enchantments 2 (6)

In the style block, write "exactly 6 emblems in a grid of 3 columns and 2 rows".

```
1. Outer Ring: a ring of small dots around an empty center
2. Right Hand: an open right hand raised
3. Twin Fangs: two curved fangs side by side
4. Twin Soul: two faces back to back, one light and one dark
5. Twin Weights: two small weights hanging from one bar
6. Weighted Die: a die with a heavy weight tied to one side
```

## Face enchantment style block

```
A pixel art sheet of seal emblems for a Roman gladiator dice game: exactly 12
emblems in a grid of 4 columns and 3 rows, equal square cells, evenly spaced,
every emblem centered in its cell and filling about 80% of it, all the same
size.
Every emblem is a round purple wax seal with wavy dripping edges, glowing
softly, with the motif pressed into it in lighter violet and gold, so all of
them clearly belong together; the motif must differ strongly from seal to seal.
Strict pixel art like 48x48 game sprites: hard pixel edges, no anti-aliasing,
no blur, no gradients, a 2-pixel dark outline around every seal.
Plain solid magenta background (#FF00FF) in every cell, no grid lines, no
frames, no text, no letters, no numbers.
Seals, in this order (left to right, top to bottom):
```

### Face enchantments (12)

```
1. Dark Rite: a skull with a lit candle on top
2. Ignition: a single bright spark bursting
3. Inspire: a horn with sound waves
4. Kindling: a bundle of crossed sticks with a small flame
5. Lucky Charm: a four-pointed star
6. Rend: three claw marks
7. Resonance: three rings spreading out from a center
8. Sweep: a wide crescent slash
9. Venom Coating: a blade edge with a dripping drop
10. War Cry: an open shouting mouth
11. Ward: a round rune circle like a shield
12. Whirlwind: two blades spinning in a spiral
```
