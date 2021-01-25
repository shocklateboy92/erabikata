import { mdiFormatListChecks } from '@mdi/js';
import { useTypedSelector } from 'app/hooks';
import { ActionButton } from 'components/button/actionButton';
import React, { FC, useEffect } from 'react';
import { useDispatch } from 'react-redux';
import { initializeTodoist } from './api';

export const TodoistSettings: FC = () => {
    const dispatch = useDispatch();
    useEffect(
        () => {
            dispatch(initializeTodoist());
        },
        // hopefully this only needs to be done once
        [dispatch]
    );

    const isInitialized = useTypedSelector(
        (state) => !!state.todoist.authToken
    );

    return (
        <ActionButton icon={mdiFormatListChecks}>
            Todoist {isInitialized ? 'Ready' : 'Unavailable'}
        </ActionButton>
    );
};
