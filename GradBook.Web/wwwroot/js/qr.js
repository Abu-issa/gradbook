(function () {
  "use strict";
  const url = window.location.origin;
  document.getElementById("siteUrl").value = url;
  const container = document.getElementById("qrcode");
  const printButton = document.getElementById("printQr");
  try {
    if (!window.QRCode) throw new Error("QR library unavailable");
    container.replaceChildren();
    new QRCode(container, {
      text: url,
      width: 212,
      height: 212,
      colorDark: "#242820",
      colorLight: "#ffffff",
      correctLevel: QRCode.CorrectLevel.M,
    });
    const image = container.querySelector("img");
    if (image) image.alt = "Scan to open the graduation website";
    printButton.disabled = false;
  } catch (_) {
    container.textContent =
      "The QR code could not load. Please refresh, or share the site link.";
  }
  printButton.addEventListener("click", () => window.print());
})();
