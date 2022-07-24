import { isTest } from 'testSelectors';

export const getInitialBaseUrl = () =>
    isTest
        ? 'https://erabikata4.apps.lasath.org'
        : new URLSearchParams(window.location.search).get('env') ??
          // Convenience override for frontend-only local dev
          (window.location.origin === 'http://localhost:3000'
              ? 'https://erabikata4.apps.lasath.org'
              : window.location.origin);
