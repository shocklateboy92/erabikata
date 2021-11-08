import { FullWidthText } from '../fullWidth';
import React from 'react';
import { Column } from 'components/layout';

export const QueryPlaceholder = ({
    result,
    quiet
}: {
    result: { isLoading: boolean; error?: unknown };
    quiet?: boolean;
}) => {
    if (result.isLoading) {
        return quiet ? null : (
            <Column>
                <FullWidthText>Now Loading...</FullWidthText>
            </Column>
        );
    }

    return <pre>{JSON.stringify(result.error, null, 2)}</pre>;
};
