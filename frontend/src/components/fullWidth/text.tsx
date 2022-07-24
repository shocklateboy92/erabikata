import React, { FC, PropsWithChildren } from 'react';
import styles from './fullWidth.module.scss';

export const FullWidthText: FC<PropsWithChildren<{}>> = ({ children }) => (
    <div className={styles.container}>{children}</div>
);
