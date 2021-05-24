import classNames from 'classnames';
import { Separator } from 'components/separator';
import { FC } from 'react';
import './fieldView.scss';

export const FieldView: FC<{
    title: string;
    toggleActive?: () => void;
    active?: boolean;
}> = ({ title, active, toggleActive, children }) => (
    <section className={classNames('ankiField', { active })}>
        <h3 className="title" onClick={toggleActive}>
            {title}
        </h3>
        {children}
        <Separator />
    </section>
);

export const ActionFieldView: FC = ({ children }) => (
    <section className="ankiActionField">{children}</section>
);
