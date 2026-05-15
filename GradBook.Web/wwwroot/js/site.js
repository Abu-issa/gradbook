/**
 * GradBook — Main Site JavaScript
 */

/* ── AOS INIT ── */
document.addEventListener('DOMContentLoaded', function () {
  AOS.init({
    duration: 900,
    easing: 'ease-out-cubic',
    once: true,
    offset: 60,
  });

  initNavbar();
  initTheme();
  initTypingEffect();
  highlightActiveNav();
});

/* ── NAVBAR SCROLL ── */
function initNavbar() {
  const nav = document.getElementById('mainNav');
  if (!nav) return;
  window.addEventListener('scroll', () => {
    nav.classList.toggle('scrolled', window.scrollY > 40);
  }, { passive: true });
}

/* ── ACTIVE NAV HIGHLIGHT ── */
function highlightActiveNav() {
  const path = window.location.pathname.toLowerCase();
  document.querySelectorAll('.nav-link').forEach(link => {
    const href = (link.getAttribute('href') || '').toLowerCase();
    if (href && href !== '/' && path.startsWith(href)) {
      link.classList.add('active-nav');
      link.style.color = 'var(--gold)';
    } else if (href === '/' && path === '/') {
      link.classList.add('active-nav');
      link.style.color = 'var(--gold)';
    }
  });
}

/* ── THEME TOGGLE ── */
function initTheme() {
  const btn = document.getElementById('themeToggle');
  const icon = document.getElementById('themeIcon');
  const saved = localStorage.getItem('gradbook-theme') || 'dark';
  applyTheme(saved);

  if (btn) {
    btn.addEventListener('click', () => {
      const current = document.documentElement.getAttribute('data-theme');
      const next = current === 'dark' ? 'light' : 'dark';
      applyTheme(next);
      localStorage.setItem('gradbook-theme', next);
    });
  }

  function applyTheme(theme) {
    document.documentElement.setAttribute('data-theme', theme);
    if (icon) {
      icon.className = theme === 'dark' ? 'fas fa-sun' : 'fas fa-moon';
    }
  }
}

/* ── TYPING EFFECT ── */
function initTypingEffect() {
  const el = document.querySelector('.title-line-2');
  if (!el) return;
  const text = el.textContent.trim();
  el.textContent = '';
  el.style.opacity = '1';
  let i = 0;
  const timer = setInterval(() => {
    if (i < text.length) {
      el.textContent += text[i++];
    } else {
      clearInterval(timer);
    }
  }, 55);
}

/* ── COPY LINK UTILITY ── */
function copyLink() {
  const url = window.location.origin;
  if (navigator.clipboard) {
    navigator.clipboard.writeText(url).then(() => {
      showToast('Link copied to clipboard! 🔗');
    });
  } else {
    const tmp = document.createElement('textarea');
    tmp.value = url;
    document.body.appendChild(tmp);
    tmp.select();
    document.execCommand('copy');
    document.body.removeChild(tmp);
    showToast('Link copied! 🔗');
  }
}

/* ── TOAST NOTIFICATION ── */
function showToast(message, duration = 3500) {
  const container = document.getElementById('toastContainer');
  if (!container) return;

  const toast = document.createElement('div');
  toast.className = 'toast-notification';
  toast.innerHTML = `<span class="text-gold me-2">✦</span>${message}`;
  container.appendChild(toast);

  setTimeout(() => {
    toast.style.opacity = '0';
    toast.style.transform = 'translateX(100%)';
    toast.style.transition = 'all 0.4s ease';
    setTimeout(() => toast.remove(), 400);
  }, duration);
}

/* ── SMOOTH SCROLL FOR ANCHOR LINKS ── */
document.addEventListener('click', function (e) {
  const target = e.target.closest('a[href^="#"]');
  if (target) {
    e.preventDefault();
    const el = document.querySelector(target.getAttribute('href'));
    if (el) el.scrollIntoView({ behavior: 'smooth', block: 'start' });
  }
});

/* ── INTERSECTION OBSERVER for lazy loading ── */
if ('IntersectionObserver' in window) {
  const lazyImgs = document.querySelectorAll('img[loading="lazy"]');
  const observer = new IntersectionObserver((entries) => {
    entries.forEach(e => {
      if (e.isIntersecting) {
        const img = e.target;
        if (img.dataset.src) { img.src = img.dataset.src; delete img.dataset.src; }
        observer.unobserve(img);
      }
    });
  }, { rootMargin: '100px' });
  lazyImgs.forEach(img => observer.observe(img));
}
