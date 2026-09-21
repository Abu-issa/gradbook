# GradBook — The Graduation Edition

## Presentation architecture

The redesign stays in ASP.NET Core MVC and Razor. It does not change controllers,
services, entities, view models, authentication, authorization, upload services,
route configuration, or SignalR contracts.

- `wwwroot/css/site.css`: semantic theme tokens, public layouts, shared controls,
  image viewer, forms, responsive compositions, and reduced-motion rules.
- `wwwroot/css/admin.css`: workspace navigation, moderation, metrics, journal
  management, login, and printable QR invitation.
- `wwwroot/js/site.js`: theme persistence, navigation, image viewing, upload
  previews, validation-aware loading, countdown, copying, and optional reveals.
- `wwwroot/js/message-wall.js`: the existing `/messageHub` and `ReceiveMessage`
  event, with safe text insertion, connection feedback, and empty-wall recovery.
- `wwwroot/js/admin.js`: mobile drawer, local search/filtering, and delete
  confirmation. Existing POST destinations and hidden identifiers are retained.
- `wwwroot/js/qr.js`: client-side QR generation and print action.

Shared Razor partials provide the head, message cards, memory cards, image viewer,
closing invitation, and moderation actions. Message-card markup in the live
renderer intentionally matches `_MessageCard.cshtml`; keep both in sync.

The palette uses charcoal, ivory, and champagne, with Manrope for the interface
and Cormorant Garamond for editorial headings. Public and admin themes use the
existing `gradbook-theme` storage key. The cinematic portrait remains dark in
both themes. The supplied portrait is copied unchanged into `wwwroot/images` so
it is a normal publishable static asset.

Bootstrap, Font Awesome, AOS, SweetAlert, and particle scripts are no longer loaded
by the redesigned layouts. Google Fonts, jQuery validation, the existing SignalR
client, and the QR library still use external CDNs. No frontend build step is
required. The unused legacy particle file is retained.

## Verification completed

- `dotnet build GradBook.sln --no-restore --nologo` passed.
- JavaScript syntax checks passed with `node --check`.
- `git diff --check` passed.
- Git comparison confirmed no changes to backend/application/domain/infrastructure
  code, controller contracts, project dependencies, or startup configuration.
- All 11 screens rendered in an isolated MVC preview using the actual compiled
  Razor views and local assets, with representative sample models.
- Widths 320, 375, 768, 1024, and 1440 were checked; no page-level horizontal
  overflow was found. Light and dark themes were inspected.
- Invalid message submission displayed errors and kept the submit button usable.
  A valid sample submission reached the confirmation page in the preview.
- Reaction selection, initial character count, photo preview, photo removal,
  and unsupported-file recovery were checked.
- Moderation filters, text search, filtered-empty state, and cancellation of
  deletion were checked. Mobile drawer opening, Escape, and focus restoration
  were checked.
- Image viewing handled quoted captions, Escape, and returning focus to the
  originating image control.
- A real SignalR event in the isolated preview populated an initially empty wall,
  updated its count, and enabled the new photo viewer. HTML-like message content
  remained literal text.
- QR generation produced a canvas/image and enabled printing. A print stylesheet
  isolates the invitation from the workspace.

## Verification boundary

The temporary preview is separate from the application startup and database. Its
content is sample data; its form handlers do not persist messages, authenticate
users, or upload to Cloudinary. It is a presentation preview, not a production
or staging deployment. No preview controllers or sample content were added to
the application repository.

Real cookie authentication, database writes, Cloudinary uploads, destructive
moderation, printer output, and cross-browser/device testing remain integration
checks for a configured non-production environment. The existing database/startup
configuration and missing Home/Error and Home/Privacy views were not changed.
The existing obsolete Npgsql option warning can appear on a full rebuild.

## Regression checklist for deployment

1. Log in and out; verify anonymous users cannot access protected admin actions.
2. Submit messages with and without photos; check validation and pending status.
3. Approve a test message while a second browser has the wall open.
4. Create and delete disposable memories and messages; verify redirects and alerts.
5. Exercise long names, 2,000-character messages, Arabic content, missing images,
   and empty collections in both themes.
6. Check the deployed QR address and print/scan an invitation.
7. Verify keyboard navigation, zoom, reduced motion, and mobile touch behavior.
