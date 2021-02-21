import { FullWidthText } from '../fullWidth';
import React from 'react';

export const QueryPlaceholder = ({
    result,
    quiet
}: {
    result: { isLoading: boolean; error?: unknown };
    quiet?: boolean;
}) => {
    if (result.isLoading) {
        return quiet ? null : <FullWidthText>Now Loading...</FullWidthText>;
    }

    return <pre>{JSON.stringify(result.error, null, 2)}</pre>;
};
