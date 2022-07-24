import { useTypedSelector } from 'app/hooks';
import { useAppDispatch } from 'app/store';
import { FC } from 'react';
import { enabledToggle } from './slice';

const ELEMENT_ID = 'option-wakeLock-enabled';

export const WakeLockOption: FC = () => {
    const dispatch = useAppDispatch();
    const enabled = useTypedSelector((state) => state.wakeLock.enabled);
    return (
        <>
            <input
                id={ELEMENT_ID}
                type="checkbox"
                checked={enabled}
                onChange={() => {
                    dispatch(enabledToggle());
                }}
            />
            <label htmlFor={ELEMENT_ID}>
                Keep screen awake during playback
            </label>
        </>
    );
};
