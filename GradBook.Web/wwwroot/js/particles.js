/**
 * GradBook Particle System
 * Floating graduation-themed particles
 */
(function () {
  const canvas = document.getElementById('particleCanvas');
  if (!canvas) return;

  const ctx = canvas.getContext('2d');
  let W, H, particles = [], animId;

  const SYMBOLS = ['✦', '⭐', '✨', '◆', '·', '•', '▪'];
  const GOLD = 'rgba(201,168,76,';
  const COUNT = 55;

  function resize() {
    W = canvas.width = window.innerWidth;
    H = canvas.height = window.innerHeight;
  }

  function rand(min, max) { return Math.random() * (max - min) + min; }

  function createParticle() {
    return {
      x: rand(0, W),
      y: rand(0, H),
      size: rand(0.6, 2.2),
      symbol: SYMBOLS[Math.floor(Math.random() * SYMBOLS.length)],
      fontSize: rand(8, 18),
      opacity: rand(0.05, 0.35),
      vx: rand(-0.18, 0.18),
      vy: rand(-0.22, -0.06),
      twinkleSpeed: rand(0.005, 0.02),
      twinkleDir: Math.random() > 0.5 ? 1 : -1,
      useSymbol: Math.random() > 0.6
    };
  }

  function init() {
    resize();
    particles = Array.from({ length: COUNT }, createParticle);
  }

  function update() {
    particles.forEach(p => {
      p.x += p.vx;
      p.y += p.vy;
      p.opacity += p.twinkleSpeed * p.twinkleDir;
      if (p.opacity >= 0.4 || p.opacity <= 0.03) p.twinkleDir *= -1;
      if (p.y < -20) { p.y = H + 10; p.x = rand(0, W); }
      if (p.x < -20) p.x = W + 10;
      if (p.x > W + 20) p.x = -10;
    });
  }

  function draw() {
    ctx.clearRect(0, 0, W, H);
    particles.forEach(p => {
      ctx.save();
      ctx.globalAlpha = p.opacity;
      if (p.useSymbol) {
        ctx.fillStyle = GOLD + '1)';
        ctx.font = `${p.fontSize}px serif`;
        ctx.fillText(p.symbol, p.x, p.y);
      } else {
        ctx.fillStyle = GOLD + '1)';
        ctx.beginPath();
        ctx.arc(p.x, p.y, p.size, 0, Math.PI * 2);
        ctx.fill();
      }
      ctx.restore();
    });
  }

  function loop() {
    update();
    draw();
    animId = requestAnimationFrame(loop);
  }

  window.addEventListener('resize', () => {
    cancelAnimationFrame(animId);
    resize();
    loop();
  });

  init();
  loop();
})();
