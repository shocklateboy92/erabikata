import {
    useAppInsightsContext,
    useTrackMetric
} from '@microsoft/applicationinsights-react-js';
import { useAppDispatch } from 'app/store';
import classNames from 'classnames';
import { fetchWordsWithHiddenFurigana } from 'features/furigana';
import { AppHeader } from 'features/header';
import { NotifcationsView } from 'features/notifications';
import { SelectedWord, selectionClearRequest } from 'features/selectedWord';
import 'features/shortcuts';
import { FC, ReactNode, useEffect, useState } from 'react';
import styles from './page.module.scss';

export interface IPageProps {
    title?: string;
    children: ReactNode;
}

export const Page: FC<IPageProps> = (props) => {
    const dispatch = useAppDispatch();
    const appInsights = useAppInsightsContext();
    useTrackMetric(appInsights, 'Page');

    // Much cheaper than including react-helmet
    useEffect(() => {
        document.title = props.title
            ? `${props.title} - Erabikata`
            : 'Erabikata';
    }, [props.title]);

    const [touchState, setTouchState] = useState<{
        position: number;
        shouldClose: boolean;
    }>();

    useEffect(() => {
        dispatch(fetchWordsWithHiddenFurigana());
    });

    return (
        <div className="App">
            <AppHeader>{props.title}</AppHeader>
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
                            dispatch(selectionClearRequest());
                        }
                    }}
                >
                    <SelectedWord />
                </div>
                <div className={styles.separator} />
                <div className={styles.primary}>{props.children}</div>
            </div>
            <NotifcationsView />
        </div>
    );
};
