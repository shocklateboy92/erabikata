import { useTypedSelector } from 'app/hooks';
import classNames from 'classnames';
import { pause, selectIsCurrentPlayerActive } from 'features/hass';
import React, { FC } from 'react';
import { useDispatch } from 'react-redux';
import logo from './logo.svg';
import styles from './spinnerTop.module.scss';

export const SpinnerTop: FC = () => {
    const dispatch = useDispatch();
    const shouldSpin = useTypedSelector(
        (state) => Object.keys(state.spinnerTop.requests).length > 0
    );
    const isActive = useTypedSelector(selectIsCurrentPlayerActive);

    return (
        <div
            className={classNames({
                [styles.spinning]: shouldSpin,
                [styles.inactive]: !isActive
            })}
            onClick={() => {
                dispatch(pause());
            }}
        >
            <img src={logo} className={styles.logo} alt="logo" />
        </div>
    );
};
