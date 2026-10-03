"""Make Blender render on the GPU (AMD HIP on this machine), never the CPU.

Two uses:
  1. Once per Blender install, to save the preference in the user profile (keeps the MCP add-on enabled):
       blender.exe -b --python scripts/fork/blender_gpu.py -- --save
  2. At the start of every Blender MCP session or background script (agents):
       exec(open(r"<repo>/scripts/fork/blender_gpu.py").read())   # via execute_blender_code
     It applies the GPU settings to the open file's scenes without saving preferences.

What it does: picks the best GPU backend available (HIP for AMD, then OPTIX/CUDA, then ONEAPI/METAL), enables only
GPU devices (the CPU device is disabled so Cycles never falls back to it silently), sets every scene to render on
the GPU, caps Cycles samples and enables GPU-friendly denoising, limits CPU threads used by Blender itself, and
prefers EEVEE (GPU rasterizer) for quick previews. A render_pre handler sets Cycles back to the GPU at every render
start. In a background job (-b) with no GPU backend it raises SystemExit instead of falling back to the CPU.
"""
import sys
import bpy

PREVIEW_SAMPLES = 64       # Cycles samples for agent renders; raise only for final beauty shots
CPU_THREADS = 4            # cap for Blender's own CPU work (BVH builds, compositor); the render itself is on the GPU


def setup_devices():
    prefs = bpy.context.preferences
    cycles = prefs.addons.get('cycles')
    if cycles is None:
        print('[gpu] Cycles add-on not available')
        return None
    cp = cycles.preferences
    chosen = None
    for backend in ('HIP', 'OPTIX', 'CUDA', 'ONEAPI', 'METAL'):
        try:
            cp.compute_device_type = backend
        except TypeError:
            continue
        cp.refresh_devices() if hasattr(cp, 'refresh_devices') else cp.get_devices()
        gpus = [d for d in cp.devices if d.type == backend]
        if gpus:
            chosen = backend
            for d in cp.devices:
                d.use = (d.type == backend)
            break
    if chosen is None:
        cp.compute_device_type = 'NONE'
        print('[gpu] WARNING: no GPU backend found; Cycles would use the CPU. Use EEVEE instead.')
    else:
        print('[gpu] backend', chosen, 'devices:', [(d.name, d.type, d.use) for d in cp.devices])
    return chosen


def setup_scenes(backend):
    for scene in bpy.data.scenes:
        scene.render.threads_mode = 'FIXED'
        scene.render.threads = CPU_THREADS
        if hasattr(scene, 'cycles'):
            if backend:
                scene.cycles.device = 'GPU'
            scene.cycles.samples = min(scene.cycles.samples, PREVIEW_SAMPLES) if scene.cycles.samples else PREVIEW_SAMPLES
            scene.cycles.use_denoising = True
            try:
                scene.cycles.denoising_use_gpu = True
            except AttributeError:
                pass
        if not backend:
            # No GPU path for Cycles: switch to EEVEE, which rasterizes on the GPU.
            for engine in ('BLENDER_EEVEE_NEXT', 'BLENDER_EEVEE'):
                try:
                    scene.render.engine = engine
                    break
                except TypeError:
                    continue
    print('[gpu] scenes:', [(s.name, s.render.engine, getattr(getattr(s, 'cycles', None), 'device', '-')) for s in bpy.data.scenes])


def _force_gpu(scene, *_):
    # Re-asserted at every render start, so a script that switches to Cycles after this file ran still uses the GPU.
    if scene.render.engine == 'CYCLES' and hasattr(scene, 'cycles'):
        scene.cycles.device = 'GPU'


backend = setup_devices()
if backend is None and bpy.app.background:
    # Owner rule: GPU only. A background job with no GPU backend stops here instead of rendering on the CPU.
    raise SystemExit('[gpu] ERROR: no GPU backend (HIP/OptiX/CUDA) found; refusing to run on the CPU')
setup_scenes(backend)
if backend:
    bpy.app.handlers.render_pre[:] = [h for h in bpy.app.handlers.render_pre if getattr(h, '__name__', '') != '_force_gpu']
    bpy.app.handlers.render_pre.append(_force_gpu)
if '--save' in sys.argv:
    bpy.context.preferences.use_preferences_save = True
    bpy.ops.wm.save_userpref()
    print('[gpu] preferences saved')
