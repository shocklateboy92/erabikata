import { AsyncThunk, createAsyncThunk } from '@reduxjs/toolkit';
import { RootState } from 'app/rootReducer';
import { UserClient } from 'backend.generated';
import { createApiCallThunk } from 'features/auth/api';
import { selectIsTodoistInitialized } from './selectors';
import todoist, { TodoistTask } from 'todoist-rest-api';
import { notification } from 'features/notifications';

export const initializeTodoist: AsyncThunk<
    string,
    void,
    {}
> = createApiCallThunk(
    UserClient,
    'todoist/init',
    (client) => client.getTodoistToken(),
    { condition: (_, { getState }) => !selectIsTodoistInitialized(getState()) }
);

export const fetchCandidateTasks: AsyncThunk<
    TodoistTask[],
    void,
    { state: RootState }
> = createAsyncThunk(
    'fetchTasks',
    async (_, { getState, dispatch }) => {
        // from the condition, we know this is not null
        const api = todoist(getState().todoist.authToken!);
        const labels = await api.v1.label.findAll({});

        const label_id = labels.find((l) => l.name === 'erabikata_root_task')
            ?.id;
        if (!label_id) {
            dispatch(
                notification({
                    title: 'Something went wrong with todoist',
                    text:
                        "We couldn't find a label with the name `erabikata_root_task`."
                })
            );
            return [];
        }

        return await api.v1.task.findAll({ label_id });
    },
    { condition: (_, { getState }) => selectIsTodoistInitialized(getState()) }
);
