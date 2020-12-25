import React from 'react';
import { FC } from 'react';
import styles from './spinner.module.scss';

export const Spinner: FC = () => (
    <div className={styles['lds-spinner']}>
        <div></div>
        <div></div>
        <div></div>
        <div></div>
        <div></div>
        <div></div>
        <div></div>
        <div></div>
        <div></div>
        <div></div>
        <div></div>
        <div></div>
    </div>
);

export const SpinnerContainer: FC = ({ children }) => (
    <div className={styles.container}>{children}</div>
);
