import React, { FC } from 'react';
import { Page } from 'components/page';
import { Anki } from './component';
import { SelectedWord } from 'features/selectedWord';

export const AnkiPage: FC = () => (
    <Page title="Send to Anki" secondaryChildren={() => <SelectedWord />}>
        <Anki />
    </Page>
);
