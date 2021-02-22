import React, { FC } from 'react';
import './tag.scss';

export const Pill: FC<{
    children: [React.ReactChild, React.ReactChild] | React.ReactChild;
}> = ({ children }) => {
    return (
        <span className="pill">
            {React.Children.map(children, (value, index) => (
                <span key={index}>{value}</span>
            ))}
        </span>
    );
};
