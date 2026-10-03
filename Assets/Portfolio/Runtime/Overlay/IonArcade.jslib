// The Grand Gallery's arcade overlay: a project demo in an iframe inside an arcade-cabinet bezel.
// IonArcadeOpen(url, title) shows it (and releases the pointer lock); IonArcadeIsOpen() is polled by C#.
// Closing: the button, Esc (also inside a same-origin demo), a click on the dimmed backdrop, or the demo posting
// the message 'ion-arcade:close' to its parent. Nothing here may throw (exceptions are off).
mergeInto(LibraryManager.library, {
  IonArcadeOpen: function (urlPtr, titlePtr) {
    try {
      var url = UTF8ToString(urlPtr), title = UTF8ToString(titlePtr) || 'Arcade';
      var root = document.getElementById('ion-arcade');
      if (!root) {
        root = document.createElement('div');
        root.id = 'ion-arcade';
        root.setAttribute('role', 'dialog');
        root.setAttribute('aria-modal', 'true');
        root.style.cssText = 'position:fixed;inset:0;z-index:2147483000;display:flex;align-items:center;' +
          'justify-content:center;background:rgba(10,6,8,0.86);font:15px/1.4 Georgia,serif;color:#EFE3CF';

        var cab = document.createElement('div');
        cab.style.cssText = 'display:flex;flex-direction:column;align-items:center;gap:10px;padding:18px 22px 16px;' +
          'background:linear-gradient(180deg,#5A3A22,#2A1418);border:2px solid #C59A45;border-radius:10px;' +
          'box-shadow:0 0 0 6px rgba(197,154,69,0.12),0 30px 80px rgba(0,0,0,0.7)';

        var marquee = document.createElement('div');
        marquee.id = 'ion-arcade-title';
        marquee.style.cssText = 'padding:6px 18px;border-radius:4px;background:#FFD9A0;color:#2A1418;' +
          'letter-spacing:0.12em;text-transform:uppercase;box-shadow:0 0 18px rgba(255,190,110,0.7)';

        var frame = document.createElement('iframe');
        frame.id = 'ion-arcade-screen';
        frame.title = 'Arcade demo';
        frame.setAttribute('sandbox', 'allow-scripts allow-same-origin allow-pointer-lock');
        frame.style.cssText = 'width:min(86vw,114vh);height:min(64.5vw,85.5vh);border:0;border-radius:6px;' +
          'background:#0E0A0C;box-shadow:inset 0 0 0 4px #140E10';

        var close = document.createElement('button');
        close.type = 'button';
        close.textContent = 'Back to the gallery  [Esc]';
        close.style.cssText = 'padding:8px 16px;border:1px solid #C59A45;border-radius:4px;background:transparent;' +
          'color:#EFE3CF;font:600 14px/1 system-ui,sans-serif;cursor:pointer';

        cab.appendChild(marquee);
        cab.appendChild(frame);
        cab.appendChild(close);
        root.appendChild(cab);
        document.body.appendChild(root);

        var shut = function () {
          var r = document.getElementById('ion-arcade');
          if (!r) return;
          var f = document.getElementById('ion-arcade-screen');
          if (f) f.src = 'about:blank';
          r.style.display = 'none';
          window.__ionArcadeOpen = false;
          var canvas = document.getElementById('unity-canvas');
          if (canvas) canvas.focus();
        };
        close.addEventListener('click', shut);
        root.addEventListener('click', function (e) { if (e.target === root) shut(); });
        var onKey = function (e) {
          if (window.__ionArcadeOpen && e.key === 'Escape') { e.preventDefault(); shut(); }
        };
        window.addEventListener('keydown', onKey, true);
        window.addEventListener('message', function (e) {
          var f = document.getElementById('ion-arcade-screen');
          if (window.__ionArcadeOpen && f && e.source === f.contentWindow && e.data === 'ion-arcade:close') shut();
        });
        // Keys typed into the demo stay inside the iframe, so Esc is also hooked there (same-origin demos only).
        frame.onload = function () {
          if (!window.__ionArcadeOpen) return;
          try { frame.focus(); } catch (e) {}
          try { frame.contentWindow.addEventListener('keydown', onKey, true); } catch (e) {}
        };
      }
      document.getElementById('ion-arcade-title').textContent = title;
      var screen = document.getElementById('ion-arcade-screen');
      screen.src = url;
      root.style.display = 'flex';
      window.__ionArcadeOpen = true;
      if (document.exitPointerLock && document.pointerLockElement) document.exitPointerLock();
    } catch (e) {
      window.__ionArcadeOpen = false;
    }
  },

  IonArcadeIsOpen: function () {
    return window.__ionArcadeOpen ? 1 : 0;
  }
});
