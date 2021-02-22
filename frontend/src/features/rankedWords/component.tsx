import { mdiChevronLeft, mdiChevronRight, mdiShare } from '@mdi/js';
import Icon from '@mdi/react';
import { useTypedSelector } from 'app/hooks';
import { RootState } from 'app/rootReducer';
import { useWordsRanked2Query } from 'backend';
import classNames from 'classnames';
import {
    newWordSelected,
    selectSelectedWord,
    selectSelectedWords,
    wordSelectionV2
} from 'features/selectedWord';
import { WordContext } from 'features/wordContext';
import React, { FC, useEffect } from 'react';
import { shallowEqual, useDispatch, useSelector } from 'react-redux';
import { Link, useParams } from 'react-router-dom';
import styles from './rankedWords.module.scss';
import { fetchRankedWords, selectRankedWordsArray } from './slice';
import { selectAnalyzer } from '../backendSelection';
import { QueryPlaceholder } from '../../components/placeholder/queryPlaceholder';
import { WordRankInfo } from '../../backend-rtk.generated';

const SelectableDiv: FC<WordRankInfo> = ({ text, count, id, rank }) => {
    const isActive = useTypedSelector((state) =>
        selectSelectedWords(state).includes(id)
    );
    const dispatch = useDispatch();
    return (
        <div
            className={classNames(styles.word, {
                [styles.active]: isActive
            })}
            onClick={() => dispatch(wordSelectionV2([id]))}
        >
            <ruby>{text}</ruby>
            <div className={styles.rank}>
                Rank: {rank ?? '????'}
                <br />
                Occurrences: {count ?? '????'}
            </div>
            <Link to={`/ui/word/${id}`}>
                <Icon path={mdiShare} size="2em" />
            </Link>
        </div>
    );
};

const ChangePageLink: FC<{ pageNum: number }> = ({ pageNum, children }) => (
    <div className={styles.pageLink}>
        <Link to={`/ui/rankedWords/${pageNum}`}>{children}</Link>
    </div>
);

const ChangePageIcon: FC<{ path: string }> = ({ path }) => (
    <Icon path={path} size={'1.5em'} />
);

const max = 100;
export const RankedWords: FC = () => {
    const pageParam = parseInt(useParams<{ pageNum: string }>().pageNum);
    const pageNum = isNaN(pageParam) ? 0 : pageParam;
    const skip = pageNum * max;

    const analyzer = useTypedSelector(selectAnalyzer);
    const response = useWordsRanked2Query({ analyzer, skip, max });
    if (!response.data) {
        return <QueryPlaceholder result={response} />;
    }
    const words = response.data;

    return (
        <div className={styles.outer}>
            {pageNum > 0 && (
                <ChangePageLink pageNum={pageNum - 1}>
                    <ChangePageIcon path={mdiChevronLeft} />
                    Previous Page
                </ChangePageLink>
            )}
            <div className={styles.container}>
                {words.map((word) => (
                    <SelectableDiv key={word.id} {...word} />
                ))}
            </div>
            <ChangePageLink pageNum={pageNum + 1}>
                Next Page
                <ChangePageIcon path={mdiChevronRight} />
            </ChangePageLink>
        </div>
    );
};
