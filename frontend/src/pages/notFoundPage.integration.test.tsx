import { render, screen } from '@testing-library/react';
import { App } from 'App';
import appHistory from 'appHistory';

describe('not found page', () => {
    test('renders on random URL', async () => {
        appHistory.push('/ui/random');
        render(<App />);

        await screen.findByText("Error: Page '/ui/random' does not exist", {
            collapseWhitespace: true
        });
    });

    test('renders on random URL with sub path', async () => {
        appHistory.push('/ui/random/subPath');
        render(<App />);

        await screen.findByText(
            "Error: Page '/ui/random/subPath' does not exist",
            {
                collapseWhitespace: true
            }
        );
    });

    test('renders on random URL without UI', async () => {
        appHistory.push('/random');
        render(<App />);

        await screen.findByText("Error: Page '/ui/random' does not exist", {
            collapseWhitespace: true
        });
    });
});
