import { FullWidthText } from 'components/fullWidth';
import { Page } from 'components/page';
import React, { FC } from 'react';
import preval from 'preval.macro';
import { HassCheck } from 'features/hass';

export const InfoPage: FC = () => {
    return (
        <Page>
            <FullWidthText>
                <h2>App Info</h2>
                Build Version
                <br />
                {preval`module.exports = new Date().toLocaleString();`}
            </FullWidthText>
            <HassCheck />
        </Page>
    );
};
