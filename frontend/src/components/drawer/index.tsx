import { mdiChevronDoubleDown, mdiChevronDoubleRight } from '@mdi/js';
import Icon from '@mdi/react';
import { InlineButton } from 'components/button';
import React, { FC, ReactNode, useState } from 'react';
import styles from './drawer.module.scss';

interface IDrawerProps {
    summary: string;
    startOpen?: boolean;
    onToggle?: () => void;
    extraActions?: (iconSize: string) => ReactNode;
}

export const Drawer: FC<IDrawerProps> = ({
    summary,
    startOpen,
    children,
    onToggle,
    ...props
}) => {
    const [isOpen, setIsOpen] = useState(startOpen);

    return (
        <div className={styles.container}>
            <Icon
                className={styles.icon}
                path={isOpen ? mdiChevronDoubleDown : mdiChevronDoubleRight}
                size="2em"
            />
            <InlineButton
                className={styles.summary}
                onClick={() => {
                    onToggle?.();
                    setIsOpen(!isOpen);
                }}
            >
                {summary}
            </InlineButton>
            {props.extraActions && (
                <div className={styles.actions}>
                    {props.extraActions('2em')}
                </div>
            )}
            {isOpen && <div className={styles.content}>{children}</div>}
        </div>
    );
};
