import React, { FC, useRef } from 'react';
import { setToken } from 'api/plexToken';
import styles from './login.module.scss';

export const LoginPage: FC = () => {
    const inputRef = useRef<HTMLInputElement>(null);

    return (
        <form className={styles.mainForm}>
            <label htmlFor="tokenInput">PlexのTokenを入力して:</label>
            <input id="tokenInput" name="tokenInput" ref={inputRef} />
            <button
                onClick={(e) => {
                    e.preventDefault();
                    setToken(inputRef.current?.value);
                    window.location.href = '/';
                }}
            >
                ログイン
            </button>
        </form>
    );
};
