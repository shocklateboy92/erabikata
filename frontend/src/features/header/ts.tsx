import { mdiClose } from '@mdi/js';
import { useTypedSelector } from 'app/hooks';
import { useAppDispatch } from 'app/store';
import { selectionClearRequest } from 'features/selectedWord/actions';
import { shouldShowPanel } from 'features/selectedWord/selectors';
import { FC } from 'react';
import { HeaderButton } from './headerButton';

export const CloseButton: FC = () => {
    const dispatch = useAppDispatch();
    const shouldShow = useTypedSelector(shouldShowPanel);
    if (!shouldShow) {
        return null;
    }

    return (
        <HeaderButton
            hideOnMobile
            icon={mdiClose}
            onClick={() => {
                dispatch(selectionClearRequest());
            }}
        >
            Collapse
        </HeaderButton>
    );
};
