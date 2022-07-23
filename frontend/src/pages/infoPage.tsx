import { FullWidthText } from 'components/fullWidth';
import { Column } from 'components/layout';
import { Page } from 'components/page';
import { AuthSettings } from 'features/auth';
import { HassCheck } from 'features/hass';
import { UpdateCheck } from 'features/update';
import { WakeLockOption } from 'features/wakeLock';
import { FC } from 'react';
export const InfoPage: FC = () => {
    return (
        <Page>
            <Column>
                <UpdateCheck />
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
                </FullWidthText>
            </Column>
        </Page>
    );
};
