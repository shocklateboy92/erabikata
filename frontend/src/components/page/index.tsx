import classNames from 'classnames';
import { AppHeader } from 'features/header';
import { selectionClearRequested } from 'features/selectedWord';
import React, { FC, ReactNode, useState } from 'react';
import { useDispatch } from 'react-redux';
import styles from './page.module.scss';
import Helmet from 'react-helmet';

export interface IPageProps {
    secondaryChildren?: () => ReactNode;
    title?: string;
}

export const Page: FC<IPageProps> = (props) => {
    const dispatch = useDispatch();
    const [touchState, setTouchState] = useState<{
        position: number;
        shouldClose: boolean;
    }>();
    const secondary = props.secondaryChildren?.();

    return (
        <div className="App">
            <AppHeader>{props.title}</AppHeader>
            <Helmet>
                <title>
                    {props.title ? `${props.title} - Erabikata` : 'Erabikata'}
                </title>
            </Helmet>
            <div className={styles.container}>
                <div
                    className={classNames(styles.secondary, {
                        [styles.closing]: touchState?.shouldClose
                    })}
                    onTouchStart={(e) =>
                        setTouchState(
                            window.innerHeight > window.innerWidth &&
                                e.currentTarget.scrollTop === 0
                                ? {
                                      position: e.changedTouches[0].screenY,
                                      shouldClose: false
                                  }
                                : undefined
                        )
                    }
                    onTouchMove={(e) => {
                        if (!touchState) {
                            return;
                        }
                        const shouldClose =
                            e.changedTouches[0].screenY - touchState.position >
                            100;
                        if (touchState.shouldClose !== shouldClose) {
                            setTouchState({ ...touchState, shouldClose });
                        }
                    }}
                    onTouchEnd={() => {
                        if (touchState?.shouldClose) {
                            setTouchState(undefined);
                            dispatch(selectionClearRequested());
                        }
                    }}
                >
                    {secondary}
                </div>
                <div className={styles.separator} />
                <div className={styles.primary}>{props.children}</div>
            </div>
        </div>
    );
};
