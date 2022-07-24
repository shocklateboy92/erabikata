import classNames from 'classnames';
import { Separator } from 'components/separator';
import { FC, PropsWithChildren, ReactNode } from 'react';
import './fieldView.scss';

export const FieldView: FC<{
    title: string;
    toggleActive?: () => void;
    active?: boolean;
    children: ReactNode;
}> = ({ title, active, toggleActive, children }) => (
    <section className={classNames('ankiField', { active })}>
        <h3 className="title" onClick={toggleActive}>
            {title}
        </h3>
        {children}
        <Separator />
    </section>
);

export const ActionFieldView: FC<PropsWithChildren<{}>> = ({ children }) => (
    <section className="ankiActionField">{children}</section>
);
