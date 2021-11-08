import { Page } from 'components/page';
import { RankedWords } from 'features/rankedWords';
import React, { FC } from 'react';

export const RankedWordsPage: FC = () => {
    return (
        <Page title="Ranked Words">
            <RankedWords />
        </Page>
    );
};
