import { ApplicationInsights } from '@microsoft/applicationinsights-web';
import { ReactPlugin } from '@microsoft/applicationinsights-react-js';
import history from 'appHistory';

const reactPlugin = new ReactPlugin();
const appInsights = new ApplicationInsights({
    config: {
        connectionString:
            'InstrumentationKey=592695e1-bcd2-4086-bcea-8c68cbee6156;IngestionEndpoint=https://westus2-2.in.applicationinsights.azure.com/',
        extensions: [reactPlugin],
        extensionConfig: {
            [reactPlugin.identifier]: { history }
        },
        disableFetchTracking: false,
        enableCorsCorrelation: true,
        correlationHeaderExcludedDomains: [
            'home-assistant.apps.lasath.org',
            '38hh4o99fm2up9sg9py0pyqklmepmzju.ui.nabu.casa'
        ]
    }
});

appInsights.loadAppInsights();
export { reactPlugin, appInsights };
