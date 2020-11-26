import { mdiTelevisionPlay, mdiInformationOutline } from '@mdi/js';
import { Separator } from 'components/separator';
import { FuriganaOption } from 'features/furigana/option';
import { SpinnerTop } from 'features/spinnerTop/component';
import React from 'react';
import { FC } from 'react';
import { NavLink } from 'react-router-dom';
import { HeaderItem } from '.';
import styles from './header.module.scss';

export const AppHeader: FC = ({ children }) => (
    <header className={styles.container}>
        <SpinnerTop />
        <Separator navBar />
        <div className={styles.title}>{children}</div>
        <Separator navBar />
        <NavLink to="/nowPlaying">
            <HeaderItem icon={mdiTelevisionPlay}>Now Playing</HeaderItem>
        </NavLink>
        <NavLink to="/settings">
            <HeaderItem icon={mdiInformationOutline}>App Info</HeaderItem>
        </NavLink>
        <FuriganaOption />
    </header>
);
