(function () {
  "use strict";
  const sidebar = document.getElementById("adminSidebar");
  const toggle = document.querySelector("[data-admin-toggle]");
  const backdrop = document.querySelector("[data-admin-close]");
  const mobile = window.matchMedia("(max-width: 768px)");
  function setMenu(open, restore = false) {
    if (!sidebar || !toggle) return;
    sidebar.classList.toggle("open", open);
    toggle.setAttribute("aria-expanded", String(open));
    toggle.setAttribute(
      "aria-label",
      open ? "Close workspace navigation" : "Open workspace navigation",
    );
    backdrop.hidden = !open;
    document.body.style.overflow = open ? "hidden" : "";
    if (open) sidebar.querySelector("a").focus();
    if (restore) toggle.focus();
  }
  toggle?.addEventListener("click", () =>
    setMenu(!sidebar.classList.contains("open")),
  );
  backdrop?.addEventListener("click", () => setMenu(false, true));
  mobile.addEventListener("change", () => setMenu(false));
  document.addEventListener("keydown", (event) => {
    if (!sidebar?.classList.contains("open")) return;
    if (event.key === "Escape") setMenu(false, true);
    if (event.key === "Tab") {
      const links = Array.from(sidebar.querySelectorAll("a, button"));
      const first = links[0],
        last = links[links.length - 1];
      if (event.shiftKey && document.activeElement === first) {
        event.preventDefault();
        last.focus();
      } else if (!event.shiftKey && document.activeElement === last) {
        event.preventDefault();
        first.focus();
      }
    }
  });
  document
    .querySelectorAll("[data-dismiss-notice]")
    .forEach((button) =>
      button.addEventListener("click", () =>
        button.closest(".notice").remove(),
      ),
    );
  const dialog = document.getElementById("confirmDialog");
  let pendingForm, opener;
  document.querySelectorAll("[data-delete-form]").forEach((form) =>
    form.addEventListener("submit", (event) => {
      if (form.dataset.confirmed === "true") return;
      event.preventDefault();
      pendingForm = form;
      opener = event.submitter;
      document.getElementById("confirmTitle").textContent =
        form.dataset.deleteTitle || "Delete this item?";
      dialog.showModal();
      dialog.querySelector("[data-cancel-delete]").focus();
    }),
  );
  dialog
    ?.querySelector("[data-cancel-delete]")
    .addEventListener("click", () => dialog.close());
  dialog
    ?.querySelector("[data-confirm-delete]")
    .addEventListener("click", (event) => {
      if (!pendingForm) return;
      event.currentTarget.disabled = true;
      event.currentTarget.textContent = "Deleting…";
      pendingForm.dataset.confirmed = "true";
      pendingForm.requestSubmit();
    });
  dialog?.addEventListener("close", () => opener?.focus());
  const filters = Array.from(document.querySelectorAll("[data-filter]"));
  const rows = Array.from(document.querySelectorAll("[data-message-row]"));
  const search = document.getElementById("messageSearch");
  let filter = "all";
  function applyFilters() {
    const query = search.value.trim().toLocaleLowerCase();
    let total = 0;
    rows.forEach((row) => {
      const matches =
        (filter === "all" || row.dataset.status === filter) &&
        row.textContent.toLocaleLowerCase().includes(query);
      row.hidden = !matches;
      if (matches) total++;
    });
    document.getElementById("filteredCount").textContent =
      total + (total === 1 ? " message" : " messages");
    const empty = document.getElementById("filteredEmpty");
    empty.hidden = total !== 0;
    if (!total) {
      empty.querySelector("h2").textContent = rows.length
        ? "No notes match this view."
        : "No messages here yet.";
      empty.querySelector("p").textContent = rows.length
        ? "Try another name or choose a different filter."
        : "New notes will appear here, ready for your review.";
    }
  }
  filters.forEach((button) =>
    button.addEventListener("click", () => {
      filter = button.dataset.filter;
      filters.forEach((item) =>
        item.setAttribute("aria-pressed", String(item === button)),
      );
      applyFilters();
    }),
  );
  search?.addEventListener("input", applyFilters);
})();
