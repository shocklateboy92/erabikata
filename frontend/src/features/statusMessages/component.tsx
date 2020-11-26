import { FC } from 'react';
import { getToken } from '../../api/plexToken';
import { Link } from 'react-router-dom';
import React from 'react';

export const StatusMessages: FC = () => {
    if (!getToken()) {
        return (
            <div>
                <p>エラー: Tokenをつかめませんでした!</p>
                <Link to="/login">ログインしてお願いします</Link>
            </div>
        );
    }
    return null;
};
