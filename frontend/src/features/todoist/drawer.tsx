import { Drawer } from 'components/drawer';
import React, { FC } from 'react';
import { TodoistContainer } from './component';

export const TodoistDrawer: FC = () => (
    <Drawer summary="Todoist">
        <TodoistContainer />
    </Drawer>
);
