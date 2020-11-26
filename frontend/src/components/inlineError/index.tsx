import React, { FC } from 'react';

export const InlineError: FC = ({ children }) => {
    return <div className="inline-error">Error: {children}</div>;
};
