import React, { FC } from 'react';
import { Page } from 'components/page';
import { Anki } from './component';

export const AnkiPage: FC = () => (
    <Page title="Send to Anki">
        <Anki />
    </Page>
);
