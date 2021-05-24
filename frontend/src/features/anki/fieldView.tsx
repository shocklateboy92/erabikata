import { Separator } from 'components/separator';
import { FC } from 'react';
import './fieldView.scss';

export const FieldView: FC<{ title: string }> = ({ title, children }) => (
    <section className="ankiField">
        <h3 className="title">{title}</h3>
        {children}
        <Separator />
    </section>
);

export const ActionFieldView: FC = ({ children }) => (
    <section className="ankiActionField">{children}</section>
);
