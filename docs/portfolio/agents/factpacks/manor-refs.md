> **Fact pack `manor-refs`**, Wave 0.5 (2026-10-02). It was researched by a Sonnet specialist and spot-checked by an independent skeptic,
> whose verdict was `fix`. **The corrections at the end override the text above them.** Claims are labelled
> measured, documented or estimate.

# Manor / Painting-World reference fact pack

Tags: [D] documented (URL), [M] measured by me from a public-domain image, [E] estimate or derived arithmetic. Pages used: about 17. No Hermitage or Wallace room dimensions were found (see Gaps).

## 1. Royal interiors

**Versailles, Hall of Mirrors [D]:** 73 m long, 10.50 m wide, 12.30 m high. 17 windows face 17 mirror arches (350+ mirror pieces); 9 large ceiling paintings; Rouge de Rance marble pilasters with gilded bronze capitals; green marble pier glasses; 3,000+ candles in its heyday. Source: en.wikipedia.org/wiki/Hall_of_Mirrors.
- Derived [E]: length:width:height = 7.0 : 1 : 1.17. Window pitch is about 73 / 17 = 4.3 m, so bays are about 0.41 of the room width. Height is only slightly more than width, so the room reads as a corridor, not a hall.

**Schönbrunn Great Gallery [D]:** 43 m long, almost 10 m wide (about 4.3 : 1). Two carved gilt chandeliers with 72 candles each, mirrors on the wall opposite the windows, white and gold stucco, three ceiling frescoes (Guglielmi, 1761), one marble statue on the centre line. Electrified in 1898 with 868 bulbs; the restoration used flickering candle-shaped LEDs. Source: schoenbrunn.at Great Gallery page.
- Derived [E]: two chandeliers in 43 m gives a spacing of about 14 m with about 7 m end margins (each at 1/3 and 2/3 of the length if placed symmetrically).

**Wallace Collection Great Gallery [D]:** walls in crimson silk with a cast gilt fillet border; conservation-controlled daylight through oculi. No dimensions found. Sources: Country Life and Museums Association search snippets.

**Wainscot [D]:** the lower wall treatment is "roughly a meter, 3 to 4 feet". Versailles-style boiserie is carved wood panelling. Source: en.wikipedia.org/wiki/Wainscoting. Door and carpet-runner widths are not documented in any page I could retrieve. Carpet width: **[E] 1.0 to 1.5 m** (a runner leaving 15 to 20 cm of floor visible on each side of a 1.2 m corridor is typical stately-home practice, not verified).

**Palette approximations [E]:**
- Gilt #C9A24B (highlight #E6C971)
- Ivory stucco #F2EEE4
- Crimson silk #8B1A2B
- Rouge de Rance marble #8E3B34
- Pier-glass green #2F5B49
- Candle-warm key #FF9329 (see section 3)

**Royal vs fancy hotel [E, my synthesis of the documented facts above]:**
1. Repeated bay rhythm with a fixed pitch (about 4.3 m).
2. Mirrors facing windows, so the room doubles in depth.
3. One long axis (4:1 to 7:1) with a focal object on the centre line.
4. Gilding only on relief edges (capitals, fillets, frames), never on flat fields.
5. Saturated field colour (crimson, green) or white; never beige.
6. Real materials (marble, silk) with a visible grain.
7. Ceiling treated as a painted surface.

## 2. Monumental display

**David [D]:** 5.17 m tall, about 8.5 t (en.wikipedia.org/wiki/David_(Michelangelo)). Moved 1873 to the Tribune by architect Emilio De Fabris: a vaulted exedra/apse under a glazed dome, daylight from above, in place since 1882. The 19th-century pedestal is **1.60 m** high, smaller than the original (Vacatis/University of Bologna snippet; single secondary source, treat as [D-weak]). Total height about 6.8 m.

