import React, { FC, useEffect } from 'react';
import { Redirect, useLocation, useParams } from 'react-router-dom';
import { Page } from 'components/page';
import { fetchFullWordIfNeeded, selectWordInfo } from 'features/wordContext';
import { useDispatch } from 'react-redux';
import { useAppSelector } from 'app/hooks';
import { Dialog } from 'features/dialog/Dialog';
import { SelectedWord } from 'features/selectedWord';
import { FullPageError } from 'components/fullPageError';
import { FullWidthText } from 'components/fullWidth';

export const WordPage: FC = () => {
    const { dictionaryForm } = useParams<{
        dictionaryForm: string;
    }>();
    const location = useLocation();
    const dispatch = useDispatch();
    useEffect(() => {
        dispatch(fetchFullWordIfNeeded(dictionaryForm, location));
    }, [dispatch, dictionaryForm, location]);

    const context = useAppSelector(selectWordInfo.bind(null, dictionaryForm));
    if (context === undefined) {
        return <FullWidthText>少々お待ちください</FullWidthText>;
    }

    if (context === null) {
        return (
            <FullPageError>
                「{dictionaryForm}」という言葉が存在してないみたいです
            </FullPageError>
        );
    }

    return (
        <Page title={dictionaryForm} secondaryChildren={() => <SelectedWord />}>
            {context.occurrences.map((con) => (
                <div key={con.episodeName + con.time}>
                    <Dialog
                        episode={con.episodeId}
                        time={con.time}
                        title={con.episodeName}
                    ></Dialog>
                </div>
            ))}
        </Page>
    );
};

export const SearchWordPage: FC = () => {
    const params = new URLSearchParams(window.location.search);
    const word = params.get('word');

    if (!word) {
        return <FullPageError>Invalid Word '{word}'</FullPageError>;
    }

    return <Redirect to={`/ui/word/${word}`} />;
};
