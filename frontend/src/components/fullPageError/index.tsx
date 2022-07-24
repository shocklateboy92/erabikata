import { FullWidthText } from 'components/fullWidth';
import { Column } from 'components/layout';
import { Page } from 'components/page';
import React, { FC, PropsWithChildren } from 'react';

export const FullPageError: FC<PropsWithChildren<{ title?: string }>> = (
    props
) => (
    <Page title={props.title}>
        <Column>
            <FullWidthText>Error: {props.children}</FullWidthText>
        </Column>
    </Page>
);
