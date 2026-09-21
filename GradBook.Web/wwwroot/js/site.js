/* Shared presentation interactions. No application endpoint or payload changes. */
(function () {
  "use strict";
  const reducedMotion = window.matchMedia("(prefers-reduced-motion: reduce)");
  const $ = (selector, root = document) => root.querySelector(selector);
  const $$ = (selector, root = document) =>
    Array.from(root.querySelectorAll(selector));
  window.showToast = function (message) {
    const region = $("#toastContainer");
    if (!region) return;
    const toast = document.createElement("div");
    toast.className = "toast-notification";
    toast.textContent = message;
    region.append(toast);
    setTimeout(() => toast.remove(), 5000);
  };
  window.copyLink = async function () {
    try {
      if (navigator.clipboard && window.isSecureContext)
        await navigator.clipboard.writeText(window.location.origin);
      else {
        const field = document.createElement("textarea");
        field.value = window.location.origin;
        field.style.position = "fixed";
        field.style.opacity = "0";
        document.body.append(field);
        field.select();
        const copied = document.execCommand("copy");
        field.remove();
        if (!copied) throw new Error("Copy unavailable");
      }
      window.showToast("The story is ready to share. Link copied.");
    } catch (_) {
      window.showToast(
        "Could not copy automatically. Copy the site address from your browser.",
      );
    }
  };
  $$("[data-copy-link]").forEach((button) =>
    button.addEventListener("click", window.copyLink),
  );
  $$("[data-whatsapp]").forEach((link) => {
    link.href =
      "https://wa.me/?text=" +
      encodeURIComponent(
        "Celebrate Eng. Mohammed Abu-Issa’s graduation. Leave a little love: " +
          window.location.origin,
      );
  });
  function updateThemeLabels() {
    const isDark = document.documentElement.dataset.theme !== "light";
    $$("[data-theme-toggle]").forEach((button) => {
      button.setAttribute(
        "aria-label",
        "Switch to " + (isDark ? "light" : "dark") + " theme",
      );
      button.title = button.getAttribute("aria-label");
    });
  }
  updateThemeLabels();
  $$("[data-theme-toggle]").forEach((button) =>
    button.addEventListener("click", () => {
      const theme =
        document.documentElement.dataset.theme === "light" ? "dark" : "light";
      document.documentElement.dataset.theme = theme;
      try {
        localStorage.setItem("gradbook-theme", theme);
      } catch (_) {}
      updateThemeLabels();
    }),
  );
  const menuToggle = $("[data-menu-toggle]");
  const menu = $("#publicNav");
  function closeMenu(restoreFocus = false) {
    if (!menuToggle || !menu) return;
    menu.classList.remove("open");
    menuToggle.setAttribute("aria-expanded", "false");
    menuToggle.setAttribute("aria-label", "Open navigation");
    if (restoreFocus) menuToggle.focus();
  }
  if (menuToggle && menu) {
    menuToggle.addEventListener("click", () => {
      const open = menu.classList.toggle("open");
      menuToggle.setAttribute("aria-expanded", String(open));
      menuToggle.setAttribute(
        "aria-label",
        open ? "Close navigation" : "Open navigation",
      );
    });
    document.addEventListener("click", (event) => {
      if (!event.target.closest(".site-header")) closeMenu();
    });
    menu.addEventListener("click", (event) => {
      if (event.target.closest("a")) closeMenu();
    });
    document.addEventListener("keydown", (event) => {
      if (event.key === "Escape" && menu.classList.contains("open"))
        closeMenu(true);
    });
    window
      .matchMedia("(min-width: 701px)")
      .addEventListener("change", (event) => {
        if (event.matches) closeMenu();
      });
  }
  const viewer = $("#imageViewer");
  let imageOpener;
  if (viewer) {
    document.addEventListener("click", (event) => {
      const trigger = event.target.closest("[data-image]");
      if (!trigger) return;
      const source = trigger.dataset.image;
      if (!source) return;
      let url;
      try {
        url = new URL(source, location.href);
      } catch (_) {
        return;
      }
      if (!["http:", "https:"].includes(url.protocol)) return;
      imageOpener = trigger;
      $("#viewerImage").src = url.href;
      $("#viewerImage").alt = trigger.dataset.title || "Shared photograph";
      $("#viewerTitle").textContent =
        trigger.dataset.title || "A moment to keep";
      $("#viewerDescription").textContent = trigger.dataset.description || "";
      viewer.showModal();
      $("[data-close-viewer]", viewer).focus();
    });
    $("[data-close-viewer]", viewer).addEventListener("click", () =>
      viewer.close(),
    );
    viewer.addEventListener("click", (event) => {
      if (
        event.target === viewer ||
        event.target.classList.contains("viewer-stage")
      )
        viewer.close();
    });
    viewer.addEventListener("close", () => {
      $("#viewerImage").removeAttribute("src");
      if (imageOpener && imageOpener.isConnected) imageOpener.focus();
    });
  }
  $$("[data-character-count]").forEach((input) => {
    const output = $("[data-count-output]", input.closest(".field"));
    const refresh = () => {
      if (output) output.textContent = input.value.length.toLocaleString();
    };
    input.addEventListener("input", refresh);
    refresh();
  });
  $$("[data-upload]").forEach((zone) => {
    const input = $("input[type=file]", zone);
    const prompt = $(".upload-prompt", zone);
    const preview = $(".upload-preview", zone);
    const img = $("img", preview);
    const remove = $(".upload-remove", zone);
    const error = $("[data-upload-error]", zone.closest(".field"));
    let objectUrl;
    const reset = () => {
      if (objectUrl) URL.revokeObjectURL(objectUrl);
      objectUrl = undefined;
      img.removeAttribute("src");
      preview.hidden = true;
      remove.hidden = true;
      prompt.hidden = false;
      input.value = "";
      input.setCustomValidity("");
      if (error) error.textContent = "";
    };
    const showPreview = () => {
      const file = input.files[0];
      if (!file) {
        reset();
        return;
      }
      const extension = "." + file.name.split(".").pop().toLowerCase();
      const allowedExtensions = input.accept.startsWith(".")
        ? input.accept.split(",")
        : null;
      const isImage =
        file.type.startsWith("image/") ||
        (!file.type &&
          [".jpg", ".jpeg", ".png", ".gif", ".webp"].includes(extension));
      if (
        !isImage ||
        (allowedExtensions && !allowedExtensions.includes(extension))
      ) {
        reset();
        if (error)
          error.textContent =
            "That file was not attached. Choose a supported image, or continue without a photograph.";
        return;
      }
      input.setCustomValidity("");
      if (error) error.textContent = "";
      if (objectUrl) URL.revokeObjectURL(objectUrl);
      objectUrl = URL.createObjectURL(file);
      img.src = objectUrl;
      $(".upload-filename", preview).textContent = file.name;
      preview.hidden = false;
      remove.hidden = false;
      prompt.hidden = true;
    };
    input.addEventListener("change", showPreview);
    remove.addEventListener("click", () => {
      reset();
      input.focus();
    });
    zone.addEventListener("dragover", (event) => {
      event.preventDefault();
      zone.classList.add("drag-over");
    });
    zone.addEventListener("dragleave", () =>
      zone.classList.remove("drag-over"),
    );
    zone.addEventListener("drop", (event) => {
      event.preventDefault();
      zone.classList.remove("drag-over");
      if (!event.dataTransfer.files.length) return;
      const transfer = new DataTransfer();
      transfer.items.add(event.dataTransfer.files[0]);
      input.files = transfer.files;
      showPreview();
    });
  });
  $$("[data-loading-form]").forEach((form) => {
    form.addEventListener("submit", (event) => {
      if (event.defaultPrevented) return;
      if (!form.checkValidity()) {
        event.preventDefault();
        form.reportValidity();
        return;
      }
      if (
        window.jQuery &&
        window.jQuery.fn.valid &&
        !window.jQuery(form).valid()
      ) {
        event.preventDefault();
        return;
      }
      const button = $("button[type=submit]", form);
      if (!button) return;
      button.dataset.originalText = button.textContent;
      button.textContent = button.dataset.loadingLabel || "Please wait…";
      button.disabled = true;
      form.setAttribute("aria-busy", "true");
    });
  });
  window.addEventListener("pageshow", () => {
    $$("[data-loading-form]").forEach((form) => {
      form.removeAttribute("aria-busy");
      const button = $("button[type=submit]", form);
      if (button && button.dataset.originalText) {
        button.textContent = button.dataset.originalText;
        button.disabled = false;
      }
    });
  });
  $$("[data-password-toggle]").forEach((button) =>
    button.addEventListener("click", () => {
      const input = $("input", button.parentElement);
      const showing = input.type === "password";
      input.type = showing ? "text" : "password";
      button.textContent = showing ? "Hide" : "Show";
      button.setAttribute(
        "aria-label",
        showing ? "Hide password" : "Show password",
      );
    }),
  );
  const countdown = $("[data-countdown]");
  if (countdown) {
    const date = new Date(countdown.dataset.countdown).getTime();
    let timer;
    const tick = () => {
      const distance = date - Date.now();
      const output = $("#countdown");
      if (distance <= 0 || !Number.isFinite(distance)) {
        output.hidden = true;
        $("#milestoneStatus").hidden = false;
        clearInterval(timer);
        return;
      }
      output.hidden = false;
      $("#milestoneStatus").hidden = true;
      const days = Math.floor(distance / 86400000);
      const hours = Math.floor((distance % 86400000) / 3600000);
      const minutes = Math.floor((distance % 3600000) / 60000);
      const seconds = Math.floor((distance % 60000) / 1000);
      output.textContent =
        days + "d  /  " + hours + "h  /  " + minutes + "m  /  " + seconds + "s";
    };
    tick();
    if (date > Date.now()) timer = setInterval(tick, 1000);
  }
  if ("IntersectionObserver" in window && !reducedMotion.matches) {
    const observer = new IntersectionObserver(
      (entries) =>
        entries.forEach((entry) => {
          if (entry.isIntersecting) {
            entry.target.classList.add("is-visible");
            observer.unobserve(entry.target);
          }
        }),
      { threshold: 0.08 },
    );
    $$(".reveal").forEach((element) => {
      // Only animate below-the-fold content; the first screen never waits for JS.
      if (element.getBoundingClientRect().top > window.innerHeight) {
        element.classList.add("reveal-ready");
        observer.observe(element);
      }
    });
  }
})();