**Derived viewing rules [E]:**
- Total object height H = 6.8 m. To fit it in a 30 degree vertical field of view the eye needs about 12.6 m (H / (2 tan 15 degrees)). This matches the "first view at about 2 to 3 H" rule of thumb; use 12 to 16 m.
- Plinth about 0.25 to 0.3 of the figure height (1.6 / 5.17 = 0.31).
- Approach axis: long hall leading to the apse, the statue only revealed on the centre line; side walls dim, apse bright.
- Backdrop: a curved, plain, light-mid wall (niche), no pattern within 1 m of the silhouette.
- Light: from above and slightly in front (top-lit dome); the face gets about 30 to 45 degrees of downward key, which gives a readable shadow under the brow.
- Ceiling or dome about 2x the statue-plus-plinth height (about 12 to 14 m) so the object does not touch the ceiling visually.

## 3. Candlelight in stylised real-time

**Physical anchor [D]:** a candle flame is about 1,850 K (en.wikipedia.org/wiki/Color_temperature), roughly #FF9329 in sRGB [E].

**Techniques [D, Unity Learn "Simulating Candle Lighting with Point Lights"; gameinspired.substack.com]:**
- Point light with intensity driven by Perlin noise (lerp between min and max), plus an emissive unlit flame card with UV-distorted noise.
- Flicker of textured/cookie lights makes shadows move.

