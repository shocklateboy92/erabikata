import {
    mdiTelevisionPlay,
    mdiInformationOutline,
    mdiSortAscending
} from '@mdi/js';
import { Separator } from 'components/separator';
import { FuriganaOption } from 'features/furigana/option';
import { SpinnerTop } from 'features/spinnerTop/component';
import React, { PropsWithChildren } from 'react';
import { FC } from 'react';
import { NavLink } from 'react-router-dom';
import { HeaderItem } from '.';
import styles from './header.module.scss';
import { CloseButton } from './ts';

export const AppHeader: FC<PropsWithChildren<{}>> = ({ children }) => (
    <header className={styles.container}>
        <SpinnerTop />
        <Separator navBar />
        <div className={styles.title}>{children}</div>
        <Separator navBar />
        <NavLink to="/ui/rankedWords">
            <HeaderItem icon={mdiSortAscending}>Ranked Words</HeaderItem>
        </NavLink>
        <Separator navBar />
        <NavLink to="/ui/nowPlaying">
            <HeaderItem icon={mdiTelevisionPlay}>Now Playing</HeaderItem>
        </NavLink>
        <NavLink to="/ui/settings">
            <HeaderItem icon={mdiInformationOutline}>App Info</HeaderItem>
        </NavLink>
        <FuriganaOption />
        <CloseButton />
    </header>
);
