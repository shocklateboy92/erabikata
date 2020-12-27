import { Page } from 'components/page';
import { RankedWords } from 'features/rankedWords';
import { WordSummary } from 'features/wordSummary/component';
import React, { FC } from 'react';

export const RankedWordsPage: FC = () => {
    return (
        <Page title="Ranked Words" secondaryChildren={() => <WordSummary />}>
            <RankedWords />
        </Page>
    );
};
