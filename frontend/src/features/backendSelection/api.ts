export const getInitialBaseUrl = () =>
    new URLSearchParams(window.location.search).get('env') ??
    // Convenience override for frontend-only local dev
    (window.location.origin === 'http://localhost:3000'
        ? 'https://erabikata3.apps.lasath.org'
        : window.location.origin);
