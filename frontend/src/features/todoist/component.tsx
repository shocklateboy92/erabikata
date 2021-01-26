import { useTypedSelector } from 'app/hooks';
import { selectNowPlayingEpisodeId } from 'features/hass';
import React from 'react';
import { FC, useEffect } from 'react';
import { useDispatch } from 'react-redux';
import { fetchCandidateTasks } from './api';
import { selectIsTodoistInitialized } from './selectors';

export const TodoistContainer: FC = () => {
    const dispatch = useDispatch();
    useEffect(() => {
        dispatch(fetchCandidateTasks());
    }, [dispatch]);

    const isInitialized = useTypedSelector(selectIsTodoistInitialized);
    const task = useTypedSelector((state) => {
        const todoist = state.todoist;
        if (todoist.userSelectedTaskId) {
            return todoist.candidateTasks.find(
                (t) => t.id === todoist.userSelectedTaskId
            );
        }

        const nowPlaying = selectNowPlayingEpisodeId(state);
        if (nowPlaying) {
            return todoist.candidateTasks.find((t) =>
                t.content.includes('episode=' + nowPlaying)
            );
        }
    });

    if (!isInitialized) {
        return <div>Todoist is Unavailable or has not initialized yet.</div>;
    }

    if (!task) {
        return <div>タスクを選んでね！</div>;
    }

    return <div>{task.content}</div>;
};
