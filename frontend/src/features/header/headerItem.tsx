import Icon from '@mdi/react';
import React, { FC } from 'react';
import styles from './header.module.scss';

interface IHeaderItem {
    icon: string;
}

export const HeaderItem: FC<IHeaderItem> = ({ icon, children }) => (
    <span className={styles.item}>
        <Icon path={icon} size="2em" />
        <span className="hideOnMobile">{children}</span>
    </span>
);
