import React, { FC } from 'react';
import './tag.scss';

export const Tag: FC<{ children: [React.ReactChild, React.ReactChild] }> = ({
    children
}) => {
    return (
        <span className="tag">
            {children.map((value, index) => (
                <span key={index}>{value}</span>
            ))}
        </span>
    );
};
