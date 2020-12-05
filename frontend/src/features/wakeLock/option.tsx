import { compose } from '@reduxjs/toolkit';
import { useTypedSelector } from 'app/hooks';
import React from 'react';
import { FC } from 'react';
import { useDispatch } from 'react-redux';
import { enabledToggle } from './slice';

const ELEMENT_ID = 'option-wakeLock-enabled';

export const WakeLockOption: FC = () => {
    const dispatch = useDispatch();
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
