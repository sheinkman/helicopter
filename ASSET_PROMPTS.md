# Skyjoust — Asset Generation Prompts

One detailed prompt per asset, ready to paste into an image generator
(Gemini Pro / Imagen, etc.). Generate **every sprite in the same style** so the
game looks cohesive.

## Shared style (keep consistent across all sprites)

> Flat-shaded vector game art, clean bold outlines, subtle cel-shading,
> vibrant retro sci-fi arcade aesthetic, crisp edges, side-view orthographic
> perspective, no perspective distortion, no text or watermarks.

## Critical rules

- **Transparent background** on every sprite. If the tool cannot do alpha,
  request a *flat solid magenta (#FF00FF) background* and key it out afterward.
- **`drone.png` and `orb.png` must be pure white / light grey only** — the game
  tints them at runtime (red/orange/purple for drone tiers; gold/blue/green for
  orb kinds). Any baked-in color will corrupt the tint.
- The **aircraft and drone face RIGHT** — the engine mirrors them for left.
- One subject per image, centered, fully inside frame with a small margin.

---

## Sprites

### `aircraft.png` — player craft (~256×128, 2:1)

```
A sleek single-seat sci-fi attack aircraft shown in pure side profile, facing
right. Aerodynamic teardrop fuselage with a glowing cyan glass cockpit canopy
near the front, a swept-back stabilizer fin, two short downward-angled wings,
and a single rear thruster nozzle. Painted in vivid emerald green with white
trim accents and dark metallic panel lines. Flat-shaded vector game art, bold
clean outlines, subtle cel-shading, vibrant retro sci-fi arcade aesthetic.
Centered, full craft in frame, transparent background, no ground, no shadow,
no text.
```

### `drone.png` — enemy craft (~256×128, 2:1) — GREYSCALE ONLY

```
An aggressive angular enemy combat drone aircraft in pure side profile, facing
right. Sharp arrow-like hull with jagged forward-swept wings, a menacing single
sensor eye at the nose, exposed mechanical panels and a rear thruster. Rendered
ONLY in white, light grey and mid grey — strictly greyscale, absolutely no
color, so it can be tinted in-engine. Flat-shaded vector game art, bold clean
black outlines, subtle cel-shading, retro sci-fi arcade style. Centered, full
craft in frame, transparent background, no shadow, no text.
```

### `orb.png` — power orb (~128×128, 1:1) — WHITE ONLY

```
A glowing spherical energy orb, perfectly round, with a bright white luminous
core, a soft concentric inner glow and a faint outer halo. Smooth gradient from
solid white center to a translucent feathered edge. Rendered ONLY in white and
the palest grey — no color at all, so it can be tinted in-engine. Clean vector
art, soft radiance, subtle sci-fi energy look. Perfectly centered, transparent
background, no shadow, no text.
```

### `shield.png` — shield bubble (~256×256, 1:1)

```
A translucent spherical energy shield bubble, a thin glowing hexagonal-mesh
sphere that is mostly transparent in the middle with a brighter rim. Faint
pale-cyan / white tint, very low opacity, soft outer glow. Clean sci-fi vector
style, subtle hex pattern. Perfectly centered, transparent background, no
solid fill, no shadow, no text.
```

### `platform.png` — floating ledge (~512×64, 8:1, horizontally tileable)

```
A horizontal floating sci-fi platform ledge seen from a flat side view. Solid
metal slab with a glowing blue strip along the flat top edge, dark riveted
metal body, and a slightly darker underside. Designed to tile seamlessly left
to right — the left and right edges must match perfectly with no end caps.
Flat-shaded vector game art, bold outlines, retro sci-fi arcade style.
Transparent background above and below the slab, no shadow, no text.
```

### `ground.png` — floor strip (~1024×128, 8:1, horizontally tileable)

```
A wide horizontal sci-fi metal floor strip seen from a flat side view, forming
the bottom ground of an arcade level. Industrial riveted metal plating with
panel seams and a faint glowing accent line along the top surface. Must tile
seamlessly left to right with matching edges. Flat-shaded vector game art,
bold outlines, retro sci-fi arcade style. Transparent background above the
strip, no shadow, no text.
```

### `lava.png` — molten hazard (~1024×128, 8:1, horizontally tileable)

```
A horizontal river of glowing molten lava seen from a flat side view. Bright
yellow-white hot crest along the top with small rising bubbles, blending down
into deep orange and dark red toward the bottom. Intense inner glow, dangerous
and luminous. Must tile seamlessly left to right. Flat-shaded vector game art
with smooth gradients, retro sci-fi arcade style. Transparent background above
the lava surface, no text.
```

### `background.png` — backdrop (1920×1080, 16:9)

```
A static parallax game background of a deep-space night sky above a distant
alien horizon. Dark navy-to-purple gradient sky, scattered small stars, a faint
soft nebula glow, and a low silhouette of distant jagged mountains or a ruined
sci-fi cityscape along the bottom. Kept dark, low-contrast and uncluttered so
bright gameplay sprites stand out on top. Painterly flat vector style, no
foreground objects, no characters, no text. Full-bleed 16:9 composition.
```

### `spark.png` — particle texture (~64×64, 1:1)

```
A single soft round particle dot: a pure white luminous center fading smoothly
through a radial gradient to fully transparent at the edges. No hard edge, no
outline, no color. Perfectly centered, square image, transparent background.
```

---

## UI

### `logo.png` — title wordmark (~1024×512, 2:1)

```
A bold arcade game logo with the single word "SKYJOUST" in chunky futuristic
all-caps lettering. Metallic chrome letters with an emerald-green and gold gradient,
a thick dark outline, and a soft cyan outer glow. Slight upward dynamic tilt,
videogame title-screen energy. Transparent background, just the wordmark, no
extra graphics, spelled exactly S-K-Y-J-O-U-S-T.
```

### `button.png` — UI button (~256×96, 8:3, 9-slice friendly)

```
A clean sci-fi UI button shape: a horizontal rounded rectangle with a subtle
dark blue-grey metallic fill, a thin glowing cyan border and a soft top
highlight. No text on the button. Simple uniform borders and corners so it can
be 9-sliced and stretched. Flat vector UI style. Transparent background outside
the rounded rectangle, no shadow.
```

### `panel.png` — UI panel (~512×384, 4:3, 9-slice friendly)

```
A clean sci-fi UI panel: a large rounded rectangle with a dark semi-transparent
navy fill (about 80% opacity), a thin glowing cyan border and softly rounded
corners. No text, no icons, no content inside. Simple uniform border so it can
be 9-sliced. Flat vector UI style. Transparent background outside the rounded
rectangle.
```

---

## Audio prompts (for a text-to-audio tool — not an image generator)

- **thrust** — "Short punchy retro arcade jet-thruster whoosh, airy boost, ~0.4s."
- **joust_hit** — "Sharp metallic clang impact with a bright synth zap, arcade hit, ~0.5s."
- **orb_pickup** — "Bright cheerful ascending chime sparkle, collectible pickup, ~0.4s."
- **drone_explode** — "Crunchy retro 8-bit explosion with a low boom, ~0.7s."
- **player_death** — "Descending sad arcade power-down warble, ~1s."
- **wave_start** — "Short triumphant synth fanfare, three rising notes, ~1.2s."
- **music** — "Upbeat looping retro synthwave arcade track, driving bassline,
  energetic, seamless loop."
