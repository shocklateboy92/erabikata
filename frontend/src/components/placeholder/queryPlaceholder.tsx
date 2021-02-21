import { FullWidthText } from '../fullWidth';
import React from 'react';

export const QueryPlaceholder = ({
    result
}: {
    result: { isLoading: boolean; error?: unknown };
}) => {
    if (result.isLoading) {
        return <FullWidthText>Now Loading...</FullWidthText>;
    }

    return <pre>{JSON.stringify(result.error, null, 2)}</pre>;
};
