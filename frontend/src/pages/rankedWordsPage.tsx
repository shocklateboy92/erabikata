import { Page } from 'components/page';
import { RankedWords } from 'features/rankedWords';
import { SelectedWord } from 'features/selectedWord';
import React, { FC } from 'react';

export const RankedWordsPage: FC = () => {
    return (
        <Page title="Ranked Words" secondaryChildren={() => <SelectedWord />}>
            <RankedWords />
        </Page>
    );
};
