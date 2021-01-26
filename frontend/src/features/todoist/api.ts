import { AsyncThunk, createAsyncThunk } from '@reduxjs/toolkit';
import { RootState } from 'app/rootReducer';
import axios from 'axios';
import { UserClient } from 'backend.generated';
import { createApiCallThunk } from 'features/auth/api';
import { TodoistTask } from 'todoist-rest-api';
import { selectIsTodoistInitialized } from './selectors';

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
    async (_, { getState }) => {
        const response = await axios.get<TodoistTask[]>(
            'https://api.todoist.com/rest/v1/tasks',
            {
                params: {
                    filter: '@erabikata_root_task'
                },
                headers: {
                    Authorization: `Bearer ${getState().todoist.authToken}`
                }
            }
        );
        return response.data;
    },
    { condition: (_, { getState }) => selectIsTodoistInitialized(getState()) }
);
