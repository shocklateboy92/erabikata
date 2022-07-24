import { mdiFuriganaHorizontal } from '@mdi/js';
import { useAppSelector } from 'app/hooks';
import { useAppDispatch } from 'app/store';
import { HeaderButton } from 'features/header';
import { FC } from 'react';
import { selectIsFuriganaEnabled, toggleFurigana } from './slice';

export const FuriganaOption: FC = () => {
    const dispatch = useAppDispatch();
    const selected = useAppSelector(selectIsFuriganaEnabled);

    return (
        <HeaderButton
            active={selected}
            icon={mdiFuriganaHorizontal}
            onClick={() => dispatch(toggleFurigana())}
        >
            Furigana
        </HeaderButton>
    );
};
