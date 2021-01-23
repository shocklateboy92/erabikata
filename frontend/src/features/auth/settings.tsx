import { mdiAccount } from '@mdi/js';
import { ActionButton } from 'components/button/actionButton';
import { FullWidthText } from 'components/fullWidth';
import React, { FC, useEffect } from 'react';
import { useDispatch } from 'react-redux';
import { checkSignIn, signIn, useAuth } from './api';

export const AuthSettings: FC = () => {
    const auth = useAuth();
    const dispatch = useDispatch();
    useEffect(() => {
        dispatch(checkSignIn(auth));
    }, [auth, dispatch]);

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
