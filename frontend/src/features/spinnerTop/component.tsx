import { useTypedSelector } from 'app/hooks';
import { useAppDispatch } from 'app/store';
import classNames from 'classnames';
import { selectIsCurrentPlayerActive, togglePlayback } from 'features/hass';
import { FC } from 'react';
import logo from './logo.svg';
import styles from './spinnerTop.module.scss';

export const SpinnerTop: FC = () => {
    const dispatch = useAppDispatch();
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
                dispatch(togglePlayback());
            }}
        >
            <img src={logo} className={styles.logo} alt="logo" />
        </div>
    );
};
