const LOCAL_STORAGE_KEY = 'token';

export const getToken = () =>
    new URL(window.location.href).searchParams.get('token') ||
    window.localStorage.getItem(LOCAL_STORAGE_KEY);

export const setToken = (token?: string) => {
    if (token) {
        window.localStorage.setItem(LOCAL_STORAGE_KEY, token);
    }
};
