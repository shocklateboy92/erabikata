import { FC } from 'react';
import './layout.scss';

export const Row: FC = ({ children }) => (
    <div className="layout row">{children}</div>
);
