import { mdiRefresh } from '@mdi/js';
import { ActionButton } from 'components/button/actionButton';
import { FullWidthText } from 'components/fullWidth';
import { BackendInfo } from 'features/backendSelection';
import preval from 'preval.macro';
import React, { FC } from 'react';

export const UpdateComponent: FC<{
    onUpdateClick: () => void;
    canUpdate: boolean;
}> = ({ onUpdateClick, canUpdate }) => (
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
        {canUpdate && (
            <ActionButton icon={mdiRefresh} onClick={onUpdateClick}>
                Update App
            </ActionButton>
        )}
    </FullWidthText>
);
