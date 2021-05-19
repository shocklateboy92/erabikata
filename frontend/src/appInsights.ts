import { ApplicationInsights } from '@microsoft/applicationinsights-web';
import { ReactPlugin } from '@microsoft/applicationinsights-react-js';
import history from 'appHistory';

const reactPlugin = new ReactPlugin();
const appInsights = new ApplicationInsights({
    config: {
        instrumentationKey: '592695e1-bcd2-4086-bcea-8c68cbee6156',
        extensions: [reactPlugin],
        extensionConfig: {
            [reactPlugin.identifier]: { history }
        }
    }
});

appInsights.loadAppInsights();
export { reactPlugin, appInsights };
