import { useTypedSelector } from 'app/hooks';
import { useAppDispatch } from 'app/store';
import { selectNowPlayingMedia } from 'features/hass';
import React, { FC, useEffect } from 'react';
import { fetchCandidateTasks, initializeTodoist } from './api';
import { selectIsTodoistInitialized } from './selectors';

const TaskCreator: FC = () => {
    const media = useTypedSelector(selectNowPlayingMedia);
    if (!media) {
        return <div>今、何も再生されていません </div>;
    }

    return (
        <div>
            Create new task for currently playing episode?
            <br />
            Do words for '{media.title}'
        </div>
    );
};

export const TodoistContainer: FC = () => {
    const dispatch = useAppDispatch();
    useEffect(() => {
        dispatch(initializeTodoist()).then(() =>
            dispatch(fetchCandidateTasks())
        );
    }, [dispatch]);

    const isInitialized = useTypedSelector(selectIsTodoistInitialized);
    const task = useTypedSelector((state) => {
        const todoist = state.todoist;
        if (todoist.userSelectedTaskId) {
            return todoist.candidateTasks.find(
                (t) => t.id === todoist.userSelectedTaskId
            );
        }

        const nowPlaying = selectNowPlayingMedia(state);
        if (nowPlaying) {
            return todoist.candidateTasks.find((t) =>
                t.content.includes('episode=' + nowPlaying.id)
            );
        }
    });

    if (!isInitialized) {
        return <div>Todoist is Unavailable or has not initialized yet.</div>;
    }

    if (!task) {
        return <TaskCreator />;
    }

    return <div>{task.content}</div>;
};
