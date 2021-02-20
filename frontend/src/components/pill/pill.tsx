import React, { FC } from 'react';
import './tag.scss';

export const Pill: FC<{ children: [React.ReactChild, React.ReactChild] }> = ({
    children
}) => {
    return (
        <span className="pill">
            {children.map((value, index) => (
                <span key={index}>{value}</span>
            ))}
        </span>
    );
};
