import { FC } from 'react';
import { HeaderButton } from './headerButton';
import { mdiClose } from '@mdi/js';
import { useDispatch } from 'react-redux';
import { useTypedSelector } from 'app/hooks';
import { selectionClearRequest } from 'features/selectedWord/actions';
import { shouldShowPanel } from 'features/selectedWord/selectors';

export const CloseButton: FC = () => {
    const dispatch = useDispatch();
    const shouldShow = useTypedSelector(shouldShowPanel);
    if (!shouldShow) {
        return null;
    }

    return (
        <HeaderButton
            icon={mdiClose}
            onClick={() => {
                dispatch(selectionClearRequest());
            }}
        >
            Collapse
        </HeaderButton>
    );
};
