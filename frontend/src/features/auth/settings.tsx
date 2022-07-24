import { mdiAccount } from '@mdi/js';
import { useTypedSelector } from 'app/hooks';
import { useAppDispatch } from 'app/store';
import { ActionButton } from 'components/button/actionButton';
import { FC } from 'react';
import { signIn } from './api';
import { selectIsUserSignedIn } from './slice';

export const AuthSettings: FC = () => {
    const dispatch = useAppDispatch();
    const signedIn = useTypedSelector(selectIsUserSignedIn);

    return signedIn ? (
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
    );
};
