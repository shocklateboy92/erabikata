import { FullPageError } from 'components/fullPageError';
import { FC } from 'react';
import { useLocation } from 'react-router-dom';

export const NotFoundPage: FC = () => {
    const location = useLocation();
    return (
        <FullPageError title="Not Found">
            Page '{location.pathname}' does not exist
        </FullPageError>
    );
};
