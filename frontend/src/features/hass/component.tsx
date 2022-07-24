import { useAppSelector } from 'app/hooks';
import { useAppDispatch } from 'app/store';
import { FullWidthText } from 'components/fullWidth';
import { FC, useEffect } from 'react';
import { useSelector } from 'react-redux';
import { playerSelection } from './actions';
import { updatePlayerList } from './api';
import styles from './hass.module.scss';
import { selectPlayerList, selectSelectedPlayerId } from './selectors';

export const HassCheck: FC = () => {
    const dispatch = useAppDispatch();
    useEffect(() => {
        dispatch(updatePlayerList());
    }, [dispatch]);
    const players = useAppSelector(selectPlayerList);
    const selectedId = useSelector(selectSelectedPlayerId);

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
