import React, { FC } from 'react';
import styles from './fullWidth.module.scss';

export const FullWidthText: FC = ({ children }) => (
    <div className={styles.container}>{children}</div>
);
