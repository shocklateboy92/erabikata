import { mdiFormatListChecks } from '@mdi/js';
import { useTypedSelector } from 'app/hooks';
import { ActionButton } from 'components/button/actionButton';
import { selectIsUserSignedIn } from 'features/auth/slice';
import React, { FC, useEffect } from 'react';
import { useDispatch } from 'react-redux';
import { initializeTodoist } from './api';
import { selectIsTodoistInitialized } from './selectors';

export const TodoistSettings: FC = () => {
    const dispatch = useDispatch();
    const isInitialized = useTypedSelector(selectIsTodoistInitialized);
    const isSignedIn = useTypedSelector(selectIsUserSignedIn);

    useEffect(
        () => {
            dispatch(initializeTodoist());
        },
        // We want to try again if the user signs in after loading
        [dispatch, isSignedIn]
    );

    return (
        <ActionButton icon={mdiFormatListChecks}>
            Todoist {isInitialized ? 'Ready' : 'Unavailable'}
        </ActionButton>
    );
};
