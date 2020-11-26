import { mdiFuriganaHorizontal } from '@mdi/js';
import { useAppSelector } from 'app/hooks';
import { HeaderButton } from 'features/header';
import React, { FC } from 'react';
import { useDispatch } from 'react-redux';
import { selectIsFuriganaEnabled, toggleFurigana } from './slice';

export const FuriganaOption: FC = () => {
    const dispatch = useDispatch();
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
