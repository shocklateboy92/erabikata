import {
    fireEvent,
    render,
    screen,
    waitForElementToBeRemoved
} from '@testing-library/react';
import { App } from 'App';
import appHistory from 'appHistory';

describe('ranked words page', () => {
    let wordText: string | null = null;
    test('renders', async () => {
        appHistory.replace('/ui/rankedWords');
        render(<App />);

        await waitForElementToBeRemoved(
            () => screen.queryByText('Now Loading...'),
            { timeout: 2000 }
        );

        const text = await getFirstWordText();

        wordText = text!;
    });

    test('navigates to next page', async () => {
        render(<App />);
        const next = await screen.findByText('Next Page');
        fireEvent.click(next);

        expect(wordText).toBeTruthy();
        await waitForElementToBeRemoved(() => screen.queryByText(wordText!), {
            timeout: 2000
        });

        const newText = await getFirstWordText();
        expect(appHistory.location.pathname).toBe('/ui/rankedWords/1');

        render(<App />);
        const refreshedText = await getFirstWordText();
        expect(refreshedText).toBe(newText);
    });
});

async function getFirstWordText() {
    const word = await screen.findAllByText('Rank:', { exact: false });
    const text = word[0].parentElement?.querySelector('ruby')?.textContent;
    expect(text).toBeTruthy();

    return text!;
}
