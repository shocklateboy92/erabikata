import { FC, PropsWithChildren } from 'react';

export const Column: FC<PropsWithChildren<{}>> = ({ children }) => (
    <div className="layout column">{children}</div>
);
