import React, { PropsWithChildren } from 'react';
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

export const SpinnerContainer: FC<PropsWithChildren<{}>> = ({ children }) => (
    <div className={styles.container}>{children}</div>
);
