import {
    useEngSubsActiveStylesForQuery,
    useEngSubsByStyleNameQuery,
    useEngSubsStylesOfQuery,
    useExecuteActionMutation
} from 'backend';
import { FC, Fragment } from 'react';
import { Page } from '../../components/page';
import { useParams } from 'react-router-dom';
import { FullPageError } from '../../components/fullPageError';
import { QueryPlaceholder } from '../../components/placeholder/queryPlaceholder';
import { Drawer } from '../../components/drawer';
import { Separator } from '../../components/separator';
import { EngDialog } from './engDialog';
import { SelectedWord } from '../selectedWord';
import { ActionButton } from '../../components/button/actionButton';
import { mdiToggleSwitchOffOutline, mdiToggleSwitchOutline } from '@mdi/js';

const StyleView: FC<{ showId: number; styleName: string }> = ({
    showId,
    styleName
}) => {
    const response = useEngSubsByStyleNameQuery({
        showId,
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
                <EngDialog key={sub.id} content={sub} />
            ))}
        </>
    );
};

const ToggleStyleAction: FC<{ showId: number; styleName: string }> = ({
    showId,
    styleName
}) => {
    const { enabled } = useEngSubsActiveStylesForQuery(
        { showId },
        {
            selectFromResult: (response) => ({
                enabled: response.data?.includes(styleName)
            })
        }
    );

    const [executeAction, { isLoading }] = useExecuteActionMutation();

    if (enabled === undefined || isLoading) {
        return null;
    }

    return (
        <ActionButton
            icon={enabled ? mdiToggleSwitchOutline : mdiToggleSwitchOffOutline}
            onClick={() =>
                executeAction({
                    activityType: enabled ? 'DisableStyle' : 'EnableStyle',
                    showId,
                    styleName
                })
            }
        />
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
                    <Drawer
                        summary={style.id}
                        extraActions={() => (
                            <ToggleStyleAction
                                showId={showId}
                                styleName={style.id}
                            />
                        )}
                    >
                        <StyleView styleName={style.id} showId={showId} />
                    </Drawer>
                </Fragment>
            ))}
        </Page>
    );
};
