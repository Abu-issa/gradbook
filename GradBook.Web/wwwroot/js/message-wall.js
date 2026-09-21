/* Preserve /messageHub and ReceiveMessage(senderName, content, reactionType, imageUrl, createdAt). */
(function () {
  "use strict";
  const wall = document.getElementById("messageWall");
  const label = document.getElementById("connectionLabel");
  const status = document.getElementById("liveStatus");
  const count = document.getElementById("messageCount");
  const empty = document.getElementById("wallEmpty");
  if (!wall) return;
  const setStatus = (state, text) => {
    status.dataset.state = state;
    label.textContent = text;
  };
  if (!window.signalR) {
    setStatus("offline", "Live updates unavailable. Refresh to see new notes.");
    return;
  }
  const connection = new signalR.HubConnectionBuilder()
    .withUrl("/messageHub")
    .withAutomaticReconnect()
    .build();
  connection.on(
    "ReceiveMessage",
    function (senderName, content, reactionType, imageUrl, createdAt) {
      const card = document.createElement("article");
      card.className = "note-card new-note";
      // Only constant markup is parsed. All received content is inserted as text.
      card.innerHTML =
        '<div class="note-top"><span class="eyebrow">WORDS TO KEEP</span><span class="note-reaction" aria-label="Reaction"></span></div><blockquote class="note-content" dir="auto"></blockquote><footer class="note-footer"><span class="sender-initial" aria-hidden="true"></span><div><span class="sender-name" dir="auto"></span><time></time></div><span class="note-sign" aria-hidden="true">↗</span></footer>';
      const name = String(senderName || "Guest");
      card.querySelector(".note-reaction").textContent = reactionType || "🎓";
      card.querySelector(".note-content").textContent = content || "";
      card.querySelector(".sender-initial").textContent =
        Array.from(name.trim())[0]?.toUpperCase() || "·";
      card.querySelector(".sender-name").textContent = name;
      card.querySelector("time").textContent = createdAt || "";
      if (imageUrl) {
        let url;
        try {
          url = new URL(imageUrl, location.href);
        } catch (_) {}
        if (url && ["http:", "https:"].includes(url.protocol)) {
          const button = document.createElement("button");
          button.type = "button";
          button.className = "note-image image-trigger";
          button.dataset.image = url.href;
          button.dataset.title = "A memory from " + name;
          button.setAttribute("aria-label", "View photo from " + name);
          const image = document.createElement("img");
          image.src = url.href;
          image.alt = "Photo shared by " + name;
          image.loading = "lazy";
          const arrow = document.createElement("span");
          arrow.textContent = "↗";
          arrow.setAttribute("aria-hidden", "true");
          button.append(image, arrow);
          card.querySelector(".note-footer").before(button);
        }
      }
      empty.hidden = true;
      wall.prepend(card);
      count.textContent = String(Number(count.textContent) + 1);
      window.showToast("A new note from " + name + ".");
    },
  );
  connection.onreconnecting(() =>
    setStatus("offline", "Reconnecting. Your memories are still here."),
  );
  connection.onreconnected(() =>
    setStatus("connected", "Live again · refresh for any missed notes"),
  );
  let retry;
  async function start() {
    try {
      await connection.start();
      setStatus("connected", "Live · new words arrive here");
    } catch (_) {
      setStatus("offline", "Reconnecting · you can also refresh for new notes");
      retry = setTimeout(start, 15000);
    }
  }
  connection.onclose(() => {
    setStatus("offline", "Connection paused · reconnecting shortly");
    clearTimeout(retry);
    retry = setTimeout(start, 15000);
  });
  start();
})();
