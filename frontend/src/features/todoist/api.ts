import { AsyncThunk } from '@reduxjs/toolkit';
import { UserClient } from 'backend.generated';
import { createApiCallThunk } from 'features/auth/api';

export const initializeTodoist: AsyncThunk<
    string,
    void,
    {}
> = createApiCallThunk(
    UserClient,
    'todoist/init',
    (client) => client.getTodoistToken(),
    { condition: (_, { getState }) => !getState().todoist.authToken }
);
