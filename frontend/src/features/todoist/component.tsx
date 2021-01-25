import { useTypedSelector } from 'app/hooks';
import React from 'react';
import { FC, useEffect } from 'react';
import { useDispatch } from 'react-redux';
import { fetchCandidateTasks } from './api';
import { selectIsTodoistInitialized } from './selectors';

export const TodoistView: FC = () => {
    const isInitialized = useTypedSelector(selectIsTodoistInitialized);
    const dispatch = useDispatch();
    useEffect(() => {
        dispatch(fetchCandidateTasks());
    }, [dispatch]);

    if (!isInitialized) {
        return <div>Todoist is Unavailable or has not initialized yet.</div>;
    }
};
