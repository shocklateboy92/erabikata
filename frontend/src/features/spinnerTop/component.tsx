import { useAppSelector } from 'app/hooks';
import classNames from 'classnames';
import { play, useHass } from 'features/hass';
import { fetchNowPlayingSessions } from 'features/nowPlaying';
import React, { FC } from 'react';
import { useDispatch } from 'react-redux';
import logo from './logo.svg';
import styles from './spinnerTop.module.scss';

export const SpinnerTop: FC = () => {
    const dispatch = useDispatch();
    const hass = useHass();
    const shouldSpin = useAppSelector(
        (state) =>
            state.spinnerTop.activeRequests +
                Object.keys(state.spinnerTop.requests).length >
            0
    );

    return (
        <div
            className={classNames({
                [styles.spinning]: shouldSpin
            })}
            onClick={() => {
                dispatch(play(hass));
                dispatch(fetchNowPlayingSessions());
            }}
        >
            <img src={logo} className={styles.logo} alt="logo" />
        </div>
    );
};
