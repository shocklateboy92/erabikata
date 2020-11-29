import { FullWidthText } from 'components/fullWidth';
import React, { FC, useEffect } from 'react';
import styles from './hass.module.scss';
import { selectPlayerList, selectSelectedPlayer } from './selectors';
import { useAppSelector } from 'app/hooks';
import { useDispatch, useSelector } from 'react-redux';
import { updatePlayerList, useHass } from './api';
import { playerSelection } from './actions';

export const HassCheck: FC = () => {
    const [dispatch, hass] = [useDispatch(), useHass()];
    useEffect(() => {
        dispatch(updatePlayerList(hass));
    }, [dispatch, hass]);
    const players = useAppSelector(selectPlayerList);
    const selectedId = useSelector(selectSelectedPlayer);

    return (
        <FullWidthText>
            <h2>Use Player</h2>
            <form className={styles.playerList}>
                {Object.values(players).map((player) => (
                    <div key={player.id}>
                        <input
                            name="playerList"
                            id={createId(player.id)}
                            type="radio"
                            checked={player.id === selectedId}
                            onChange={(e) =>
                                e.target.checked &&
                                dispatch(playerSelection(player.id))
                            }
                        />
                        <label htmlFor={createId(player.id)}>
                            {player.name}
                        </label>
                    </div>
                ))}
            </form>
        </FullWidthText>
    );
};

// HTML forms don't allow `null` as an identifier
const createId = (id: string | null) => 'player' + id;
