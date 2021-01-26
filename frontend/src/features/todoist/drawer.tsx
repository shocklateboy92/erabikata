import { Drawer } from 'components/drawer';
import React, { FC } from 'react';

export const TodoistDrawer: FC = () => (
    <Drawer summary="Todoist">
        <TodoistDrawer />
    </Drawer>
);
