import { mdiAccount } from '@mdi/js';
import { useTypedSelector } from 'app/hooks';
import { ActionButton } from 'components/button/actionButton';
import { FullWidthText } from 'components/fullWidth';
import React, { FC } from 'react';
import { useDispatch } from 'react-redux';
import { signIn } from './api';
import { selectIsUserSignedIn } from './slice';

export const AuthSettings: FC = () => {
    const dispatch = useDispatch();
    const signedIn = useTypedSelector(selectIsUserSignedIn);

    return (
        <FullWidthText>
            {signedIn ? (
                <ActionButton icon={mdiAccount}>Sign Out</ActionButton>
            ) : (
                <ActionButton
                    icon={mdiAccount}
                    onClick={() => {
                        dispatch(signIn());
                    }}
                >
                    Sign In
                </ActionButton>
            )}
        </FullWidthText>
    );
};
