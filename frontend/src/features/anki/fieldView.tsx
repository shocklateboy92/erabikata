import { FC } from 'react';
import './fieldView.scss';

export const FieldView: FC<{ title: string }> = ({ title, children }) => (
    <div className="ankiField">
        <div className="title">{title}</div>
        {children}
    </div>
);
