import {
    mdiFastForward,
    mdiRewind,
    mdiSkipBackward,
    mdiSkipForward,
    mdiStepBackward,
    mdiStepForward
} from '@mdi/js';
import { useAppDispatch } from 'app/store';
import { ActionButton } from 'components/button/actionButton';
import { Row } from 'components/layout';
import { Drawer } from 'features/drawer';
import { episodeTimeShift } from 'features/selectedWord';
import { FC } from 'react';

export const MediaControlsDrawer: FC = () => {
    const dispatch = useAppDispatch();
    return (
        <Drawer summary="Media Controls">
            <Row centerChildren>
                <ActionButton
                    icon={mdiSkipBackward}
                    standalone
                    large
                    onClick={() => dispatch(episodeTimeShift(-5))}
                />
                <ActionButton
                    icon={mdiRewind}
                    standalone
                    large
                    onClick={() => dispatch(episodeTimeShift(-1))}
                />
                <ActionButton
                    icon={mdiStepBackward}
                    standalone
                    large
                    onClick={() => dispatch(episodeTimeShift(-0.1))}
                />
                <ActionButton
                    icon={mdiStepForward}
                    standalone
                    large
                    onClick={() => dispatch(episodeTimeShift(0.1))}
                />
                <ActionButton
                    icon={mdiFastForward}
                    standalone
                    large
                    onClick={() => dispatch(episodeTimeShift(1))}
                />
                <ActionButton
                    icon={mdiSkipForward}
                    standalone
                    large
                    onClick={() => dispatch(episodeTimeShift(5))}
                />
            </Row>
        </Drawer>
    );
};
