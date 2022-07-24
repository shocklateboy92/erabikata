import { mdiChevronDoubleDown, mdiChevronDoubleRight } from '@mdi/js';
import Icon from '@mdi/react';
import { useTypedSelector } from 'app/hooks';
import { useAppDispatch } from 'app/store';
import { InlineButton } from 'components/button';
import { Row } from 'components/layout';
import React, { FC, ReactNode } from 'react';
import styles from './drawer.module.scss';
import { drawerToggleRequest } from './slice';

interface IDrawerProps {
    summary: string;
    startOpen?: boolean;
    onToggle?: () => void;
    extraActions?: (iconSize: string) => ReactNode;
    children?: ReactNode;
}

export const Drawer: FC<IDrawerProps> = ({
    summary,
    startOpen,
    children,
    onToggle,
    ...props
}) => {
    const isOpen = useTypedSelector((state) => state.drawer.open[summary]);
    const dispatch = useAppDispatch();

    if (React.Children.count(children) === 0) {
        return null;
    }

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
                    dispatch(drawerToggleRequest(summary));
                }}
            >
                {summary}
            </InlineButton>
            {props.extraActions && (
                <div className={styles.actions}>
                    <Row>{props.extraActions('32px')}</Row>
                </div>
            )}
            {isOpen && <div className={styles.content}>{children}</div>}
        </div>
    );
};
