import { useEngSubsStylesOfQuery } from 'backend';
import { FC } from 'react';
import { Page } from '../../components/page';
import { useParams } from 'react-router-dom';
import { FullPageError } from '../../components/fullPageError';
import { QueryPlaceholder } from '../../components/placeholder/queryPlaceholder';
import { Drawer } from '../../components/drawer';
import { Separator } from '../../components/separator';

const StyleView: FC<{ style: string }> = ({ style }) => {
    return <Drawer summary={style}>yolo?</Drawer>;
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
        <Page title="Style Filter">
            {response.data.allStyles.map((style, index) => (
                <>
                    {index > 0 && <Separator />}
                    <StyleView key={style.id} style={style.id} />
                </>
            ))}
        </Page>
    );
};
