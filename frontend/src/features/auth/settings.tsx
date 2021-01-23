import { mdiAccount } from '@mdi/js';
import { ActionButton } from 'components/button/actionButton';
import { FullWidthText } from 'components/fullWidth';
import React, { FC } from 'react';
import { useDispatch } from 'react-redux';
import { signIn, useAuth } from './api';

export const AuthSettings: FC = () => {
    const auth = useAuth();
    const dispatch = useDispatch();
    return (
        <FullWidthText>
            <ActionButton
                icon={mdiAccount}
                onClick={() => {
                    dispatch(signIn(auth));
                }}
            >
                Sign In
            </ActionButton>
        </FullWidthText>
    );
};
