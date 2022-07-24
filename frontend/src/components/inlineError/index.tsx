import React, { FC, PropsWithChildren } from 'react';

export const InlineError: FC<PropsWithChildren<{}>> = ({ children }) => {
    return <div className="inline-error">Error: {children}</div>;
};
