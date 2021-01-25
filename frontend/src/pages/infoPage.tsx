import { FullWidthText } from 'components/fullWidth';
import { Page } from 'components/page';
import { AuthSettings } from 'features/auth';
import { AnalyzerSelector } from 'features/backendSelection/analyzerSelector';
import { HassCheck } from 'features/hass';
import { TodoistSettings } from 'features/todoist/settings';
import { UpdateCheck } from 'features/update';
import { WakeLockOption } from 'features/wakeLock';
import React, { FC } from 'react';
export const InfoPage: FC = () => {
    return (
        <Page>
            <UpdateCheck />
            <AnalyzerSelector />
            <HassCheck />
            <FullWidthText>
                <h2>Playback Options</h2>
                <WakeLockOption />
            </FullWidthText>
            <FullWidthText>
                <h2>Authentication</h2>
                <div>
                    <AuthSettings />
                </div>
                <div>
                    <TodoistSettings />
                </div>
            </FullWidthText>
        </Page>
    );
};