**Ratio and avoidance [E, no single source]:**
- Warm light on about 30 to 40% of the frame (pools around candles, 2 to 3 m radius), the rest cool shadow (#25376A to #3E5C8D, the Starry Night sky blues).
- Flicker amplitude about +/-10 to 15% at 6 to 10 Hz, from smooth noise, never random per frame.
- Muddy orange comes from tinting everything warm. Keep the ambient/fill blue-violet, let the key go to yellow-white at the flame core (#FFE8B0), and keep orange only in the falloff.
- Limit real lights to the nearest 4 to 8; fake the rest with additive emissive cards and baked glow so WebGL stays cheap.

## 4. Van Gogh, The Starry Night (1889)

**Facts [D]:** oil on canvas, 73.7 x 92.1 cm (aspect 1.25), painted June 1889 at Saint-Rémy, MoMA since 1941 (en.wikipedia.org/wiki/The_Starry_Night). Public domain: Van Gogh died 1890, and the Commons file tag reads "public domain in its country of origin and other countries where the term is life plus 100 years or fewer" (commons.wikimedia.org/wiki/File:Van_Gogh_-_Starry_Night_-_Google_Art_Project.jpg; I confirmed licence "Public domain" via the Commons API).

**Composition:**
- Sky fills the upper two thirds, village and hills the lower third [D, secondary]. My brightness profile of the image [M] shows the sharp drop at about 62 to 65% down from the top, so put the horizon/hill line at about 0.63 H.
- A huge cypress on the left runs from the bottom edge nearly to the top (about 5 to 28% of the width [M/E]).
- Venus-and-star count: Wikipedia says about 11; other sources count 10 stars plus Venus. Use 11 stars + moon at upper right (about 76 to 95% x, 5 to 25% y [M]), which satisfies both counts.
- Swirl structure: a large S-shaped central spiral flanked by two smaller vortices. Brushwork: continuous spirals in the sky, long curved flame strokes on the cypress, short sharp dashes in the village [D, secondary].

**Palette [M] (median cut on the Commons Google Art Project scan; the scan reads cooler and greyer than the oil, so push saturation about +15% [E]):**
- Sky deep #25376A
- Sky mid #3E5C8D
- Sky light #69839B
- Pale green-grey #909F98 / #B5BE9D
- Moon/star yellow #C4CB8E (core), #AFB270 (edge)
- Cypress near-black #121617
- Ground dark #0B1118 / #2F3945

Pigments [D, secondary]: ultramarine and cobalt in the sky; zinc/chrome yellow and Indian yellow for stars and moon.

**Brush-stroke rules for a shader/world [E]:** stroke length 1 to 3% of canvas width in the sky; direction follows the tangent of the spiral field; the cypress uses long vertical-curved strokes about 5 to 8% of the canvas height; village strokes are short axis-aligned dashes of about 1%.

**Codas:**
- **Café Terrace at Night [D]:** Arles, September 1888, 80.7 x 65.3 cm, Kröller-Müller. Funnel-like perspective into the street; painted "without black" using blue, violet and green with a "pale sulphur, lemon green" lit square; dated to about 11 pm by the Aquarius constellation. Palette [M]: #D1C771, #AC925F, #416889, #293231. Commons image (public domain): Van_Gogh_-_Terrasse_des_Cafés_an_der_Place_du_Forum_in_Arles_am_Abend1.jpeg.
- **Bedroom in Arles [D]:** three versions (Oct 1888, 72 x 90 cm, Van Gogh Museum; Sept 1889, 72 x 90 cm, Art Institute of Chicago; 57.5 x 74 cm, Orsay). The real room was a trapezoid with an obtuse angle at the left and an acute angle at the right, so the floor and bed are skewed and there are no cast shadows (flat tints after Japanese prints). Letter palette: violet walls, red tile floor, "fresh butter yellow" bed and chairs, lemon-green sheets, scarlet blanket, green window. Palette [M]: #A8B8D6 walls (now blue because the violet has faded [E]), #C59B4D, #A06646 floor. Commons: Vincent_van_Gogh_-_De_slaapkamer_-_Google_Art_Project_adjusted.jpg (public domain).

## 5. Game references: one principle each (no copying)

- **Viewfinder** (Sad Owl, 2023) [D]: a photo is projected back and overwrites the world's geometry. Transfer: a captured frame is a persistent object whose depth must match the camera pose at capture; store camera position, FOV and aspect with every print.
- **Superliminal** [D]: a held object keeps its apparent screen size, so scale is proportional to distance change. Transfer: scale = distance_new / distance_old, hold the angular size constant.
- **ULTRAKILL Layer 8 Fraud** [D, Wikipedia]: the layer distorts the player's perception (mirrors, the Mirror Reaper). The wiki does not describe the technique, so only the design intent transfers: break a trusted spatial cue (mirror reflection) once, deliberately.
- **The Witness** [D]: path fragments scattered in the environment connect only from one viewpoint. Transfer: place a floor marker at the one viewpoint where a painting-world alignment completes; tolerance about 0.3 m.
- **Gorogoa** [D]: stacked panels where a hole in one image becomes a mask for another. Transfer: use a doorway or window cut-out as a portal to the next painting.
- **Return of the Obra Dinn** [D]: 1-bit dithered rendering; answers validate only in sets of three. Transfer: limiting the palette strengthens the style (a dither or posterise pass costs about 1 fullscreen pass), and gating confirmation reduces guessing.

## Rules of thumb for builders

1. Main gallery: 4.3 : 1 length-to-width (Schönbrunn) up to 7 : 1 (Versailles); ceiling height = 1.0 to 1.2 x width.
2. Window/mirror bay pitch of 4.3 m, 17 bays per 73 m; repeat exactly, never jitter more than 2 cm.
3. Wainscot/dado height 1.0 m (3 to 4 ft), about 1/3 of a 3.0 m wall or 1/12 of the Versailles height.
4. Chandelier of 72 candles-equivalent every 14 m along a 43 m hall; at most 4 real point lights active.
5. Carpet runner 1.2 m wide (estimate), leaving 0.15 to 0.2 m of floor each side.
6. Gilt only on relief edges: cap gold area at 10% of the visible wall.
7. Plinth height = 0.3 x statue height (1.6 / 5.17).
8. Statue = 2 to 4x life size (David 5.17 m vs 1.75 m is 3x); first-view distance 12 to 16 m (about 2 to 3x total height).
9. Keep a clear 1 m silhouette margin around a statue's niche backdrop; key light 30 to 45 degrees above the eye line.
10. Candle key colour 1,850 K (#FF9329); core #FFE8B0; flicker +/-12% at 8 Hz from smooth noise.
11. Warm pools on at most 35% of the frame; the rest cool (#25376A to #3E5C8D).
12. Starry Night world: horizon at 0.63 of canvas height, canvas aspect 1.25, sky 2/3, 11 stars, moon at upper right.
13. Sky brush strokes 1 to 3% of canvas width, oriented along the spiral tangent; cypress strokes 5 to 8% of the height.
14. Café Terrace coda: use no black; darkest value #121617 only on the cypress, #293231 elsewhere.
15. Bedroom coda: skew the floor plane by an obtuse and an acute corner (for example 100 and 80 degrees), no cast shadows.

## Verification corrections (these override the text above)

- **Claim:** Star count: "Wikipedia says about 11"; so use 11 stars + moon, which "satisfies both counts".
  - **Correction:** Wikipedia's Starry Night article gives 10 stars in an image caption ("10 stars with swirls, Venus, and a bright yellow crescent Moon"). "Eleven" appears only in Loevgren's symbolist theory about the eleven stars in Joseph's dream, not as a count of the painting. The 10-stars-plus-Venus count is the one the article supports. Eleven stars plus Venus would be 12 bright objects, so it does not match both counts. (source: https://en.wikipedia.org/wiki/The_Starry_Night)
- **Claim:** Derived [E]: two chandeliers in 43 m give a spacing of about 14 m "with about 7 m end margins (each at 1/3 and 2/3 of the length)". Rule of thumb 4 repeats "every 14 m along a 43 m hall".
  - **Correction:** The arithmetic is inconsistent. At 1/3 and 2/3 of 43 m the chandeliers sit about 14.3 m from each end and about 14.3 m apart. A 7 m end margin with two chandeliers would give about 29 m between them. Schönbrunn's page gives no chandelier positions. Rule 4 also implies more than two chandeliers (43/14 is about 3), which contradicts the documented two. (source: https://www.schoenbrunn.at/en/about-schoenbrunn/the-palace/tour-of-the-palace/great-gallery (gives only 43 m, almost 10 m wide, two chandeliers of 72 candles; no positions))
- **Claim:** Candle techniques [D]: point light with intensity driven by Perlin noise (lerp between min and max), cited to the Unity Learn "Simulating Candle Lighting with Point Lights" tutorial.
  - **Correction:** The Unity Learn page exists but only covers adding a point light in the flame and adjusting its colour and intensity. It has no Perlin noise, lerp or flicker content. Treat the Perlin and lerp technique as [E] or re-source it. The gameinspired.substack.com source is not listed in the sources array and I did not check it. (source: https://learn.unity.com/tutorial/challenge-1-simulating-candle-lighting-with-point-lights)
- **Claim:** David pedestal is 1.60 m [D-weak, Vacatis snippet]; total height about 6.8 m; plinth ratio 0.31.
  - **Correction:** The cited Vacatis page gives no pedestal figure, and the Wikipedia David article gives none either. The 1.60 m is not supported by either listed source. The 6.8 m total, the 0.31 ratio, the 12.6 m viewing distance and rules 7 and 8 all depend on it, so they should be marked [E] or dropped until a real source is found. (source: https://vacatis.com/the-tribune-accademia-gallery ; https://en.wikipedia.org/wiki/David_(Michelangelo))

## Unsupported claims (treat as estimates until re-sourced)

- Hall of Mirrors "9 large ceiling paintings" is correct, but the article says nine large plus numerous smaller ones. Builders should not read nine as the total.
- David "first view at about 2 to 3 H": the pack's own 12.6 to 16 m on a 6.8 m total is only about 1.9 to 2.4 H.
- "Statue = 2 to 4x life size" has no source; only the David 3x figure is arithmetic.
- Return of the Obra Dinn "sets of three": the article adds that the last six fates are validated in sets of two. Rule 15 drops that nuance.
- Wallace Collection crimson silk, gilt fillet and oculi come from search snippets only. The Country Life page returned navigation text, so nothing was verified.
- The Starry Night composition claims (sky two thirds, horizon at 0.63 H, cypress 5 to 28% of width, moon at x 76 to 95%, y 5 to 25%) rest on the pack's own image measurements. I did not open the evidence JPGs or recompute them.
- All palette hex values are median-cut samples tagged [M] and were not re-measured. The colourlex pigment claims were not verified.
- The Café Terrace and Bedroom palettes were not re-measured. The Commons licence and filenames were not re-checked.
- Gorogoa "2x2 grid, up to 4 images", The Witness "tolerance 0.3 m" and Superliminal were not fetched. The Witness tolerance is an estimate.
- Carpet runner widths, flicker amplitude and rate, the 35% warm-area cap and the 10% gilt cap are all [E] with no source. The pack labels them so, which is acceptable.

Verifier notes: I checked 17 claims against live pages. These matched: Hall of Mirrors 73 x 10.50 x 12.30 m, 17 windows, 350+ mirror surfaces, Rouge de Rance pilasters, green marble pier glasses and 3,000+ candles; Schönbrunn 43 m, almost 10 m wide, 72-candle chandeliers, 868 bulbs in 1898, Guglielmi 1761, the Maria Theresa statue and the 2011/12 LED candles; David 5.17 m, 8.5 t, 1873 move, De Fabris and 1882; Starry Night 73.7 x 92.1 cm, June 1889 and MoMA 1941; candle flame 1,850 K, though the table row is labelled \"candle flame, sunset/sunrise\"; Wainscot \"roughly a meter, 3–4 feet\"; Café Terrace 80.7 x 65.3 cm, September 1888, Kröller-Müller, about 11 pm and the \"without black\" quote; the three Bedroom versions with their sizes and museums, and the trapezoid room; Viewfinder released 2023-07-18 by Sad Owl; and ULTRAKILL Layer 8 Fraud with the Mirror Reaper. The simple derived arithmetic also checks out (73/17 = 4.3 m, 4.3/10.5 = 0.41, 1.6/5.17 = 0.31, 12.6 m at a 30 degree field of view). Fetches went through a summarising model, so figures are second-hand. The four wrong claims above should be fixed before agents build on rules 4, 7, 8 and 12. The 11-star decision in rule 12 is a design choice that Wikipedia does not support, so it should stay a design choice rather than be presented as sourced.

## Known gaps

- No dimensions found for Hermitage halls (area only: Great Throne Room 800 sqm, Armorial Hall 1000 sqm) or the Wallace Great Gallery.
- Door widths, carpet runner widths and chandelier spacing are not documented; given as estimates.
- David pedestal height 1.60 m comes from one secondary snippet; Wikipedia gave no pedestal figure and the Tribune dimensions were not found.
- Star count differs by source (about 11 vs 10 plus Venus); Wikipedia's statement of about 11 was used.
- Palette hex values are median-cut samples of Google Art Project scans, which have a colour cast versus the real oils; no direct MoMA data (moma.org returned 403).
- Warm/cool ratio, flicker rates, brush-stroke sizes and viewing-distance rules are estimates, not sourced figures.
- ULTRAKILL Fraud technique details were not found; only design intent.

## Local evidence (not committed; third-party screenshots stay outside the repos)

- `C:\Users\Aaron\Documents\GitHub\portfolio-evidence\factpacks\manor-refs\starry_night_1000.jpg`
- `C:\Users\Aaron\Documents\GitHub\portfolio-evidence\factpacks\manor-refs\cafe_terrace_800.jpg`
- `C:\Users\Aaron\Documents\GitHub\portfolio-evidence\factpacks\manor-refs\bedroom_arles_1000.jpg`
- `C:\Users\Aaron\Documents\GitHub\portfolio-evidence\factpacks\manor-refs\commons_starry_night_page.png`
- `C:\Users\Aaron\Documents\GitHub\portfolio-evidence\factpacks\manor-refs\commons_bedroom_page.png`
