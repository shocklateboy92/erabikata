import { useTypedSelector } from 'app/hooks';
import React, { FC } from 'react';
import { selectBaseUrl } from './slice';

export const BackendInfo: FC = () => {
    const backend = useTypedSelector(selectBaseUrl);

    return <>{backend}</>;
};
