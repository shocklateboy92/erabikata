import { FullWidthText } from 'components/fullWidth';
import { Page } from 'components/page';
import React, { FC } from 'react';
import preval from 'preval.macro';
import { HassCheck } from 'features/hass';
import { BackendInfo } from 'features/backendSelection';

export const InfoPage: FC = () => {
    return (
        <Page>
            <FullWidthText>
                <h2>App Info</h2>
                Build Version
                <br />
                {preval`module.exports = new Date().toLocaleString();`}
                <p>
                    Backend Url
                    <br />
                    <BackendInfo />
                </p>
            </FullWidthText>
            <HassCheck />
        </Page>
    );
};
