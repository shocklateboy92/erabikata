import { useEngSubsByStyleNameQuery, useEngSubsStylesOfQuery } from 'backend';
import { FC, Fragment } from 'react';
import { Page } from '../../components/page';
import { useParams } from 'react-router-dom';
import { FullPageError } from '../../components/fullPageError';
import { QueryPlaceholder } from '../../components/placeholder/queryPlaceholder';
import { Drawer } from '../../components/drawer';
import { Separator } from '../../components/separator';
import { EngDialog } from './engDialog';
import { SelectedWord } from '../selectedWord';

const StyleView: FC<{ styleName: string }> = ({ styleName }) => {
    const response = useEngSubsByStyleNameQuery({
        styleName,
        skip: 0,
        max: 30
    });
    if (!response.data) {
        return <QueryPlaceholder result={response} />;
    }

    return (
        <>
            {response.data.dialog.map((sub) => (
                <EngDialog content={sub} />
            ))}
        </>
    );
};

export const StylesPage: FC = () => {
    const params = useParams<{ showId: string }>();
    const showId = parseInt(params.showId);
    const response = useEngSubsStylesOfQuery({ showId }, { skip: !showId });

    if (!showId) {
        return (
            <Page title="Style Filter">
                <FullPageError>Invalid Show Id: {showId}</FullPageError>
            </Page>
        );
    }
    if (!response.data) {
        return (
            <Page title="Style Filter">
                <QueryPlaceholder result={response} />
            </Page>
        );
    }

    return (
        <Page title="Style Filter" secondaryChildren={() => <SelectedWord />}>
            {response.data.allStyles.map((style, index) => (
                <Fragment key={style.id}>
                    {index > 0 && <Separator />}
                    <Drawer summary={style.id}>
                        <StyleView styleName={style.id} />
                    </Drawer>
                </Fragment>
            ))}
        </Page>
    );
};
